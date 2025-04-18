using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class FPSController : MonoBehaviour
{
    [Header("Параметры движения")]
    [Tooltip("Сила, прикладываемая при движении персонажа")]
    public float moveForce = 10f;

    [Header("Параметры прыжка")]
    [Tooltip("Сила, прикладываемая при прыжке")]
    public float jumpForce = 300f;

    [Header("Коэфицикент трения")]
    [Tooltip("Ограничевает максимальную скорость на земле")]
    [Range(0, 1)]
    public float alpha = 1.0f;
    [Tooltip("Ограничевает максимальную скорость в воздухе")]
    [Range(0, 1)]
    public float beta = 1.0f;

    [Header("Максимальный предел обзора")]
    [Tooltip("Насколько бошку поднять-то")]
    [Range(10, 85)]
    public float maxAngle = 80f;

    readonly float cameraSpeed = 1000f;

    private Rigidbody rb;
    private new Collider collider;
    private Transform cameraTransform;

    private void Start () {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;

        collider = GetComponent<Collider>();

        cameraTransform = Camera.main.transform;
    }

    private void Update () {
        HandleMouse();
    }

    private void HandleMouse () { 
        // Получаем значения ввода мыши
        float mouseX =  Input.GetAxis("Mouse X") * cameraSpeed * Time.deltaTime;
        float mouseY = -Input.GetAxis("Mouse Y") * cameraSpeed * Time.deltaTime;
        
        // Вращение по горизонтали (ось Y)
        cameraTransform.RotateAround(
            cameraTransform.position, 
            Vector3.up, 
            mouseX
        );
        
        float angle = Vector3.SignedAngle(
            cameraTransform.forward, 
            Vector3.Scale(cameraTransform.forward, new Vector3(1, 0, 1)),
            cameraTransform.right
        );
        
        if (angle >  maxAngle) mouseY = Mathf.Max(0, mouseY);
        if (angle < -maxAngle) mouseY = Mathf.Min(0, mouseY);

        cameraTransform.RotateAround(
            cameraTransform.position, 
            cameraTransform.right,
            mouseY
        );
    }

    private void FixedUpdate () {
        if (IsGrounded()) {
            MoveGrounded();
        } else {
            MoveNongrounded();
        }
    }

    private bool IsGrounded () {
        return Physics.Raycast(
            new Ray(
                rb.transform.position,
                -rb.transform.up
            ), 
            out _,
            1.2f // TODO: убрать магические константы
        );
    }

    private void MoveNongrounded () {
        Vector3 moveDirection = 
            Input.GetAxis("Vertical")   * Camera.main.transform.forward + 
            Input.GetAxis("Horizontal") * Camera.main.transform.right;

        if (moveDirection.sqrMagnitude > 1) moveDirection.Normalize();

        rb.AddForce(moveForce * moveDirection, ForceMode.Acceleration);
        rb.AddForce(- beta * rb.velocity.magnitude * rb.velocity, ForceMode.Acceleration);
    }

    private void MoveGrounded () {
        Vector3 moveDirection = 
            Input.GetAxis("Vertical")   * Vector3.Scale(Camera.main.transform.forward, new Vector3(1, 0, 1)).normalized + 
            Input.GetAxis("Horizontal") * Camera.main.transform.right;

        if (moveDirection.sqrMagnitude > 1) moveDirection.Normalize();

        rb.AddForce(moveForce * moveDirection, ForceMode.Acceleration);
        rb.AddForce(- alpha * rb.velocity, ForceMode.Acceleration);
    }

    private void DrawCross (Vector3 pos, float size) {
        Debug.DrawLine(pos + Vector3.down * size, pos + Vector3.up      * size, Color.green);
        Debug.DrawLine(pos + Vector3.left * size, pos + Vector3.right   * size, Color.green);
        Debug.DrawLine(pos + Vector3.back * size, pos + Vector3.forward * size, Color.green);
    }
}
