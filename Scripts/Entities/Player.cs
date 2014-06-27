using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{

    public float rotatePower = 5f;
    public float movePower = 50f;
    public float jumpPower = 10f;
    public World world;
    private Vector3 vel;

    CharacterController controller;

    // Use this for initialization
    void Start()
    {
        controller = GetComponent<CharacterController>();
        Screen.lockCursor = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            Screen.lockCursor = !Screen.lockCursor;
        }

        transform.Rotate(Vector3.up * Input.GetAxis("Mouse X") * rotatePower * Time.deltaTime);
        transform.Rotate(Vector3.right * -Input.GetAxis("Mouse Y") * rotatePower * Time.deltaTime);
        transform.rotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, 0);

        vel += Physics.gravity;
        if (controller.isGrounded)
        {
            vel = Vector3.zero;
            vel += transform.forward * Input.GetAxis("Vertical") * movePower;
            vel += transform.right * Input.GetAxis("Horizontal") * movePower;

        }
        controller.Move(vel * Time.deltaTime);

        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;

            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100f))
            {
                IntVector3 position = world.RaycastHitToBlock(hit);
                world.SetBlockWorldCoordinate(position, null);
            }
        }
        if (Input.GetMouseButtonDown(1))
        {
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100f))
            {
                IntVector3 position = world.RaycastHitToFace(hit);
                world.SetBlockWorldCoordinate(position, new SlantedGrassBlock());

            }
        }

    }
}
