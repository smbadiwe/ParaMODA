using System.IO;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;

namespace MODA.Impl
{
    public class MySerializer
    {
        /// <summary>
        /// Serializes the object.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        public static byte[] Serialize(object obj)
        {
            using (var ms = new MemoryStream())
            {
                BinaryFormatter bf = new BinaryFormatter
                {
                    TypeFormat = FormatterTypeStyle.TypesAlways,
                    AssemblyFormat = FormatterAssemblyStyle.Full,
                };
                bf.Serialize(ms, obj);

                return ms.ToArray();
            }
        }

        /// <summary>
        /// Deserializes the object.
        /// </summary>
        /// <param name="serializedObject">The serialized object.</param>
        /// <returns></returns>
        public static T DeSerialize<T>(byte[] serializedObject) where T : class
        {
            if (serializedObject != null)
            {
                using (var ms = new MemoryStream(serializedObject))
                {
                    BinaryFormatter bf = new BinaryFormatter
                    {
                        TypeFormat = FormatterTypeStyle.TypesAlways,
                        AssemblyFormat = FormatterAssemblyStyle.Full,
                    };
                    return bf.Deserialize(ms) as T;
                }
            }
            return null;
        }

    }
}
