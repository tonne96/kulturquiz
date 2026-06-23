using System;
using System.Collections.Generic;
using System.Net;

/// Entspricht der JSON-Antwort von https://opentdb.com/api.php
/// Wichtig: JsonUtility matcht Feldnamen exakt -> deshalb snake_case wie im JSON.
[Serializable]
public class TriviaResponse
{
    public int response_code;             // 0 = OK, 1 = keine Ergebnisse, 5 = Rate Limit ...
    public List<TriviaQuestion> results;
}

[Serializable]
public class TriviaQuestion
{
    public string category;
    public string type;                   // "multiple" oder "boolean"
    public string difficulty;             // "easy" / "medium" / "hard"
    public string question;
    public string correct_answer;
    public string[] incorrect_answers;

    /// <summary>
    /// OpenTDB liefert HTML-Entities (z.B. &quot; &#039; &amp;). Hier dekodieren,
    /// damit die Texte sauber in der UI angezeigt werden.
    /// </summary>
    public void DecodeHtml()
    {
        question = WebUtility.HtmlDecode(question);
        correct_answer = WebUtility.HtmlDecode(correct_answer);
        if (incorrect_answers != null)
        {
            for (int i = 0; i < incorrect_answers.Length; i++)
                incorrect_answers[i] = WebUtility.HtmlDecode(incorrect_answers[i]);
        }
    }

    /// <summary>
    /// Liefert alle Antworten (richtig + falsch) in zufälliger Reihenfolge zurück.
    /// </summary>
    public List<string> GetShuffledAnswers()
    {
        var all = new List<string>();
        if (incorrect_answers != null) all.AddRange(incorrect_answers);
        all.Add(correct_answer);

        // Fisher-Yates Shuffle
        for (int i = all.Count - 1; i > 0; i--)
        {
            int j = UnityEngine.Random.Range(0, i + 1);
            string tmp = all[i];
            all[i] = all[j];
            all[j] = tmp;
        }
        return all;
    }
}