using UnityEngine;

 
// Dreht dieses Objekt immer zur Kamera, damit ein schwebender Text aus jeder
// Richtung lesbar bleibt. Kommt auf den schwebenden Text bzw. dessen Canvas.

public class Billboard : MonoBehaviour
{
    private Camera cam;

    private void LateUpdate()
    {
        if (cam == null) cam = Camera.main;
        if (cam == null) return;

        // Textebene parallel zur Kamera ausrichten -> Text bleibt immer lesbar
        transform.forward = cam.transform.forward;
    }
}