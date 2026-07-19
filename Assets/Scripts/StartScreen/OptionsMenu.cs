using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OptionsMenu : MonoBehaviour
{
    [SerializeField] private TMP_Text difficultyText;
    [SerializeField] private Slider difficultySlider;

    private readonly string[] difficultyDisplay =
    {
        "EASY",
        "NORMAL",
        "HARD",
        "ANY"
    };

    private readonly string[] difficultyValues =
    {
        "easy",
        "medium",
        "hard",
        null
    };


    private void Start()
    {
        // Restore previous selection
        difficultySlider.value = GameSettings.DifficultySliderValue;

        UpdateDifficultyText(difficultySlider.value);
    }


    public void UpdateDifficultyText(float value)
    {
        int index = Mathf.RoundToInt(value) - 1;

        difficultyText.text = difficultyDisplay[index];

        GameSettings.Difficulty = difficultyValues[index];
        GameSettings.DifficultySliderValue = value;
    }
}