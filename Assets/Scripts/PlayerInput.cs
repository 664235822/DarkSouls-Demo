using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    [Header("Keys Setting")]
    public string keyUp;
    public string keyDown;
    public string keyLeft;
    public string keyRight;
    
    public string keyA;
    public string keyB;
    public string keyX;
    public string keyY;

    [Header("Direction Outputs")]
    public float directionUp;
    public float directionRight;

    public float directionMagnitude;
    public Vector3 directionVector;

    public bool run;

    [Header("Other Settings")]
    public bool inputEnabled = true;

    private float targetDirectionUp;
    private float targetDirectionRight;
    private float velocityDirectionUp;
    private float velocityDirectionRight;

    // Update is called once per frame
    void Update()
    {
        targetDirectionUp = (Input.GetKey(keyUp) ? 1.0f : 0) - (Input.GetKey(keyDown) ? 1.0f : 0);
        targetDirectionRight = (Input.GetKey(keyRight) ? 1.0f : 0) - (Input.GetKey(keyLeft) ? 1.0f : 0);

        if (!inputEnabled)
        {
            targetDirectionUp = 0;
            targetDirectionRight = 0;
        }
        
        directionUp = Mathf.SmoothDamp(directionUp, targetDirectionUp, ref velocityDirectionUp, 0.1f);
        directionRight = Mathf.SmoothDamp(directionRight, targetDirectionRight, ref velocityDirectionRight, 0.1f);

        Vector2 directionAxis = SquareToCircle(new Vector2(directionRight, directionUp));
        float tempDirectionRight = directionAxis.x;
        float tempDirectionUp = directionAxis.y;
        
        directionMagnitude = Mathf.Sqrt((tempDirectionUp * tempDirectionUp) + (tempDirectionRight * tempDirectionRight));
        directionVector = transform.forward * tempDirectionUp + transform.right * tempDirectionRight;

        run = Input.GetKey(keyA);
    }

    Vector2 SquareToCircle(Vector2 input)
    {
        Vector2 output = Vector2.zero;

        output.x = input.x * Mathf.Sqrt(1 - (input.y * input.y) / 2.0f);
        output.y = input.y * Mathf.Sqrt(1 - (input.x * input.x) / 2.0f);

        return output;
    }
}
