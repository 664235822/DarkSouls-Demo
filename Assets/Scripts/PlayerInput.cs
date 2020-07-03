using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class PlayerInput : MonoBehaviour
{
    [Header("Keys Setting")]
    public string keyDirectionUp;
    public string keyDirectionRight;

    public string keyCameraUp;
    public string keyCameraRight;
    
    public string keyA;
    public string keyB;
    public string keyX;
    public string keyY;

    public string keyDefence;

    public string keyLock;

    [Header("Direction Outputs")]
    public float directionUp;
    public float directionRight;

    public float cameraUp;
    public float cameraRight;

    public float directionMagnitude;
    public Vector3 directionVector;

    public bool run;
    public bool jump;
    public bool attackRight;
    public bool attackLeft;
    public bool defense;
    public bool lockTarget;

    [Header("Other Settings")]
    public bool inputEnabled = true;

    private float targetDirectionUp;
    private float targetDirectionRight;
    private float velocityDirectionUp;
    private float velocityDirectionRight;

    // Update is called once per frame
    void Update()
    {
        
        targetDirectionRight = CrossPlatformInputManager.GetAxis(keyDirectionRight);
        targetDirectionUp = CrossPlatformInputManager.GetAxis(keyDirectionUp);

        cameraUp = CrossPlatformInputManager.GetAxis(keyCameraUp);
        cameraRight = CrossPlatformInputManager.GetAxis(keyCameraRight);

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

        directionMagnitude =
            Mathf.Sqrt((tempDirectionUp * tempDirectionUp) + (tempDirectionRight * tempDirectionRight));
        directionVector = transform.forward * tempDirectionUp + transform.right * tempDirectionRight;
        
        run = CrossPlatformInputManager.GetButton(keyA);
        defense = CrossPlatformInputManager.GetButton(keyDefence);

        jump = CrossPlatformInputManager.GetButtonDown(keyB);
        attackRight = CrossPlatformInputManager.GetButtonDown(keyX);
        attackLeft = CrossPlatformInputManager.GetButtonDown(keyY);

        lockTarget = CrossPlatformInputManager.GetButtonDown(keyLock);
    }

    Vector2 SquareToCircle(Vector2 input)
    {
        Vector2 output = Vector2.zero;

        output.x = input.x * Mathf.Sqrt(1 - (input.y * input.y) / 2.0f);
        output.y = input.y * Mathf.Sqrt(1 - (input.x * input.x) / 2.0f);

        return output;
    }
}
