using MLAPI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Camera MainCamera;

    private void Start()
    {
        var NetObject = GetComponent<NetworkedObject>();

        if (!NetObject.IsOwner)
            MainCamera.enabled = false;
    }
}
