using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//-------------------------
using System.ComponentModel;
using CatalogoDVD_consola;

using System.Windows.Input; // PARA ICOMMAND

namespace catalogo2018
{
    // Realmente hay que crear una clase intermediaria por cada tabla. Habría que crear una -> DvdVM y PaisVM
    class CatalogoVM : INotifyPropertyChanged
    {
        #region Campos
        IDAO _dao;
        bool _tipoConexion = true; // MySQL -> true, SQlite -> false
        string _mensaje = "<Sin datos>";

        List<Dvd> _listado;
        Dvd _unDvd;

        #endregion 


        #region Propiedades
        public bool TipoConexion
        {
            get { return _tipoConexion; }
            set {
                if (_tipoConexion != value)
                {
                    _tipoConexion = value;
                    NotificarCambioDePropiedad("TipoConexion");
                }
            }
        }

        public string Mensaje
        {
            get { return _mensaje; }
            set {
                    if (_mensaje != value)
                    {
                        _mensaje = value;
                        NotificarCambioDePropiedad("Mensaje");
                    }
                }
        }

        // Propiedad de solo lectura
        public bool Conectado
        {
            get
            {
                if (_dao == null)
                    return false;
                else
                    return _dao.Conectado();
            }
        }

        // Un color puede ser un string 
        public string ColorConectar
        {
            get 
            {
                if (Conectado)
                    return "green";
                else
                    return "red";
            }
            set 
            {
                NotificarCambioDePropiedad("ColorConectar");
            }
        }

        public List<Dvd> Listado
        {
            get { return _listado; }
            set {
                    if (_listado != value)
                    {
                        _listado = value;
                        NotificarCambioDePropiedad("Listado");
                    }
                }
        }

        public Dvd DVDSeleccionado
        {
            get { return _unDvd; }
            set {
                    if (_unDvd != value)
                    {                        
                        _unDvd = value;                        

                        if (_dao.Conectado() && _unDvd != null)
                            Mensaje = _dao.SeleccionarPais(_unDvd.Pais).Nombre; // Recojo el nombre del país
                        else
                            Mensaje = "<Desconocido>";

                    }
                }
        }

        #endregion

        #region Comandos

        // COMO SI FUERA UNA PROPIEDAD (La implementación de un comando)
        public ICommand ConectarBD_Click
        {
            get
            {
                return new RelayCommand(o => ConectarBD(), o => true); // Coge un objeto que le llamo genéricamente o 
            } // Pongo canExecute a true
        }

        public ICommand DesconectarBD_Click
        {
            get
            {
                return new RelayCommand(o => DesconectarBD(), o => true);
            }
        }

        public ICommand ListarTodosDVD_Click
        {
            get
            {
                return new RelayCommand(o => ListarTodosDVD(), o => true);
            }
        }


        // Todo método para el comando obligatoriamente devuelve VOID y NO lleva parámetros:
        // Todo ICommand va a asociado a un método de la VM 
        private void ConectarBD()
        {
            _dao = null;

            try
            {
                if (_tipoConexion) // Es decir, si es MySql
                {
                    _dao = new DAO_MySQL();
                    _dao.Conectar("localhost", "3306", "catalogo", "usr_catalogo", "123");
                    Mensaje = "Conectado con éxito a la BD";
                }
                else // SQlite
                {

                }
            }
            catch (Exception e)
            {
                Mensaje = e.Message;
            }

            NotificarCambioDePropiedad("ColorConectar");
            NotificarCambioDePropiedad("Conectado");
            // De mensaje no hace falta noficiar el cambio porque está en su propiedad en su SET cada vez que cambia de valor. Pero sí hay que comprobar las otras 2 propiedades 
        }

        private void DesconectarBD()
        {
            _dao.Desconectar();
            Mensaje = "Desconectado de la BD";

            Listado = null; // en el set de la propiedad ya está puesta la notificación así que no es necesaria ahora
            NotificarCambioDePropiedad("ColorConectar");
            NotificarCambioDePropiedad("Conectado");
        }

        private void ListarTodosDVD()
        {
            if (TipoConexion) // MYSQL
            {
                int nFilas = 0;
                Listado = _dao.SeleccionarPA(null, out nFilas);
                Mensaje = string.Format("Filas encontradas: {0}", nFilas);
            }

            else
            {

            }
        }

        #endregion 

        #region Eventos
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotificarCambioDePropiedad(string propiedad)
        {
            PropertyChangedEventHandler manejador = PropertyChanged;

            if (manejador != null)
                manejador(this, new PropertyChangedEventArgs(propiedad));
        }
        #endregion 

    }
}
