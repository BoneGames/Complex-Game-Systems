using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraOrbit : MonoBehaviour
{

    // target to roatate around
    public Transform target;
    // Distance the camera is from world zero
    public float distance = 10f;
    // X and Y rotation speed
    public float xSpeed = 120f;
    public float ySpeed = 120f;
    // X and Y rotation limits
    public float yMin = 15f;
    public float yMax = 80f;
    // Current x and y rotation
    private float x = 0f;
    private float y = 0f;
    void Start()
    {
        // Get current rotation of camera
        Vector3 euler = transform.eulerAngles;
        x = euler.y;
        y = euler.x;
    }


    void LateUpdate()
    {
        // Get input x and y offsets
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");
        // Is the Right mouse button pressed?
        if (Input.GetMouseButton(1))
        {
            // hide cursor
            Cursor.visible = false;
            // Offset rotation with mouse X and Y offsets
            x += mouseX * xSpeed * Time.deltaTime;
            y -= mouseY * ySpeed * Time.deltaTime;
            // Clamp the Y between mins and max limits
            y = Mathf.Clamp(y, yMin, yMax);
        }
        // right mouse button up
        else
        {
            Cursor.visible = true;
        }
        // move camera pivot point
        if (Input.GetMouseButton(2))
        {
            Cursor.visible = false;
            // Get input x and y offsets
            target.transform.position += this.transform.right * mouseX * xSpeed * Time.deltaTime;
            target.transform.position += this.transform.up * mouseY * ySpeed * Time.deltaTime;
        }
        // Update Transform
        transform.rotation = Quaternion.Euler(y, x, 0);
        transform.position = target.position - transform.forward * distance;
    }
}
