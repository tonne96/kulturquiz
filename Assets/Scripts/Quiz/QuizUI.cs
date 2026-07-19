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
    public GameObject quizPanel;
    public TMP_Text questionText;
    public Transform answersContainer;
    public Button answerButtonPrefab;

    [Header("Optional")]
    public TMP_Text scoreText;
    public GameObject endPanel;
    public TMP_Text endText;
    public Button continueButton;

    [Header("Verhalten")]
    public float delayAfterAnswer = 1.2f;

    public event System.Action OnQuizClosed;


    private TriviaQuestion currentQuestion;



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
        if (quizPanel != null)
            quizPanel.SetActive(false);

        if (endPanel != null)
            endPanel.SetActive(false);

        if (continueButton != null)
            continueButton.onClick.AddListener(CloseQuiz);
    }



    /// <summary>
    /// Vom Spieler aufgerufen: blendet das Quiz ein und lädt eine Kategorie.
    /// </summary>
    public void BeginQuiz(int category)
    {
        if (quizPanel != null)
            quizPanel.SetActive(true);

        if (endPanel != null)
            endPanel.SetActive(false);


        ClearAnswers();

        questionText.text = "loading…";

        quizManager.StartQuiz(category);
    }



    private void ShowQuestion(TriviaQuestion q)
    {
        currentQuestion = q;


        questionText.text = q.question;

        UpdateScore();

        ClearAnswers();



        foreach (string answer in q.GetShuffledAnswers())
        {
            Button btn = Instantiate(answerButtonPrefab, answersContainer);


            TMP_Text label = btn.GetComponentInChildren<TMP_Text>();

            if (label != null)
                label.text = answer;



            string captured = answer;


            btn.onClick.AddListener(() =>
                OnAnswerClicked(captured, btn));
        }
    }





    private void OnAnswerClicked(string answer, Button clicked)
    {
        bool correct = quizManager.SubmitAnswer(answer);



        // Disable all buttons
        foreach (Transform child in answersContainer)
        {
            Button b = child.GetComponent<Button>();

            if (b != null)
                b.interactable = false;
        }



        // Color selected answer

        var clickedColors = clicked.colors;


        if (correct)
        {
            // Strong green for chosen correct answer
            clickedColors.disabledColor =
                new Color(0.35f, 1f, 0.35f);
        }
        else
        {
            // Red for wrong chosen answer
            clickedColors.disabledColor =
                new Color(1f, 0.4f, 0.4f);



            // Find and highlight correct answer
            foreach (Transform child in answersContainer)
            {
                Button b = child.GetComponent<Button>();

                if (b == null)
                    continue;


                TMP_Text label = b.GetComponentInChildren<TMP_Text>();


                if (label != null &&
                    label.text == currentQuestion.correct_answer)
                {
                    var correctColors = b.colors;


                    // Softer green for revealed answer
                    correctColors.disabledColor =
                        new Color(0.55f, 1f, 0.55f);


                    b.colors = correctColors;
                }
            }
        }


        clicked.colors = clickedColors;



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


        if (endPanel != null)
            endPanel.SetActive(true);


        if (endText != null)
            endText.text =
                $"Fertig!\n{score} / {total} richtig";
    }





    private void ShowError(string message)
    {
        questionText.text =
            $"Fehler: {message}";
    }





    /// <summary>
    /// Schließt das Fenster und meldet das an den Spieler.
    /// </summary>
    public void CloseQuiz()
    {
        if (quizPanel != null)
            quizPanel.SetActive(false);


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
            scoreText.text =
                $"Punkte: {quizManager.Score} / {quizManager.Total}";
    }
}