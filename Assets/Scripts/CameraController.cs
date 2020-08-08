using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraController : MonoBehaviour
{
    public PlayerInput playerInput;
    public GameObject playerHandle;
    public GameObject cameraHandle;
    public GameObject model;
    public GameObject camera;
    public Image lockSprite;

    public float MouseXSpeed = 200.0f;
    public float MouseYSpeed = 100.0f;
    public float cameraDampValue = 0.01f;

    public bool lockState;
    public bool isAI = false;

    private float tempEulerX = 20.0f;
    private Vector3 smoothDampVelocity;

    private class LockTargetObj
    {
        public GameObject obj;
        public float halfHeight;
        public ActorManager actorManager;

        public LockTargetObj(GameObject _obj,float _halfHeight)
        {
            obj = _obj;
            halfHeight = _halfHeight;
            actorManager = _obj.GetComponent<ActorManager>();
        }
    }
    
    [SerializeField]
    private LockTargetObj lockTarget;

    // Start is called before the first frame update
    void Start()
    {
        lockSprite.enabled = false;
    }
    
    // Update is called once per frame
    void Update()
    {
        if (lockSprite.enabled)
        {
            if (!isAI)
            {
                lockSprite.rectTransform.position =
                    camera.GetComponent<Camera>().WorldToScreenPoint(lockTarget.obj.transform.position +
                                                                     new Vector3(0, lockTarget.halfHeight, 0));
            }

            if (Vector3.Distance(model.transform.position, lockTarget.obj.transform.position) > 10.0f)
            {
                LockProcess(null, false, false);
            }

            if (lockTarget.actorManager != null)
            {
                if (lockTarget.actorManager.stateManager.isDie)
                {
                    LockProcess(null, false, false);
                }
            }
        }
    }

    void FixedUpdate()
    {
        if (lockTarget == null)
        {
            Vector3 tempModelEuler = model.transform.eulerAngles;

            playerHandle.transform.Rotate(Vector3.up, playerInput.cameraRight * MouseXSpeed * Time.fixedDeltaTime);

            tempEulerX -= playerInput.cameraUp * MouseYSpeed * Time.fixedDeltaTime;
            tempEulerX = Mathf.Clamp(tempEulerX, -20, 30);
            cameraHandle.transform.localEulerAngles = new Vector3(tempEulerX, 0, 0);

            model.transform.eulerAngles = tempModelEuler;
        }
        else
        {
            Vector3 tempForward = lockTarget.obj.transform.position - model.transform.position;
            tempForward.y = 0;
            playerHandle.transform.forward = tempForward;
            cameraHandle.transform.LookAt(lockTarget.obj.transform);
        }


        if (!isAI)
        {
            camera.transform.position = Vector3.SmoothDamp(camera.GetComponent<Camera>().transform.position,
                transform.position,
                ref smoothDampVelocity, cameraDampValue);
            camera.transform.LookAt(cameraHandle.transform);
        }
    }

    public void LockTarget()
    {
        Vector3 modelOrigin1 = model.transform.position;
        Vector3 modelOrigin2 = modelOrigin1 + new Vector3(0, 1, 0);
        Vector3 boxCenter = modelOrigin2 + model.transform.forward * 5.0f;
        Collider[] cols = Physics.OverlapBox(boxCenter, new Vector3(0.5f, 0.5f, 5f), model.transform.rotation,
            LayerMask.GetMask(isAI ? "Player" : "Enemy"));

        if (cols.Length == 0)
        {
            LockProcess(null, false, false);
        }
        else
        {
            foreach (Collider col in cols)
            {
                if (lockTarget != null && lockTarget.obj == col.gameObject)
                {
                    LockProcess(null, false, false);
                    break;
                }
                LockProcess(new LockTargetObj(col.gameObject, col.bounds.extents.y), true, true);
                break;
            }
        }
    }

    private void LockProcess(LockTargetObj _lockTarget, bool _lockSpriteEnable, bool _lockState)
    {
        lockTarget = _lockTarget;
        if (!isAI)
        {
            lockSprite.enabled = _lockSpriteEnable;
        }

        lockState = _lockState;
    }
}

