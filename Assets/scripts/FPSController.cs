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
    [Tooltip("Ограничевает максимальную скорость")]
    public float alpha = 1.0f;

    float cameraSpeed = 1000f;

    private Rigidbody rb;

    private void Start () {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
    }

    private void Update () {
        HandleMouse();        
    }

    private void HandleMouse () {
        // Кэшируем ссылки на камеру и её трансформ для производительности
        Camera mainCamera = Camera.main;
        Transform cameraTransform = mainCamera.transform;
        
        // Получаем значения ввода мыши
        float mouseX = Input.GetAxis("Mouse X") * cameraSpeed * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * cameraSpeed * Time.deltaTime;
        
        // Вращение по горизонтали (ось Y)
        cameraTransform.RotateAround(
            cameraTransform.position, 
            Vector3.up, 
            mouseX
        );
        
        // Вращение по вертикали (ось right камеры)
        // Добавляем ограничение угла, чтобы избежать переворота камеры
        Vector3 right = cameraTransform.right;


        float newVerticalAngle = Vector3.Angle(
            cameraTransform.forward, 
            new Vector3(cameraTransform.position.x, 0, cameraTransform.position.z)
            );
        
        float verticalRotation = -mouseY;
        
        // Ограничиваем угол вращения по вертикали (например, от -80 до 80 градусов)
        if (newVerticalAngle > 80f && newVerticalAngle < 80f) {
            cameraTransform.RotateAround(
                cameraTransform.position, 
                right, 
                verticalRotation
            );
        }
    }

    private void FixedUpdate () {
        Vector3 moveDirection = 
            Input.GetAxis("Vertical")   * Camera.main.transform.forward + 
            Input.GetAxis("Horizontal") * Camera.main.transform.right;

        if (moveDirection.sqrMagnitude > 1) moveDirection.Normalize();

        rb.AddForce(moveForce * moveDirection, ForceMode.Acceleration);
        rb.AddForce(- alpha * rb.velocity.magnitude * rb.velocity, ForceMode.Acceleration);
    }

    private void DrawCross (Vector3 pos, float size) {
        Debug.DrawLine(pos + Vector3.down * size, pos + Vector3.up      * size, Color.green);
        Debug.DrawLine(pos + Vector3.left * size, pos + Vector3.right   * size, Color.green);
        Debug.DrawLine(pos + Vector3.back * size, pos + Vector3.forward * size, Color.green);
    }
}
