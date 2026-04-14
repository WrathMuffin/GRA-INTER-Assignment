using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Playables;

public class characterController : MonoBehaviour
{
    public GameObject cutscene2, prompt;
    public Animator playerAnime;
    bool isCutscene2Trigger = false;

    [Header("Movement")]
    public float baseSpd = 3f, runMulti = 2f,
        groundDrag, 
        jumpforce, 
        jumpCooldown, 
        airMult, 
        stepHeight = .5f, 
        stepSmooth = .1f;

    bool isReadyJump;

    public KeyCode jumpKey = KeyCode.Space;

    [Header("Ground check")]
    public float playerHeight;
    public LayerMask groundLayer;
    bool isGrounded;

    public Transform orientation;

    float moveSpd, horiInput, vertiInput;

    Vector3 moveDir;
    Rigidbody rb;

    private void Start()
    {
        isReadyJump = true;
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        prompt.SetActive(false);
    }

    void Update()
    {
        Vector3 rayStart = transform.position + Vector3.up * .1f;
        Debug.DrawRay(transform.position, Vector3.down * 0.2f, Color.red);
        isGrounded = Physics.Raycast(rayStart, Vector3.down, 0.2f, groundLayer);

        SetInput();
        SpeedControl();

        // drag handling
        if (isGrounded)
        {
            rb.linearDamping = groundDrag;
        }

        else
        {
            rb.linearDamping = 0;
        }

        if (isCutscene2Trigger)
        {
            prompt.SetActive(true);

            if (Input.GetKeyDown(KeyCode.E))
            {
                prompt.SetActive(false);
                cutscene2.SetActive(true);
                transform.position = new Vector3(-6.1f, 10.618f, 98.335f);
                transform.rotation = Quaternion.Euler(0, -45f, 0);
            }
        }

        else
        {
            prompt.SetActive(false);
        }

        float speed = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z).magnitude;
        playerAnime.SetFloat("speed", speed * 10f);
        playerAnime.SetBool("isGrounded", isGrounded);

        bool isRunning = Input.GetKey(KeyCode.LeftShift) && isGrounded;
        playerAnime.SetBool("isRunning", isRunning);
    }

    private void FixedUpdate()
    {
        MovePlayer();
        HandleStep();
    }

    private void SetInput()
    {
        horiInput = Input.GetAxisRaw("Horizontal");
        vertiInput = Input.GetAxisRaw("Vertical");

        if (Input.GetKey(jumpKey) && isReadyJump && isGrounded)
        {
            isReadyJump = false;

            Jump();
            Invoke(nameof(ResetJump), jumpCooldown);
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            moveSpd = baseSpd * runMulti;
        }

        else
        {
            moveSpd = baseSpd;
        }
    }

    private void MovePlayer()
    {
        moveDir = orientation.forward * vertiInput + orientation.right * horiInput;

        if (isGrounded)
        {
            rb.AddForce(moveDir.normalized * moveSpd * 10f, ForceMode.Force);
        }

        else if (!isGrounded)
        {
            rb.AddForce(moveDir.normalized * moveSpd * 10f * airMult, ForceMode.Force);
        }
    }

    void HandleStep()
    {
        Vector3 origin = transform.position + Vector3.up * .05f;

        if (Physics.Raycast(origin, moveDir, .5f))
        {
            Vector3 checkStepOrigin = transform.position + Vector3.up * stepHeight;

            if(!Physics.Raycast(checkStepOrigin, moveDir, .5f))
            {
                rb.position += Vector3.up * stepSmooth;
            }
        }
    }

    private void SpeedControl()
    {         
        Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        if (flatVel.magnitude > moveSpd)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpd;

            rb.linearVelocity = new Vector3(limitedVel.x, rb.linearVelocity.y, limitedVel.z);
        }
    }

    private void Jump()
    {
        // reset y velo
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        rb.AddForce(transform.up * jumpforce, ForceMode.Impulse);

        playerAnime.SetTrigger("jump");
    }

    private void ResetJump()
    {
        isReadyJump = true;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Cutscene 2 trigger")
        {
            isCutscene2Trigger = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Cutscene 2 trigger")
        {
            isCutscene2Trigger = false;
        }
    }
}