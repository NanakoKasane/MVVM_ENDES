using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatalogoDVD_consola // Cambio todos los namespace a ese 
{
    class UI
    {
        // Campos
        static DAO_MySQL dao;
        static string host = "localhost";
        static string bd = "catalogo";
        static string usr = "usr_catalogo";
        static string pwd = "123";
        static string port = "3306";

        // Constructor
        public UI()
        {
            dao = new DAO_MySQL();
            PedirOpcion();
        }

        // Método del Menú
        public static void Menu()
        {

            Console.Clear();
            Console.WriteLine("CATÁLOGO DE DVDs - Opciones");
            Console.WriteLine("===========================\n");
            Console.WriteLine("[0] CONECTAR a la BD");
            Console.WriteLine("[1] DESCONECTAR a la BD");
            Console.WriteLine("[2] Comprobar si hay conexión");
            Console.WriteLine("[3] Seleccionar un código de DVD (genérico)");
            Console.WriteLine("[4] Seleccionar a toda la tabla (genérico)");
            Console.WriteLine("[5] Seleccionar toda la tabla mediante un PA");
            Console.WriteLine("[6] Borrar un DVD de la tabla");
            Console.WriteLine("[7] Insertar un DVD");


            Console.WriteLine("[S] Fin del programa");
            Console.Write("\nOpción: ");

        }

        public static void PedirOpcion()
        {
            ConsoleKeyInfo tecla;
            bool correcto = false;
            string codigo = string.Empty;
            List<Dvd> listado = new List<Dvd>();


            do{

                Menu();

            try
            {
                    tecla = Console.ReadKey();
                    if (tecla.KeyChar != '0' && tecla.KeyChar != '1' && tecla.KeyChar != '2' && tecla.KeyChar != '3' && tecla.KeyChar != '4' && tecla.KeyChar != '5' && tecla.KeyChar != '6' && tecla.KeyChar != '7' && tecla.Key != ConsoleKey.S)
                        correcto = false;
                    else
                        correcto = true;

                    switch (tecla.KeyChar)
                    {
                        case '0': // Conexión a la BD
                            Console.Clear();
                            if (!dao.Conectado())
                            {
                                if (dao.Conectar(host, port, bd, usr, pwd))
                                    Console.WriteLine("Conexión con éxito a la BD");
                            }
                            else
                                Console.WriteLine("Ya hay una conexión establecida");
                            Console.ReadKey();
                            break;                    
                    
                        case '1': // Desconexión a la BD
                            Console.Clear();
                            if (dao.Conectado())
                            {
                                dao.Desconectar();
                                Console.WriteLine("Se ha desconectado con éxito");
                            }
                            else
                                Console.WriteLine("No hay ninguna conexión activa a la BD");
                            Console.ReadKey();
                            break;

                        case '2': // Comprobar la conexión
                            Console.Clear();
                            if (dao.Conectado())
                            {
                                Console.WriteLine("Está conectado");
                            }
                            else
                                Console.WriteLine("No está conectado");
                            Console.ReadKey();
                            break;

                        case '3': // Select con código
                            Console.Clear();
                            Console.Write("\nIntroduzca el código a seleccionar: ");
                            codigo = Console.ReadLine();
                            listado = dao.Seleccionar(codigo);
                            MostrarListado(listado);                            
                            Console.ReadLine();
                            correcto = true;
                            break;

                        case '4': // Select genérico
                            Console.Clear();
                            listado = dao.Seleccionar(null);
                            MostrarListado(listado);                            
                            Console.ReadLine();
                            break;

                        case '5': // Seleccionar toda la tabla a través de un Procedimiento Almacenado
                            Console.Clear();
                            int resultado;
                            listado = dao.SeleccionarPA(null, out resultado);
                            Console.WriteLine("Número de filas retornadas: {0}\n", resultado);

                            if (resultado != 0) // Si saca más de 0 filas, las muestra. resultado obtenía el número de filas encontradas
                                MostrarListado(listado);
                            else
                                Console.WriteLine("No hay resultados");

                            Console.ReadLine();
                            break;

                        case '6': 
                            Console.Clear();
                            Console.WriteLine("¿Código del DVD a eliminar?");
                            string codigoDel = Console.ReadLine();

                            Console.WriteLine(dao.Borrar(codigoDel) + " filas borradas");
                            Console.ReadLine();
                            break;

                        case '7':
                            Console.Clear();

                            // Pido para rellenar el DVD:
                            Dvd unDvd = new Dvd();
                            unDvd.Pais = "GB"; // El país tiene que ser uno de la tabla paises

                            Console.Write("\nCódigo: ");
                            unDvd.Codigo = int.Parse(Console.ReadLine());
                            Console.Write("\nArtista: ");
                            unDvd.Artista = Console.ReadLine();                                                                                     
                            Console.Write("\nCompania: ");
                            unDvd.Compania = Console.ReadLine();                            
                            Console.Write("\nAnio: ");
                            unDvd.Anio = Console.ReadLine();
                            Console.Write("\nTitulo: ");
                            unDvd.Titulo = Console.ReadLine();
                            Console.Write("\nPrecio: ");
                            unDvd.Precio = double.Parse(Console.ReadLine());

                            Console.WriteLine("\n" + dao.Insertar(unDvd) + " filas afectadas");
                            Console.ReadLine();
                            break;

                        default:
                            correcto = true;
                            return;
                    }
        

                }

                catch (Exception e)
                {
                    correcto = false;
                    Console.WriteLine("Ha ocurrido un error: " +  e.Message);
                    Console.ReadLine();
                }

            } while (correcto); // Mientras sea correcto se sigue repitiendo. Si no, saldrá del bucle.

        }

        private static void MostrarListado(List<Dvd> listado)
        {
            if (dao.Conectado())
                foreach (Dvd unDvd in listado)
                {
                    Console.WriteLine(unDvd.ToString());
                }
            else
                Console.WriteLine("No hay conexión válida");
        }

    }
}
