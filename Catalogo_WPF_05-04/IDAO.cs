using System.Collections.ObjectModel;
using System.Data;
using System.Collections.Generic;

namespace CatalogoDVD_consola
{
    interface IDAO // DAO -> Data Access Object. Un DAO suele poder implementar el CRUD (Create, Read = select, Update, Delete).
    {
        bool Conectar(string srv, string port, string db, string user, string pwd);
        void Desconectar();
        bool Conectado();

        // Select
        List<Dvd> SeleccionarPA(string codigo, out int resultado); // Seleccionar mediante un PA (Procedimiento almacenado). En SQLlite no se pueden crear PA y ahí fallará
        DataTable SeleccionarTB(string codigo); // Seleccionar mediante una TABLA (TB)
        List<Dvd> Seleccionar(string codigo); // Select a la base de datos que devuelve una colección
        Pais SeleccionarPais(string iso2);

        // Delete
        int Borrar(string codigo);


        // Update
        int Actualizar(Dvd unDVD);

        // Insert
        int Insertar(Dvd unDVD);
    }
}
