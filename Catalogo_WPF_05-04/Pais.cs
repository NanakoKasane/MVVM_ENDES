using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CatalogoDVD_consola
{
    public class Pais
    {
        private string _nombre;

        public string Nombre
        {
            get { return _nombre; }
            set { _nombre = value; }
        }
        private string _iso2;

        public string Iso2
        {
            get { return _iso2; }
            set { _iso2 = value; }
        }
    }
}
