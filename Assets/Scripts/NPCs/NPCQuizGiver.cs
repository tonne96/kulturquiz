using UnityEngine;

/// <summary>
/// Kommt auf jeden NPC. Hält Name und Quiz-Kategorie dieses NPCs.
/// Kategorie-IDs von OpenTDB, z.B.:
///   9 = Allgemeinwissen, 17 = Wissenschaft, 21 = Sport, 22 = Geografie,
///   23 = Geschichte, 11 = Film, 12 = Musik, 18 = Computer, 27 = Tiere.
/// (Volle Liste: https://opentdb.com/api_config.php)
/// </summary>
public class NPCQuizGiver : MonoBehaviour
{
    [Header("Quiz dieses NPCs")]
    public string npcName = "Quizmaster";
    public int category = 9;

    [TextArea]
    public string greeting = "Ready for some ___ questions?";
}