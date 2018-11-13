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
        /// 
        /// </summary>
        /// <typeparam name="Tipo"></typeparam>
        /// <param name="objeto"></param>
        /// <returns></returns>
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
