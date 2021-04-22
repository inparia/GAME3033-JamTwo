using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    private CharacterController myCharacterController;

    private float walkMovementX, walkMovementZ;
    private Vector2 mousePos;
    private float mouseDown;
    private bool isMouseDown;
    private bool groundedPlayer;
    private float gravityValue = -9.81f;
    private Vector3 playerVelocity;
    private float speed;
    public bool underCeiling;
    private Vector3 moveDirection = Vector3.zero;


    private float sensitivity;
    private Vector2 mouseReference;
    private Vector2 mouseOffset;
    private Vector2 rotation;
    private bool isRotating;

    private bool ableToTake, readyToTake;

    private float range;
    private Transform target;
    private Box targetbox;
    private string boxTag = "Box";
    public TMPro.TextMeshProUGUI boxText;
    private Animator animator;
    public int counter;
    public int playerHealth;
    // Start is called before the first frame update
    void Start()
    {
        myCharacterController = GetComponent<CharacterController>();
        underCeiling = false;
        sensitivity = 0.2f;
        rotation = Vector2.zero;
        isMouseDown = false;
        Cursor.lockState = CursorLockMode.Locked;
        InvokeRepeating("UpdateTarget", 0f, 0.5f);
        range = 5f;
        counter = 0;
        ableToTake = false;
        readyToTake = false;
        animator = GetComponent<Animator>();
        speed = 10;
        playerHealth = 100;
    }

    void UpdateTarget()
    {
        GameObject[] boxes = GameObject.FindGameObjectsWithTag(boxTag);

        float shortestDistance = Mathf.Infinity;
        GameObject nearestBox = null;

        foreach (GameObject box in boxes)
        {
            float distanceToBox = Vector3.Distance(transform.position, box.transform.position);
            if (distanceToBox < shortestDistance)
            {
                shortestDistance = distanceToBox;
                nearestBox = box;
            }
        }

        if (nearestBox != null && shortestDistance <= range)
        {
            target = nearestBox.transform;
            targetbox = nearestBox.GetComponent<Box>();
            ableToTake = true;

            if (readyToTake)
            {
                Destroy(nearestBox);
                counter++;
                ableToTake = false;
                readyToTake = false;
            }
            boxText.gameObject.SetActive(true);
        }
        else
        {
            boxText.gameObject.SetActive(false);
            target = null;
            ableToTake = false;
            readyToTake = false;
        }

    }
    // Update is called once per frame
    void Update()
    {
        playerVelocity.y += gravityValue * Time.deltaTime;

        moveDirection = transform.TransformDirection(new Vector3(walkMovementX * Time.deltaTime * speed, playerVelocity.y * Time.deltaTime, walkMovementZ * Time.deltaTime * speed));

        if (walkMovementX == 0 && walkMovementZ == 0)
        {
            animator.SetBool("isWalk", false);
            animator.SetBool("isWalkBackward", false);
        }
        myCharacterController.Move(moveDirection);

        groundedPlayer = myCharacterController.isGrounded;

        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        }


        if (isRotating)
        {
            // offset
            mouseOffset = (mousePos - mouseReference);

            // apply rotation
            rotation.y = (mouseOffset.x + mouseOffset.y) * sensitivity;

            // rotate
            transform.Rotate(rotation);

            // store mouse
            if(!isMouseDown)
                mouseReference = mousePos;
        }
        else
        {
            rotation = Vector2.zero;
        }

    }
    public void takeBox()
    {
        if(ableToTake)
        {
            readyToTake = true;
        }
    }
    public void upDown(InputAction.CallbackContext context)
    {
        walkMovementZ = context.ReadValue<float>();
        if (walkMovementZ > 0)
        {
            speed = 10;
            animator.SetBool("isWalk", true);
        }
        else if(walkMovementZ < 0)
        {
            speed = 3;
            animator.SetBool("isWalkBackward", true);
        }
    }

    public void LeftRight(InputAction.CallbackContext context)
    {
        walkMovementX = context.ReadValue<float>();
        animator.SetBool("isWalk", true);
    }

    public void getMousePos(InputAction.CallbackContext context)
    {
        mousePos = context.ReadValue<Vector2>();
        Debug.Log(mousePos);
    }
    public void onMouseDown(InputAction.CallbackContext context)
    {
        mouseDown = context.ReadValue<float>();
        if(mouseDown == 1)
        {
            isRotating = true;
            mouseReference = Vector2.zero;
            if (!isMouseDown)
                mouseReference = mousePos;
            isMouseDown = true;
        }
        else
        {
            isMouseDown = false;
            isRotating = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Building")
        {
            underCeiling = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Building")
        {
            underCeiling = false;
        }
    }

}
