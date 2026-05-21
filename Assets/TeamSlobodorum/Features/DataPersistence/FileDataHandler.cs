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

        public GameData Load()
        {
            string fullPath = Path.Combine(dataDirPath, dataFileName);
            GameData loadedData = null;
            if (File.Exists(fullPath))
            {
                try
                {
                    string dataToLoad = File.ReadAllText(fullPath);
                    loadedData = JsonUtility.FromJson<GameData>(dataToLoad);
                }
                catch (System.Exception e) { Debug.LogError("Error loading data: " + e); }
            }
            return loadedData;
        }

        public void Save(GameData data)
        {
            string fullPath = Path.Combine(dataDirPath, dataFileName);
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
                string dataToStore = JsonUtility.ToJson(data, true);
                File.WriteAllText(fullPath, dataToStore);
            }
            catch (System.Exception e) { Debug.LogError("Error saving data: " + e); }
        }
    }
}
