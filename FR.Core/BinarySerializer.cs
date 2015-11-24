/*
 * Created by: Milton García Borroto (milton.garcia@gmail.com) 
 * Created: 
 * Comments by: Miguel Angel Medina Pérez (miguel.medina.perez@gmail.com)
 */

using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace PatternRecognition.FingerprintRecognition.Core
{
    /// <summary>
    ///     Provides basic functionality to serialize and deserialize objects in binary format.
    /// </summary>
    public static class BinarySerializer
    {
        /// <summary>
        ///     Serialize in binary format the specified object to the provided location.
        /// </summary>
        /// <param name="obj">
        ///     The object to serialize.
        /// </param>
        /// <param name="FileName">
        ///     The location where the specified object will be serialized.
        /// </param>
        public static void Serialize(object obj, string FileName)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(FileName, FileMode.Create);
            formatter.Serialize(stream, obj);
            stream.Close();
        }

        /// <summary>
        ///     Serialize the specified object to byte array.
        /// </summary>
        /// <param name="obj">
        ///     The object to serialize.
        /// </param>
        /// <returns>
        ///     The serialized object in byte array format.
        /// </returns>
        public static byte[] Serialize(object obj)
        {
            MemoryStream stream = new MemoryStream();
            Serialize(obj, stream);
            return stream.ToArray();
        }

        /// <summary>
        ///     Serialize in binary format the specified object to the specified <see cref="Stream"/>.
        /// </summary>
        /// <param name="obj">
        ///     The object to serialize.
        /// </param>
        /// <param name="stream">
        ///     The <see cref="Stream"/> where the specified object will be serialized.
        /// </param>
        public static void Serialize(object obj, Stream stream)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, obj);
        }

        /// <summary>
        ///     Deserialize the object contained in the specified <see cref="Stream"/>.
        /// </summary>
        /// <param name="stream">
        ///     The <see cref="Stream"/> containing the object to be deserialized.
        /// </param>
        /// <returns>
        ///     The object deserialized from the specified <see cref="Stream"/>.
        /// </returns>
        public static object Deserialize(Stream stream)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            return formatter.Deserialize(stream);
        }

        /// <summary>
        ///     Deserialize the object saved in the specified location.
        /// </summary>
        /// <param name="fileName">
        ///     The location containing the object to be deserialized.
        /// </param>
        /// <returns>
        ///     The object deserialized from the specified location.
        /// </returns>
        public static object Deserialize(string fileName)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(fileName, FileMode.Open);
            object Result = formatter.Deserialize(stream);
            stream.Close();
            return Result;
        }

        /// <summary>
        ///     Deserialize the object contained in the specified byte array.
        /// </summary>
        /// <param name="data">
        ///     The byte array containing the object to be deserialized.
        /// </param>
        /// <returns>
        ///     The object deserialized from the specified byte array.
        /// </returns>
        public static object Deserialize(byte[] data)
        {
            MemoryStream stream = new MemoryStream(data);
            return Deserialize(stream);
        }
    }
}