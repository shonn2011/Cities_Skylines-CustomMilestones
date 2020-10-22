using LitJson;
using System.IO;

namespace CustomMilestones.Helpers
{
    public class JsonHelper
    {
        public static T FromJsonFile<T>(string filePath)
        {
            T t = default;
            if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
            {
                using (StreamReader streamReader = File.OpenText(filePath))
                {
                    t = JsonMapper.ToObject<T>(streamReader.ReadToEnd());
                }
            }
            return t;
        }

        public static void ToJsonFile<T>(T t, string filePath)
        {
            if (t != null)
            {
                string content = JsonMapper.ToJson(t);
                using (StreamWriter stringWriter = new StreamWriter(filePath))
                {
                    stringWriter.Write(content);
                }
            }
        }
    }
}
