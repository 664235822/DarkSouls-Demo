using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public PlayerInput playerInput;
    public GameObject playerHandle;
    public GameObject cameraHandle;
    public GameObject model;
    public GameObject camera;

    public float MouseXSpeed = 200.0f;
    public float MouseYSpeed = 100.0f;
    public float cameraDampValue = 0.01f;

    private float tempEulerX = 20.0f;
    private Vector3 smoothDampVelocity;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 tempModelEuler = model.transform.eulerAngles;
        
        playerHandle.transform.Rotate(Vector3.up, playerInput.cameraRight * MouseXSpeed * Time.fixedDeltaTime);
        
        tempEulerX -= playerInput.cameraUp * MouseYSpeed * Time.fixedDeltaTime;
        tempEulerX = Mathf.Clamp(tempEulerX, -40, 30);
        cameraHandle.transform.localEulerAngles = new Vector3(tempEulerX, 0, 0);

        model.transform.eulerAngles = tempModelEuler;

        camera.transform.position = Vector3.SmoothDamp(camera.transform.position, transform.position,
            ref smoothDampVelocity, cameraDampValue);
        camera.transform.eulerAngles = transform.eulerAngles;
    }
}
