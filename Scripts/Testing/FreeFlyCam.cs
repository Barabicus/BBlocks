using UnityEngine;
using System.Collections;

public class FreeFlyCam : MonoBehaviour
{

    public float moveSpeed = 10f;
    public float rotateSpeed = 10f;
    public World world;

    private Texture2D cursor;
    private const int cursorWidth = 20;

    // Use this for initialization
    void Start()
    {
        // Screen.lockCursor = true;

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

    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.T))
            Screen.lockCursor = !Screen.lockCursor;

        transform.position += transform.forward * Input.GetAxis("Vertical") * Time.deltaTime * moveSpeed;
        transform.position += transform.right * Input.GetAxis("Horizontal") * Time.deltaTime * moveSpeed;
        transform.position += Vector3.up * Input.GetAxis("UpDown") * Time.deltaTime * moveSpeed;
        if (Screen.lockCursor)
        {
            transform.Rotate(Vector3.up, Input.GetAxis("Mouse X") * Time.deltaTime * rotateSpeed, Space.World);
            transform.Rotate(Vector3.right, -Input.GetAxis("Mouse Y") * Time.deltaTime * rotateSpeed);
            transform.rotation = Quaternion.Euler(transform.localEulerAngles.x, transform.localEulerAngles.y, 0);
        }

        if (Input.GetMouseButton(2))
        {
            RaycastHit hit;

            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100f))
            {
                IntVector3 position = world.RaycastHitToBlock(hit);
                world.SetBlockWorldCoordinate(position, null);
            }
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
                world.SetBlockWorldCoordinate(position, new SlantedGrassBlock());

            }
        }
    }



    void OnGUI()
    {
        GUI.DrawTexture(new Rect((Screen.width / 2) - cursorWidth / 2, (Screen.height / 2) - cursorWidth / 2, cursorWidth, cursorWidth), cursor);
    }
}
