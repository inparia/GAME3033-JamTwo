using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public enum CameraStatus
{
    SetOne,
    SetTwo,
    SetThree,
    SetFour,
    SetFive
}
public class Player : MonoBehaviour
{
    private CharacterController myCharacterController;

    [Header("Player Status")]
    private float playerHealth;
    private float walkMovementX, walkMovementZ;
    private bool groundedPlayer;
    private float gravityValue = -9.81f;
    private Vector3 playerVelocity;
    private float speedX, speedZ;
    private bool underCeiling;
    private Vector3 moveDirection = Vector3.zero;

    [Header("Mouse Input")]
    private Vector2 mousePos;
    private float mouseDown;
    private bool isMouseDown;
    private float sensitivity;
    private Vector2 mouseReference;
    private Vector2 mouseOffset;
    private Vector2 rotation;
    private bool isRotating;

    [Header("Item")]
    private bool ableToTake, readyToTake;
    private float range;
    private Transform target;
    private Box targetbox;
    private string boxTag = "Box";

    [Header("UI")]
    public TMPro.TextMeshProUGUI boxText;

    [Header("Animator")]
    private Animator animator;
    
    [Header("Weather System")]
    public GameObject weatherSystem;

    [Header("Camera")]
    public Camera camera;
    public CameraStatus cameraStatus;

    [Header("Game Status")]
    public float gameTimer;
    public int counter;
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
        speedX = 5;
        speedZ = 5;
        playerHealth = 100;
        gameTimer = 0;
        cameraStatus = CameraStatus.SetThree;
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
        

        gameTimer += Time.deltaTime;

        playerVelocity.y += gravityValue * Time.deltaTime;

        moveDirection = transform.TransformDirection(new Vector3(walkMovementX * Time.deltaTime * speedX, playerVelocity.y * Time.deltaTime, walkMovementZ * Time.deltaTime * speedZ));

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

        if(weatherSystem.GetComponent<WeatherSystem>().rainStart)
        {
            if(!underCeiling)
            {
                playerHealth -= Time.deltaTime * 1;
            }
        }

        if(counter == 7)
        {
            SceneManager.LoadScene("Win");
        }

        if(playerHealth <= 0)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            myCharacterController.enabled = false;
            animator.SetBool("isDead", true);
            delayScene(3, "Lose");
        }
        checkCameraStatus();
        HealthBar.SetHealthBarValue(playerHealth / 100);
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
            speedZ = 5;
            animator.SetBool("isWalk", true);
            animator.SetBool("isWalkBackward", false);
        }
        else if(walkMovementZ < 0)
        {
            speedZ = 3;
            animator.SetBool("isWalk", false);
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

    public void mouseScroll(InputAction.CallbackContext context)
    {
        Debug.Log(context.ReadValue<float>());
        if(context.ReadValue<float>() > 0)
        {
            switch(cameraStatus)
            {
                case CameraStatus.SetOne:
                    break;
                case CameraStatus.SetTwo:
                    cameraStatus = CameraStatus.SetOne;
                    break;
                case CameraStatus.SetThree:
                    cameraStatus = CameraStatus.SetTwo;
                    break;
                case CameraStatus.SetFour:
                    cameraStatus = CameraStatus.SetThree;
                    break;
                case CameraStatus.SetFive:
                    cameraStatus = CameraStatus.SetFour;
                    break;
            }
        }
        else if (context.ReadValue<float>() < 0)
        {
            switch (cameraStatus)
            {
                case CameraStatus.SetOne:
                    cameraStatus = CameraStatus.SetTwo;
                    break;
                case CameraStatus.SetTwo:
                    cameraStatus = CameraStatus.SetThree;
                    break;
                case CameraStatus.SetThree:
                    cameraStatus = CameraStatus.SetFour;
                    break;
                case CameraStatus.SetFour:
                    cameraStatus = CameraStatus.SetFive;
                    break;
                case CameraStatus.SetFive:
                    break;
            }
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
    public void delayScene(float delay, string SceneName)
    {
        StartCoroutine(LoadLevelAfterDelay(delay, SceneName));
    }
    IEnumerator LoadLevelAfterDelay(float delay, string SceneName)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(SceneName);
    }

    void checkCameraStatus()
    {
        switch (cameraStatus)
        {
            case CameraStatus.SetOne:
                camera.transform.localPosition = new Vector3(0, 2, -4);
                break;
            case CameraStatus.SetTwo:
                camera.transform.localPosition = new Vector3(0, 3, -6);
                break;
            case CameraStatus.SetThree:
                camera.transform.localPosition = new Vector3(0, 4, -8);
                break;
            case CameraStatus.SetFour:
                camera.transform.localPosition = new Vector3(0, 5, -10);
                break;
            case CameraStatus.SetFive:
                camera.transform.localPosition = new Vector3(0, 6, -12);
                break;
        }
    }
}
