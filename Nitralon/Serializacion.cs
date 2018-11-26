using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Windows;

namespace Nitralon
{   
    class Serializacion
    {
        /// <summary>
        /// Serializa un objeto enmarcado con el atributo "[Serializable]" en la direccion especificada.
        /// </summary>
        /// <param name="objeto">Objeto a serializar.</param>
        /// <param name="Direccion">SaveFileDialog que porta la direccion donde se guardara el archivo.</param>
        /// <returns>True si la operacion fue exitosa, false de lo contrario</returns>
        public  static bool Serializar(object objeto, SaveFileDialog Direccion )
        {
            FileStream fileStream = new FileStream(Direccion.FileName, FileMode.Create,FileAccess.Write);
            BinaryFormatter formateador = new BinaryFormatter();

            try
            {
               
                formateador.Serialize(fileStream, objeto);
                return true;
            }
            catch (SerializationException e)
            {
                MessageBox.Show("Serializacion.Serializar() : " + e.Message);
                return false;
            }   
            finally
            {

                fileStream.Close();
                
            }
        }
        /// <summary>
        /// Serializa un objeto en el path donde se ejecuta la aplicacion, con el nombre Data.neuron
        /// </summary>
        /// <param name="objeto">Objeto a serializar</param>
        /// <returns>True si la serializacion fue exitosa, de lo contrario false</returns>
        public static bool Serializar(object objeto)
        {
            FileStream fileStream = new FileStream(Directory.GetCurrentDirectory() + @"\Data.neuron" , FileMode.Create);
            BinaryFormatter formateador = new BinaryFormatter();

            try
            {
                formateador.Serialize(fileStream, objeto);
                return true;
            }
            catch (SerializationException e)
            {
                MessageBox.Show(e.Message);
                return false;
            }
            finally
            {

                fileStream.Close();

            }
        }
        /// <summary>
        /// Deserializa un archivo y lo convierte al tipo deseado.
        /// </summary>
        /// <typeparam name="Tipo">Tipo en que se convertira el archivo serializado, este debe ser compatible con el tipo.</typeparam>
        /// <param name="archivo">OpenFileDialog que porta la ubicacion del archivo a serializar.</param>
        /// <returns>Tipo</returns>
        public static Tipo Deserializar<Tipo>(OpenFileDialog archivo) where Tipo : new()
        {
            FileStream fileStream = new FileStream(archivo.FileName, FileMode.Open, FileAccess.Read);
            fileStream.Position = 0;
            BinaryFormatter formateador = new BinaryFormatter();
            object o = new object();
            try
            {
                if (fileStream != null)
                {
                    o = formateador.Deserialize(fileStream);
                    Tipo tipo = (Tipo)o;
                    return tipo;

                }
                else
                {
                    throw new FileNotFoundException();
                }

            }
            catch (FileNotFoundException a)
            {
                MessageBox.Show(a.Message);
                return new Tipo();
               
            }
            catch(SerializationException b)
            {
                MessageBox.Show(b.Message);
                return new Tipo();
            }

            finally
            {
                fileStream.Close();
            }


        }
        /// <summary>
        ///  Deserializa un archivo ubicado en el path del ejecutable, Data.neuron, en caso de que no se encuentre, devuelve una nueva instancia del Tipo.
        /// </summary>
        /// <typeparam name="Tipo"></typeparam>
        /// <param name="objeto"></param>
        /// <returns>True si la operacion fue exitosa, de lo contrario false.</returns>
        public static bool Deserializar<Tipo> (ref Tipo objeto) where Tipo : new()
        {

            FileStream fileStream;

            try
            {
                 fileStream = new FileStream(Directory.GetCurrentDirectory() + @"\Data.neuron", FileMode.Open);
                BinaryFormatter formateador = new BinaryFormatter();
                fileStream.Position = 0;

                if (fileStream != null)
                {
                    objeto = (Tipo)formateador.Deserialize(fileStream);
                    return true;

                }
                else
                {
                    objeto = new Tipo();
                    return false;
                }

            }
            catch (FileNotFoundException a)
            {
                MessageBox.Show(a.Message);
                objeto = new Tipo();
                return false;

            }
            catch (SerializationException b)
            {
                MessageBox.Show(b.Message);
                objeto = new Tipo();
                return false;
            }

            finally
            {
               
            }


        }

      
    }
}
