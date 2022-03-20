using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveDataManager : MonoBehaviour
{
    public static SaveDataManager Instance;

    private int highScore = 0;

    public int HighScore { get => highScore; set => highScore = value; }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    [System.Serializable]
    class Data
    {
        public int saveHighScore;
    }

    public void SaveData()
    {
        Data data = new Data
        {
            saveHighScore = HighScore,
        };
        string json = JsonUtility.ToJson(data);

        File.WriteAllText(Application.persistentDataPath + "/savefile.json", json);
    }

    public void LoadData()
    {
        string path = Application.persistentDataPath + "/savefile.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);

            Data data = JsonUtility.FromJson<Data>(json);

            HighScore = data.saveHighScore;
        }
    }
}
