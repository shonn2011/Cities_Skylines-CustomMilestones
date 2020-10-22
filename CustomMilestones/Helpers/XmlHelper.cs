using System.IO;
using System.Xml.Serialization;

namespace CustomMilestones.Helpers
{
    public class XmlHelper
    {
        public static T FromXmlFile<T>(string filePath)
        {
            T t = default;
            if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
                using (StreamReader streamReader = new StreamReader(filePath))
                {
                    t = (T)xmlSerializer.Deserialize(streamReader);
                }
            }
            return t;
        }

        public static void ToXmlFile<T>(T t, string filePath)
        {
            if (t != null)
            {
                string content = string.Empty;
                XmlSerializer xmlSerializer = new XmlSerializer(t.GetType());
                using (StringWriter stringWriter = new StringWriter())
                {
                    xmlSerializer.Serialize(stringWriter, t);
                    content = stringWriter.ToString();
                }
                using (StreamWriter stringWriter = new StreamWriter(filePath))
                {
                    stringWriter.Write(content);
                }
            }
        }
    }
}
