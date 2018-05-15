using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//---------------------------
using System.Data.SQLite; // Install-package System.Data.SQLite 
using System.Data;
using CatalogoDVD_consola;


namespace CatalogoDVD
{            
    public class DAO_SQLite : IDAO
    {    
        // Instanciamos un objeto MySQlConnection. Para ello necesitamos descargarnos la librería correspondiente
        public SQLiteConnection conexion;

        #region Conectar, desconectar, conectado
        public bool Conectar(string srv, string port, string db, string user, string pwd)
        {
            string cadenaConexion = String.Format("Data Source={0};Version=3;FailIfMissing=true", db);
            // FailIfMissing=true -> Fallará si no encuentra el fichero .db (si no, lo crearía)
            
            try
            {
                conexion = new SQLiteConnection(cadenaConexion);
                conexion.Open(); // Abrimos la sesión MySQL.  Ya conecta
                //return true;
            }
            catch (SQLiteException e)
            {
                throw new Exception("Error: " + e.ErrorCode);              
            }

            return true;
        }

        public void Desconectar()
        {
            try
            {            
                conexion.Close();
            }

            catch (SQLiteException)
            {
                throw;
            }
        }

        public bool Conectado()
        {
            if (conexion != null)
                return conexion.State == ConnectionState.Open; // Si está abierta la conexión (Si el estado coincide con el de Conectado, es que está conectado)
            else
                return false;
        }
        #endregion 

        #region Select
        public List<Dvd> Seleccionar(string codigo)
        {
            string orden;
            List<Dvd> listaDvd = new List<Dvd>(); // Lista que recoge el resultado del select

            // No es lo suyo hacer un select * nunca desde aquí, porque se pueden añadir campos con el tiempo y no interesa seleccionarlos, etc.
            // select codigo, titulo, artista, pais, compania from dvd where codigo = 1000;


            if (codigo == null) // Si no hay código, hará un select sin condición (sin el where codigo)
                orden = "select codigo, titulo, artista, pais, compania, precio, anio from dvd;"; // No hace falta el ";" final como haría falta por la consola.
            else
                orden = string.Format("select codigo, titulo, artista, pais, compania, precio, anio from dvd where codigo = {0}", codigo);
           

            // Ahora construimos un comando:
            SQLiteCommand comando = new SQLiteCommand(orden, conexion); // Se construye mediante la orden SQL (string con la orden cmd) y una conexión activa.                
            
            // Objeto que te permite guardar la lista/colección:
            SQLiteDataReader lector = null;

            try
            {
                lector = comando.ExecuteReader(); // ESTO LANZA LA ORDEN sobre el comando ya armado. ExecuteReader() devuelve una colección para recogerla.

                // ExecuteReader(); ->  Si devuelve una colección.
                // comando.ExecuteNonQuery(); -> Lanza la orden y no espera resultado excepto si ha ido bien o no (Ejemplo, devuelve-> Query OK 2000 rows afected, el número de filas afectadas). NO DEVUELVE NI UN NÚMERO NI UNA COLECCIÓN. Ejemplo-> Update, insert... No será un select.
                // ExecuteScalar() -> Devuelve un número (un único número). Lanza la orden y espera un resultado, que puedes recoger. Por ejemplo, si esperas un resultado, por ejemplo el count(*) y quieres recogerlo, se hará con este método.


                // Recorremos la lista lector:
                while (lector.Read()) // Mientras pueda leer... Si no hay nada que recorrer, lector.Read() es falso y no entre en el while.
                {
                    Dvd unDvd = new Dvd(); // Creo un dvd y lo lleno
                    unDvd.Codigo = int.Parse(lector["codigo"].ToString()); // Accedes al código por su nombre. En la colección lector, puedes acceder a las columnas recogidas por su nombre.
                    unDvd.Titulo = lector["titulo"].ToString();
                    unDvd.Artista = lector["artista"].ToString();
                    unDvd.Pais = lector["pais"].ToString();
                    unDvd.Compania = lector["compania"].ToString();
                    unDvd.Precio = double.Parse(lector["precio"].ToString());
                    unDvd.Anio = lector["anio"].ToString();

                    listaDvd.Add(unDvd);
                }

                return listaDvd;

            }

            catch (SQLiteException)
            {
                throw new Exception("No tiene permisos para ejecutar esta orden");
            }

            finally
            {
                if (lector != null)
                   lector.Close();
            }

        }

        public List<Dvd> SeleccionarPA(string codigo, out int resultado) // Resultado es el parámetro de salida
        {
            throw new Exception("No hay procedimientos almacenados en SQLite");

        }

        public System.Data.DataTable SeleccionarTB(string codigo)
        {
            DataTable dt = new DataTable(); // Objeto DataTable
            string orden;

            if (codigo == null) // Si no hay código, hará un select sin condición (sin el where codigo)
                orden = "select codigo, titulo, artista, pais, compania, precio, anio from dvd;"; // No hace falta el ";" final como haría falta por la consola.
            else
                orden = string.Format("select codigo, titulo, artista, pais, compania, precio, anio from dvd where codigo = {0}", codigo);


            SQLiteCommand cmd = new SQLiteCommand(orden, conexion);
            SQLiteDataAdapter da = new SQLiteDataAdapter(cmd); // Lo guardas en un adaptador de datos. Una tabla que se adapta

            da.Fill(dt); // Fill rellena la tabla con el adaptador y mete los datos en el objeto DataTable (dt)

            return dt;

        }


        public Pais SeleccionarPais(string iso2)
        {
            Pais paisResultado = new Pais();
            string orden = string.Format("select nombre from pais where iso2 = '{0}'", iso2);
            SQLiteCommand cmd = new SQLiteCommand(orden, conexion);

            object salida = cmd.ExecuteScalar();
            if (salida != null)
            {
                paisResultado.Iso2 = iso2;
                paisResultado.Nombre = salida.ToString();
            }

            return paisResultado;
        }

        #endregion 

        public int Borrar(string codigo)
        {
            string orden = string.Empty;

            if (codigo != null)
            {
                orden = String.Format("delete from dvd where codigo = {0}", codigo);
                SQLiteCommand cmd = new SQLiteCommand(orden, conexion);

                return cmd.ExecuteNonQuery(); // Devuelve el número de filas afectadas por la operación.
            }

            else
                return -1;

        }

        public int Actualizar(Dvd unDVD)
        {
            string orden;

            if (unDVD != null)
            {
                orden = string.Format("update dvd set titulo='{0}', artista='{1}', pais='{2}', compania='{3}', precio={4}, anio='{5}' where codigo='{6}'",
                    unDVD.Titulo, unDVD.Artista, unDVD.Pais, unDVD.Pais, unDVD.Compania, unDVD.Precio, unDVD.Anio, unDVD.Codigo);

                SQLiteCommand cmd = new SQLiteCommand(orden, conexion);
                return cmd.ExecuteNonQuery();
            }

            else
                return -1;
        }

        public int Insertar(Dvd unDVD)
        {
            string orden;

            if (unDVD != null)
            {
                orden = String.Format("insert into dvd (codigo,titulo,artista,pais,compania,precio,anio) values ('{0}','{1}','{2}','{3}','{4}',{5},'{6}')",
                    unDVD.Codigo, unDVD.Titulo, unDVD.Artista, unDVD.Pais, unDVD.Compania, unDVD.Precio, unDVD.Anio);

                SQLiteCommand cmd = new SQLiteCommand(orden, conexion);
                return cmd.ExecuteNonQuery();
            }

            return -1;
        }


    }
}
