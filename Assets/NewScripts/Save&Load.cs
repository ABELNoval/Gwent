
using Console;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;

namespace Scripts
{

    public class SaveAndLoad : MonoBehaviour
    {
        GameManager gameManager;
        public string fileName;

        private void Awake()
        {
            fileName = Application.dataPath + "dataGame.json";
            gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
            gameManager.save += SaveConfiguration;
            gameManager.load += LoadConfiguration;
        }

        private void SaveConfiguration(Store store)
        {
            string cadenaJNSON = JsonConvert.SerializeObject(store, Formatting.Indented);
            File.WriteAllText(fileName, cadenaJNSON);
            Debug.Log("Archivo guardado");
        }

        private Store LoadConfiguration()
        {
            if (File.Exists(fileName))
            {
                Debug.Log("Archivo cargado");
                string conf = File.ReadAllText(fileName);
                return JsonConvert.DeserializeObject<Store>(conf);
            }
            return null;
        }
    }
}
