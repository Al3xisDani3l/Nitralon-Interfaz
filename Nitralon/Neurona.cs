using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nitralon
{  
  ///<summary>
  ///Representa la unidad minima de procesamiento neural, emula su analogo biologico.
  /// </summary>  
  [Serializable]
  public class Neurona
  {
        #region Variables
        /// <summary>
        /// Matriz de pesos para cada neurona creada.
        /// </summary>
        public double[] pesos;
        /// <summary>
        /// Variable que guarda la activacion X sub -1.
        /// </summary>
        public double activacionPasada;
        /// <summary>
        /// variable que agrega un peso constante unico para cada neurona creada en cada paso de su renovacion.
        /// </summary>
        public double bia;
        #endregion

        /// <summary>
        /// Constructor del objeto neurona, en el se le asigna un numero aleatorio a las bias y se definen sus entradas.
        /// </summary>
        /// <param name="numeroDeEntradas">Cantidad de neuronas de la capa subindice - 1</param>
        /// <param name="Aleatorizacion">Semilla de aleatorizacion unica para cada proceso de entrenamiento.</param>
        public Neurona(int numeroDeEntradas, Random Aleatorizacion)
        {
            bia = 10 * Aleatorizacion.NextDouble() - 5; // Asignacion de un numero aleatorio a la bia
            pesos = new double[numeroDeEntradas]; // Se define el tamaño de la matriz de pesos en base a las entradas.
            for (int i = 0; i < numeroDeEntradas; i++)
            {
                pesos[i] = 10 * Aleatorizacion.NextDouble() - 5; // Se asigna a cada peso un numero aleatorio.
            }
        }

        /// <summary>
        /// Suma los pesos de las neuronas de la capa anterior y los pasa a la funcion de activacion sigmoidea.
        /// </summary>
        /// <param name="entradas">valores de la capa anterior</param>
        /// <returns></returns>
        public double Activacion(double[] entradas)
        {
            double activation = bia; //se crea una Variable buffer y se le asigna el valor aleatorio bia.

            for (int i = 0; i < entradas.Length; i++)
            {
                activation += pesos[i] * entradas[i]; // Se multiplica cada peso con su entrada correspondiente y se suma al buffer.
            }

            activacionPasada = activation; // se guarda el estado actual para su uso posterior.
            return Sigmoidea(activation); // se pasa la sumatoria de la multiplicacion de los pesos por las entradas para hacerla pasar por la funcion de activacion.

        }
        /// <summary>
        /// Funcion de activacion que se pronuncia muncho cuando se aleja del 0, funcion sigmoidea
        /// </summary>
        /// <param name="entrada"> valor de entrada, Variable independiente </param>
        /// <returns></returns>
        public static double Sigmoidea(double entrada)
        {
            return 1 / (1 + Math.Exp(-entrada));
        }
        /// <summary>
        /// Devuelve la deriva con respecto a un punto de la sigmoidea.
        /// </summary>
        /// <param name="entrada">Valor de entrada</param>
        /// <returns></returns>
        public static double DerivadaSigmoidea(double entrada)
        {
            double y = Sigmoidea(entrada);
            return y * (1 - y);
        }




    }
}
