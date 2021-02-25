//Interpreto que se trabaja con los datos de empleados aunque la tabla se llama Cliente
//Se supone que los dos ultimos numeros de una altura son sus cifras decimales

using System.IO;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Data;
using Nancy.Json;

namespace HelloWorld
{
    class Program
    {
        static List<clsEmpleado> createListFromPath(String dir)
        {
            String line;

            //Creo la lista de empleados 
            List<clsEmpleado> listaEmpleados = new List<clsEmpleado>();
            try
            {
                //Obtengo el archivo mediante su Path y obtengo el primer empleado
                StreamReader sr = new StreamReader(dir);
                line = sr.ReadLine();



                //parceo los datos y los agrego a la lista de empleados
                while (line != null)
                {
                    string[] persona = line.Split(", ");
                    string nombre = persona[0];
                    string dni = persona[1];
                    decimal altura = decimal.Parse(persona[2]);
                    bool estado = persona[3] == "ACTIVO" ? true : false;

                    listaEmpleados.Add(new clsEmpleado(nombre, dni, altura, estado));
                    line = sr.ReadLine();
                }

            }

            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }
            return listaEmpleados;

        }

        static void pushList(List<clsEmpleado> listEmpleados)
        {
            //Creo y abro la coneccion
            SqlConnection myConect = new SqlConnection("server=DESKTOP-V2RAHDL ; database=PantoC ; integrated security = true");
            myConect.Open();


            //recorro la lista de empleados agregandolos a la DB
            foreach (clsEmpleado empleado in listEmpleados)
            {
                string query = string.Format("Insert into Cliente (DNI,Nombre,Altura,Estado) VALUES ('{0}','{1}','{2}','{3}')",
                                                                                                                                empleado.DNI,
                                                                                                                                empleado.Nombre,
                                                                                                                                empleado.Altura,
                                                                                                                                empleado.Estado);
                SqlCommand comando = new SqlCommand(query, myConect);
                comando.ExecuteNonQuery();

            }
            //cierro la coneccion
            myConect.Close();
        }

        private static clsEmpleado addToClsEmpleado(IDataRecord record)
        {
            string dni = record.GetString(0);
            string nombre = record.GetString(1);
            decimal altura = record.GetDecimal(2);
            bool estado = record.GetBoolean(3);

            clsEmpleado empleado = new clsEmpleado(nombre, dni, altura, estado);
            return empleado;
        }

        static List<clsEmpleado> filtrarEstadoYAltura(decimal altura, bool estado)
        {
            
            //Creo y abro la coneccion
            SqlConnection myConect = new SqlConnection("server=DESKTOP-V2RAHDL ; database=PantoC ; integrated security = true");
            myConect.Open();


            //Traigo la tabla y cierro la coneccion
            SqlCommand comando = new SqlCommand("Select * from Cliente", myConect);
            comando.ExecuteNonQuery();

            SqlDataReader data = comando.ExecuteReader();
            List<clsEmpleado> listFiltred = new List<clsEmpleado>();

            //transformo los datos que traje de la tabla a la lista de empleados
            while (data.Read())
                listFiltred.Add(addToClsEmpleado((IDataRecord)data));

            data.Close();
            myConect.Close();

            //Filtro 
            listFiltred = listFiltred.Where(empleado => empleado.Estado == true
                                                             &&
                                                 empleado.Altura > 756).ToList();

            return listFiltred;
            
        }

        static void convertJSONAndPrint(List<clsEmpleado> listEmpleados)
        {
            //Convierto a JSON la lista que recibo y la imprimo.
            var jsonStringName = new JavaScriptSerializer();
            var jsonStringResult = jsonStringName.Serialize(listEmpleados);
            Console.WriteLine(jsonStringResult);
        }


        static void Main(string[] args)
        {
            //Absorvo el txt y agrego los empleados a la lista
            String dir = "C:\\Users\\ale_4\\Downloads\\TestDaiana.txt";
            List<clsEmpleado> listEmpleados = createListFromPath(dir);


            //Ingreso los empleados de la lista a la tabla
            pushList(listEmpleados);

            //Filtro los empleados con altuma mayor a 765 y estado true
            List<clsEmpleado> listFiltred = filtrarEstadoYAltura(765, true);

            //Imprimo en JSON los datos filtrados
            convertJSONAndPrint(listFiltred);

            Console.ReadLine();
        }

    }
}
