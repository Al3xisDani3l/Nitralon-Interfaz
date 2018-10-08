using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;


namespace Nitralon
{   
    class Serializacion
    {
      public  static bool Serializar(object objeto, SaveFileDialog Direccion )
        {
            FileStream fileStream = new FileStream(Direccion.FileName, FileMode.Create);
            BinaryFormatter formateador = new BinaryFormatter();

            try
            {
                formateador.Serialize(fileStream, objeto);
                return true;
            }
            catch (SerializationException e)
            {

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

                return false;
            }
            finally
            {

                fileStream.Close();

            }
        }





        public  static bool Deserializar<Tipo>(ref Tipo objeto, OpenFileDialog archivo)
        {
            FileStream fileStream = new FileStream(archivo.FileName, FileMode.Open);
            BinaryFormatter formateador = new BinaryFormatter();

            try
            {
                if (fileStream != null)
                {
                    objeto = (Tipo)formateador.Deserialize(fileStream);
                    return true;

                }
                else
                {
                    throw new FileNotFoundException();
                }

            }
            catch (FileNotFoundException a)
            {

                return false;
               
            }
            catch(SerializationException b)
            {
                return false;
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
                objeto = new Tipo();
                return false;

            }
            catch (SerializationException b)
            {
                objeto = new Tipo();
                return false;
            }

            finally
            {
              
            }


        }
    }
}
