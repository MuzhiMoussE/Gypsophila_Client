using UnityEngine;

namespace ReflectiveProjectionDemo
{
    public class FirstPersonCamera : MonoBehaviour
    {
        public float walkingSpeed = 7.5f;
        public float runningSpeed = 11.5f;
        public float gravity = 20.0f;
        public Camera playerCamera;
        public float lookSpeed = 2.0f;
        public float lookXLimit = 45.0f;

        CharacterController characterController;
        Vector3 moveDirection = Vector3.zero;
        float rotationX = 0;

        [HideInInspector] public bool canMove = true;


        void Start()
        {
            characterController = GetComponent<CharacterController>();

            // Lock cursor
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }


        void Update()
        {
            // Player and Camera rotation
            if (canMove) {
                rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
                rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
                playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
                transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
            }
        }
    }
}

