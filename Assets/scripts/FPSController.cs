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

    float cameraSpeed = 1000f;

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        rb.useGravity = false;
    }

    private void Update () {
        

        Camera.main.transform.RotateAround(
            Camera.main.transform.position, 
            Vector3.up,
            Input.GetAxis("Mouse X") * cameraSpeed * Time.deltaTime
        );

        Camera.main.transform.RotateAround(
            Camera.main.transform.position, 
            Camera.main.transform.right,
            -Input.GetAxis("Mouse Y") * cameraSpeed * Time.deltaTime
        );
    }

    private void FixedUpdate () {
        Vector3 moveDirection = Input.GetAxis("Vertical") * new Vector3(
            Camera.main.transform.forward.x,
            0,
            Camera.main.transform.forward.z
        ).normalized;

        
        rb.AddForce(moveForce * moveDirection);
    }
}
