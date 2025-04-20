using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactor : MonoBehaviour {
    [SerializeField]
    [Range(0, 5)]
    private float interactiveDistance = 1.6f;
    
    [Header("Максимальный предел обзора")]
    [Tooltip("Насколько бошку поднять-то")]
    [Range(10, 85)]
    public float maxAngle = 80f;

    readonly float cameraSpeed = 800f;

    private Transform cameraTransform;
    GameObject cursor;
    
    private BasicInteractive currInteractive = null;

    void Start () {
        // Блокируем курсор в центре экрана
        Cursor.lockState = CursorLockMode.Locked;
        
        // Скрываем курсор (опционально)
        Cursor.visible = false;
        
        cursor = GameObject.Find("cursor");
        cameraTransform = Camera.main.transform;
    }

    private void HandleMouse () {
        if (Cursor.lockState != CursorLockMode.Locked) return;

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

    void Update() {
        HandleMouse();

        if (currInteractive == null) {
            NonActiveUpdate();
        } else {
            ActiveUpdate();
        }
    }

    private void ActiveUpdate () {
        if (Input.GetKeyUp(KeyCode.Mouse1)) {
            currInteractive.Release();
            currInteractive = null;
        }

        if (Input.GetKeyUp(KeyCode.Mouse0)) {
            currInteractive.Act();
        }
    }

    private void NonActiveUpdate () {
        if (
            Physics.Raycast(
                cameraTransform.position, 
                cameraTransform.forward, 
                out var hit, 
                interactiveDistance
            ) &&
            (hit.collider.includeLayers & LayerMask.GetMask("Interactive")) != 0
        ) {
            cursor.SetActive(true);

            if (Input.GetKeyUp(KeyCode.Mouse0)) {
                hit.collider.gameObject.GetComponent<BasicInteractive>().Act();
            }

            if (Input.GetKeyUp(KeyCode.Mouse1)) {
                currInteractive ??= hit.collider.gameObject.GetComponent<BasicInteractive>().Take(gameObject, Camera.main);

                if (currInteractive != null) cursor.SetActive(false);
            }
        } else {
            cursor.SetActive(false);
        }
    }
}
