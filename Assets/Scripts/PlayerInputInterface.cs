using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputInterface : MonoBehaviour
{
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
    public bool defence;
    public bool lockTarget;
    public bool counterBack;

    [Header("Other Settings")]
    public bool inputEnabled = true;

    protected float targetDirectionUp;
    protected float targetDirectionRight;
    protected float velocityDirectionUp;
    protected float velocityDirectionRight;

    protected Vector2 SquareToCircle(Vector2 input)
    {
        Vector2 output = Vector2.zero;

        output.x = input.x * Mathf.Sqrt(1 - (input.y * input.y) / 2.0f);
        output.y = input.y * Mathf.Sqrt(1 - (input.x * input.x) / 2.0f);

        return output;
    }

    protected void UpdateMovement()
    {
        directionUp = Mathf.SmoothDamp(directionUp, targetDirectionUp, ref velocityDirectionUp, 0.1f);
        directionRight = Mathf.SmoothDamp(directionRight, targetDirectionRight, ref velocityDirectionRight, 0.1f);

        Vector2 directionAxis = SquareToCircle(new Vector2(directionRight, directionUp));
        float tempDirectionRight = directionAxis.x;
        float tempDirectionUp = directionAxis.y;

        directionMagnitude =
            Mathf.Sqrt((tempDirectionUp * tempDirectionUp) + (tempDirectionRight * tempDirectionRight));
        directionVector = transform.forward * tempDirectionUp + transform.right * tempDirectionRight;
    }
}
