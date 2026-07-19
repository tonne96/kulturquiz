using UnityEngine;

public class GameScoreManager : MonoBehaviour
{
    public static GameScoreManager Instance;

    public int TotalScore { get; private set; }
    public int CurrentStreak { get; private set; } = 0;

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
        int basePoints = difficulty switch
        {
            "easy" => 10,
            "medium" => 20,
            "hard" => 30,
            _ => 0
        };

        float multiplier = 1f + (CurrentStreak * 0.1f);

        int earnedPoints = Mathf.RoundToInt(basePoints * multiplier);

        TotalScore += earnedPoints;

        CurrentStreak++;

        Debug.Log(
            $"Correct! +{earnedPoints} points | " +
            $"Streak: {CurrentStreak} | " +
            $"Total Score: {TotalScore}");
    }


    public void ResetScore()
    {
        TotalScore = 0;
    }

    public void ResetStreak()
    {
        CurrentStreak = 0;
        Debug.Log("Streak Reset");
    }
}