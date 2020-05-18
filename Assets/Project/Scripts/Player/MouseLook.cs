using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public float sensitivity = 5f;
    public float headMinY = -40f;
    public float headMaxY = 40f;

    public Transform Model;
    public Transform HeadBone;

    private float rotationY;

    [SerializeField]
    private Player PlayerComponent;

    [SerializeField]
    private NetworkIdentity playerIdentity;

    private void Update()
    {
        if (playerIdentity.isLocalPlayer)
            Moving();
    }

    private void LateUpdate()
    {
        BodyLook();
        HeadLook();
    }

    private void Moving()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        float rotationX = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * sensitivity;
        rotationY += Input.GetAxis("Mouse Y") * sensitivity;
        rotationY = Mathf.Clamp(rotationY, headMinY, headMaxY);
        transform.localEulerAngles = new Vector3(-rotationY, rotationX, 0);
    }

    private void BodyLook()
    {
        bool diff = AngleDiff(transform.eulerAngles.y, Model.eulerAngles.y);
        if (!diff || PlayerComponent.IsMovement)
        {
            Vector3 ModelAngle = Model.eulerAngles;
            Vector3 CameraAngle = transform.eulerAngles;
            Vector3 NewVector = new Vector3(ModelAngle.x, CameraAngle.y, ModelAngle.z);
            Model.eulerAngles = AngleLerp(ModelAngle, NewVector, 0.1f);
        }
    }

    private Vector3 AngleLerp(Vector3 StartAngle, Vector3 FinishAngle, float t)
    {
        float xLerp = Mathf.LerpAngle(StartAngle.x, FinishAngle.x, t);
        float yLerp = Mathf.LerpAngle(StartAngle.y, FinishAngle.y, t);
        float zLerp = Mathf.LerpAngle(StartAngle.z, FinishAngle.z, t);
        Vector3 Lerped = new Vector3(xLerp, yLerp, zLerp);
        return Lerped;
    }

    private void HeadLook()
    {
        Vector3 HeadAngle = HeadBone.eulerAngles;
        Vector3 CameraAngle = transform.eulerAngles;
        Vector3 NewVector = new Vector3(HeadAngle.x, CameraAngle.y + 90, HeadUpMinMax(CameraAngle.x - 90));

        HeadBone.eulerAngles = NewVector;
    }

    private float HeadUpMinMax(float x)
    {
        if (x > 225 || x < -40)
            return x;

        if (x < 0)
            return -40;
        else
            return 225;
    }

    private bool AngleDiff(float AngleY1, float AngleY2)
    {
        float result = AngleY1 - AngleY2;

        if (Mathf.Abs(result) > 40)
            return false;

        return true;
    }
}
