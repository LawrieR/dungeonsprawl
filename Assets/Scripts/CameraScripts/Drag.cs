using UnityEngine;

public class Drag : MonoBehaviour
{
    bool bDragging = false;
    Vector3 oldPos;
    Vector3 panOrigin;
    float panSpeed = 1.9f;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            bDragging = true;
            oldPos = transform.position;
            panOrigin = Camera.main.ScreenToViewportPoint(Input.mousePosition);                    //Get the ScreenVector the mouse clicked
        }

        if (Input.GetMouseButton(0))
        {
            Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition) - panOrigin;    //Get the difference between where the mouse clicked and where it moved
            //Move the position of the camera to simulate a drag, speed * 10 for screen to worldspace conversion
            transform.position = oldPos + -pos * panSpeed * Camera.main.orthographicSize; //Adjust by orth size to maintain drag speed when zooming
            

        }

        if (Input.GetMouseButtonUp(0))
        {
            bDragging = false;
        }
    }


}