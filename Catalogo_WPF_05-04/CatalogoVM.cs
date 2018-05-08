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

            NotificarCambioDePropiedad("ColorConectar");
            NotificarCambioDePropiedad("Conectado");
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
