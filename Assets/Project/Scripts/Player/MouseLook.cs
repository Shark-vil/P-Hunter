using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    /**
     * Player Components
     */

    [Header("Player Components")]

    [SerializeField]
    [Tooltip("Main player component")]
    // Main player component
    private Player PlayerComponent;

    [SerializeField]
    [Tooltip("Player network identity")]
    // Player network identity
    private NetworkIdentity playerIdentity;

    [SerializeField]
    [Tooltip("Player model container transform")]
    // Player model container transform
    private Transform Model;

    [SerializeField]
    [Tooltip("Player head bone transform")]
    // Player head bone transform
    private Transform HeadBone;

    /**
     * Camera parametrs
     */

    [Header("Camera parametrs")]

    [SerializeField]
    [Tooltip("Camera rotation sensitivity")]
    // Camera rotation sensitivity
    private float Sensitivity = 5f;

    [SerializeField]
    [Tooltip("Minimum camera tilt")]
    // Minimum camera tilt
    public float HeadMinY = -80f;

    [SerializeField]
    [Tooltip("Maximum camera tilt")]
    // Maximum camera tilt
    public float HeadMaxY = 80f;

    /**
     * Other variables
     */

    // Buffer for storing camera rotation
    private float rotationY;

    /// <summary>
    /// Called every time a frame is updated.
    /// </summary>
    private void Update()
    {
        if (playerIdentity.isLocalPlayer)
            Moving();
    }

    /// <summary>
    /// Called every time an animation is updated.
    /// </summary>
    private void LateUpdate()
    {
        BodyLook();
        HeadLook();
    }

    /// <summary>
    /// Controls the movement of the camera.
    /// </summary>
    private void Moving()
    {
        float rotationX = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * Sensitivity;
        rotationY += Input.GetAxis("Mouse Y") * Sensitivity;
        rotationY = Mathf.Clamp(rotationY, HeadMinY, HeadMaxY);
        transform.localEulerAngles = new Vector3(-rotationY, rotationX, 0);
    }

    /// <summary>
    /// Controls the rotation of the model relative to the rotation of the camera.
    /// </summary>
    private void BodyLook()
    {
        bool diff = AngleDiff(transform.eulerAngles.y, Model.eulerAngles.y, 40);
        if (!diff || PlayerComponent.IsMovement)
        {
            Vector3 ModelAngle = Model.eulerAngles;
            Vector3 CameraAngle = transform.eulerAngles;
            Vector3 NewVector = new Vector3(ModelAngle.x, CameraAngle.y, ModelAngle.z);
            Model.eulerAngles = AngleLerp(ModelAngle, NewVector, 0.1f);
        }
    }

    /// <summary>
    /// Gets a smooth rotation on the vector.
    /// </summary>
    /// <param name="StartAngle">Start angle position</param>
    /// <param name="FinishAngle">End angle position</param>
    /// <param name="t">Smoothing time</param>
    /// <returns>New Vector3 for eulerAngles</returns>
    private Vector3 AngleLerp(Vector3 StartAngle, Vector3 FinishAngle, float t)
    {
        float xLerp = Mathf.LerpAngle(StartAngle.x, FinishAngle.x, t);
        float yLerp = Mathf.LerpAngle(StartAngle.y, FinishAngle.y, t);
        float zLerp = Mathf.LerpAngle(StartAngle.z, FinishAngle.z, t);
        Vector3 Lerped = new Vector3(xLerp, yLerp, zLerp);
        return Lerped;
    }

    /// <summary>
    /// Controls the rotation of the head relative to the rotation of the camera.
    /// </summary>
    private void HeadLook()
    {
        Vector3 HeadAngle = HeadBone.eulerAngles;
        Vector3 CameraAngle = transform.eulerAngles;
        Vector3 NewVector = new Vector3(HeadAngle.x, CameraAngle.y + 90, HeadUpMinMax(CameraAngle.x - 90));

        HeadBone.eulerAngles = NewVector;
    }

    /// <summary>
    /// Returns the correct maximum and minimum head angle.
    /// </summary>
    /// <param name="x">X axis rotation</param>
    /// <returns>X-axis normal rotation</returns>
    private float HeadUpMinMax(float x)
    {
        if (x > 225 || x < -40)
            return x;

        if (x < 0)
            return -40;
        else
            return 225;
    }

    /// <summary>
    /// Calculates the difference between two Y axes.
    /// </summary>
    /// <param name="AngleY1">First Y axis</param>
    /// <param name="AngleY2">Second Y axis</param>
    /// <param name="MaxAngle">Maximum rotation angle</param>
    /// <returns>Will return TRUE if the angle is not greater than allowed</returns>
    private bool AngleDiff(float AngleY1, float AngleY2, float MaxAngle)
    {
        float result = AngleY1 - AngleY2;

        if (Mathf.Abs(result) > MaxAngle)
            return false;

        return true;
    }
}
