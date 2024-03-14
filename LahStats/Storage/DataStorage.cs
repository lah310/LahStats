using System.IO;
using Newtonsoft.Json;

namespace DataStorage
{
    public class DataStorage<T> where T : class
    {
        public string DataPath { get; private set; }

        public DataStorage(string dir, string fileName)
        {
            DataPath = Path.Combine(dir, fileName);
        }

        public void Save(T obj)
        {
            string value = JsonConvert.SerializeObject((object)obj, (Formatting)1);
            using StreamWriter streamWriter = new StreamWriter(DataPath, append: false);
            streamWriter.Write(value);
        }

        public T Read()
        {
            if (File.Exists(DataPath))
            {
                string text;
                using (StreamReader streamReader = File.OpenText(DataPath))
                {
                    text = streamReader.ReadToEnd();
                }
                return JsonConvert.DeserializeObject<T>(text);
            }
            return null;
        }
    }
}
