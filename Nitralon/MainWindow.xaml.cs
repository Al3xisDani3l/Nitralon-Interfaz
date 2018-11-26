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
using System.Windows.Shapes;
using System.Runtime.InteropServices;
using System.Runtime;
using System.Windows.Interop;
using Microsoft.Win32;
using System.IO;
using System.Text.RegularExpressions;
using MaterialDesignThemes;
using MaterialDesignThemes.Wpf;
using System.Data;
using LiveCharts;
using LiveCharts.Configurations;
using System.ComponentModel;
using System.Threading;
using Nitralon.Graficos;

//   Proyecto final para calculo I
//   Interfaz diseñada por Melissa Jazmin Carrera Ramos
//   Algoritmo de la red neuronal diseñada por Alexis Daniel.
//   Presentacion powerpoint adjunta al proyecto, diseñada por Melissa.
//   Derecho libre de distribucion, copia y modificacion del codigo, siempre y cuando se realize una referencia a la autoria.
//   Este codigo utiliza libreria de terceros.

namespace Nitralon
{




    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IDisposable
    {

        #region Variables
        /// <summary>
        /// OpenFileDialog que contiene la direccion del archivo de entrenamiento.
        /// </summary>
        public OpenFileDialog archivo;
        /// <summary>
        /// Contiene toda la informacion respectiva de los controles del programa.
        /// </summary>
        public Configuracion configuracion;
        /// <summary>
        /// Representa la red neuronal y se encarga de modelar el aprendisaje de la misma.
        /// </summary>
       public Percepcion perceptron;
        /// <summary>
        /// Representa una grafica configurada en calidad baja para la alta cantidad de datos.
        /// </summary>
        GraficoCalidadLow graficaLow;
        /// <summary>
        /// Representa una lista de Texboxes que se generara en tiempo de ejecucion para la configuracion de las neuronas por capa.
        /// </summary>
        List<TextBox> textBoxesConfiguracion = new List<TextBox>();
        /// <summary>
        /// Representa una lista de Texboxes que se generara en tiempo de ejecucion para las entradas de prueba.
        /// </summary>
        List<TextBox> textBoxesEntradas = new List<TextBox>();
        /// <summary>
        /// Representa una lista de Texblock's que se generara en tiempo de ejecucion para mostrar los resultados de la red.
        /// </summary>
        List<TextBlock> textBlocksSalidas = new List<TextBlock>();
        /// <summary>
        /// Representa un objeto capas de procesar archivos delimitados por ";" o dependiendo del entorno de ejecucion.
        /// </summary>
        ProcesadorCSV Procesador;
        /// <summary>
        /// Representa un valor que indica si la red debe empezar o detener el entrenamiento de la red.
        /// </summary>
        bool SeEntrena = false;
        /// <summary>
        /// objeto que activara la cancelacion de los procedimientos asincronos.
        /// </summary>
        private CancellationTokenSource tokenSource = new CancellationTokenSource();
        /// <summary>
        /// Objeto que propaga la cancelacion de los procedimientos asincronos
        /// </summary>
        private CancellationToken token;

        bool disposed;
        #endregion

      
       /// <summary>
       /// Inicializa y asigna valores a los componentes de esta ventana.
       /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            this.Background = new SolidColorBrush(Color.FromArgb((byte)SliderTransparencia.Value, 0, 0, 0));
            Serializacion.Deserializar(ref configuracion);
            if (configuracion.DataTable != null)
            {
             DataGridCSV.ItemsSource = configuracion.DataTable.DefaultView;
            }
            EstablecerControles(configuracion);
           
            Boton_EmpezarEscaneo.IsEnabled = false;
            Boton_ConfirmarPerceptron.IsEnabled = false;
            Boton_Prueba.IsEnabled = false;
            Grid_Entrenamiento.IsEnabled = false;
            // GraficoDeErro.Series.Add(new LiveCharts.Wpf.LineSeries { Values = new ChartValues<double> { 9999 } });

            //contenedor.Children.Add(grafica = new GraficaDeErrorxaml() );
            contenedor.Children.Add(graficaLow = new Graficos.GraficoCalidadLow());
          
           


        }

     

        #region Datos

        #region Procedimientos de los botones



        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            this.DragMove();
        }

        private void Boton_Cerrar_Click(object sender, RoutedEventArgs e)
        {

            this.Close();
        }

        private void Boton_Maximizar_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Normal)
            {
                this.WindowState = WindowState.Maximized;
                Icon_Maximize.Kind = MaterialDesignThemes.Wpf.PackIconKind.WindowRestore;
            }
            else
            {
                this.WindowState = WindowState.Normal;
                Icon_Maximize.Kind = MaterialDesignThemes.Wpf.PackIconKind.WindowMaximize;
            }
        }

        private void Boton_Minimizar_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Fluent.ActivarTransparencia(this);

        }

        private void Boton_archivo_Click(object sender, RoutedEventArgs e)
        {
            archivo = new OpenFileDialog();
            archivo.Filter = "Archivos de Entrenamiento (*.csv)(*.txt)|*.csv;*.txt";

            try
            {
                archivo.ShowDialog();
                ComprobacionDeDatos(configuracion.ModoDeEscaneo);
                if (string.IsNullOrEmpty(archivo.FileName))
                {
                    Boton_EmpezarEscaneo.IsEnabled = false;
                }
                else
                {
                    Boton_EmpezarEscaneo.IsEnabled = true;
                }

            }
            catch (Exception a)
            {

                MessageBox.Show("Boton_archivo_Click : " + a.Message);
            }


        }

        private void txt_ValorMinimoEntrada_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (ComprobarFormato(txt_ValorMinimoEntrada, Boton_EmpezarEscaneo,false))
            {
                txt_ValorMinimoEntrada.SetValue(HintAssist.HintProperty, "Entrada Min");
                ComprobacionDeDatos(configuracion.ModoDeEscaneo);
            }
            else
            {
                txt_ValorMinimoEntrada.SetValue(HintAssist.HintProperty, "No es un numero!");
            }
        }

        private void txt_ValorMaximoEntrada_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (ComprobarFormato(txt_ValorMaximoEntrada, Boton_EmpezarEscaneo, false))
            {
                txt_ValorMaximoEntrada.SetValue(HintAssist.HintProperty, "Entrada Max");
                ComprobacionDeDatos(configuracion.ModoDeEscaneo);
            }
            else
            {
                txt_ValorMaximoEntrada.SetValue(HintAssist.HintProperty, "No es un numero!");
            }
        }

        private void RadioBoton_Inteligencia_Click(object sender, RoutedEventArgs e)
        {
            if (archivo != null)
            {
                Boton_EmpezarEscaneo.IsEnabled = true;
            }
            configuracion.ModoDeEscaneo = Modo.Automatico;
            Menus(configuracion.ModoDeEscaneo);

        }

        private void RadioBoton_Semi_Inteligente_Click(object sender, RoutedEventArgs e)
        {
            Boton_EmpezarEscaneo.IsEnabled = false;
            configuracion.ModoDeEscaneo = Modo.Semiautomatico;
            Menus(configuracion.ModoDeEscaneo);
        }

        private void RadioBoton_Manual_Click(object sender, RoutedEventArgs e)
        {
            Boton_EmpezarEscaneo.IsEnabled = false;
            configuracion.ModoDeEscaneo = Modo.manual;
            Menus(configuracion.ModoDeEscaneo);
        }

        private void Boton_EmpezarEscaneo_Click(object sender, RoutedEventArgs e)
        {
            DataGridCSV.InvalidateVisual();

            switch (configuracion.ModoDeEscaneo)
            {
                case Modo.Automatico:
                    Procesador = new ProcesadorCSV(archivo);
                    configuracion.Entradas = Procesador.ConteoDeEntradas;
                    configuracion.Salidas = Procesador.ConteoDeSalidas;
                    configuracion.ValorMaxSalida = Procesador.ValorMaxS;
                    configuracion.ValorMinSalida = Procesador.ValorMinS;
                    configuracion.ValorMinEntradas = Procesador.ValorMinE;
                    configuracion.ValorMaxEntradas = Procesador.ValorMaxE;  
                    configuracion.DataTable = Procesador.DataTable;
                    configuracion.DatosDeEntrada = Procesador.Entradas;
                    configuracion.DatosDeSalida = Procesador.Salidas;
                    txt_ValorMinimoEntrada.Text = configuracion.ValorMinEntradas.ToString();
                    txt_ValorMaximoEntrada.Text = configuracion.ValorMaxEntradas.ToString();
                    txt_ValorMaximo.Text = configuracion.ValorMaxSalida.ToString();
                    txt_ValorMinimo.Text = configuracion.ValorMinSalida.ToString();
                    txt_CantidadEntradas.Text = configuracion.Entradas.ToString();
                    txt_CantidadSalidas.Text = configuracion.Salidas.ToString();



                    break;
                case Modo.Semiautomatico:
                    Procesador = new ProcesadorCSV(archivo, Convert.ToDouble(txt_ValorMinimo.Text), Convert.ToDouble(txt_ValorMaximo.Text), Convert.ToDouble(txt_ValorMinimoEntrada.Text), Convert.ToDouble(txt_ValorMaximoEntrada.Text));
                    configuracion.Entradas = Procesador.ConteoDeEntradas;
                    configuracion.Salidas = Procesador.ConteoDeSalidas;
                    configuracion.ValorMaxSalida = Convert.ToDouble(txt_ValorMaximo.Text);
                    configuracion.ValorMaxSalida = Convert.ToDouble(txt_ValorMinimo.Text);
                    configuracion.ValorMinSalida = Convert.ToDouble(txt_ValorMinimoEntrada.Text);
                    configuracion.ValorMinEntradas = Convert.ToDouble(txt_ValorMaximoEntrada.Text);
                    configuracion.DataTable = Procesador.DataTable;
                    configuracion.DatosDeEntrada = Procesador.Entradas;
                    configuracion.DatosDeSalida = Procesador.Salidas;
                    txt_CantidadEntradas.Text = configuracion.Entradas.ToString();
                    txt_CantidadSalidas.Text = configuracion.Salidas.ToString();


                    break;
                case Modo.manual:
                    Procesador = new ProcesadorCSV(archivo,Convert.ToInt32(txt_CantidadEntradas.Text),Convert.ToInt32(txt_CantidadSalidas.Text), Convert.ToDouble(txt_ValorMinimo.Text), Convert.ToDouble(txt_ValorMaximo.Text), Convert.ToDouble(txt_ValorMinimoEntrada.Text), Convert.ToDouble(txt_ValorMaximoEntrada.Text));
                    configuracion.Entradas = Convert.ToInt32(txt_CantidadEntradas.Text);
                    configuracion.Salidas = Convert.ToInt32(txt_CantidadSalidas.Text);
                    configuracion.ValorMaxSalida = Convert.ToDouble(txt_ValorMaximo.Text);
                    configuracion.ValorMaxSalida = Convert.ToDouble(txt_ValorMinimo.Text);
                    configuracion.ValorMinSalida = Convert.ToDouble(txt_ValorMinimoEntrada.Text);
                    configuracion.ValorMinEntradas = Convert.ToDouble(txt_ValorMaximoEntrada.Text);
                    configuracion.DataTable = Procesador.DataTable;
                    configuracion.DatosDeEntrada = Procesador.Entradas;
                    configuracion.DatosDeSalida = Procesador.Salidas;
                    break;
                default:
                    break;
            }

            configuracion.DataTable = Procesador.DataTable;
            DataGridCSV.ItemsSource = Procesador.DataTable.DefaultView;

            GeneradorDeES(configuracion.Entradas, configuracion.Salidas);
           

          

          



        }

        private void txt_ValorMinimo_TextChanged(object sender, TextChangedEventArgs e)
        {

            if (ComprobarFormato(txt_ValorMinimo, Boton_EmpezarEscaneo, false))
            {
                txt_ValorMinimo.SetValue(HintAssist.HintProperty, "Salida Min");
                ComprobacionDeDatos(configuracion.ModoDeEscaneo);
            }
            else
            {
                txt_ValorMinimo.SetValue(HintAssist.HintProperty, "No es un numero!");
            }
        }

        private void txt_CantidadEntradas_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (ComprobarFormato(txt_CantidadEntradas, Boton_EmpezarEscaneo))
            {
                txt_CantidadEntradas.SetValue(HintAssist.HintProperty, "Entradas");
                ComprobacionDeDatos(configuracion.ModoDeEscaneo);
                configuracion.Entradas = Convert.ToInt32(txt_CantidadEntradas.Text);
                if (ComprobarFormato(txt_CantidadSalidas))
                {
                    configuracion.Salidas = Convert.ToInt32(txt_CantidadSalidas.Text);
                    GeneradorDeES(configuracion.Entradas, configuracion.Salidas);
                }
            }
            else
            {
               txt_CantidadEntradas.SetValue(HintAssist.HintProperty, "No es un numero!");
            }
        }

        private void txt_ValorMaximo_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (ComprobarFormato(txt_ValorMaximo, Boton_EmpezarEscaneo, false))
            {
                txt_ValorMaximo.SetValue(HintAssist.HintProperty, "Valor Maximo");
                ComprobacionDeDatos(configuracion.ModoDeEscaneo);
            }
            else
            {
                txt_ValorMaximo.SetValue(HintAssist.HintProperty, "No es un numero!");
            }
        }

        private void txt_CantidadSalidas_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (ComprobarFormato(txt_CantidadSalidas, Boton_EmpezarEscaneo))
            {
                txt_CantidadSalidas.SetValue(HintAssist.HintProperty, "Salidas");
                ComprobacionDeDatos(configuracion.ModoDeEscaneo);
                configuracion.Salidas = Convert.ToInt32(txt_CantidadSalidas.Text);
                if (ComprobarFormato(txt_CantidadEntradas))
                {
                    configuracion.Entradas = Convert.ToInt32(txt_CantidadEntradas.Text);
                    GeneradorDeES(configuracion.Entradas, configuracion.Salidas);

                }
            }
            else
            {
                txt_CantidadSalidas.SetValue(HintAssist.HintProperty, "No es un numero!");
            }
        }
    
        #endregion

        #endregion

        #region Configuracion

        private void txt_Capas_TextChanged(object sender, TextChangedEventArgs e)
        {
            Boton_ConfirmarPerceptron.IsEnabled = false;
            foreach (TextBox item in textBoxesConfiguracion)
            {
                item.TextChanged -= Text_TextChanged;
            }
          
            textBoxesConfiguracion.Clear();
            if (ComprobarFormato(txt_Capas))
            {
                WrapPanel_TextBox.Children.Clear();
                int tamano = Convert.ToInt32(txt_Capas.Text);

                if (tamano >= 50)
                {
                    tamano = 50;
                }

                configuracion.Capas = tamano;

                
                for (int i = 0; i < tamano; i++)
                {
                    GroupBox group = new GroupBox();
                    TextBox textBox = new TextBox();
                    group.Header = "Capa " + (i + 1);
                    group.Height = 200;
                    group.Width = 200;
                    group.Margin = new Thickness(25);
                    textBox.SetValue(HintAssist.IsFloatingProperty, true);
                    textBox.SetValue(HintAssist.HintProperty, "Neuronas");
                
                    //textBox.Margin = new Thickness(25, 25, 25, 25);
                    textBox.Foreground = Brushes.White;
                    textBox.BorderBrush = new SolidColorBrush(Color.FromRgb(133, 133, 133));
                    textBox.FontSize = 20;
                    textBox.Foreground = Brushes.White;
                    textBox.HorizontalAlignment = HorizontalAlignment.Center;
                    textBox.VerticalAlignment = VerticalAlignment.Center;
                    group.Content = textBox;

                  
                   


                    WrapPanel_TextBox.Children.Add(group);
                }
             
            }

            foreach (GroupBox box in WrapPanel_TextBox.Children )
            {
                textBoxesConfiguracion.Add((TextBox)box.Content);
            }
            foreach (TextBox text in textBoxesConfiguracion)
            {
                text.TextChanged += Text_TextChanged;
            }




        }

        private void Text_TextChanged(object sender, TextChangedEventArgs e)
        {
            bool all = true;

            foreach (TextBox cajas in textBoxesConfiguracion)
            {
                all = all && ComprobarFormatoColectivo(cajas);
            }

            if (all)
            {
                Boton_ConfirmarPerceptron.IsEnabled = true;
            }
            else
            {
                Boton_ConfirmarPerceptron.IsEnabled = false;
            }

         
           
        }

        private void Boton_ConfirmarPerceptron_Click(object sender, RoutedEventArgs e)
        {
            int[] bufferNeuronas = new int[textBoxesConfiguracion.Count];

            for (int i = 0; i < textBoxesConfiguracion.Count; i++)
            {
                if (Convert.ToInt32(textBoxesConfiguracion[i].Text) <= 0)
                {
                    bufferNeuronas[i] = 1;
                }
                else
                {
                    bufferNeuronas[i] = Convert.ToInt32(textBoxesConfiguracion[i].Text);
                }
               
               
            }
            configuracion.NeurnasPorCapa = bufferNeuronas;
            Grid_Entrenamiento.IsEnabled = true;
            perceptron = new Percepcion(configuracion.Entradas, configuracion.NeurnasPorCapa,configuracion.Salidas);

        }

        #endregion

        #region Entrenamiento

        private void txt_Interacciones_TextChanged(object sender, TextChangedEventArgs e)
        {
            if ( ComprobarFormato(txt_Interacciones, Boton_Entrenamiento))
            {
                configuracion.CiclosDeInteraccion = Convert.ToInt32(txt_Interacciones.Text);
            }
        }

        private void txt_PasosDelta_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (ComprobarFormato(txt_PasosDelta, Boton_Entrenamiento, false))
            {
                configuracion.SaltosDelta = Convert.ToDouble(txt_PasosDelta.Text);
                
            }
           
        }

        private void txt_ErrorAceptable_TextChanged(object sender, TextChangedEventArgs e)
        {
           
            if (ComprobarFormato(txt_ErrorAceptable, Boton_Entrenamiento, false))
            {
                configuracion.ErrorAceptable = Convert.ToDouble(txt_ErrorAceptable.Text);
            }
        }

        private void Boton_Entrenamiento_Click(object sender, RoutedEventArgs e)
        {

            SeEntrena = !SeEntrena;

            if (SeEntrena)
            {
                Boton_Entrenamiento.Content = "Detener";
                graficaLow.Reset();
                tokenSource = new CancellationTokenSource();
                token = tokenSource.Token;
                perceptron = new Percepcion(configuracion.Entradas, configuracion.NeurnasPorCapa, configuracion.Salidas);
                perceptron.MarcadorDeError += Perceptron_MarcadorDeError;
                Task.Factory.StartNew(() => Sub(token), token);
               
               


            }
            else
            {

                Boton_Entrenamiento.Content = "Entrenar";
                tokenSource.Cancel();

            }





            //SeEntrena = !SeEntrena;
            //if (!SeEntrena)
            //{
            //    tokenSource.Cancel();
            //}
            //else
            //{
            //    grafica.Reset();
            //    perceptron = new Percepcion(configuracion.Entradas, configuracion.NeurnasPorCapa, configuracion.Salidas);
            //    perceptron.MarcadorDeError += Perceptron_MarcadorDeError;
            //   Task.Factory.StartNew(() => Sub(token), token);
            //}




            //tokenSource.Cancel();
            //if (SeEntrena)
            //{
            //    Percepcion.Entrenar = true;

            //grafica.Reset();
            //perceptron = new Percepcion(configuracion.Entradas, configuracion.NeurnasPorCapa, configuracion.Salidas);
            //perceptron.MarcadorDeError += Perceptron_MarcadorDeError;
            //var tarea = Task.Factory.StartNew(() => Sub(token), token);
            //}
            //else
            //{
            //    tokenSource.Cancel();     
            //}
            //if (SeEntrena)
            //{
            //    Task.Factory.StartNew(() =>
            //    {
            //        if (!token.IsCancellationRequested)
            //        {
            //            Percepcion.Entrenar = true;
            //            grafica.Reset();
            //            perceptron = new Percepcion(configuracion.Entradas, configuracion.NeurnasPorCapa, configuracion.Salidas);
            //            perceptron.MarcadorDeError += Perceptron_MarcadorDeError;
            //            Sub();

            //        }
            //        else
            //        {
            //            Percepcion.Entrenar = false;
            //        }

            //    }, token);
            //}
            //else
            //{
            //    tokenSource.Cancel();
            //}





            //SeEntrena = !SeEntrena;

            //Thread.Sleep(100);
            //tokenSource = new CancellationTokenSource();
            //token = tokenSource.Token;

            //if (SeEntrena)
            //{

            //    Percepcion.Entrenar = true;
            //    grafica.Reset();
            //    perceptron = new Percepcion(configuracion.Entradas, configuracion.NeurnasPorCapa, configuracion.Salidas);
            //    Task.Factory.StartNew(() => Sub(token), token);


            //    perceptron.MarcadorDeError += Perceptron_MarcadorDeError;

            //}
            //else
            //{
            //    tokenSource.Cancel();
            //    Percepcion.Entrenar = false;
            //}


        }

        private void Perceptron_MarcadorDeError(object sender, MarcadoresDeErrorEventArgs e)
        {

            this.Dispatcher.Invoke(() => text_error.Text = string.Format("Error : {0}",e.Error));
            this.Dispatcher.Invoke(() => text_Interacciones.Text = string.Format("Interaccion : {0}",e.Interacciones));
            graficaLow.Read(e.Interacciones,e.Error);


            ////GraficoDeErro.Series[0].Values.Add(e.Error);
            //serieError.Values.Add(new MeasureModel { Interaccion = e.Interacciones, Value = e.Error });
            //axisX.MaxValue = e.Interacciones + 50;
            //axisX.MinValue = e.Interacciones - 50;

            //if (serieError.Values.Count > 100)
            //{
            //   // GraficoDeErro.Series[0].Values.RemoveAt(0);
            //    serieError.Values.RemoveAt(0);
            //}

        }

        //private void Perceptron_MarcadorDeError(object sender, MarcadoresDeErrorEventArgs e)
        //{
        //    graficoDeError.Read(e.Error,serie,true);
        //}




        #endregion

        #region Pruebas

        private void Texboxes_Entradas_Changed(object sender, TextChangedEventArgs e)
        {
            bool all = true;
            foreach (TextBox item in textBoxesEntradas)
            {
                all = all && ComprobarFormatoColectivo(item, false);
            }
            if (all)
            {
                Boton_Prueba.IsEnabled = true;
            }
            else
            {
                Boton_Prueba.IsEnabled = false;
            }
        }

        private void Boton_Prueba_Click(object sender, RoutedEventArgs e)
        {
            stackPanel_TexboxSalidas.Children.Clear();

            double[] entradas = new double[textBoxesEntradas.Count];
            for (int i = 0; i < textBoxesEntradas.Count; i++)
            {
                entradas[i] = ProcesadorCSV.Normalize(Convert.ToDouble(textBoxesEntradas[i].Text), configuracion.ValorMinEntradas, configuracion.ValorMaxEntradas);
            }
            if (perceptron == null)
            {
                MessageBox.Show("No se ha configurado ningun perceptron, configure o habra un previo entrenamiento");
                return;
            }

         double[] salidas = perceptron.Activacion(entradas);

            for (int i = 0; i < configuracion.Salidas; i++)
            {
                TextBlock salida = new TextBlock();
                salida.Text = string.Format("Salida {0} : {1}", i + 1, ProcesadorCSV.InverseNormalize(salidas[i],configuracion.ValorMinSalida, configuracion.ValorMaxSalida));
                salida.FontSize = 40;
                salida.Margin = new Thickness(15, 10, 10, 10);
               salida.Foreground = new SolidColorBrush(Color.FromRgb(19,143,19));
                stackPanel_TexboxSalidas.Children.Add(salida);
            }

        }

        private void Button_AbrirEntrenamiento_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Archivos de Entrenamiento (*.rednn)|*.rednn;";
            Rednn rednn = new Rednn();
            try
            {
                openFileDialog.ShowDialog();
                rednn = Serializacion.Deserializar<Rednn>(openFileDialog);

                perceptron = rednn.Percepcion;
                configuracion = rednn.Configuracion;
                EstablecerControles(configuracion);
                txt_Capas.TextChanged -= txt_Capas_TextChanged;
                txt_Capas.Text = configuracion.Capas.ToString();
               
                foreach (TextBox item in textBoxesConfiguracion)
                {
                    item.TextChanged -= Text_TextChanged;
                }
                WrapPanel_TextBox.Children.Clear();
                textBoxesConfiguracion.Clear();
                for (int i = 0; i < configuracion.NeurnasPorCapa.Length; i++)
                {
                    GroupBox group = new GroupBox();
                    TextBox textBox = new TextBox();
                    group.Header = "Capa " + (i + 1);
                    group.Height = 200;
                    group.Width = 200;
                    group.Margin = new Thickness(25);
                    textBox.SetValue(HintAssist.IsFloatingProperty, true);
                    textBox.SetValue(HintAssist.HintProperty, "Neuronas");

                    //textBox.Margin = new Thickness(25, 25, 25, 25);
                    textBox.Foreground = Brushes.White;
                    textBox.BorderBrush = new SolidColorBrush(Color.FromRgb(133, 133, 133));
                    textBox.FontSize = 20;
                    textBox.Foreground = Brushes.White;
                    textBox.HorizontalAlignment = HorizontalAlignment.Center;
                    textBox.VerticalAlignment = VerticalAlignment.Center;
                    textBox.Text = configuracion.NeurnasPorCapa[i].ToString();
                    group.Content = textBox;





                    WrapPanel_TextBox.Children.Add(group);
                }

                foreach (GroupBox box in WrapPanel_TextBox.Children)
                {
                    textBoxesConfiguracion.Add((TextBox)box.Content);
                }
                foreach (TextBox text in textBoxesConfiguracion)
                {
                    text.TextChanged += Text_TextChanged;
                }
                txt_Capas.TextChanged += txt_Capas_TextChanged;
            }
            catch (Exception a)
            {

                MessageBox.Show(a.Message);
            }
        }

        private void Button_Guardar_Click(object sender, RoutedEventArgs e)
        {







            Rednn rednn = new Rednn();
            perceptron.MarcadorDeError -= Perceptron_MarcadorDeError;
          
            rednn.Configuracion = configuracion;
            rednn.Percepcion = perceptron;

            SaveFileDialog ruta = new SaveFileDialog();

            ruta.Filter = "Archivos de Entrenamiento (*.rednn)|*.rednn;";

            try
            {
                ruta.ShowDialog();

                Serializacion.Serializar(rednn, ruta);






            }
            catch (Exception a)
            {

                MessageBox.Show("Button_Guardar_Click : " + a.Message);

            }


        }

        #endregion

        #region Sobre

        private void SliderTransparencia_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

            this.Background = new SolidColorBrush(Color.FromArgb((byte)SliderTransparencia.Value, 0, 0, 0));

          


           
            
        }

        #endregion

        #region Funciones globales

        /// <summary>
        /// Define los menus que estaran disponibles segun el modo de lectura seleccionado.
        /// </summary>
        /// <param name="modo"> Tipo de escaneo</param>
        public void Menus(Modo modo)
        {
            switch (modo)
            {
                case Modo.Automatico:
                    RadioBoton_Inteligencia.IsChecked = true;
                    RadioBoton_Semi_Inteligente.IsChecked = false;
                    RadioBoton_Manual.IsChecked = false;
                    BorderParametrosAuto.IsEnabled = false;

                    break;
                case Modo.Semiautomatico:

                    RadioBoton_Inteligencia.IsChecked = false;
                    RadioBoton_Semi_Inteligente.IsChecked = true;
                    RadioBoton_Manual.IsChecked = false;
                    BorderParametrosAuto.IsEnabled = true;

                    Stackpanel_ParametrosSemi.IsEnabled = false;

                    break;
                case Modo.manual:
                    RadioBoton_Inteligencia.IsChecked = false;
                    RadioBoton_Semi_Inteligente.IsChecked = false;
                    RadioBoton_Manual.IsChecked = true;
                    BorderParametrosAuto.IsEnabled = true;
                    Stackpanel_ParametrosSemi.IsEnabled = true;
                    break;
                default:
                    break;
            }

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="modo"></param>
        public void ComprobacionDeDatos(Modo modo)
        {
            switch (modo)
            {
                case Modo.Automatico:
                    if (archivo != null)
                    {
                        Boton_EmpezarEscaneo.IsEnabled = true;
                    }
                    else
                    {
                        Boton_EmpezarEscaneo.IsEnabled = false;
                    }

                    break;
                case Modo.Semiautomatico:
                    if (archivo != null)
                    {
                        if (!string.IsNullOrEmpty(txt_ValorMinimo.Text) && !string.IsNullOrEmpty(txt_ValorMaximo.Text) && !string.IsNullOrEmpty(txt_ValorMinimoEntrada.Text) && !string.IsNullOrEmpty(txt_ValorMaximoEntrada.Text))
                        {
                            if (IsDouble(txt_ValorMinimo.Text) && IsDouble(txt_ValorMaximo.Text)&& IsDouble(txt_ValorMaximoEntrada.Text) && IsDouble(txt_ValorMinimoEntrada.Text))
                            {
                                Boton_EmpezarEscaneo.IsEnabled = true;
                            }
                            else
                            {
                                Boton_EmpezarEscaneo.IsEnabled = false;
                            }
                        }
                        else
                        {
                            Boton_EmpezarEscaneo.IsEnabled = false;
                        }

                    }
                    else
                    {
                        Boton_EmpezarEscaneo.IsEnabled = false;
                    }

                    break;
                case Modo.manual:
                    if (archivo != null)
                    {
                        if (!string.IsNullOrEmpty(txt_ValorMinimo.Text) && !string.IsNullOrEmpty(txt_ValorMaximo.Text))
                        {
                            if (IsDouble(txt_ValorMinimo.Text) && IsDouble(txt_ValorMaximo.Text) && IsEntero(txt_CantidadEntradas.Text) && IsEntero(txt_CantidadSalidas.Text)&& IsDouble(txt_ValorMaximoEntrada.Text) && IsDouble(txt_ValorMinimoEntrada.Text))
                            {
                                Boton_EmpezarEscaneo.IsEnabled = true;
                            }
                            else
                            {
                                Boton_EmpezarEscaneo.IsEnabled = false;
                            }
                        }
                        else
                        {
                            Boton_EmpezarEscaneo.IsEnabled = false;
                        }
                    }
                    else
                    {
                        Boton_EmpezarEscaneo.IsEnabled = false;
                    }


                    break;
                default:
                    Boton_EmpezarEscaneo.IsEnabled = false;
                    break;
            }
        }
        /// <summary>
        /// Devuelve un un bool en caso de tener el formato correcto
        /// </summary>
        /// <param name="numero">Cadena de la que se requiere comprobar un formato</param>
        /// <returns>verdadero si es un entero, de lo controrio falso</returns>
        public bool IsEntero(string numero)
        {
            return Regex.IsMatch(numero, @"^\d+$");
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="numero"></param>
        /// <returns></returns>
        public bool IsDouble(string numero)
        {
            return Regex.IsMatch(numero, @"^(\d|-)?(\d|,)*\.?\d*$");
        }
        /// <summary>
        /// Comprueba el formato de texto y lo valida en caso de ser correcto
        /// </summary>
        /// <param name="textBox">TextBox del origen de texto</param>
        /// <param name="boton">boton para validar </param>
        /// <param name="Entero">Indica si el texto debe ser un entero. Default = true</param>
        /// <returns>true en caso de que los datos sean correctos, de lo contrario false</returns>
        public bool ComprobarFormato(TextBox textBox, Button boton, bool Entero = true)
        {
            if (!string.IsNullOrEmpty(textBox.Text))
            {

               

                if (Entero)
                {
                    if (IsEntero(textBox.Text))
                    {
                        textBox.Foreground = Brushes.White;
                       
                        boton.IsEnabled = true;
                        return true;

                    }
                    else
                    {
                        textBox.Foreground = new SolidColorBrush(Color.FromRgb(221, 79, 67));
                      
                        textBox.SetCurrentValue(HintAssist.IsFloatingProperty, true);
                        boton.IsEnabled = false;
                        return false;
                    }
                }
                else
                {
                    if (IsDouble(textBox.Text))
                    {
                        textBox.Foreground = Brushes.White;
                      
                        textBox.IsEnabled = true;
                        boton.IsEnabled = true;
                        return true;

                    }
                    else
                    {
                        textBox.Foreground = new SolidColorBrush(Color.FromRgb(221, 79, 67));
                       
                        textBox.SetCurrentValue(HintAssist.IsFloatingProperty, true);
                        boton.IsEnabled = false;
                        return false;
                    }
                }
            }
            else
            {
                textBox.Foreground = new SolidColorBrush(Color.FromRgb(221, 79, 67));
               
                textBox.SetCurrentValue(HintAssist.IsFloatingProperty, true);
                boton.IsEnabled = false;
                return false;
            }


        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="textBox"></param>
        /// <param name="Entero"></param>
        /// <returns></returns>
        public bool ComprobarFormato(TextBox textBox, bool Entero = true)
        {
          
           
          
            if (!string.IsNullOrEmpty(textBox.Text))
            {


                if (Entero)
                {
                    if (IsEntero(textBox.Text))
                    { 
                        textBox.Foreground = Brushes.White;
                      
                        return true;

                    }
                    else
                    {
                        textBox.Foreground = new SolidColorBrush(Color.FromRgb(221, 79, 67));
                        
                        textBox.SetCurrentValue(HintAssist.IsFloatingProperty, true);
                       
                        return false;
                    }
                }
                else
                {
                    if (IsDouble(textBox.Text))
                    {
                        textBox.Foreground = Brushes.White;
                      
                        return true;

                    }
                    else
                    {
                        textBox.Foreground = new SolidColorBrush(Color.FromRgb(221, 79, 67));
                        
                        textBox.SetCurrentValue(HintAssist.IsFloatingProperty, true);
                        return false;
                    }
                }
            }
            else
            {
                textBox.Foreground = new SolidColorBrush(Color.FromRgb(221, 79, 67));
               
                textBox.SetCurrentValue(HintAssist.IsFloatingProperty, true);
                return false;
            }


        }

        public bool ComprobarFormatoColectivo(TextBox textBox, bool Entero = true)
        {
            if (!string.IsNullOrWhiteSpace(textBox.Text))
            {
                if (Entero)
                {
                    if (IsEntero(textBox.Text))
                    {
                        textBox.Foreground = Brushes.White;
                        return true;
                    }
                    else
                    {
                        textBox.Foreground = new SolidColorBrush(Color.FromRgb(221, 79, 67));
                        return false;
                    }
                }
                else
                {
                    if (IsDouble(textBox.Text))
                    {
                        textBox.Foreground = Brushes.White;
                        return true;

                    }
                    else
                    {
                        textBox.Foreground = new SolidColorBrush(Color.FromRgb(221, 79, 67));
                        return false;
                    }
                }
            
            }
            else
            {
                textBox.Foreground = new SolidColorBrush(Color.FromRgb(221, 79, 67));
                return false;
            }
        }

        public void GeneradorDeES(int entradas, int salidas)
        {
            foreach (TextBox texboxes in textBoxesEntradas)
            {
                texboxes.TextChanged -= Texboxes_Entradas_Changed;
            }
            WrapPanel_TexboxEntradas.Children.Clear();
            textBoxesEntradas.Clear();
         

            for (int i = 0; i < entradas; i++)
            {
                GroupBox groupBoxEntradas = new GroupBox();
                groupBoxEntradas.Header = "Entrada " + (i + 1);
                TextBox textBox = new TextBox();
                textBox.Margin = new Thickness(35);
                textBox.VerticalAlignment = VerticalAlignment.Center;
                textBox.HorizontalAlignment = HorizontalAlignment.Center;
                textBox.Foreground = Brushes.White;
                groupBoxEntradas.Height = 200;
                groupBoxEntradas.Width = 200;
                groupBoxEntradas.Margin = new Thickness(25);
                textBox.BorderBrush = new SolidColorBrush(Color.FromRgb(133, 133, 133));
                textBox.FontSize = 20;
                textBox.Foreground = Brushes.White;
                textBox.HorizontalAlignment = HorizontalAlignment.Center;
                textBox.VerticalAlignment = VerticalAlignment.Center;
                textBox.SetValue(HintAssist.IsFloatingProperty, true);
                textBox.SetValue(HintAssist.HintProperty, "Introduce un dato");
                groupBoxEntradas.Content = textBox;
                textBox.TextChanged += Texboxes_Entradas_Changed;
                WrapPanel_TexboxEntradas.Children.Add(groupBoxEntradas);

            }
           

            foreach (GroupBox GroupEntradas in WrapPanel_TexboxEntradas.Children)
            {
                textBoxesEntradas.Add((TextBox)GroupEntradas.Content);
            }
          


        }

       

      

        public bool Sub(CancellationToken token)
        {
            if (!token.IsCancellationRequested)
            {
                while (!perceptron.Entrenamiento(configuracion.DatosDeEntrada, configuracion.DatosDeSalida, configuracion.CiclosDeInteraccion, configuracion.ErrorAceptable, configuracion.SaltosDelta,token))
                {
                    perceptron = new Percepcion(configuracion.Entradas, configuracion.NeurnasPorCapa, configuracion.Salidas);
                }
                return true;
            }
            else
            {

                return false;
            }
            

        }

        public void EstablecerControles(Configuracion configuraciones)
        {
            txt_ValorMinimoEntrada.Text = configuraciones.ValorMinEntradas.ToString();
            txt_ValorMaximoEntrada.Text = configuraciones.ValorMaxEntradas.ToString();
            txt_ValorMaximo.Text = configuraciones.ValorMaxSalida.ToString();
            txt_ValorMinimo.Text = configuraciones.ValorMinSalida.ToString();
            txt_CantidadEntradas.Text = configuraciones.Entradas.ToString();
            txt_CantidadSalidas.Text = configuraciones.Salidas.ToString();
            DataGridCSV.ItemsSource = configuracion.DataTable.DefaultView;
            Menus(configuraciones.ModoDeEscaneo);
            txt_Interacciones.Text = configuraciones.CiclosDeInteraccion.ToString();
            txt_ErrorAceptable.Text = configuraciones.ErrorAceptable.ToString();
            txt_PasosDelta.Text = configuraciones.SaltosDelta.ToString();
            GeneradorDeES(configuraciones.Entradas, configuraciones.Salidas);
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool dispose)
        {
            if (!this.disposed)
            {
                if (null != this.tokenSource)
                {
                    this.tokenSource.Dispose();
                    this.tokenSource = null;
                }
                this.disposed = true;
            }
        }




        #endregion





        ~MainWindow()
        {
            Serializacion.Serializar(configuracion);
            tokenSource.Dispose();
        }

       

     
    }

  

    //public class GraphicValues : INotifyPropertyChanged
    //{
    //    private double _axisMax;
    //    private double _axisMin;
       
      
    //    public GraphicValues()
    //    {
    //        //var mapper = Mappers.Xy<ModeloDeAxes>()
    //        //    .X(model => model.Interacciones)
    //        //    .Y(model => model.Porcentajes);
    //        //Charting.For<ModeloDeAxes>(mapper);
    //        //ChartValues = new ChartValues<ModeloDeAxes>();
    //        //AxisStep = 10;
    //        //AxisMax = 100;
    //        //AxisMin = 0;

    //        var mapper = Mappers.Xy<MeasureModel>()
    //            .X(model => model.DateTime.Ticks)   //use DateTime.Ticks as X
    //            .Y(model => model.Value);           //use the value property as Y

    //        //lets save the mapper globally.
    //        Charting.For<MeasureModel>(mapper);

    //        //the values property will store our values array
    //        ChartValues = new ChartValues<MeasureModel>();

    //        //lets set how to display the X Labels
    //        DateTimeFormatter = value => new DateTime((long)value).ToString("mm:ss");

    //        //AxisStep forces the distance between each separator in the X axis
    //        AxisStep = TimeSpan.FromSeconds(1).Ticks;
    //        //AxisUnit forces lets the axis know that we are plotting seconds
    //        //this is not always necessary, but it can prevent wrong labeling
    //        AxisUnit = TimeSpan.TicksPerSecond;

    //        SetAxisLimits(DateTime.Now);

    //        //The next code simulates data changes every 300 ms


    //    }
    //    public ChartValues<MeasureModel> ChartValues { get; set; }

    //    public Func<double, string> DateTimeFormatter { get; set; }


    //    public double AxisStep { get; set; }
    //    public double AxisUnit { get; set; }

    //    public double AxisMax
    //    {
    //        get { return _axisMax; }
    //        set
    //        {
    //            _axisMax = value;
    //            OnPropertyChanged("AxisMax");
    //        }
    //    }
    //    public double AxisMin
    //    {
    //        get { return _axisMin; }
    //        set
    //        {
    //            _axisMin = value;
    //            OnPropertyChanged("AxisMin");
    //        }
    //    }

    //    public bool IsReading { get; set; }

    //    public void Read(double value, LiveCharts.Wpf.LineSeries series, bool leer = false)
    //    { 
          

    //        while (leer)
    //        {
    //            Thread.Sleep(150);
    //            var now = DateTime.Now;


    //            series.Values.Add(new MeasureModel
    //            {
    //                DateTime = now,
    //                Value = value
                    

    //            });
              

    //            SetAxisLimits(now);

    //            //lets only use the last 150 values
    //            if (ChartValues.Count > 150) ChartValues.RemoveAt(0);
    //        }
    //    }

    //    //public void Graficar(double interaccion, double porcentaje)
    //    //{
    //    //    ChartValues.Add(new ModeloDeAxes
    //    //    {
    //    //        Porcentajes = porcentaje,
    //    //        Interacciones = interaccion
    //    //    });

    //    //    EstablecesAxisLimites(interaccion);
    //    //    if (ChartValues.Count > 150)
    //    //    {
    //    //        ChartValues.RemoveAt(0);
    //    //    }
    //    //}

    //    private void SetAxisLimits(DateTime now)
    //    {
    //        AxisMax = now.Ticks + TimeSpan.FromSeconds(1).Ticks; // lets force the axis to be 1 second ahead
    //        AxisMin = now.Ticks - TimeSpan.FromSeconds(8).Ticks; // and 8 seconds behind
    //    }

    //    //private void EstablecesAxisLimites(double interacciones)
    //    //{
    //    //    AxisMax = interacciones + 5;
    //    //    AxisMin = interacciones - 50;
    //    //}

      

    //    #region INotifyPropertyChanged implementation

    //    public event PropertyChangedEventHandler PropertyChanged;

    //    protected virtual void OnPropertyChanged(string propertyName = null)
    //    {
    //        if (PropertyChanged != null)
    //            PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
    //    }

    //    #endregion




      
    //}

    //public class ModeloDeAxes
    //{
    //    public double Porcentajes { get; set; }
    //    public double Interacciones { get; set; }

    //}

  

}




