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

    private Rigidbody rb;

    private void Start () {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
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
