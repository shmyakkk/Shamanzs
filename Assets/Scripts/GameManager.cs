using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public sealed class GameManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI highScoreText;


    private void Awake()
    {
        SaveDataManager.Instance.LoadData();
    }
    private void Start()
    {
        highScoreText.SetText(SaveDataManager.Instance.HighScore.ToString());
    }

    public void RestartGame()
    {
        if (ScoreCounter.Instance.Score > SaveDataManager.Instance.HighScore) SaveDataManager.Instance.HighScore = ScoreCounter.Instance.Score;

        SaveDataManager.Instance.SaveData();

        SceneManager.LoadScene("Game");
    }
}
