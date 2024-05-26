using UnityEngine;
using TMPro; 
public class BowlingScoreManager : MonoBehaviour
{
    public TMP_Text scoreText;
    private int Score = 0;

    void Start()
    {
        UpdateScoreText();
    }

    public void RecordRoll(int points)
    {
        Score += points;
        Debug.Log("Current Score: " + Score);
        UpdateScoreText();
    }
    public void UpdateScoreText()
    {
        scoreText.text = "Score: " + Score;
    }
}