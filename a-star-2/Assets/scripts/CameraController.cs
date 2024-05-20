// setari camera
// using UnityEngine;

// public class CameraController : MonoBehaviour
// {
//     public Maze maze;
//     public float height = 170.0f; // e float nu doub;e

//     void Start()
//     {
//         // centrul labirint
//         float centerX = (maze.width / 2) * maze.scale;
//         float centerZ = (maze.depth / 2) * maze.scale;
//         Vector3 mazeCenter = new Vector3(centerX, 0, centerZ);

//         // camera deasupra labirintului
//         Camera.main.transform.position = new Vector3(centerX, height, centerZ-10);

//         // camera in jos cu putina perspectiva
//         Camera.main.transform.rotation = Quaternion.Euler(85, -3, 0);
//         Camera.main.fieldOfView = 110;
//     }
// }




using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Maze maze;
    public float height = 160.0f;
    public float dragSpeed= 2.0f;
    public float zoomSpeed =10.0f;
    public float minZoom = 5.0f;
    public float maxZoom = 500.0f;
    public float rotationSpeed = 100.0f;

    private Vector3 dragOrigin;
    private Vector3 mazeCenter;

    void Start()
    {
        //centru maze
        float centerX = (maze.width / 2) * maze.scale;
        float centerZ = (maze.depth / 2) * maze.scale;
        mazeCenter = new Vector3(centerX, 0, centerZ);

        //camera deasupra
        Camera.main.transform.position = new Vector3(centerX, height, centerZ - 10);

        //roteste camera
        Camera.main.transform.LookAt(mazeCenter);
    }

    void Update()
    {
        HandleMouseDrag();
        HandleMouseZoom();
        HandleMouseRotation();
    }

    void HandleMouseDrag()
    {
        if (Input.GetMouseButtonDown(0))
        {
            dragOrigin = Input.mousePosition;
            return;
        }

        if (!Input.GetMouseButton(0)) return;

        Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - dragOrigin);
        Vector3 move = new Vector3(pos.x * dragSpeed, 0, pos.y * dragSpeed);

        transform.Translate(move, Space.World);
        dragOrigin = Input.mousePosition;
    }

    void HandleMouseZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0.0f)
        {
            //calcul zoom
            Vector3 direction = Camera.main.transform.position - mazeCenter;
            float distance = direction.magnitude;
            direction.Normalize();

            // updat epozitie cmaera
            distance = Mathf.Clamp(distance - scroll * zoomSpeed, minZoom, maxZoom);
            Camera.main.transform.position = mazeCenter + direction * distance;
        }
    }

    void HandleMouseRotation()
    {
        if (Input.GetMouseButtonDown(1))
        {
            dragOrigin = Input.mousePosition;
            return;
        }

        if (!Input.GetMouseButton(1)) return;

        float rotX = Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
        float rotY = -Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime;

        // rotire 360
        Camera.main.transform.RotateAround(mazeCenter, Vector3.up, rotX);
        Camera.main.transform.RotateAround(mazeCenter, Camera.main.transform.right, rotY);

        Camera.main.transform.LookAt(mazeCenter);
    }
}



