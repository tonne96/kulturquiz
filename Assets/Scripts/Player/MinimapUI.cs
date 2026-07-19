using UnityEngine;

// Zeichnet eine einfache Radar-Minimap unten rechts.
// Zeigt zusätzlich Score, aktuelle Streak und höchste Streak über der Minimap.
public class MinimapUI : MonoBehaviour
{
    [Header("Spieler-Referenz")]
    public Transform player;

    [Header("Minimap-Einstellungen")]
    public float mapSize = 200f;
    public float worldRadius = 50f;
    public int margin = 16;

    [Header("Farben")]
    public Color backgroundColor = new Color(0f, 0f, 0f, 0.6f);
    public Color borderColor = new Color(1f, 1f, 1f, 0.5f);
    public Color playerColor = Color.cyan;
    public Color npcColor = Color.yellow;

    [Header("Punkt-Größen")]
    public float playerDotSize = 10f;
    public float npcDotSize = 8f;


    private Texture2D _bgTex;
    private Texture2D _playerTex;
    private Texture2D _npcTex;

    private NPCQuizGiver[] _npcs;
    public static bool QuizActive = false;

    private GUIStyle _hudStyle;
    private GUIStyle _streakStyle;
    private bool _stylesInitialized = false;


    private void Start()
    {
        if (player == null)
            player = transform;


        _bgTex = MakeTex(backgroundColor);
        _playerTex = MakeTex(playerColor);
        _npcTex = MakeTex(npcColor);


        _npcs = FindObjectsByType<NPCQuizGiver>(FindObjectsSortMode.None);
    }


    private void InitializeGUIStyles()
    {
        _hudStyle = new GUIStyle(GUI.skin.label);

        _hudStyle.fontSize = 18;
        _hudStyle.fontStyle = FontStyle.Bold;
        _hudStyle.normal.textColor = Color.white;
        _hudStyle.alignment = TextAnchor.UpperRight;


        _streakStyle = new GUIStyle(_hudStyle);

        _streakStyle.fontSize = 22;
        _streakStyle.normal.textColor = Color.white;


        _stylesInitialized = true;
    }


    private void OnGUI()
    {
        if (QuizActive)
            return;

        if (!_stylesInitialized)
            InitializeGUIStyles();


        float x = Screen.width - mapSize - margin;
        float y = Screen.height - mapSize - margin;



        //--------------------------------------------------
        // Score / Streak HUD
        //--------------------------------------------------

        if (GameScoreManager.Instance != null)
        {
            int streak = GameScoreManager.Instance.CurrentStreak;


            Color streakColor = Color.white;


            if (streak >= 20)
                streakColor = new Color(0.7f, 0.2f, 1f); // Purple
            else if (streak >= 15)
                streakColor = Color.red;
            else if (streak >= 10)
                streakColor = new Color(1f, 0.55f, 0f); // Orange
            else if (streak >= 5)
                streakColor = Color.yellow;


            _streakStyle.normal.textColor = streakColor;


            GUI.Label(
                new Rect(x, y - 125, mapSize, 25),
                $"Best Score: {GameScoreManager.Instance.HighScore}",
                _hudStyle);

            GUI.Label(
                new Rect(x, y - 95, mapSize, 25),
                $"Highest Streak: x{GameScoreManager.Instance.HighestStreak}",
                _hudStyle);


            GUI.Label(
                new Rect(x, y - 65, mapSize, 30),
                $"Streak: x{GameScoreManager.Instance.CurrentStreak}",
                _streakStyle);


            GUI.Label(
                new Rect(x, y - 35, mapSize, 25),
                $"Score: {GameScoreManager.Instance.TotalScore}",
                _hudStyle);
        }



        //--------------------------------------------------
        // Minimap
        //--------------------------------------------------

        GUI.DrawTexture(
            new Rect(x - 2, y - 2, mapSize + 4, mapSize + 4),
            _bgTex);


        DrawBorder(
            new Rect(x - 2, y - 2, mapSize + 4, mapSize + 4),
            2,
            borderColor);



        Vector3 playerPos = player.position;



        // NPCs anzeigen
        foreach (var npc in _npcs)
        {
            if (npc == null)
                continue;


            Vector3 offset = npc.transform.position - playerPos;


            float dx = offset.x / worldRadius;
            float dz = offset.z / worldRadius;


            if (Mathf.Abs(dx) > 1f || Mathf.Abs(dz) > 1f)
                continue;



            float dotX =
                x + (dx + 1f) * 0.5f * mapSize - npcDotSize * 0.5f;


            float dotY =
                y + (1f - (dz + 1f) * 0.5f) * mapSize - npcDotSize * 0.5f;



            GUI.DrawTexture(
                new Rect(dotX, dotY, npcDotSize, npcDotSize),
                _npcTex);
        }



        // Spieler in der Mitte

        float px =
            x + mapSize * 0.5f - playerDotSize * 0.5f;


        float py =
            y + mapSize * 0.5f - playerDotSize * 0.5f;



        GUI.DrawTexture(
            new Rect(px, py, playerDotSize, playerDotSize),
            _playerTex);



        // Blickrichtung
        DrawDirectionArrow(
            new Rect(x, y, mapSize, mapSize),
            player.eulerAngles.y);
    }



    private void DrawDirectionArrow(Rect area, float yaw)
    {
        float rad = (yaw - 90f) * Mathf.Deg2Rad;


        float cx = area.x + area.width * 0.5f;
        float cy = area.y + area.height * 0.5f;


        float len = playerDotSize * 1.8f;


        float ex = cx + Mathf.Cos(rad) * len;
        float ey = cy + Mathf.Sin(rad) * len;



        DrawLine(
            new Vector2(cx, cy),
            new Vector2(ex, ey),
            playerColor,
            2f);
    }



    private static void DrawLine(Vector2 from, Vector2 to, Color color, float thickness)
    {
        Color previous = GUI.color;

        GUI.color = color;


        Vector2 direction = to - from;

        float angle =
            Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;


        float length = direction.magnitude;



        GUIUtility.RotateAroundPivot(angle, from);


        GUI.DrawTexture(
            new Rect(from.x, from.y - thickness * 0.5f, length, thickness),
            Texture2D.whiteTexture);



        GUIUtility.RotateAroundPivot(-angle, from);


        GUI.color = previous;
    }



    private static void DrawBorder(Rect rect, float thickness, Color color)
    {
        Color previous = GUI.color;

        GUI.color = color;



        GUI.DrawTexture(
            new Rect(rect.x, rect.y, rect.width, thickness),
            Texture2D.whiteTexture);


        GUI.DrawTexture(
            new Rect(rect.x, rect.yMax - thickness, rect.width, thickness),
            Texture2D.whiteTexture);


        GUI.DrawTexture(
            new Rect(rect.x, rect.y, thickness, rect.height),
            Texture2D.whiteTexture);


        GUI.DrawTexture(
            new Rect(rect.xMax - thickness, rect.y, thickness, rect.height),
            Texture2D.whiteTexture);



        GUI.color = previous;
    }



    private static Texture2D MakeTex(Color color)
    {
        Texture2D texture = new Texture2D(1, 1);

        texture.SetPixel(0, 0, color);

        texture.Apply();

        return texture;
    }
}