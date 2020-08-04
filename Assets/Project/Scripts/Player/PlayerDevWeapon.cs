using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDevWeapon : NetworkBehaviour
{
    private bool IsPickup;
    private GameObject EntityObject;
    private NetworkEntity Entity;
    private Player player;
    private Vector3 EntityEndPos;

    public LayerMask PickupLayers;

    private float PickupDistance; 

    private void Start()
    {
        player = GetComponent<Player>();
    }

    private void FixedUpdate()
    {
        if (isServer && IsPickup)
        {
            if (EntityObject == null)
            {
                UnEquip();
                return;
            }

            Vector3 StartPos = player.MainCamera.transform.position;
            Vector3 EndPos = player.MainCamera.transform.forward;

            EntityEndPos = StartPos + (EndPos * PickupDistance);

            if (!Entity.IsCollisionFixed)
            {
                Vector3 LookForward = EntityObject.transform.position - player.MainCamera.transform.position;

                var LookAt = Quaternion.LookRotation(LookForward);
                //LookAt = Quaternion.Slerp(EntityObject.transform.rotation, LookAt, 10);

                Entity.Phys.MoveRotation(Quaternion.Lerp(EntityObject.transform.rotation, LookAt, 0.2f));
            }

            Vector3 LerpVelocity = Vector3.Lerp(EntityObject.transform.position, EntityEndPos, 0.1f);
            Entity.Phys.MovePosition(LerpVelocity);
        }
    }

    private void Update()
    {
        if (isClient)
        {
            if (Input.GetButtonDown("Fire1"))
                CmdEquip();

            float MouseWheel = Input.GetAxis("Mouse ScrollWheel");

            if (MouseWheel > 0)
            {
                CmdChangePickupDistance(true);
            }
            else if (MouseWheel < 0)
            {
                CmdChangePickupDistance(false);
            }
        }
    }

    [Command]
    private void CmdChangePickupDistance(bool IsForward)
    {
        if (!IsPickup)
            return;

        float Dist = Vector3.Distance(EntityObject.transform.position, player.MainCamera.transform.position);
        float Num = 0.1f * Dist;

        if (IsForward)
        {
            if (PickupDistance + Num > 500)
                PickupDistance = 500;
            else
                PickupDistance += Num;
        }
        else
        {
            if (PickupDistance - Num < 5)
                PickupDistance = 5;
            else
                PickupDistance -= Num;
        }
    }

    private void PickupDistanceNormalize()
    {
        if (PickupDistance + 1 > 500)
            PickupDistance = 500;

        if (PickupDistance - 1 < 5)
            PickupDistance = 5;
    }

    [Command]
    private void CmdEquip()
    {
        if (!IsPickup)
        {
            Equip();
        }
        else
        {
            UnEquip();
        }
    }

    private void Equip()
    {
        Camera camera = player.MainCamera;

        RaycastHit hit;
        // Дистанция была - 5
        if (Physics.Raycast(camera.transform.position, camera.transform.forward, out hit, 100, PickupLayers))
            if (hit.collider != null && hit.collider.gameObject.GetComponent<Rigidbody>() != null)
            {
                GameObject TempEntityObject = hit.collider.gameObject;
                NetworkEntity TempEntity = TempEntityObject.GetComponent<NetworkEntity>();

                if (TempEntity == null || TempEntity.Type == NetworkEntity.EntityType.StaticProp)
                    return;

                PickupDistance = hit.distance;
                PickupDistanceNormalize();

                Entity = TempEntity;
                Entity.EnableGravity(false);
                Entity.SetPickup(gameObject);
                EntityObject = TempEntityObject;

                IsPickup = true;
            }
    }

    private void UnEquip()
    {
        Vector3 StartPos = EntityObject.transform.position;
        Vector3 Dist = StartPos - EntityEndPos;

        IsPickup = false;

        Vector3 NewVelocity = Vector3.zero;

        if (Mathf.Abs(Dist.x) >= 0.1f)
            NewVelocity.x = -Dist.x;

        if (Mathf.Abs(Dist.y) >= 0.1f)
            NewVelocity.y = -Dist.y;

        if (Mathf.Abs(Dist.z) >= 0.1f)
            NewVelocity.z = -Dist.z;

        NewVelocity = NewVelocity * Mathf.Abs(Vector3.Distance(StartPos, EntityEndPos)) / (Entity.Phys.mass / 2);

        Entity.Phys.velocity = NewVelocity;

        EntityObject = null;
        Entity.EnableGravity(true);
        Entity.UnPickup(gameObject);
        Entity = null;
        EntityEndPos = Vector3.zero;
    }
}
