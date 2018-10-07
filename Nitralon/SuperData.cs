using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nitralon
{
    class SuperData
    {
        private Percepcion _perceptron;

        private Configuracion _Configuracion;

        public Percepcion perceptron
        {
            get
            {
                if (_perceptron != null)
                {
                    return _perceptron;
                }
                return null;
              
            }
            set
            {
             
                this._perceptron = value;
            }
        }

        public Configuracion configuracion
        {
            get
            {
                if (configuracion != null)
                {
                    return configuracion;
                }
                return new Configuracion();
            }
            set
            {
                _Configuracion = value;
            }
        }

    }
}
