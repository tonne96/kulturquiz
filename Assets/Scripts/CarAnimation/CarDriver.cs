using UnityEngine;

// Auf das Auto-Root-GameObject legen.
// Das Auto fährt von startPoint zu endPoint, verschwindet und startet neu.
public class CarDriver : MonoBehaviour
{
    [Header("Strecke")]
    public Vector3 startPoint;
    public Vector3 endPoint;

    [Header("Fahrt")]
    public float speed = 8f;
    [Tooltip("Zufällige Startversetzung entlang der Strecke (0–1), damit Autos nicht alle gleichzeitig starten")]
    [Range(0f, 1f)]
    public float startOffset = 0f;

    [Header("Räder (optional)")]
    public Transform wheelFL;
    public Transform wheelFR;
    public Transform wheelRL;
    public Transform wheelRR;
    [Tooltip("Radius eines Rades in World Units")]
    public float wheelRadius = 0.35f;

    private float _progress;   // 0 = Start, 1 = Ende
    private float _totalLength;

    private void Start()
    {
        _totalLength = Vector3.Distance(startPoint, endPoint);
        _progress = startOffset;
        PlaceCar();

        // Fahrtrichtung einmalig ausrichten
        if (_totalLength > 0.001f)
            transform.forward = (endPoint - startPoint).normalized;
    }

    private void Update()
    {
        if (_totalLength < 0.001f) return;

        _progress += speed * Time.deltaTime / _totalLength;

        if (_progress >= 1f)
        {
            _progress = 0f;
        }

        PlaceCar();
        SpinWheels();
    }

    private void PlaceCar()
    {
        transform.position = Vector3.Lerp(startPoint, endPoint, _progress);
    }

    private void SpinWheels()
    {
        // Bogenlänge pro Frame → Winkel = arc / radius (in Grad)
        float distPerFrame = speed * Time.deltaTime;
        float angleDeg = (distPerFrame / wheelRadius) * Mathf.Rad2Deg;

        RotateWheel(wheelFL, angleDeg);
        RotateWheel(wheelFR, angleDeg);
        RotateWheel(wheelRL, angleDeg);
        RotateWheel(wheelRR, angleDeg);
    }

    private static void RotateWheel(Transform wheel, float angleDeg)
    {
        if (wheel != null)
            wheel.Rotate(Vector3.right, angleDeg, Space.Self);
    }

    // Gizmos: Start/End als Kugeln + Linie in der Scene-View anzeigen
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(startPoint, 0.4f);
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(endPoint, 0.4f);
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(startPoint, endPoint);
    }
}
