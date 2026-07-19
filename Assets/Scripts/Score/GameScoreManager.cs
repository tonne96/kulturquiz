using UnityEngine;

public class GameScoreManager : MonoBehaviour
{
    public static GameScoreManager Instance;

    public int TotalScore { get; private set; }


    private void Awake()
    {   
        Debug.Log("GameScoreManager initialized");

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }


    public void AddQuestionScore(string difficulty)
    {
        int points = difficulty switch
        {
            "easy" => 10,
            "medium" => 20,
            "hard" => 30,
            _ => 0
        };

        TotalScore += points;

        Debug.Log($"Added {points} points. Total score: {TotalScore}");
    }


    public void ResetScore()
    {
        TotalScore = 0;
    }
}