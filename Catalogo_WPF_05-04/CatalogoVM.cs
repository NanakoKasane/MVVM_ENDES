using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//-------------------------
using System.ComponentModel;

namespace catalogo2018
{
    // Realmente hay que crear una clase intermediaria por cada tabla. Habría que crear una -> DvdVM y PaisVM
    class CatalogoVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotificarCambioDePropiedad(string propiedad)
        {
            PropertyChangedEventHandler manejador = PropertyChanged;

            if (manejador != null)
                manejador(this, new PropertyChangedEventArgs(propiedad));
        }
    }
}
