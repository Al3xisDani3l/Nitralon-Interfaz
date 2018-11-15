using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using LiveCharts.Charts;
using LiveCharts.Configurations;
using System.ComponentModel;
using LiveCharts.Defaults;
using LiveCharts;

namespace Nitralon.Graficos
{
    /// <summary>
    /// Lógica de interacción para GraficoCalidadLow.xaml
    /// </summary>
    [Serializable]
    public partial class GraficoCalidadLow : UserControl, INotifyPropertyChanged
    {
        double _MaxValueX = 10;
        double _MaxValuey = 1;

        #region BindingPropeties

        public ChartValues<ObservablePoint> Values { get; set; }

        public double MaxValueY
        {
            get
            {
                return _MaxValuey;
            }
            set
            {
                _MaxValuey = value;
                OnPropertyChanged("MaxValueY");
            }
        }


        public double MaxValueX
        {
            get { return _MaxValueX; }
            set
            {
                _MaxValueX = value;
                OnPropertyChanged("MaxValueX");
            }
        }

   

        #endregion




        public GraficoCalidadLow()
        {
            InitializeComponent();
            var mapper = Mappers.Xy<ObservablePoint>().X(punto => (punto.X)).Y(punto => punto.Y);

            Charting.For<ObservablePoint>(mapper);

            Values = new ChartValues<ObservablePoint>();
            DataContext = this;


        }


        public void Read(double x, double y)
        {
            MaxValueX = x;

            double buffery = 1;

            if (y > buffery)
            {
                buffery = y;
                MaxValueY = buffery;
            }
           
            Values.Add(new ObservablePoint(x, y));
        }

        public void Reset()
        {
            MaxValueY = 1;
            MaxValueX = 1;
            Values.Clear();
         
        }



        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged( string Property = null)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(Property));
            }
        }
    }
}
