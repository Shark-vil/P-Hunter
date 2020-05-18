using MLAPI;
using MLAPI.Messaging;
using MLAPI.Spawning;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkSceneHelper : MonoBehaviour
{
    protected internal static bool IsHost = false;

    private void Start()
    {
        if (IsHost)
            NetworkingManager.Singleton.StartHost();
    }
}
