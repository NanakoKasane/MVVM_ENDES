using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//---------------------------
using MySql.Data.MySqlClient; // Descargar la librería. Nos la descargamos y estaba en MySql.Data.dll    
// Y la referenciamos. Pero no es compatible con el Framework así que nos bajamos una anterior con NuGet.
// PARA SOLUCIONARLO HACEMOS --> Herramientas --> Administracion de paquetes NuQet --> Consola de Administrador de paquetes, y ponemos -> Install-package MySql.Data -version 6.8.8

// Importante: MySqlData --> Copia local --> True

//Añado el siguiente namespace:
using System.Data;

namespace CatalogoDVD_consola
{            
    class DAO_MySQL : IDAO
    {    
        // Instanciamos un objeto MySQlConnection. Para ello necesitamos descargarnos la librería correspondiente
        public MySqlConnection conexion;

        #region Conectar, desconectar, conectado
        public bool Conectar(string srv, string port, string db, string user, string pwd)
        {
            string cadenaConexion = String.Format("server={0};port={1};database={2};uid={3};pwd={4};", srv, port, db, user, pwd);

            try
            {
                conexion = new MySqlConnection(cadenaConexion);
                conexion.Open(); // Abrimos la sesión MySQL.  Ya conecta
                return true;
            }
            catch (MySqlException e)
            {
                switch (e.Number) // Número de error. Pondremos los códigos de error que pueden causarse
                {
                    case 0:
                        throw new Exception("Error de conexión: " + e.ErrorCode);

                    case 1045: 
                        throw new Exception("Usuario o contraseña incorrectos");

                    default:
                        throw; // Propaga la excepción
                }
            }


        }

        public void Desconectar()
        {
            try
            {            
                conexion.Close();
            }

            catch (MySqlException)
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
            MySqlCommand comando = new MySqlCommand(orden, conexion); // Se construye mediante la orden SQL (string con la orden cmd) y una conexión activa.                
            
            // Objeto que te permite guardar la lista/colección:
            MySqlDataReader lector = null;

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

            catch (MySqlException)
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
            List<Dvd> lista = new List<Dvd>();

            MySqlCommand cmd = new MySqlCommand("selectDVD", conexion); // "selectDVD" -> Nombre del Procedimiento Almacenado en la BD
            cmd.CommandType = CommandType.StoredProcedure; // Cambiar el tipo del comando a lanzar a un StoredProcedure -> Procedimiento Almacenado.  Por defecto el tipo era texto, le pasabas la orden por texto
            
            // Darle los parámetros al Procedimiento Almacenado:
            //if (codigo == null)
            //{
            //    cmd.Parameters.AddWithValue("@codi", null); // @codi --> Debe coincidir exactamente con la definición del PA. Tendrá que llamarse así en la BD.
            //}
            //else


            // DARLE PARÁMETROS AL PA:
            cmd.Parameters.AddWithValue("@codi", codigo); //La '@' indica que es un parámetro. Pero en el .sql del PA no se pondrá al '@'
            // Relleno el parámetro @codi con el valor de codigo
            cmd.Parameters["@codi"].Direction = ParameterDirection.Input; // NO HACE FALTA, ES POR DEFECTO


            // Informar del tipo de dato devuelto por la ejecucion del Procedimiento Almacenado:
            cmd.Parameters.AddWithValue("@resul", MySqlDbType.Int32); // Parámetro de salida. En vez de darle el valor puedes darle el tipo.
            cmd.Parameters["@resul"].Direction = ParameterDirection.Output; // Out -> Indicas que el parámetro @resultado es de salida.



            // Creo un parámetro (Versión larga):
            //MySqlParameter elResultado = new MySqlParameter();            
            //elResultado.ParameterName = "@resul";
            //elResultado.Direction = ParameterDirection.Output;
            //elResultado.MySqlDbType = MySqlDbType.Int32;
            //cmd.Parameters.Add(elResultado);


            // AHORA LO MISMO QUE EL SELECT GENÉRICO:
            MySqlDataReader lector = null;
            try
            {
                lector = cmd.ExecuteReader(); // ESTO LANZA LA ORDEN sobre el comando ya armado. ExecuteReader() devuelve una colección para recogerla.


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

                    lista.Add(unDvd);
                }

                return lista;

            }

            catch (MySqlException)
            {
                throw;
            }

            finally
            {
                if (lector != null)
                    lector.Close();

                // LOS PARÁMETROS DE SALIDA DE LOS PA TOMAN VALOR DESPUÉS DE CERRAR EL CURSOR

                // RECOJO EL RESULTADO (parámetro de salida):
                resultado = (int)cmd.Parameters["@resul"].Value; 
            }



        }

        public System.Data.DataTable SeleccionarTB(string codigo)
        {
            DataTable dt = new DataTable(); // Objeto DataTable
            string orden;

            if (codigo == null) // Si no hay código, hará un select sin condición (sin el where codigo)
                orden = "select codigo, titulo, artista, pais, compania, precio, anio from dvd;"; // No hace falta el ";" final como haría falta por la consola.
            else
                orden = string.Format("select codigo, titulo, artista, pais, compania, precio, anio from dvd where codigo = {0}", codigo);


            MySqlCommand cmd = new MySqlCommand(orden, conexion);
            MySqlDataAdapter da = new MySqlDataAdapter(cmd); // Lo guardas en un adaptador de datos. Una tabla que se adapta

            da.Fill(dt); // Fill rellena la tabla con el adaptador y mete los datos en el objeto DataTable (dt)

            return dt;

        }


        public Pais SeleccionarPais(string iso2)
        {
            throw new NotImplementedException();
        }

        #endregion 

        public int Borrar(string codigo)
        {
            string orden = string.Empty;

            if (codigo != null)
            {
                orden = String.Format("delete from dvd where codigo = {0}", codigo);
                MySqlCommand cmd = new MySqlCommand(orden, conexion);

                return cmd.ExecuteNonQuery(); // Devuelve el número de filas afectadas por la operación.
            }

            else
                return -1;

        }

        public int Actualizar(Dvd unDVD)
        {
            throw new NotImplementedException();
        }

        public int Insertar(Dvd unDVD)
        {
            string orden;

            if (unDVD != null)
            {
                orden = String.Format("insert into dvd (codigo,titulo,artista,pais,compania,precio,anio) values ('{0}','{1}','{2}','{3}','{4}',{5},'{6}')",
                    unDVD.Codigo, unDVD.Titulo, unDVD.Artista, unDVD.Pais, unDVD.Compania, unDVD.Precio, unDVD.Anio);

                MySqlCommand cmd = new MySqlCommand(orden, conexion);
                return cmd.ExecuteNonQuery();
            }

            return -1;
        }


    }
}
