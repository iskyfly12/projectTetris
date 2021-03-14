using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Score : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TextMeshProUGUI scoreText;

    [Header("Score")]
    [SerializeField] int[] scoreMultiplyByLines;

    public int currentScore { get; private set; }

    public void Init()
    {
        currentScore = 0;
        scoreText.text = currentScore.ToString();
    }

    public void AddScore(int level, int pointsForLines)
    {
        if (scoreMultiplyByLines == null)
            return;

        currentScore += level * scoreMultiplyByLines[pointsForLines - 1];
        scoreText.text = currentScore.ToString();
    }
}
