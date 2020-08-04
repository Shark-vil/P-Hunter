using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkEntity : NetworkBehaviour
{
    public enum EntityType
    {
        Player,
        Prop,
        StaticProp
    }

    public EntityType Type;

    [SerializeField]
    [Tooltip("Player rigidbody component")]
    // Player rigidbody component
    public Rigidbody Phys;

    [SyncVar]
    public bool IsPickup;

    [SyncVar]
    public bool IsCollisionFixed;

    public List<GameObject> Pickers = new List<GameObject>();

    internal void EnableCollisionFixed(bool Toggle)
    {
        if (IsPickup && Toggle)
        {
            Phys.velocity = Phys.velocity * 0.1f;
            IsCollisionFixed = true;
        }
        else if (!Toggle)
        {
            IsCollisionFixed = false;
        }
    }

    internal void EnableGravity(bool Toggle)
    {
        Phys.useGravity = Toggle;
    }

    internal void SetPickup(GameObject Picker = null)
    {
        if (Picker != null && !Pickers.Exists(x => x == Picker))
            Pickers.Add(Picker);

        IsPickup = true;
    }

    internal void UnPickup(GameObject Picker = null)
    {
        if (Pickers.Count != 0 && Picker != null)
        {
            if (Pickers.Exists(x => x == Picker))
            {
                Pickers.Remove(Picker);

                if (Pickers.Count == 0)
                    IsPickup = false;
            }
        }
        else
            IsPickup = false;
    }
}
