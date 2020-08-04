using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkProp : NetworkEntity
{
    private void Start()
    {
        Phys = GetComponent<Rigidbody>();

        if (Phys == null)
            Type = EntityType.StaticProp;
        else
        {
            if (!isServer)
                Phys.isKinematic = true;
        }
    }

    private void OnCollisionStay(Collision entity)
    {
        EnableCollisionFixed(true);
    }

    private void OnCollisionExit(Collision entity)
    {
        EnableCollisionFixed(false);
    }
}
