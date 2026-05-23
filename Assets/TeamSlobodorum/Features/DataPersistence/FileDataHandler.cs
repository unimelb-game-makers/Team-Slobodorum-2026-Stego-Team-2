using UnityEngine;
using System.IO;
namespace TeamSlobodorum.DataPersistence
{
    public class FileDataHandler
    {
        private string dataDirPath = "";
        private string dataFileName = "";

        public FileDataHandler(string dataDirPath, string dataFileName)
        {
            this.dataDirPath = dataDirPath;
            this.dataFileName = dataFileName;
        }

        public T Load<T>() where T : class
        {
            string fullPath = Path.Combine(dataDirPath, dataFileName);
            T loadedData = null;
            if (File.Exists(fullPath))
            {
                try
                {
                    string dataToLoad = File.ReadAllText(fullPath);
                    loadedData = JsonUtility.FromJson<T>(dataToLoad);
                }
                catch (System.Exception e)
                {
                    Debug.LogError("Error loading data to path: " + fullPath + "\n" + e);
                }
            }
            return loadedData;
        }

        public void Save<T>(T data)
        {
            string fullPath = Path.Combine(dataDirPath, dataFileName);
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
                string dataToStore = JsonUtility.ToJson(data, true); // true = format nicely
                File.WriteAllText(fullPath, dataToStore);
            }
            catch (System.Exception e)
            {
                Debug.LogError("Error saving data to path: " + fullPath + "\n" + e);
            }
        }
    }
}
