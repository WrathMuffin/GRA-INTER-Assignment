using UnityEngine;

public class ThirdPersonCam : MonoBehaviour
{
    [Header("References")]
    public Transform orientation, player, playerObj, cam;
    public Rigidbody rb;

    public float rotSpeeed = 5f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        // rotate orientaiton
        Vector3 camForward = cam.forward;
        Vector3 flatForward = new Vector3(camForward.x, 0, camForward.z).normalized;
        orientation.forward = flatForward;

        // rotate player
        float horiInput = Input.GetAxisRaw("Horizontal");
        float vertInput = Input.GetAxisRaw("Vertical");

        Vector3 inputDir = orientation.forward * vertInput + orientation.right * horiInput;

        if(inputDir != Vector3.zero)
        {
            playerObj.forward = Vector3.Slerp(playerObj.forward, inputDir.normalized, Time.deltaTime * rotSpeeed);
        }
    }
}
