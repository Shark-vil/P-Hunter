using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NetworkManagerPoniebHunter : NetworkManager
{
    public GameObject[] playerObjects;
    public float MatchStartDelay;

    public List<GameObject> playerObjectsExists = new List<GameObject>();

    private Coroutine MatchStart = null;
    private List<NetworkConnection> connections = new List<NetworkConnection>();
    private bool GameIsReady;

    public override void OnStartServer()
    {
        base.OnStartServer();

        Debug.Log("OnStartServer");

        System.Random rnd = new System.Random();
        playerObjects = playerObjects.OrderBy(x => rnd.Next()).ToArray();

        Debug.Log("Randomize characters array");
    }

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        connections.Add(conn);

        Debug.Log(conn.connectionId + " : " + connections.Count + " - " + playerObjects.Length);

        if (connections.Count == playerObjects.Length)
        {
            conn.Disconnect();
        }else if (GameIsReady && connections.Count != playerObjects.Length)
        {
            AddToGame(conn);
        }
        else if (connections.Count == playerObjects.Length)
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

    public override void OnServerRemovePlayer(NetworkConnection conn, NetworkIdentity player)
    {
        GameObject PlayerObject = playerObjectsExists.Find(x => x == player.gameObject);

        if (PlayerObject != null)
            playerObjectsExists.Remove(PlayerObject);

        base.OnServerRemovePlayer(conn, player);
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        if (connections.Count == 0)
            MatchStartTimerStop();

        connections.Remove(conn);

        base.OnServerDisconnect(conn);
    }

    private void AddToGame(NetworkConnection conn)
    {
        GameObject[] Spawnpoints = GameObject.FindGameObjectsWithTag("Spawnpoint");

        foreach (var PlayerPrefab in playerObjects)
        {
            if (!playerObjectsExists.Exists(x => x == PlayerPrefab))
            {
                Transform Spawnpoint = Spawnpoints[Random.Range(0, Spawnpoints.Length)].transform;

                GameObject player = Instantiate(PlayerPrefab, Spawnpoint.position, Spawnpoint.rotation);
                playerObjectsExists.Add(player);

                NetworkServer.AddPlayerForConnection(conn, player);

                Debug.Log(conn.connectionId + " - " + PlayerPrefab.name);

                break;
            }
        }
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
            playerObjectsExists.Add(player);

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
