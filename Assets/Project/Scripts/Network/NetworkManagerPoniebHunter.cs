using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NetworkManagerPoniebHunter : NetworkManager
{
    public GameObject[] playerObjects;
    public float MatchStartDelay;

    private Coroutine MatchStart = null;
    private List<NetworkConnection> connections = new List<NetworkConnection>();

    public override void OnStartServer()
    {
        base.OnStartServer();

        System.Random rnd = new System.Random();
        playerObjects = playerObjects.OrderBy(x => rnd.Next()).ToArray();
    }

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        connections.Add(conn);

        Debug.Log(conn.connectionId + " : " + connections.Count + " - " + playerObjects.Length);

        if (connections.Count == playerObjects.Length)
        {
            GameStarting();
        }
        else
        {
            if (MatchStart == null)
            {
                MatchStart = StartCoroutine(MatchStartCoroutine());
            }
        }
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        MatchStartTimerStop();

        connections.Remove(conn);

        base.OnServerDisconnect(conn);
    }

    private void GameStarting()
    {
        MatchStartTimerStop();

        GameObject[] Spawnpoints = GameObject.FindGameObjectsWithTag("Spawnpoint");

        int index = 0;
        foreach(var conn in connections)
        {
            GameObject PlayerPrefab = playerObjects[index];

            Transform Spawnpoint = Spawnpoints[Random.Range(0, Spawnpoints.Length)].transform;

            GameObject player = Instantiate(PlayerPrefab, Spawnpoint.position, Spawnpoint.rotation);
            NetworkServer.AddPlayerForConnection(conn, player);

            Debug.Log(conn.connectionId + " - " + PlayerPrefab.name);

            index++;
        }
    }

    private void MatchStartTimerStop()
    {
        if (MatchStart != null)
        {
            StopCoroutine(MatchStart);
            MatchStart = null;
        }
    }

    private IEnumerator MatchStartCoroutine()
    {
        while(true)
        {
            yield return new WaitForSeconds(MatchStartDelay);

            GameStarting();

            yield break; 
        }
    }
}
