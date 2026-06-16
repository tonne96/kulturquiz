using UnityEngine;
using TMPro;

/// <summary>
/// Kommt auf den Spieler. Findet den nächsten NPC in Reichweite, zeigt einen Hinweis
/// ("[E] sprechen") und startet bei Tastendruck dessen Quiz. Während des Quiz wird die
/// Bewegung gesperrt und der Mauszeiger freigegeben.
/// </summary>
public class PlayerInteractor : MonoBehaviour
{
    [Header("Referenzen")]
    public QuizUI quizUI;
    public MonoBehaviour movementController;  // das Bewegungsskript (wird gesperrt)
    public GameObject interactPrompt;         // UI-Element "Drücke E" (optional)
    public TMP_Text promptText;               // optional

    [Header("Einstellungen")]
    public float interactRange = 3f;
    public KeyCode interactKey = KeyCode.E;
    public LayerMask npcLayer = ~0;           // ~0 = alle Layer

    private NPCQuizGiver currentTarget;
    private bool quizActive;

    private void Start()
    {
        if (interactPrompt != null) interactPrompt.SetActive(false);
        if (quizUI != null) quizUI.OnQuizClosed += EndQuiz;
        LockCursor(true);
    }

    private void OnDestroy()
    {
        if (quizUI != null) quizUI.OnQuizClosed -= EndQuiz;
    }

    private void Update()
    {
        if (quizActive) return;

        FindNearestNpc();

        if (currentTarget != null && Input.GetKeyDown(interactKey))
            BeginQuiz();
    }

    private void FindNearestNpc()
    {
        currentTarget = null;
        float best = float.MaxValue;

        var colliders = Physics.OverlapSphere(transform.position, interactRange, npcLayer);
        foreach (var col in colliders)
        {
            var npc = col.GetComponentInParent<NPCQuizGiver>();
            if (npc == null) continue;

            float d = Vector3.Distance(transform.position, npc.transform.position);
            if (d < best) { best = d; currentTarget = npc; }
        }

        if (interactPrompt != null)
        {
            interactPrompt.SetActive(currentTarget != null);
            if (currentTarget != null && promptText != null)
                promptText.text = $"[{interactKey}]\nspeak with {currentTarget.npcName}";
        }
    }

    private void BeginQuiz()
    {
        quizActive = true;
        if (interactPrompt != null) interactPrompt.SetActive(false);
        if (movementController != null) movementController.enabled = false;
        LockCursor(false);
        quizUI.BeginQuiz(currentTarget.category);
    }

    private void EndQuiz()
    {
        quizActive = false;
        if (movementController != null) movementController.enabled = true;
        LockCursor(true);
    }

    private void LockCursor(bool locked)
    {
        Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !locked;
    }
}