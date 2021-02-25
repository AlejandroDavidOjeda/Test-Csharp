using System;
using System.Collections.Generic;
using System.Text;

namespace HelloWorld
{
    public class clsEmpleado
    {
        public clsEmpleado(string nombre, string dni, decimal altura, bool estado)
        {
            Nombre = nombre;
            DNI = dni;
            Altura = altura;
            Estado = estado;
        }
        public clsEmpleado(System.Data.IDataRecord dataRecord)
        {

        }
        public string Nombre;
        public string DNI;
        public decimal Altura;
        public bool Estado;
    }
}
