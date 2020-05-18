using MLAPI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkSceneHelper : MonoBehaviour
{
    private void Start()
    {
        NetworkingManager.Singleton.StartHost();
    }
}
