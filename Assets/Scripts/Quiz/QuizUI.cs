using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// Zeigt das Quiz-Fenster, Fragen und Antwort-Buttons. Wird über BeginQuiz(category)
/// eingeblendet und meldet sich per OnQuizClosed wieder ab (damit der Spieler weiterlaufen darf).
public class QuizUI : MonoBehaviour
{
    [Header("Referenzen")]
    public QuizManager quizManager;
    public GameObject quizPanel;          // gesamtes Quiz-Fenster (wird ein-/ausgeblendet)
    public TMP_Text questionText;
    public Transform answersContainer;    // leeres Objekt mit Vertical Layout Group
    public Button answerButtonPrefab;     // Prefab: Button + TMP_Text als Kind

    [Header("Optional")]
    public TMP_Text scoreText;
    public GameObject endPanel;
    public TMP_Text endText;
    public Button continueButton;         // "Weiter"-Button auf dem End-Panel

    [Header("Verhalten")]
    public float delayAfterAnswer = 1.2f;

    public event System.Action OnQuizClosed;

    private void OnEnable()
    {
        quizManager.OnQuestionChanged += ShowQuestion;
        quizManager.OnQuizFinished += ShowResult;
        quizManager.OnError += ShowError;
    }

    private void OnDisable()
    {
        quizManager.OnQuestionChanged -= ShowQuestion;
        quizManager.OnQuizFinished -= ShowResult;
        quizManager.OnError -= ShowError;
    }

    private void Start()
    {
        if (quizPanel != null) quizPanel.SetActive(false);
        if (endPanel != null) endPanel.SetActive(false);
        if (continueButton != null) continueButton.onClick.AddListener(CloseQuiz);
    }

    /// <summary>Vom Spieler aufgerufen: blendet das Quiz ein und lädt eine Kategorie.</summary>
    public void BeginQuiz(int category)
    {
        if (quizPanel != null) quizPanel.SetActive(true);
        if (endPanel != null) endPanel.SetActive(false);
        ClearAnswers();
        questionText.text = "loading…";
        quizManager.StartQuiz(category);
    }

    private void ShowQuestion(TriviaQuestion q)
    {
        questionText.text = q.question;
        UpdateScore();
        ClearAnswers();

        foreach (string answer in q.GetShuffledAnswers())
        {
            Button btn = Instantiate(answerButtonPrefab, answersContainer);
            var label = btn.GetComponentInChildren<TMP_Text>();
            if (label != null) label.text = answer;

            string captured = answer; // lokale Kopie für den Closure
            btn.onClick.AddListener(() => OnAnswerClicked(captured, btn));
        }
    }

    private void OnAnswerClicked(string answer, Button clicked)
    {
        bool correct = quizManager.SubmitAnswer(answer);

        foreach (Transform child in answersContainer)
        {
            var b = child.GetComponent<Button>();
            if (b != null) b.interactable = false;
        }
        var colors = clicked.colors;
        colors.disabledColor = correct ? new Color(0.5f, 1f, 0.5f) : new Color(1f, 0.5f, 0.5f);
        clicked.colors = colors;

        UpdateScore();
        StartCoroutine(NextAfterDelay());
    }

    private IEnumerator NextAfterDelay()
    {
        yield return new WaitForSeconds(delayAfterAnswer);
        quizManager.NextQuestion();
    }

    private void ShowResult(int score, int total)
    {
        ClearAnswers();
        questionText.text = "";
        if (endPanel != null) endPanel.SetActive(true);
        if (endText != null) endText.text = $"Fertig!\n{score} / {total} richtig";
    }

    private void ShowError(string message)
    {
        questionText.text = $"Fehler: {message}";
    }

    /// <summary>Schließt das Fenster und meldet das an den Spieler (Bewegung wieder frei).</summary>
    public void CloseQuiz()
    {
        if (quizPanel != null) quizPanel.SetActive(false);
        OnQuizClosed?.Invoke();
    }

    private void ClearAnswers()
    {
        foreach (Transform child in answersContainer)
            Destroy(child.gameObject);
    }

    private void UpdateScore()
    {
        if (scoreText != null)
            scoreText.text = $"Punkte: {quizManager.Score} / {quizManager.Total}";
    }
}