using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : NetworkBehaviour
{
    public Camera MainCamera;
    public Rigidbody Physics;

    [SyncVar]
    public bool IsMovement;

    [SerializeField]
    private NetworkIdentity identity;

    [SerializeField]
    private SkinnedMeshRenderer[] PlayerMeshes;

    private void Start()
    {
        if (!identity.isLocalPlayer)
            MainCamera.enabled = false;
        else
        {
            foreach (var PlayerMesh in PlayerMeshes)
                PlayerMesh.enabled = false;
        }

        StartCoroutine(PlayerIsMove());
    }

    private IEnumerator PlayerIsMove()
    {
        Vector3 PlayerPosition = transform.position.normalized;

        while(true)
        {
            Vector3 SelectPlayerPosition = transform.position.normalized;

            if (PlayerPosition != SelectPlayerPosition)
            {
                if (!IsMovement)
                    IsMovement = true;

                PlayerPosition = SelectPlayerPosition;
            }
            else
            {
                if (IsMovement)
                    IsMovement = false;
            }

            yield return new WaitForSeconds(0.5f);
        }
    }

    private void FixedUpdate()
    {
        PlayerMovement();
    }

    private void PlayerMovement()
    {
        if (Input.GetButton("Horizontal") || Input.GetButton("Vertical"))
        {
            CmdPlayerMovement(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        }
    }

    [Command]
    private void CmdPlayerMovement(float horizontal, float vertical)
    {
        Vector3 MoveForward = new Vector3(MainCamera.transform.forward.x, 0, MainCamera.transform.forward.z).normalized;
        Vector3 MoveRight = new Vector3(MainCamera.transform.right.x, 0, MainCamera.transform.right.z).normalized;

        Vector3 MoveHorizontal = MoveRight * horizontal;
        Vector3 MoveVertical = MoveForward * vertical;

        Vector3 NewVelocity = MoveHorizontal + MoveVertical * 120 * Time.deltaTime;
        NewVelocity.y = Physics.velocity.y;

        Physics.velocity = NewVelocity;
    }
}
