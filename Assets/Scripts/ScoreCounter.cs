using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public sealed class ScoreCounter : MonoBehaviour
{
    public static ScoreCounter Instance { get; private set; }

    private int _score = 0;

    public int Score
    {
        get => _score;

        set
        {
            if (_score == value) return;

            _score = value;

            scoreText.SetText(_score.ToString());
        }
    }


    [SerializeField] private TextMeshProUGUI scoreText;

    private void Awake() => Instance = this;
}
