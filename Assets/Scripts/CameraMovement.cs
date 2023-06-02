using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] float moveSpeed = 0.5f, aimSpeed = 0.05f;
    float currentSpeed;
    Camera mainCam;
    // Start is called before the first frame update
    void Start()
    {
        mainCam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.W))
        {
            mainCam.transform.position += mainCam.transform.forward * currentSpeed;
        }
        if (Input.GetKey(KeyCode.A))
        {
            mainCam.transform.position -= mainCam.transform.right * currentSpeed;
        }
        if (Input.GetKey(KeyCode.S))
        {
            mainCam.transform.position -= mainCam.transform.forward * currentSpeed;
        }
        if (Input.GetKey(KeyCode.D))
        {
            mainCam.transform.position += mainCam.transform.right * currentSpeed;
        }

        if (Input.GetKey(KeyCode.LeftControl))
        {
            currentSpeed = aimSpeed;
        }
        else
        {
            currentSpeed = moveSpeed;
        }

        if (Input.GetKey(KeyCode.Mouse1))
        {
            float deltaY = Input.GetAxis("Mouse Y") * currentSpeed * 10;
            mainCam.transform.RotateAround(mainCam.transform.position, mainCam.transform.right, deltaY);
            float deltaX = Input.GetAxis("Mouse X") * currentSpeed * 10;
            mainCam.transform.RotateAround(mainCam.transform.position, transform.up, deltaX);
        }
        if (Input.GetKey(KeyCode.Mouse0))
        {
           
        }
    }
}
