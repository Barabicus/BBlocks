using UnityEngine;
using System.Collections;

public class Player : Entity
{

    public float drag = 5f;
    public float rotatePower = 5f;
    public float movePower = 50f;
    public float jumpPower = 10f;
    public World world;
    private Vector3 vel;
    private const int cursorWidth = 20;
    private Texture2D cursor;


    public CharacterController controller;

    // Use this for initialization
    void Start()
    {

        //Create the cursor
        cursor = new Texture2D(cursorWidth, cursorWidth);

        for (int x = 0; x < cursorWidth; x++)
        {
            for (int y = 0; y < cursorWidth; y++)
            {
                cursor.SetPixel(x, y, Color.clear);
            }
        }

        for (int x = 0; x < cursorWidth; x++)
        {
            cursor.SetPixel(x, cursorWidth / 2, Color.red);
        }

        for (int y = 0; y < cursorWidth; y++)
        {
            cursor.SetPixel(cursorWidth / 2, y, Color.red);
        }
        cursor.Apply();

       // controller = GetComponent<CharacterController>();
        Screen.lockCursor = true;
    }

    void Awake()
    {
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

        vel = Vector3.Lerp(vel, Physics.gravity + new Vector3(0, vel.y, 0), Time.deltaTime * drag);

        //  vel = Vector3.zero;
        //  vel += (new Vector3(transform.forward.x, 0, transform.forward.z))* Input.GetAxis("Vertical") * movePower;
        vel.x = transform.forward.x * Input.GetAxis("Vertical") * movePower;
        vel.z = transform.forward.z * Input.GetAxis("Vertical") * movePower;
        vel += transform.right * Input.GetAxis("Horizontal") * movePower;

        if (controller.isGrounded)
        {
            vel.y = -1f;
            if (Input.GetKeyDown(KeyCode.Space))
            {
                vel += -Physics.gravity * jumpPower;
            }
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
                world.SetBlockWorldCoordinate(position, new VirusBlock());

            }
        }
        if (Input.GetMouseButton(2))
        {
            RaycastHit hit;

            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100f))
            {
                IntVector3 position = world.RaycastHitToBlock(hit);
                world.SetBlockWorldCoordinate(position, null);
                world.SetBlockWorldCoordinate(position + new IntVector3(0, 1, 0), null);
                world.SetBlockWorldCoordinate(position + new IntVector3(0, -1, 0), null);
                world.SetBlockWorldCoordinate(position + new IntVector3(1, 0, 0), null);
                world.SetBlockWorldCoordinate(position + new IntVector3(-1, 0, 0), null);
           //     world.SetBlockWorldCoordinate(position + new IntVector3(0, 0, 1), null);
          //      world.SetBlockWorldCoordinate(position + new IntVector3(0, 0, -1), null);

                world.SetBlockWorldCoordinate(position + new IntVector3(1, 1, 0), null);
                world.SetBlockWorldCoordinate(position + new IntVector3(-1, 1, 0), null);
                world.SetBlockWorldCoordinate(position + new IntVector3(1, -1, 0), null);
                world.SetBlockWorldCoordinate(position + new IntVector3(-1, -1, 0), null);




            }
        }

    }

    void OnGUI()
    {
        GUI.DrawTexture(new Rect((Screen.width / 2) - cursorWidth / 2, (Screen.height / 2) - cursorWidth / 2, cursorWidth, cursorWidth), cursor);
    }
}
