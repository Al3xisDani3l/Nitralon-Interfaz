using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Win32;
using System.IO;
using System.Data;
using System.Linq.Expressions;

namespace Nitralon
{
   
    public class ProcesadorCSV
    {
        #region variables
        private double valorMaxE = -99999;
        private double valorMinE = 999999;
        private double valorMaxS = -99999;
        private double valorMinS = 99999;
        public DataTable DataTable
        {
            get
            {
                return buffer(Entradas, Salidas);
            }
        }

        public int ConteoDeEntradas { get => conteoDeEntradas; set => conteoDeEntradas = value; }
        public int ConteoDeSalidas { get => conteoDeSalidas;  set => conteoDeSalidas = value; }
        public List<double[]> Entradas { get => entradas; set => entradas = value; }
        public List<double[]> Salidas { get => salidas; set => salidas = value; }
        public double ValorMaxE { get => valorMaxE; set => valorMaxE = value; }
        public double ValorMinE { get => valorMinE; set => valorMinE = value; }
        public double ValorMaxS { get => valorMaxS; set => valorMaxS = value; }
        public double ValorMinS { get => valorMinS; set => valorMinS = value; }

        private int conteoDeEntradas = 0;// entero que lleva la cuenta de entradas.
        private  int conteoDeSalidas = 0;// entero que lleva la cuenta de salidas


        /// <summary>
        /// Lista que contiene matrizes con ejemplos de entradas.
        /// </summary>
        private List<double[]> entradas = new List<double[]>();
        /// <summary>
        /// Lista que contiene matrizes con los ejemplos de salida.
        /// </summary>
        private List<double[]> salidas = new List<double[]>();

        //double[][] Entradas;
        //double[][] Salidas;
        #endregion

        /// <summary>
        /// Procesa un archivo de texto y convierte a double su contenido, separado por entradas y salidas, funcion inteligente.
        /// </summary>
        /// <param name="archivo"> objeto que contiene la ruta del archivo CSV</param>
        public ProcesadorCSV(OpenFileDialog archivo)
        {
           

           

            string _datosBuffer = File.ReadAllText(archivo.FileName).Replace("\r", "").Trim();// leemos los datos y eliminamos todos los retornos de carro y los caracteres de espacio al principio y final del la fila, despues lo aguardamos en un buffer.
            string[] filasBuffer = _datosBuffer.Split(Environment.NewLine.ToCharArray()); //convertimos el buffer en lineas conforme al entorno y despues guardamos cada linea en una matriz de cadenas.

            string[] dataBuffer = filasBuffer[0].Split(';');// convertimos solo la primera fila, para poder determinar entradas y salidas

            for (int i = 0; i < dataBuffer.Length; i++)//
            {
                if (dataBuffer[i] == "Entrada" || dataBuffer[i] == "entrada" || dataBuffer[i] == "ENTRADA")
                {
                    conteoDeEntradas += 1;// si coincide con una entrada agregamos 1 a la cuenta
                   
                    
                }
                else if (dataBuffer[i] == "Salida" || dataBuffer[i] == "salida" || dataBuffer[i] == "SALIDA")
                {
                    conteoDeSalidas += 1;// si coincide con una salida agregamos 1 a la cuenta.
                   
                }
            }


            for (int i = 1; i < filasBuffer.Length; i++)
            {
                string[] matrixBuffer = filasBuffer[i].Split(';');

                for (int j = 0; j < conteoDeEntradas + conteoDeSalidas; j++)
                {
                    if (j < conteoDeEntradas)
                    {
                        if (double.Parse(matrixBuffer[j]) > ValorMaxE)
                        {
                            ValorMaxE = double.Parse(matrixBuffer[j]);
                        }
                        else if (double.Parse(matrixBuffer[j]) < ValorMinE)
                        {
                            ValorMinE = double.Parse(matrixBuffer[j]);
                        }
                    }
                    else
                    {
                        if (double.Parse(matrixBuffer[j]) > ValorMaxS)
                        {
                            ValorMaxS = double.Parse(matrixBuffer[j]);
                            if (double.Parse(matrixBuffer[j]) < ValorMinS)
                            {
                                ValorMinS = double.Parse(matrixBuffer[j]);
                            }
                        }
                     
                    }
                }

            }


            for (int i = 1; i < filasBuffer.Length; i++)// iteneramos empezando de la fila 2 para solo leer los datos y no las descripciones.
            {
                string[] matrixBuffer = filasBuffer[i].Split(';'); // separamos cada fila en en columnas por medio del caracter ';'.

                double[] entradas = new double[conteoDeEntradas];// Buffer de entradas, se crea en cada iteracion de i para borrar su contenido.
                double[] salidas = new double[conteoDeSalidas];//Buffer de salidas, se crea en cada iteracion de i para borrar su contenido.

                for (int j = 0; j < matrixBuffer.Length; j++)//Iteneramos cada fila y determinamos las salidas y las entradas.
                {
                    if (j < conteoDeEntradas)// Si j es menor que entradas, entonces es una entrada
                    {
                        entradas[j] = Normalize(double.Parse(matrixBuffer[j]), ValorMinE, ValorMaxE); // Normalizamos el numero
                    }
                    else //si j mayor que _entradas entonces es una salida.
                    {
                        salidas[j - conteoDeEntradas] = Normalize(double.Parse(matrixBuffer[j]), ValorMinS, ValorMaxS); //Normalizamos el numero
                    }
                }

                Entradas.Add(entradas);//agregamos la matriz de entradas-n a la lista
                Salidas.Add(salidas);//agregamos la matriz de salidas-n a la lista
            }

          




        }

        /// <summary>
        /// Procesa un archivo de texto y convierte a double su contenido, separado por Entradas y salidas, funcion semi-inteligente.
        /// </summary>
        /// <param name="_dialogoDeArchivo">Objeto que obtiene la ruta del archivo</param>
        /// <param name="salidaMinima">valor minimo contenido en el archivo</param>
        /// <param name="salidaMaxima">valor maximo contenido en el archivo</param>
        public ProcesadorCSV(OpenFileDialog _dialogoDeArchivo, double salidaMinima, double salidaMaxima)
        {
          

            string _datosBuffer = File.ReadAllText(_dialogoDeArchivo.FileName).Replace("\r", "").Trim();// leemos los datos y eliminamos todos los retornos de carro y los caracteres de espacio al principio y final del la fila, despues lo aguardamos en un buffer.
            string[] filasBuffer = _datosBuffer.Split(Environment.NewLine.ToCharArray()); //convertimos el buffer en lineas conforme al entorno y despues guardamos cada linea en una matriz de cadenas.

            string[] dataBuffer = filasBuffer[0].Split(';');// convertimos solo la primera fila, para poder determinar entradas y salidas

            for (int i = 0; i < dataBuffer.Length; i++)//
            {
                if (dataBuffer[i] == "Entrada" || dataBuffer[i] == "entrada" || dataBuffer[i] == "ENTRADA")
                {
                    conteoDeEntradas += 1;// si coincide con una entrada agregamos 1 a la cuenta
                }
                else if (dataBuffer[i] == "Salida" || dataBuffer[i] == "salida" || dataBuffer[i] == "SALIDA")
                {
                    conteoDeSalidas += 1;// si coincide con una salida agregamos 1 a la cuenta.
                }
            }

            for (int i = 1; i < filasBuffer.Length; i++)// iteneramos empezando de la fila 2 para solo leer los datos y no las descripciones.
            {
                string[] matrixBuffer = filasBuffer[i].Split(';'); // separamos cada fila en en columnas por medio del caracter ';'.

                double[] entradas = new double[conteoDeEntradas];// Buffer de entradas, se crea en cada iteracion de i para borrar su contenido.
                double[] salidas = new double[conteoDeSalidas];//Buffer de salidas, se crea en cada iteracion de i para borrar su contenido.

                for (int j = 0; j < matrixBuffer.Length; j++)//Iteneramos cada fila y determinamos las salidas y las entradas.
                {
                    if (j < conteoDeEntradas)// Si j es menor que entradas, entonces es una entrada
                    {
                        entradas[j] = double.Parse(matrixBuffer[j]); // Normalizamos el numero
                    }
                    else //si j mayor que _entradas entonces es una salida.
                    {
                        salidas[j - conteoDeEntradas] = Normalize(double.Parse(matrixBuffer[j]), salidaMinima, salidaMaxima); //Normalizamos el numero
                    }
                }

                Entradas.Add(entradas);//agregamos la matriz de entradas-n a la lista
                Salidas.Add(salidas);//agregamos la matriz de salidas-n a la lista

            }
        }

        /// <summary>
        /// Procesa un archivo de texto delimitado por comas y convierte a double su contenido, separado por salidas y entradas, funcion manual.
        /// </summary>
        /// <param name="dialogoDeArchivo">Objeto que obtiene la ruta del archivo</param>
        /// <param name="_entradas">Cantidad de entradas que tiene el archivo</param>
        /// <param name="_salidas">Cantidad de salidas que tiene el archivo</param>
        /// <param name="_salidaMinima">Valor minimo contenido en el archivo</param>
        /// <param name="_salidaMaxima">Valor maximo Contenido en el archivo</param>
        public ProcesadorCSV(OpenFileDialog dialogoDeArchivo, int _entradas, int _salidas, double _salidaMinima, double _salidaMaxima)
        {
            conteoDeEntradas = _entradas;
            conteoDeSalidas = _salidas;

            string datosBuffer = File.ReadAllText(dialogoDeArchivo.FileName).Replace("\r", "").Trim();// leemos los datos y eliminamos todos los retornos de carro y los caracteres de espacio al principio y final del la fila, despues lo aguardamos en un buffer.

            string[] filaBuffer = datosBuffer.Split(Environment.NewLine.ToCharArray()); //convertimos el buffer en lineas conforme al entorno y despues guardamos cada linea en una matriz de cadenas.

            for (int i = 1; i < filaBuffer.Length; i++)// iteneramos empezando de la fila 2 para solo leer los datos y no las descripciones.
            {
                string[] matrixBuffer = filaBuffer[i].Split(';'); // separamos cada fila en en columnas por medio del caracter ';'.

                double[] entradas = new double[_entradas];// Buffer de entradas, se crea en cada iteracion de i para borrar su contenido.
                double[] salidas = new double[_salidas];//Buffer de salidas, se crea en cada iteracion de i para borrar su contenido.

                for (int j = 0; j < matrixBuffer.Length; j++)//Iteneramos cada fila y determinamos las salidas y las entradas.
                {
                    if (j < _entradas)// Si j es menor que entradas, entonces es una entrada
                    {
                        entradas[j] = double.Parse(matrixBuffer[j]); // Normalizamos el numero
                    }
                    else //si j mayor que _entradas entonces es una salida.
                    {
                        salidas[j - _entradas] = Normalize(double.Parse(matrixBuffer[j]), _salidaMinima, _salidaMaxima); //Normalizamos el numero
                    }
                }

                Entradas.Add(entradas);//agregamos la matriz de entradas-n a la lista
                Salidas.Add(salidas);//agregamos la matriz de salidas-n a la lista
            }

            #region Codigo de prueba
            //Entradas = new double[_Entradas.Count][];

            //Salidas = new double[_Salidas.Count][];

            //   for (int i = 0; i < Entradas.Length; i++)
            //   {
            //       Entradas[i] = new double[_entradas];


            //   }
            //   for (int i = 0; i < Salidas.Length; i++)
            //   {
            //       Salidas[i] = new double[_salidas];
            //   }
            //   for (int i = 0; i < _Entradas.Count; i++)
            //   {
            //       for (int j = 0; j < _entradas; j++)
            //       {
            //           Entradas[i][j] = _Entradas[i][j];
            //       }
            //   }
            #endregion


        }

        private DataTable buffer (List<double[]> entradas, List<double[]> salidas)
        {
            DataTable buff = new DataTable("Datos");

            for (int i = 0; i < entradas[0].Length; i++)
            {
                buff.Columns.Add("Entrada " + (i + 1), typeof(double));
            }
            if (salidas[0].Length > 1)
            {
                for (int i = 0; i < salidas[0].Length; i++)
                {
                    buff.Columns.Add("Salida " + (i + 1), typeof(double));
                }
            }
            else
            {
                buff.Columns.Add("Salida", typeof(double));
            }
            

            for (int i = 0; i < entradas.Count; i++)
            {
                object[] buffer = new object[entradas[i].Length + salidas[i].Length];
                for (int j = 0; j < entradas[i].Length + salidas[i].Length; j++)
                {
                    if (j < entradas[i].Length)
                    {
                        buffer[j] = entradas[i][j];
                    }
                    else
                    {
                        buffer[j] = salidas[i][j - entradas[i].Length];
                    }
                }

                buff.Rows.Add(buffer);

            }
           
            return buff;




        }
      

        static double Normalize(double valor, double min, double maximo)
        {
            return (valor - min) / (maximo - min);

        }
        static double InverseNormalize(double valor, double min, double maximo)
        {
            return valor * (maximo - min) + min;
        }







    }
}
