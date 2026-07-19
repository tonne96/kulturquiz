using UnityEngine;

public class GameScoreManager : MonoBehaviour
{
    private static GameScoreManager _instance;
    public static GameScoreManager Instance
    {
        get
        {
            if (_instance == null)
            {
                var go = new GameObject("GameScoreManager");
                _instance = go.AddComponent<GameScoreManager>();
            }
            return _instance;
        }
    }

    private const string KeyHighScore = "HighScore";
    private const string KeyHighStreak = "HighStreak";

    public int TotalScore { get; private set; }
    public int CurrentStreak { get; private set; } = 0;
    public int HighestStreak { get; private set; } = 0;
    public int HighScore { get; private set; } = 0;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
            HighScore = PlayerPrefs.GetInt(KeyHighScore, 0);
            HighestStreak = PlayerPrefs.GetInt(KeyHighStreak, 0);
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

        float multiplier = 1f + CurrentStreak * 0.1f;

        int earnedPoints = Mathf.RoundToInt(basePoints * multiplier);

        TotalScore += earnedPoints;
        CurrentStreak++;

        if (CurrentStreak > HighestStreak)
        {
            HighestStreak = CurrentStreak;
            PlayerPrefs.SetInt(KeyHighStreak, HighestStreak);
        }

        if (TotalScore > HighScore)
        {
            HighScore = TotalScore;
            PlayerPrefs.SetInt(KeyHighScore, HighScore);
            PlayerPrefs.Save();
        }
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