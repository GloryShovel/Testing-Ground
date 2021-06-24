using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float speed, rotateSpeed;
    public bool noInvertMouse;

    float right, forward, up, rotateX, rotateY;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        right = Input.GetAxis("Horizontal") * speed * Time.deltaTime;
        forward = Input.GetAxis("Vertical") * speed * Time.deltaTime;
        up = Input.GetAxis("Up") * speed * Time.deltaTime;

        transform.Translate(right,up, forward);

        rotateX = Input.GetAxis("Mouse Y") * rotateSpeed;
        rotateY = Input.GetAxis("Mouse X") * rotateSpeed;

        if (noInvertMouse)
        {
            rotateX *= -1;
        }

        transform.RotateAround(transform.position, transform.right, rotateX);
        transform.RotateAround(transform.position, Vector3.up, rotateY);

    }
}
