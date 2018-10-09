using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;


namespace Nitralon
{
    /// <summary>
    /// Define los comportamientos de lectura que recibira el archivo CSV
    /// </summary>
    public enum Modo
    {
        Automatico,
        Semiautomatico,
        manual
    }


    [Serializable]
    class Binary
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
    [Serializable]
    public class Configuracion
    {

        private Modo modo = Modo.Automatico;

        private int entradas = 1;
        private int salidas = 1;
        private double valorMaxSalida = 1;
        private double valorMinSalida = 0;
        private double valorMaxEntrada = 1;
        private double valorMinEntradas = 0;
        private DataTable dataTable;
        private List<double[]> datosDeEntrada = new List<double[]>();
        private List<double[]> datosDeSalida = new List<double[]>();
        private int[] neurnasPorCapa;
        private int ciclosDeInteraccion;
        private double errorAceptable;
        private double saltosDelta;
        

        public int Entradas { get => entradas; set => entradas = value; }
        public int Salidas { get => salidas; set => salidas = value; }
        public double ValorMaxSalida { get => valorMaxSalida; set => valorMaxSalida = value; }
        public double ValorMinSalida { get => valorMinSalida; set => valorMinSalida = value; }
        public Modo ModoDeEscaneo { get => modo; set => modo = value; }
        public DataTable DataTable { get => dataTable; set => dataTable = value; }
        public List<double[]> DatosDeEntrada { get => datosDeEntrada; set => datosDeEntrada = value; }
        public List<double[]> DatosDeSalida { get => datosDeSalida; set => datosDeSalida = value; }
        public double ValorMaxEntrada { get => valorMaxEntrada; set => valorMaxEntrada = value; }
        public double ValorMinEntradas { get => valorMinEntradas; set => valorMinEntradas = value; }
        public int[] NeurnasPorCapa { get => neurnasPorCapa; set => neurnasPorCapa = value; }
        public int CiclosDeInteraccion { get => ciclosDeInteraccion; set => ciclosDeInteraccion = value; }
        public double ErrorAceptable { get => errorAceptable; set => errorAceptable = value; }
        public double SaltosDelta { get => saltosDelta; set => saltosDelta = value; }
    }





}
