using System.Collections;
using UnityEngine;

/// Steuert den Quiz-Ablauf. Startet NICHT mehr automatisch, sondern erst über
/// StartQuiz(category) – z.B. wenn der Spieler einen NPC anspricht.
[RequireComponent(typeof(OpenTriviaClient))]
public class QuizManager : MonoBehaviour
{
    [Header("Quiz-Einstellungen")]
    public int questionAmount = 5;
    //public string difficulty = GameSettings.Difficulty;   // "easy" / "medium" / "hard" / null |  Abfrage wird direkt aus dem Slider abgelesen
    public string type = "multiple";   // "multiple" / "boolean" / null

    // Events für die UI
    public event System.Action<TriviaQuestion> OnQuestionChanged;
    public event System.Action<int, int> OnQuizFinished;   // (score, total)
    public event System.Action<string> OnError;

    private OpenTriviaClient client;
    private TriviaQuestion[] questions;
    private int currentIndex = -1;
    private int score;
    private int category;

    public int Score => score;
    public int Total => questions?.Length ?? 0;
    public TriviaQuestion CurrentQuestion =>
        (questions != null && currentIndex >= 0 && currentIndex < questions.Length)
            ? questions[currentIndex] : null;

    private void Awake()
    {
        client = GetComponent<OpenTriviaClient>();
    }

    /// <summary>Startet ein Quiz für eine OpenTDB-Kategorie (0 = beliebig).</summary>
    public void StartQuiz(int category)
    {
        this.category = category;
        StopAllCoroutines();
        StartCoroutine(LoadQuiz());
    }

    private IEnumerator LoadQuiz()
    {
        // Hinweis: KEIN Session-Token-Request direkt davor – OpenTDB erlaubt nur
        // 1 Request alle 5 Sekunden pro IP, sonst gibt der zweite Call ein Rate Limit.
        yield return client.GetQuestions(
            questionAmount,
            onSuccess: result =>
            {
                questions = result;
                score = 0;
                currentIndex = -1;
                NextQuestion();
            },
            onError: err =>
            {
                Debug.LogError($"[Quiz] {err}");
                OnError?.Invoke(err);
            },
            category: category,
            difficulty: GameSettings.Difficulty,
            type: type
        );
    }

    public void NextQuestion()
    {
        if (questions == null) return;

        currentIndex++;
        if (currentIndex >= questions.Length)
        {
            OnQuizFinished?.Invoke(score, questions.Length);
            return;
        }
        OnQuestionChanged?.Invoke(CurrentQuestion);
    }

    public bool SubmitAnswer(string answer)
    {
        var q = CurrentQuestion;
        if (q == null) return false;

        bool correct = answer == q.correct_answer;
        if (correct)
        {
            // Counts correct answers in this quiz
            score++;

            // Adds permanent/game score
            GameScoreManager.Instance.AddQuestionScore(q.difficulty);
        }
        return correct;
    }
}