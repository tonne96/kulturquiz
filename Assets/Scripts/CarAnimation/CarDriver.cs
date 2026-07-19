using UnityEngine;

// Auf das Auto-Root-GameObject legen.
// Das Auto fährt eine Waypoint-Route ab (z.B. als Quadrat/Rechteck).
public class CarDriver : MonoBehaviour
{
    [Header("Route")]
    public Transform[] waypoints;   // Im Inspector befüllen — mind. 2 Punkte
    public bool loop = true;

    [Header("Fahrt")]
    public float speed = 8f;
    [Range(0, 10)]
    [Tooltip("Bei welchem Waypoint das Auto startet")]
    public int startWaypointIndex = 0;

    [Header("Räder (optional)")]
    public Transform wheelFL;
    public Transform wheelFR;
    public Transform wheelRL;
    public Transform wheelRR;
    public float wheelRadius = 0.35f;

    private int _current;
    private int _next;

    private void Start()
    {
        if (waypoints == null || waypoints.Length < 2) return;

        _current = startWaypointIndex % waypoints.Length;
        _next = (_current + 1) % waypoints.Length;

        transform.position = waypoints[_current].position;
        transform.forward = (waypoints[_next].position - waypoints[_current].position).normalized;
    }

    private void Update()
    {
        if (waypoints == null || waypoints.Length < 2) return;

        Vector3 target = waypoints[_next].position;
        Vector3 dir = (target - transform.position);

        float step = speed * Time.deltaTime;

        if (dir.magnitude <= step)
        {
            // Waypoint erreicht → weiter zum nächsten
            transform.position = target;
            _current = _next;
            _next = (_current + 1) % waypoints.Length;

            if (!loop && _next == 0)
            {
                enabled = false;
                return;
            }

            transform.forward = (waypoints[_next].position - transform.position).normalized;
        }
        else
        {
            transform.position += dir.normalized * step;
            transform.forward = dir.normalized;
        }

        SpinWheels();
    }

    private void SpinWheels()
    {
        float angleDeg = (speed * Time.deltaTime / wheelRadius) * Mathf.Rad2Deg;
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

    private void OnDrawGizmosSelected()
    {
        if (waypoints == null || waypoints.Length < 2) return;

        Gizmos.color = Color.yellow;
        for (int i = 0; i < waypoints.Length; i++)
        {
            if (waypoints[i] == null) continue;
            int next = (i + 1) % waypoints.Length;
            if (waypoints[next] == null) continue;
            Gizmos.DrawLine(waypoints[i].position, waypoints[next].position);
            Gizmos.DrawSphere(waypoints[i].position, 0.4f);
        }
    }
}
