using UnityEngine;


/// Einfache Ego-Perspektive: WASD zum Laufen, Maus zum Umsehen.
/// Kommt auf den Spieler (mit CharacterController). Die Kamera ist ein Kind-Objekt.

[RequireComponent(typeof(CharacterController))]
public class SimpleFirstPersonController : MonoBehaviour
{
    [Header("Bewegung")]
    public float moveSpeed = 4f;
    public float gravity = -9.81f;
    public float jumpHeight = 1.2f;

    [Header("Maus")]
    public float mouseSensitivity = 2f;
    public Transform cameraTransform;   // die Kind-Kamera

    private CharacterController controller;
    private float verticalVelocity;
    private float pitch;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;
    }

    private void Update()
    {
        // Umsehen
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;
        transform.Rotate(Vector3.up * mouseX);
        pitch = Mathf.Clamp(pitch - mouseY, -80f, 80f);
        if (cameraTransform != null)
            cameraTransform.localEulerAngles = new Vector3(pitch, 0f, 0f);

        // Laufen
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector3 move = (transform.right * h + transform.forward * v) * moveSpeed;

        // Schwerkraft und Springen
        if (controller.isGrounded && verticalVelocity < 0f) verticalVelocity = -2f;
        if (controller.isGrounded && Input.GetButtonDown("Jump"))
            verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
        verticalVelocity += gravity * Time.deltaTime;
        move.y = verticalVelocity;

        controller.Move(move * Time.deltaTime);
    }
}