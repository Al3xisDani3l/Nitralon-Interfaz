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
using System.Data;


namespace Nitralon
{




    /// <summary>
    /// Lógica de interacción para Prueba.xaml
    /// </summary>
    public partial class Prueba : Window
    {



        #region Variables
        /// <summary>
        /// OpenFileDialog que contiene la direccion de el archivo de entrenamiento.
        /// </summary>
        public OpenFileDialog archivo;
        /// <summary>
        /// Enum que contiene que tipo de tratamiento tendra el archivo CSV
        /// </summary>


        public Configuracion configuracionInterna;

        Percepcion perceptron;


        List<TextBox> textBoxes = new List<TextBox>();


        ProcesadorCSV Procesador;
        #endregion


        public Prueba()
        {
            InitializeComponent();
            Serializacion.Deserializar(ref configuracionInterna);
            txt_ValorMinimoEntrada.Text = configuracionInterna.ValorMinEntradas.ToString();
            txt_ValorMaximoEntrada.Text = configuracionInterna.ValorMaxEntrada.ToString();
            txt_ValorMaximo.Text = configuracionInterna.ValorMaxSalida.ToString();
            txt_ValorMinimo.Text = configuracionInterna.ValorMinSalida.ToString();
            txt_CantidadEntradas.Text = configuracionInterna.Entradas.ToString();
            txt_CantidadSalidas.Text = configuracionInterna.Salidas.ToString();
            Menus(configuracionInterna.ModoDeEscaneo);
            Boton_EmpezarEscaneo.IsEnabled = false;
            Boton_ConfirmarPerceptron.IsEnabled = false;
            Grid_Entrenamiento.IsEnabled = false;

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
            Fluent efect = new Fluent();
            efect.EnableBlur(this);

        }

        private void Boton_archivo_Click(object sender, RoutedEventArgs e)
        {
            archivo = new OpenFileDialog();
            archivo.Filter = "Archivos de Entrenamiento (*.csv)(*.txt)|*.csv;*.txt";

            try
            {
                archivo.ShowDialog();
                ComprobacionDeDatos(configuracionInterna.ModoDeEscaneo);
                if (string.IsNullOrEmpty(archivo.FileName))
                {
                    Boton_EmpezarEscaneo.IsEnabled = false;
                }
                else
                {
                    Boton_EmpezarEscaneo.IsEnabled = true;
                }

            }
            catch (Exception)
            {

                throw;
            }


        }

        private void txt_ValorMinimoEntrada_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (ComprobarFormato(txt_ValorMinimoEntrada, Boton_EmpezarEscaneo,false))
            {
                ComprobacionDeDatos(configuracionInterna.ModoDeEscaneo);
            }
        }

        private void txt_ValorMaximoEntrada_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (ComprobarFormato(txt_ValorMaximoEntrada, Boton_EmpezarEscaneo, false))
            {
                ComprobacionDeDatos(configuracionInterna.ModoDeEscaneo);
            }
        }

        private void RadioBoton_Inteligencia_Click(object sender, RoutedEventArgs e)
        {
            if (archivo != null)
            {
                Boton_EmpezarEscaneo.IsEnabled = true;
            }
            configuracionInterna.ModoDeEscaneo = Modo.Automatico;
            Menus(configuracionInterna.ModoDeEscaneo);

        }

        private void RadioBoton_Semi_Inteligente_Click(object sender, RoutedEventArgs e)
        {
            Boton_EmpezarEscaneo.IsEnabled = false;
            configuracionInterna.ModoDeEscaneo = Modo.Semiautomatico;
            Menus(configuracionInterna.ModoDeEscaneo);
        }

        private void RadioBoton_Manual_Click(object sender, RoutedEventArgs e)
        {
            Boton_EmpezarEscaneo.IsEnabled = false;
            configuracionInterna.ModoDeEscaneo = Modo.manual;
            Menus(configuracionInterna.ModoDeEscaneo);
        }

        private void Boton_EmpezarEscaneo_Click(object sender, RoutedEventArgs e)
        {
            DataGridCSV.InvalidateVisual();

            switch (configuracionInterna.ModoDeEscaneo)
            {
                case Modo.Automatico:
                    Procesador = new ProcesadorCSV(archivo);
                    configuracionInterna.Entradas = Procesador.ConteoDeEntradas;
                    configuracionInterna.Salidas = Procesador.ConteoDeSalidas;
                    configuracionInterna.ValorMaxSalida = Procesador.ValorMaxS;
                    configuracionInterna.ValorMinSalida = Procesador.ValorMinS;
                    configuracionInterna.ValorMinEntradas = Procesador.ValorMinE;
                    configuracionInterna.ValorMaxEntrada = Procesador.ValorMaxE;  
                    configuracionInterna.DataTable = Procesador.DataTable;
                    configuracionInterna.DatosDeEntrada = Procesador.Entradas;
                    configuracionInterna.DatosDeSalida = Procesador.Salidas;
                    txt_ValorMinimoEntrada.Text = configuracionInterna.ValorMinEntradas.ToString();
                    txt_ValorMaximoEntrada.Text = configuracionInterna.ValorMaxEntrada.ToString();
                    txt_ValorMaximo.Text = configuracionInterna.ValorMaxSalida.ToString();
                    txt_ValorMinimo.Text = configuracionInterna.ValorMinSalida.ToString();
                    txt_CantidadEntradas.Text = configuracionInterna.Entradas.ToString();
                    txt_CantidadSalidas.Text = configuracionInterna.Salidas.ToString();



                    break;
                case Modo.Semiautomatico:
                    Procesador = new ProcesadorCSV(archivo, Convert.ToDouble(txt_ValorMinimo.Text), Convert.ToDouble(txt_ValorMaximo.Text), Convert.ToDouble(txt_ValorMinimoEntrada.Text), Convert.ToDouble(txt_ValorMaximoEntrada.Text));
                    configuracionInterna.Entradas = Procesador.ConteoDeEntradas;
                    configuracionInterna.Salidas = Procesador.ConteoDeSalidas;
                    configuracionInterna.ValorMaxSalida = Convert.ToDouble(txt_ValorMaximo.Text);
                    configuracionInterna.ValorMaxSalida = Convert.ToDouble(txt_ValorMinimo.Text);
                    configuracionInterna.ValorMinSalida = Convert.ToDouble(txt_ValorMinimoEntrada.Text);
                    configuracionInterna.ValorMinEntradas = Convert.ToDouble(txt_ValorMaximoEntrada.Text);
                    configuracionInterna.DataTable = Procesador.DataTable;
                    configuracionInterna.DatosDeEntrada = Procesador.Entradas;
                    configuracionInterna.DatosDeSalida = Procesador.Salidas;
                    txt_CantidadEntradas.Text = configuracionInterna.Entradas.ToString();
                    txt_CantidadSalidas.Text = configuracionInterna.Salidas.ToString();


                    break;
                case Modo.manual:
                    Procesador = new ProcesadorCSV(archivo,Convert.ToInt32(txt_CantidadEntradas.Text),Convert.ToInt32(txt_CantidadSalidas.Text), Convert.ToDouble(txt_ValorMinimo.Text), Convert.ToDouble(txt_ValorMaximo.Text), Convert.ToDouble(txt_ValorMinimoEntrada.Text), Convert.ToDouble(txt_ValorMaximoEntrada.Text));
                    configuracionInterna.Entradas = Convert.ToInt32(txt_CantidadEntradas.Text);
                    configuracionInterna.Salidas = Convert.ToInt32(txt_CantidadSalidas.Text);
                    configuracionInterna.ValorMaxSalida = Convert.ToDouble(txt_ValorMaximo.Text);
                    configuracionInterna.ValorMaxSalida = Convert.ToDouble(txt_ValorMinimo.Text);
                    configuracionInterna.ValorMinSalida = Convert.ToDouble(txt_ValorMinimoEntrada.Text);
                    configuracionInterna.ValorMinEntradas = Convert.ToDouble(txt_ValorMaximoEntrada.Text);
                    configuracionInterna.DataTable = Procesador.DataTable;
                    configuracionInterna.DatosDeEntrada = Procesador.Entradas;
                    configuracionInterna.DatosDeSalida = Procesador.Salidas;
                    break;
                default:
                    break;
            }

           
            DataGridCSV.ItemsSource = Procesador.DataTable.DefaultView;








        }

        private void txt_ValorMinimo_TextChanged(object sender, TextChangedEventArgs e)
        {

            if (ComprobarFormato(txt_ValorMinimo, Boton_EmpezarEscaneo, false))
            {
                ComprobacionDeDatos(configuracionInterna.ModoDeEscaneo);
            }
        }

        private void txt_CantidadEntradas_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (ComprobarFormato(txt_CantidadEntradas, Boton_EmpezarEscaneo))
            {
                ComprobacionDeDatos(configuracionInterna.ModoDeEscaneo);
            }
        }

        private void txt_ValorMaximo_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (ComprobarFormato(txt_ValorMinimo, Boton_EmpezarEscaneo, false))
            {
                ComprobacionDeDatos(configuracionInterna.ModoDeEscaneo);
            }
        }

        private void txt_CantidadSalidas_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (ComprobarFormato(txt_ValorMinimo, Boton_EmpezarEscaneo))
            {
                ComprobacionDeDatos(configuracionInterna.ModoDeEscaneo);
            }
        }

        #endregion

        #endregion

        #region Configuracion

        private void txt_Capas_TextChanged(object sender, TextChangedEventArgs e)
        {
            Boton_ConfirmarPerceptron.IsEnabled = false;
            foreach (TextBox item in textBoxes)
            {
                item.TextChanged -= Text_TextChanged;
            }
          
            textBoxes.Clear();
            if (ComprobarFormato(txt_Capas))
            {
                WrapPanel_TextBox.Children.Clear();
                int tamano = Convert.ToInt32(txt_Capas.Text);
                if (tamano >= 50)
                {
                    tamano = 50;
                }

               

                
                for (int i = 0; i < tamano; i++)
                {
                    GroupBox group = new GroupBox();
                    TextBox textBox = new TextBox();
                    group.Header = "Capa " + (i + 1);
                    group.Height = 200;
                    group.Width = 200;
                    group.Margin = new Thickness(25);
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
                textBoxes.Add((TextBox)box.Content);
            }
            foreach (TextBox text in textBoxes)
            {
                text.TextChanged += Text_TextChanged;
            }




        }

        private void Text_TextChanged(object sender, TextChangedEventArgs e)
        {
            ComprobarFormato((TextBox)sender, Boton_ConfirmarPerceptron);
           
        }

        private void Boton_ConfirmarPerceptron_Click(object sender, RoutedEventArgs e)
        {
            int[] bufferNeuronas = new int[textBoxes.Count];

            for (int i = 0; i < textBoxes.Count; i++)
            {
                if (Convert.ToInt32(textBoxes[i].Text) <= 0)
                {
                    bufferNeuronas[i] = 1;
                }
                else
                {
                    bufferNeuronas[i] = Convert.ToInt32(textBoxes[i].Text);
                }
               
               
            }
            configuracionInterna.NeurnasPorCapa = bufferNeuronas;
            Grid_Entrenamiento.IsEnabled = true;
            perceptron = new Percepcion(Procesador.ConteoDeEntradas, configuracionInterna.NeurnasPorCapa, Procesador.ConteoDeSalidas);

        }

        #endregion

        #region Entrenamiento

        private void txt_Interacciones_TextChanged(object sender, TextChangedEventArgs e)
        {
            if ( ComprobarFormato(txt_Interacciones, Boton_Entrenamiento))
            {
                configuracionInterna.CiclosDeInteraccion = Convert.ToInt32(txt_Interacciones.Text);
            }
        }

        private void txt_PasosDelta_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (ComprobarFormato(txt_PasosDelta, Boton_Entrenamiento, false))
            {
                configuracionInterna.SaltosDelta = Convert.ToDouble(txt_PasosDelta.Text);
            }
           
        }

        private void txt_ErrorAceptable_TextChanged(object sender, TextChangedEventArgs e)
        {
           
            if (ComprobarFormato(txt_ErrorAceptable, Boton_Entrenamiento, false))
            {
                configuracionInterna.ErrorAceptable = Convert.ToDouble(txt_ErrorAceptable.Text);
            }
        }
        private void Boton_Entrenamiento_Click(object sender, RoutedEventArgs e)
        {
            if (!perceptron.Entrenamiento(configuracionInterna.DatosDeEntrada, configuracionInterna.DatosDeSalida, configuracionInterna.CiclosDeInteraccion, configuracionInterna.ErrorAceptable, configuracionInterna.SaltosDelta))
            {

            }
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
        /// 
        /// </summary>
        /// <param name="numero"></param>
        /// <returns></returns>
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
                        return true;

                    }
                    else
                    {
                        textBox.Foreground = new SolidColorBrush(Color.FromRgb(221, 79, 67));
                        boton.IsEnabled = false;
                        return false;
                    }
                }
            }
            else
            {
                textBox.Foreground = new SolidColorBrush(Color.FromRgb(221, 79, 67));
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

        #endregion





  
        ~Prueba()
        {
            Serializacion.Serializar(configuracionInterna);

        }

     
    }
}



        class Fluent
    {
        [DllImport("user32.dll")]
        internal static extern int SetWindowCompositionAttribute(IntPtr hand, ref WindowsCompositionAttributeData data);

        internal enum AccentState
        {
            ACCENT_DISABLED = 0,
            ACCENT_ENABLED_GRADIENT = 10,
            ACCENT_ENABLED_TRANSPARENTGRADIENT = 2,
            ACCENT_ENABLED_BLURBEHIND = 3,
            ACCENT_INVALID_STATE = 4
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct AccentPolicy
        {
            public AccentState AccentState;
            public int AccentFlags;
            public int GradeintColor;
            public int AnimationId;

        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct WindowsCompositionAttributeData
        {
            public WindowsCompositionAttribute Attribute;
            public IntPtr Data;
            public int SizeOfData;
        }

        internal enum WindowsCompositionAttribute
        {
            WCA_ACCENT_POLICY = 19
        }

        public void EnableBlur(Window window)
        {
            var windowHelper = new WindowInteropHelper(window);
            var accent = new AccentPolicy();
            accent.AccentState = AccentState.ACCENT_ENABLED_BLURBEHIND;
            var accentStructSize = Marshal.SizeOf(accent);
            var accentptr = Marshal.AllocHGlobal(accentStructSize);
            Marshal.StructureToPtr(accent, accentptr, false);
            var data = new WindowsCompositionAttributeData();
            data.Attribute = WindowsCompositionAttribute.WCA_ACCENT_POLICY;
            data.SizeOfData = accentStructSize;
            data.Data = accentptr;
            SetWindowCompositionAttribute(windowHelper.Handle, ref data);
            Marshal.FreeHGlobal(accentptr);
        }


    }
    
