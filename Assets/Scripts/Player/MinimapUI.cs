using UnityEngine;

// Zeichnet eine einfache Radar-Minimap unten rechts.
// Dieses Script auf den Spieler legen (oder ein leeres GameObject in der Szene).
public class MinimapUI : MonoBehaviour
{
    [Header("Spieler-Referenz")]
    public Transform player;

    [Header("Minimap-Einstellungen")]
    public float mapSize = 200f;          // Pixelgröße der Map (quadratisch)
    public float worldRadius = 50f;       // Wie viele Einheiten der Map-Radius abdeckt
    public int margin = 16;              // Abstand vom Bildschirmrand

    [Header("Farben")]
    public Color backgroundColor = new Color(0f, 0f, 0f, 0.6f);
    public Color borderColor = new Color(1f, 1f, 1f, 0.5f);
    public Color playerColor = Color.cyan;
    public Color npcColor = Color.yellow;
    public Color npcDoneColor = Color.gray;

    [Header("Punkt-Größen")]
    public float playerDotSize = 10f;
    public float npcDotSize = 8f;

    private Texture2D _bgTex;
    private Texture2D _playerTex;
    private Texture2D _npcTex;
    private Texture2D _npcDoneTex;
    private NPCQuizGiver[] _npcs;

    private void Start()
    {
        if (player == null)
            player = transform;

        _bgTex = MakeTex(backgroundColor);
        _playerTex = MakeTex(playerColor);
        _npcTex = MakeTex(npcColor);
        _npcDoneTex = MakeTex(npcDoneColor);

        _npcs = FindObjectsByType<NPCQuizGiver>(FindObjectsSortMode.None);
    }

    private void OnGUI()
    {
        float x = Screen.width - mapSize - margin;
        float y = Screen.height - mapSize - margin;

        // Hintergrund
        GUI.DrawTexture(new Rect(x - 2, y - 2, mapSize + 4, mapSize + 4), _bgTex);
        // Rahmen
        DrawBorder(new Rect(x - 2, y - 2, mapSize + 4, mapSize + 4), 2, borderColor);

        Vector3 playerPos = player.position;

        // NPCs einzeichnen
        foreach (var npc in _npcs)
        {
            if (npc == null) continue;
            Vector3 offset = npc.transform.position - playerPos;
            float dx = offset.x / worldRadius;
            float dz = offset.z / worldRadius;
            if (Mathf.Abs(dx) > 1f || Mathf.Abs(dz) > 1f) continue;

            float dotX = x + (dx + 1f) * 0.5f * mapSize - npcDotSize * 0.5f;
            float dotY = y + (1f - (dz + 1f) * 0.5f) * mapSize - npcDotSize * 0.5f;

            Texture2D tex = _npcTex;
            GUI.DrawTexture(new Rect(dotX, dotY, npcDotSize, npcDotSize), tex);

            // NPC-Name
            // GUI.Label(new Rect(dotX + npcDotSize + 2, dotY - 2, 80, 16), npc.npcName);
        }

        // Spieler in der Mitte
        float px = x + mapSize * 0.5f - playerDotSize * 0.5f;
        float py = y + mapSize * 0.5f - playerDotSize * 0.5f;
        GUI.DrawTexture(new Rect(px, py, playerDotSize, playerDotSize), _playerTex);

        // Himmelsrichtungs-Pfeil (zeigt Blickrichtung)
        DrawDirectionArrow(new Rect(x, y, mapSize, mapSize), player.eulerAngles.y);

        // Titel
        // GUI.Label(new Rect(x, y - 18, mapSize, 16), "Minimap");
    }

    private void DrawDirectionArrow(Rect area, float yaw)
    {
        // Kleiner Richtungspfeil: Linie vom Mittelpunkt in Blickrichtung
        float rad = (yaw - 90f) * Mathf.Deg2Rad;
        float cx = area.x + area.width * 0.5f;
        float cy = area.y + area.height * 0.5f;
        float len = playerDotSize * 1.8f;
        float ex = cx + Mathf.Cos(rad) * len;
        float ey = cy + Mathf.Sin(rad) * len;
        DrawLine(new Vector2(cx, cy), new Vector2(ex, ey), playerColor, 2f);
    }

    private static void DrawLine(Vector2 from, Vector2 to, Color color, float thickness)
    {
        Color prev = GUI.color;
        GUI.color = color;
        Vector2 d = to - from;
        float angle = Mathf.Atan2(d.y, d.x) * Mathf.Rad2Deg;
        float len = d.magnitude;
        GUIUtility.RotateAroundPivot(angle, from);
        GUI.DrawTexture(new Rect(from.x, from.y - thickness * 0.5f, len, thickness), Texture2D.whiteTexture);
        GUIUtility.RotateAroundPivot(-angle, from);
        GUI.color = prev;
    }

    private static void DrawBorder(Rect rect, float thickness, Color color)
    {
        Color prev = GUI.color;
        GUI.color = color;
        GUI.DrawTexture(new Rect(rect.x, rect.y, rect.width, thickness), Texture2D.whiteTexture);
        GUI.DrawTexture(new Rect(rect.x, rect.yMax - thickness, rect.width, thickness), Texture2D.whiteTexture);
        GUI.DrawTexture(new Rect(rect.x, rect.y, thickness, rect.height), Texture2D.whiteTexture);
        GUI.DrawTexture(new Rect(rect.xMax - thickness, rect.y, thickness, rect.height), Texture2D.whiteTexture);
        GUI.color = prev;
    }

    private static Texture2D MakeTex(Color color)
    {
        var tex = new Texture2D(1, 1);
        tex.SetPixel(0, 0, color);
        tex.Apply();
        return tex;
    }
}
