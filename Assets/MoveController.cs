using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
[RequireComponent(typeof(CharacterController))]

public class MoveController : MonoBehaviour
{
    CharacterController characterController;
    [Header("MOVEMENT")]
    public float walkingSpeed;
    public float runningSpeed;
    Vector3 moveDirection = Vector3.zero;
    [Header("JUMP")]
    [SerializeField] private float jumpTime;
    private float gravity;
    [SerializeField] private float jumpHeight;
    [Header("CAMERA")]
    [SerializeField]private float lookYLimit;
    public Camera playerCamera;
    private float sensivity = 3;
    float rotationX = 0;
    [Header("FLASHLIGHT")]
    [SerializeField] private GameObject flashlight;
    bool isFlashlightOn = false;
    [Header("RAYCAST")]
    [SerializeField] private float rayDistance;
    [Header("TIPS")]
    [SerializeField] private GameObject doorTip;
    [SerializeField] private GameObject dialogTip;  
    [Header("COMPONENTS")]
    [SerializeField] Dialog dialog;
    [HideInInspector]
    private float startJumpVelocity;
    public bool isDialogGoingOn = false;
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    void Update()
    {
        if (!isDialogGoingOn)
        {
            Vector3 forward = transform.TransformDirection(Vector3.forward).normalized;
            Vector3 right = transform.TransformDirection(Vector3.right).normalized;
            bool isRunning = Input.GetKey(KeyCode.LeftShift);
            float curSpeedX = (isRunning ? runningSpeed : walkingSpeed) * Input.GetAxis("Vertical");
            float curSpeedY = (isRunning ? runningSpeed : walkingSpeed) * Input.GetAxis("Horizontal");
            float movementDirectionY = moveDirection.y;
            float maxHeightTime = jumpTime / 2;
            gravity = (2 * jumpHeight) / Mathf.Pow(maxHeightTime, 2);
            startJumpVelocity = (2 * jumpHeight) / jumpTime;
            moveDirection = (forward * curSpeedX) + (right * curSpeedY);
            moveDirection = moveDirection.normalized * Mathf.Max(Mathf.Abs(curSpeedX), Mathf.Abs(curSpeedY));
            //if (Input.GetKey(KeyCode.Space) && characterController.isGrounded)
            //{
            //    moveDirection.y = startJumpVelocity;
            //}
            //else
            //{
            moveDirection.y = movementDirectionY;
            //}
            if (!characterController.isGrounded)
            {
                moveDirection.y -= gravity * Time.deltaTime;
            }
            characterController.Move(moveDirection * Time.deltaTime);
            rotationX += -Input.GetAxis("Mouse Y") * sensivity;
            rotationX = Mathf.Clamp(rotationX, -lookYLimit, lookYLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * sensivity, 0);
            flashlight.transform.localRotation = playerCamera.transform.localRotation;
            if (Input.GetKeyUp(KeyCode.F) && isFlashlightOn == false)
            {
                isFlashlightOn = true;
                flashlight.SetActive(isFlashlightOn);
            }
            else if(Input.GetKeyUp(KeyCode.F) && isFlashlightOn == true)
            {
                isFlashlightOn = false;
                flashlight.SetActive(isFlashlightOn);
            }
        }
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(ray, out hit, rayDistance);
        Debug.DrawLine(ray.origin, ray.origin + ray.direction * rayDistance, Color.red);
        if (hit.collider!=null &&(hit.collider.CompareTag("Door") || hit.collider.CompareTag("Npc")))
        {
            if (hit.collider.CompareTag("Door"))
            {
                doorTip.SetActive(true);
                if (Input.GetKey(KeyCode.E))
                {
                    StartCoroutine(hit.collider.GetComponent<DoorBehavior>().OpenDoor());
                }


            }
            if (hit.collider.CompareTag("Npc"))
            {
                dialogTip.SetActive(true);
                if (Input.GetKeyDown(KeyCode.E) && !isDialogGoingOn)
                {
                    isDialogGoingOn = true;
                    dialog.StartDialog(hit.collider.GetComponent<DialogMessages>());
                }
                if(isDialogGoingOn == true && Input.GetKeyDown(KeyCode.Space)) 
                {
                    dialog.ShowNextMessage();
                
                }
            }
        }
        else
        {
            doorTip.SetActive(false);
            dialogTip.SetActive(false);
        }

    }

}