using UnityEngine;
using TMPro;

public class OptionsMenu : MonoBehaviour
{
    [SerializeField] private TMP_Text difficultyText;

    private readonly string[] difficulties =
    {
        "EASY",
        "NORMAL",
        "HARD",
        "ANY"
    };

    public void UpdateDifficultyText(float value)
    {
        difficultyText.text = difficulties[Mathf.RoundToInt(value) - 1];
    }

}
