using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nitralon
{
    [Serializable]
    class Percepcion
    {
        #region Variables
      private List<Capa> capas;// lista de capas
      private List<double[]> sigmas;// lista de matrices doubles de sigmas.
      private List<double[,]> deltas;// lista de matrices bidimencionales de deltas.
        public double Error { get; private set; } = 99999;



        #endregion

        /// <summary>
        /// Controla la interaccion, inicializacion y entrenamiento de las neuronas.
        /// </summary>
        /// <param name="entradas">Cantidad de entradas que dispondra el perceptron</param>
        /// <param name="neuronasPorCapa">Cantidad de neuronas que tendra cada capa, no es necesario agregar el tamaño de las capas, ya que se puede obtener de la matriz</param>
        /// <param name="salida">Cantidad de salidas que contendra el perceptron</param>
        public Percepcion(int entradas, int[] neuronasPorCapa, int salida)
        {
            capas = new List<Capa>();// Instanciamos una lista de capas.
            Random aleatorizacion = new Random();// instanciamos un objeto de aleatorizacion.
            for (int i = 0; i < neuronasPorCapa.Length; i++)//iteramos las neuronas por capa
            {
                capas.Add(new Capa(i == 0 ? entradas : neuronasPorCapa[i],  neuronasPorCapa.Length - 1 == i ? salida : neuronasPorCapa[i], aleatorizacion));
            }
        }
        /// <summary>
        /// Desencadena el prceso de aprendisaje de las neuronas
        /// </summary>
        /// <param name="entradas"></param>
        /// <param name="salidasDeseadas"></param>
        /// <param name="factorPasos"></param>
        public void RetroPropagacion(List<double[]> entradas, List<double[]> salidasDeseadas, double factorPasos)
        {
            EstablecerDeltas();
            for (int i = 0; i < entradas.Count; i++)//iter las entradas
            {
                Activacion(entradas[i]);// activamos las neuronas con los datos de entrada
                AgregarSigmas(salidasDeseadas[i]);// agregamos los sigmas.
                ActualizarBias(factorPasos);
                AgregarDeltas();

            }
            ActualizarPesos(factorPasos);// despues de aplicar la activacion actualizamos los pesos para intentar acercanos al minimo global.

        }
        /// <summary>
        /// Inicia el entrenamiento de la red.
        /// </summary>
        /// <param name="entradas">Entradas del archivo .csv</param>
        /// <param name="salidasDeseadas">Salidas del archivo .csv</param>
        /// <param name="intercciones">Cantidad de ciclos hasta que aprenda</param>
        /// <param name="errorAceptable">Porcentaje de error hasta que se considere que la red aprendio</param>
        /// <param name="factorPasos"> factor que establece que en que tanto se debe avanzar hasta llegar al minimo local.</param>
        /// <returns></returns>
        public bool Entrenamiento(List<double[]> entradas, List<double[]> salidasDeseadas, double intercciones, double errorAceptable, double factorPasos)
        {
          // iniciamos el error con un numero muy grande para que no haiga manera de que se saltee el entrenamiento.
            while (Error > errorAceptable)
            {
                intercciones--;
                if (intercciones <= 0)
                {
                    return false;
                }
                RetroPropagacion(entradas, salidasDeseadas, factorPasos);
                Error = ErrorGeneral(entradas, salidasDeseadas);


            }
            return true;
        }
        #region Funciones internas
        /// <summary>
        /// Obtiene una matriz con la activacion de cada capa.
        /// </summary>
        /// <param name="entradas">Entradas</param>
        /// <returns></returns>
        public double[] Activacion(double[] entradas)
        {
            double[] salidas = new double[0];// inicializamos una matriz, no importa el tamño;
            for (int i = 0; i < capas.Count; i++)// iteramos las capas.
            {
                salidas = capas[i].Activacion(entradas); // llamamos a la funcion de activacion de la capa y la guardamos en la salida.
                entradas = salidas;// entradas lo igualamos a la salida para poder reutilizar el proceso con n-1.
            }
            return salidas;// retornamos el buffer de las salidas.
        }
        /// <summary>
        /// Obtiene la desviacion de los datos reales con respecto a los esperados.
        /// </summary>
        /// <param name="salidaReal">salida esperada de cada capa</param>
        /// <param name="salidaDeseada">Datos de entrenamiento</param>
        /// <returns></returns>
        private double ErrorIndividual(double[] salidaReal, double[] salidaDeseada)
        {
            double errorBuffer = 0;// iniciamos el buffer de los datos
            for (int i = 0; i < salidaReal.Length; i++)// iteramos cada salida deseada con la salida real.
            {
                errorBuffer += Math.Pow(salidaReal[i] - salidaDeseada[i], 2); //se calcula Raiz(salidaReal - SalidaDeseada) y se suma al buffer
            }
            return errorBuffer;// retornamos el resultado
        }
        /// <summary>
        /// Obtiene el error general del perceptron, evalua las salidas deseadas  con respecto a la activacion de las entradas.
        /// </summary>
        /// <param name="entradas">Entradas del archivo de entrenamiento</param>
        /// <param name="salidaDeseada">salidas del archivo de entrenamiento</param>
        /// <returns></returns>
        public double ErrorGeneral(List<double[]> entradas, List<double[]> salidaDeseada)
        {
            double errorBuffer = 0;// declaramos el buffer.
            for (int i = 0; i < entradas.Count; i++)//iteramos las entradas
            {
                errorBuffer += ErrorIndividual(Activacion(entradas[i]), salidaDeseada[i]); //calculamos el error por capa y los sumamos al buffer.
            }

            return errorBuffer;// retornamos el buffer.
        }
        /// <summary>
        /// rellena una lista con matrices de sigmas por neurona.
        /// </summary>
        /// <param name="salidasDeseadas">Salidas del archivo de entrenamiento</param>
        private void AgregarSigmas(double[] salidasDeseadas)
        {
            sigmas = new List<double[]>();// instanciamos la lista sigma.

            for (int i = 0; i < capas.Count; i++)//iteramos las capas.
            {
                sigmas.Add(new double[capas[i].numeroDeNeuronas]);// instanciamos nuevas matrices de dobles con el tamaño igual a la cantidad de neuronas de la capa en cuestion.
            }

            for (int i = capas.Count - 1; i >= 0; i--) // iteramos desde la ultima capa y nos detenemos en la primera.
            {
                for (int j = 0; j < capas[i].numeroDeNeuronas; j++)// iteramos todas las neuronas de la capa en cuestion.        
                {
                    if (i == capas.Count - 1) // si nos encontramos en la primera capa.
                    {
                        double y = capas[i].neuronas[j].activacionPasada;// obtenemos la activacion pasada de la neurona y en la capa en cuestion.
                        sigmas[i][j] = (Neurona.Sigmoidea(y) - salidasDeseadas[j]) * Neurona.DerivadaSigmoidea(y);// guardamos en la posision ij el resultado (sigmoidea(x) - SalidaDeseada) * DerivadaSigmoidea(x)

                    }
                    else
                    {
                        double buffer = 0;// inicializamos un buffer

                        for (int k = 0; k < capas[i + 1].numeroDeNeuronas; k++)// iteramos las capas iniciando desde i + 1.
                        {
                            buffer += capas[i + 1].neuronas[k].pesos[j] * sigmas[i + 1][k];// sumamos al buffer los pesos de la neurona en cuestion.
                        }
                        sigmas[i][j] = Neurona.DerivadaSigmoidea(capas[i].neuronas[j].activacionPasada) * buffer;// aplicamos la derivada de la sigmoidea a la activacion pasada de la neurona en cuestion y la multiplicamos por el buffer.
                    }
                }
            }

        }
        /// <summary>
        /// Establece los deltas
        /// </summary>
        private void EstablecerDeltas()
        {
            deltas = new List<double[,]>();
            for (int i = 0; i < capas.Count; i++)
            {
                deltas.Add(new double[capas[i].numeroDeNeuronas, capas[i].neuronas[0].pesos.Length]);
            }
        }
        /// <summary>
        /// Agrega los deltas
        /// </summary>
        private void AgregarDeltas()
        {
            for (int i = 0; i < capas.Count; i++)//itera las capas
            {
                for (int k = 0; k < capas[i].numeroDeNeuronas; k++)//itera las neuronas
                {
                    for (int j = 0; j < capas[i].neuronas[j].pesos.Length; j++)//itera los pesos
                    {
                        deltas[i][j, k] += sigmas[i][j] * Neurona.Sigmoidea(capas[i - 1].neuronas[k].activacionPasada);
                    }
                }
            }
        }
        /// <summary>
        /// Actualiza las bias en un factor "Alfa"
        /// </summary>
        /// <param name="factorPasos">Factor x mayor que cero y menor que uno. </param>
        private void ActualizarBias(double factorPasos)
        {
            for (int i = 0; i < capas.Count; i++)// itera las capas
            {
                for (int k = 0; k < capas[i].numeroDeNeuronas; k++)//itera las neuronas
                {
                    capas[i].neuronas[k].bia -= factorPasos * sigmas[i][k]; // resta los sigmas en factor de peso a las bias para acercarnos al minimo global.
                }
            }
        }
        /// <summary>
        /// Actualiza los pesos.
        /// </summary>
        /// <param name="factorPasos">Factor 0 < x <= 1</param>
        private void ActualizarPesos(double factorPasos)
        {
            for (int i  = 0; i  < capas.Count; i ++)//itera las capas
            {
                for (int j = 0; j < capas[i].numeroDeNeuronas; j++)//itera las neuronas
                {
                    for (int k = 0; k < capas[i].neuronas[j].pesos.Length; k ++)// itera los pesos
                    {
                        capas[i].neuronas[j].pesos[k] -= factorPasos * deltas[i][j, k];// resta a los pesos en un factor paso a delta.
                    }
                }
            }
        }
        #endregion
    }
}
