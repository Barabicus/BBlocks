using UnityEngine;
using System.Collections;
using System;

public class Player : Entity, IWorldAnchor
{

    public float gravity = 8f;
    public float drag = 5f;
    public float rotatePower = 5f;
    public float movePower = 50f;
    public float jumpPower = 10f;
    public World world;
    public Transform highlightBlock;
    private Vector3 vel;
    private const int cursorWidth = 20;
    private Texture2D cursor;
    private Type currentBlock = typeof(StoneBlock);


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

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
            currentBlock = typeof(VirusBlock);
        if (Input.GetKeyDown(KeyCode.Alpha2))
            currentBlock = typeof(StoneBlock);

        if (Input.GetKeyDown(KeyCode.T))
        {
            Screen.lockCursor = !Screen.lockCursor;
        }

        transform.Rotate(Vector3.up * Input.GetAxis("Mouse X") * rotatePower * Time.deltaTime);
        transform.Rotate(Vector3.right * -Input.GetAxis("Mouse Y") * rotatePower * Time.deltaTime);
        transform.rotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, 0);

        vel = Vector3.Lerp(vel, new Vector3(0, -gravity, 0) + new Vector3(0, vel.y, 0), Time.deltaTime * drag);

        vel += Quaternion.Euler(0, transform.localEulerAngles.y, transform.localEulerAngles.z) * Vector3.forward * Input.GetAxis("Vertical") * movePower;
        vel += Quaternion.Euler(0, transform.localEulerAngles.y, transform.localEulerAngles.z) * Vector3.right * Input.GetAxis("Horizontal") * movePower;

        if (controller.isGrounded)
        {
            vel.y = -1f;
            if (Input.GetKeyDown(KeyCode.Space))
            {
                vel += new Vector3(0, gravity, 0) * jumpPower;
            }
        }

        controller.Move(vel * Time.deltaTime);

        RaycastHit hitt;

        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitt, 100f))
        {
            highlightBlock.gameObject.SetActive(true);
            Vector3 pos;
            world.RaycastHitToBlock(hitt, out pos);
            pos += new Vector3(0.5f, -0.5f, 0.5f);
            highlightBlock.transform.position = pos;
        }
        else
        {
            highlightBlock.gameObject.SetActive(false);
        }


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
                    world.SetBlockWorldCoordinate(position, (IBlock)Activator.CreateInstance(currentBlock));
              //  IntVector3 position = world.RaycastHitToBlock(hit);
             //   Debug.Log("Block: " + world.GetBlockWorldCoordinate(position));

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

    public IntVector3 AnchorPosition
    {
        get { return new IntVector3(transform.position); }
    }

    public IntVector3 AnchorBounds
    {
        get { return new IntVector3(28, 10, 28); }
    }
}
