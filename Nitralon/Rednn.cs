using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Nitralon
{   [Serializable]
  public class Rednn
    {
        private Configuracion configuracion;
        private Percepcion percepcion;
        
        public Configuracion Configuracion { get => configuracion; set => configuracion = value; }
        public Percepcion Percepcion { get => percepcion; set => percepcion = value; }
    }
}
