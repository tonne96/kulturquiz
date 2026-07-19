public static class GameSettings
{
    public static string Difficulty = null;

    public static float DifficultySliderValue
    {
        get => UnityEngine.PlayerPrefs.GetFloat("DifficultySlider", 4f);
        set => UnityEngine.PlayerPrefs.SetFloat("DifficultySlider", value);
    }
}