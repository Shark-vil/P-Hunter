using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NetworkManagerPoniebHunter : NetworkManager
{
    public List<PlayerTypeObjects> PlayerPrefabs = new List<PlayerTypeObjects>();
    public List<string> PlayersTypeExists = new List<string>();

    public override void OnStartServer()
    {
        base.OnStartServer();

        System.Random rnd = new System.Random();
        PlayerPrefabs = PlayerPrefabs.OrderBy(x => rnd.Next()).ToList();
    }

    public override void OnServerConnect(NetworkConnection conn)
    {
        base.OnServerConnect(conn);

        PlayerTypeObjects PlayerNewPrefab = null;
        for (int i = 0; i < PlayerPrefabs.Count; i++)
        {
            PlayerTypeObjects PlayerPrefab = PlayerPrefabs[i];
            if (!PlayersTypeExists.Exists(x => x == PlayerPrefab.character))
            {
                PlayersTypeExists.Add(PlayerPrefab.character);
                PlayerNewPrefab = PlayerPrefab;
                break;
            }
        }

        if (PlayerNewPrefab == null)
            conn.Disconnect();

        NetworkServer.AddPlayerForConnection(conn, Instantiate(PlayerNewPrefab.playerObject));
    }
}
