using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Nitralon
{
    /// <summary>
    /// Representa una red entrenada.
    /// </summary>
    [Serializable]
    public class Rednn
    {
        private Configuracion configuracion;
        private Percepcion percepcion;
        /// <summary>
        /// Establece o devuelve la configuracion interna en la cual se entreneo el perceptron.
        /// </summary>
        public Configuracion Configuracion { get => configuracion; set => configuracion = value; }
        /// <summary>
        /// Establece o devuelve un perceptron entrenado.
        /// </summary>
        public Percepcion Percepcion { get => percepcion; set => percepcion = value; }
    }
}
