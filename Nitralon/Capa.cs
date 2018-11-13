using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nitralon
{   [Serializable] // Atributo necesario para guardar el estado de la capa.
  public  class Capa
    {
        #region Variables
        /// <summary>
        /// Lista de neuronas de la capa en cuestion.
        /// </summary>
        public List<Neurona> neuronas;
        /// <summary>
        /// Cantidad de neuronas en esta capa.
        /// </summary>
        public int numeroDeNeuronas;
        /// <summary>
        /// Matriz de resultados de esta capa y  de cada neurona.
        /// </summary>
        public double[] Salidas;
        #endregion

        #region Funciones
        /// <summary>
        /// Objeto encargado de adminsitrar las entradas y salidas de las n-neruonas.
        /// </summary>
        /// <param name="_numeroDeNeuronas">Neuronas que se crearan en esta capa</param>
        /// <param name="numeroDeEntradas">Entradas que se define con las salidas de la capa anterior.</param>
        /// <param name="aleatorios">Semilla de aleatorizacion unica para cada proceso.</param>
        public Capa(int _numeroDeNeuronas, int numeroDeEntradas, Random aleatorizacion)
        {

            numeroDeNeuronas = _numeroDeNeuronas;

            neuronas = new List<Neurona>(); // Instansacion de la lista de las neuronas.

            for (int i = 0; i < numeroDeNeuronas; i++) // Se realisa el conteo de las neuronas.
            {
                neuronas.Add(new Neurona(numeroDeEntradas, aleatorizacion)); // Se instancia cada neurona y se le agraga la cantidad de entradas que tendra cada una de ellas, ademas de agragar la semilla.

            }


        }
        /// <summary>
        /// Devuelve una matriz con las salidas producidas por cada neurona.
        /// </summary>
        /// <param name="Entradas">Las entradas son las salidas de la capa anterior</param>
        /// <returns></returns>
        public double[] Activacion(double[] entradas)
        {

            List<double> salidasBuffer = new List<double>(); // Se instancia una lista de salidas donde se aguardaran los resultados.
            for (int i = 0; i < numeroDeNeuronas; i++)
            {
                
                salidasBuffer.Add(neuronas[i].Activacion(entradas)); // Se usa la funcion de activacion de cada neurona y despues se aguarda en el buffer.
            }

            Salidas = salidasBuffer.ToArray(); // Convertimos la lista en matriz y la guardamos en la variable global "salidas"

            return salidasBuffer.ToArray(); // Retornamos la matriz de resultados.

            
        }
        #endregion
    }
}
        