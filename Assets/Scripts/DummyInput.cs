using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyInput : PlayerInputInterface
{
    // Start is called before the first frame update
    void Start()
    {
        targetDirectionUp = 0;
        targetDirectionRight = 0;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateMovement();
    }
    
}
