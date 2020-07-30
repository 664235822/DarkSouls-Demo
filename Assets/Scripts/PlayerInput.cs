using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class PlayerInput : PlayerInputInterface
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
        
        UpdateMovement();

        run = CrossPlatformInputManager.GetButton(keyA);
        defence = CrossPlatformInputManager.GetButton(keyDefence);

        jump = CrossPlatformInputManager.GetButtonDown(keyB);
        attackRight = CrossPlatformInputManager.GetButtonDown(keyX);
        attackLeft = CrossPlatformInputManager.GetButtonDown(keyY);

        lockTarget = CrossPlatformInputManager.GetButtonDown(keyLock);
    }
}
