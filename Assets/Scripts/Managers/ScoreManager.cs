using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour {

    public static ScoreManager Instance { get; private set; }

    [Header("---| In Game Score |---")]
    public TextMeshProUGUI scoreText;

    [Header("---| Revive Score |---")]
    public TextMeshProUGUI reviveScoreValue;
    public TextMeshProUGUI reviveBestScoreValue;

    [Header("---| Game Over Score |---")]
    public TextMeshProUGUI scoreValue;
    public TextMeshProUGUI bestScoreValue;

    private int currentScore;
    private int bestScore;

    public int CurrentScore {
        get {
            return currentScore;
        }
        set {
            currentScore = value;
            scoreText.SetText("Score: " + CurrentScore);
        }
    }

    void Awake() {
        Instance = this;
        CurrentScore = 0;
        bestScore = PlayerPrefs.GetInt("bestScore", 0);
    }

    public void SetReviveScore() {
        reviveScoreValue.SetText("Score: " + CurrentScore.ToString());
        if(CurrentScore > bestScore) {
            reviveBestScoreValue.SetText("Best: " + CurrentScore.ToString());
        } else {
            reviveBestScoreValue.SetText("Best: " + bestScore.ToString());
        }
    }
    
    public void SetGameOverScore() {
        scoreValue.SetText(CurrentScore.ToString());
        if(CurrentScore > bestScore) {
            bestScore = CurrentScore;
            PlayerPrefs.SetInt("bestScore", CurrentScore);
            PlayerPrefs.Save();
            bestScoreValue.SetText(CurrentScore.ToString());
        } else {
            bestScoreValue.SetText(bestScore.ToString());
        }
    }

}
