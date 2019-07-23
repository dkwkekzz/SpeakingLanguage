using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace SpeakingLanguage.Library
{
    public static class ObjectHelper
    {
        public static object DeepClone(object org)
        {
            using (var stream = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Context = new StreamingContext(StreamingContextStates.Clone);

                formatter.Serialize(stream, org);

                stream.Position = 0;

                return formatter.Deserialize(stream);
            }
        }
    }
}
