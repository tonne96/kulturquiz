using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

/// Holt Fragen von der Open Trivia Database (opentdb.com).
/// Nutzt UnityWebRequest in Coroutines mit Callbacks.
public class OpenTriviaClient : MonoBehaviour
{
    private const string ApiUrl = "https://opentdb.com/api.php";
    private const string TokenUrl = "https://opentdb.com/api_token.php";

    // Session-Token sorgt dafür, dass Fragen sich nicht wiederholen,
    // bis alle aufgebraucht sind (dann Response Code 4).
    private string sessionToken;

    /// <summary>Optional: Einmal beim Start aufrufen, um doppelte Fragen zu vermeiden.</summary>
    public IEnumerator RequestToken(Action onComplete = null)
    {
        using (var req = UnityWebRequest.Get($"{TokenUrl}?command=request"))
        {
            yield return req.SendWebRequest();

            if (req.result == UnityWebRequest.Result.Success)
            {
                var data = JsonUtility.FromJson<TokenResponse>(req.downloadHandler.text);
                sessionToken = data.token;
            }
            else
            {
                Debug.LogWarning($"[OpenTDB] Token-Request fehlgeschlagen: {req.error}");
            }
            onComplete?.Invoke();
        }
    }

    /// <summary>
    /// Fragen abrufen.
    /// 9 = General Knowledge 
    /// 10 = Entertainment: Books, 
    /// 11 = Entertainment: Film, 
    /// 12 = Entertainment: Music, 
    /// 13 = Entertainment: Musicals & Theatres, 
    /// 14 = Entertainment: Television, 
    /// 15 = Entertainment: Video Games, 
    /// 16 = Entertainment: Board Games, 
    /// 17 = Science & Nature, 
    /// 18 = Science: Computers, 
    /// 19 = Science: Mathematics, 
    /// 20 = Mythology, 
    /// 21 = Sports, 
    /// 22 = Geography, 
    /// 23 = History, 
    /// 24 = Politics, 
    /// 25 = Art, 
    /// 26 = Celebrities, 
    /// 27 = Animals, 
    /// 28 = Vehicles, 
    /// 29 = Entertainment: Comics, 
    /// 30 = Science: Gadgets, 
    /// 31 = Entertainment: Japanese Anime & Manga, 
    /// 32 = Entertainment: Cartoon & Animations.
    /// difficulty / type: null lassen für "egal".
    /// </summary>
    public IEnumerator GetQuestions(
        int amount,
        Action<TriviaQuestion[]> onSuccess,
        Action<string> onError = null,
        int category = 0,
        string difficulty = null,
        string type = null)
    {   
        string url = $"{ApiUrl}?amount={amount}";
        if (category > 0)                       url += $"&category={category}";
        if (!string.IsNullOrEmpty(difficulty))  url += $"&difficulty={GameSettings.Difficulty}";
        if (!string.IsNullOrEmpty(type))        url += $"&type={type}";
        if (!string.IsNullOrEmpty(sessionToken)) url += $"&token={sessionToken}";

        using (var req = UnityWebRequest.Get(url))
        {
            yield return req.SendWebRequest();

            // OpenTDB erlaubt nur 1 Request pro IP alle 5 Sekunden -> sonst HTTP 429 / Code 5.
            if (req.responseCode == 429)
            {
                onError?.Invoke("Rate Limit: Bitte 5 Sekunden warten und erneut versuchen.");
                yield break;
            }

            if (req.result != UnityWebRequest.Result.Success)
            {
                onError?.Invoke(req.error);
                yield break;
            }

            TriviaResponse response;
            try
            {
                response = JsonUtility.FromJson<TriviaResponse>(req.downloadHandler.text);
            }
            catch (Exception e)
            {
                onError?.Invoke($"JSON-Parsing fehlgeschlagen: {e.Message}");
                yield break;
            }

            if (response.response_code != 0)
            {
                onError?.Invoke(DescribeCode(response.response_code));
                yield break;
            }

            foreach (var q in response.results)
                q.DecodeHtml();

            onSuccess?.Invoke(response.results.ToArray());
        }
    }

    private static string DescribeCode(int code)
    {
        switch (code)
        {
            case 1: return "Keine Ergebnisse: Nicht genug Fragen für diese Auswahl.";
            case 2: return "Ungültiger Parameter.";
            case 3: return "Token nicht gefunden.";
            case 4: return "Token leer: Alle Fragen verbraucht (Token zurücksetzen).";
            case 5: return "Rate Limit: max. 1 Request alle 5 Sekunden.";
            default: return $"Unbekannter Response Code {code}.";
        }
    }

    [Serializable]
    private class TokenResponse
    {
        public int response_code;
        public string token;
        public string response_message;
    }
}