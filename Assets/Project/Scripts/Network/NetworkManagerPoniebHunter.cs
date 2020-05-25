using Mirror;
using Mirror.Websocket;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NetworkManagerPoniebHunter : NetworkManager
{
    /**
     * Player Objects
     */

    [Header("Player Objects")]

    [SerializeField]
    [Tooltip("Player Prefabs")]
    // Player Prefabs
    public GameObject[] playerObjects;

    [SerializeField]
    [Tooltip("Player Hunter Prefab")]
    // Player Hunter Prefab
    public GameObject playerHunterObject;

    /**
     * Game Manager
     */

    [Header("Game Manager")]

    public bool IsWebGL;
    public Transport NetTransport;

    [SerializeField]
    [Tooltip("Delay before the match")]
    // Delay before the match
    public float MatchStartDelay;

    [SerializeField]
    [Tooltip("Maximum number of players to automatically start a game")]
    // Maximum number of players to automatically start a game
    public int MatchMaxPlayersStart;

    /**
     * Bufer
     */

    [Header("Bufer")]

    [SerializeField]
    [Tooltip("Keeps existing player prefabs")]
    // Keeps existing player prefabs
    public List<GameObject> playerObjectsExists = new List<GameObject>();

    [SerializeField]
    [Tooltip("Determines whether a hunter exists or not")]
    // Determines whether a hunter exists or not
    public bool playerHunterExists;

    /**
     * Other variables
     */

    // It contains a coroutine of delay before the start of the round
    private Coroutine MatchStart = null;

    // It contains a list of existing connections
    private List<NetworkConnection> connections = new List<NetworkConnection>();

    // It matters true if the match has begun
    private bool MatchIsReady;

    /// <summary>
    /// Called when the server starts.
    /// </summary>
    public override void OnStartServer()
    {
        if (IsWebGL)
            NetTransport = GetComponent<WebsocketTransport>();
        else
            NetTransport = GetComponent<IgnoranceThreaded>();

        transport = NetTransport;

        base.OnStartServer();

        System.Random rnd = new System.Random();
        playerObjects = playerObjects.OrderBy(x => rnd.Next()).ToArray();
    }

    /// <summary>
    /// Called when a player connects.
    /// </summary>
    /// <param name="conn">NetworkConnection component</param>
    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        connections.Add(conn);

        // Disables the player if the limit is reached
        if (connections.Count == playerObjects.Length)
        {
            conn.Disconnect();
        }
        // Adds a player to the match if he started
        else if (MatchIsReady && connections.Count != playerObjects.Length)
        {
            AddToMatch(conn);
        }
        // Starts a match if all players are assembled
        else if (!MatchIsReady && connections.Count == playerObjects.Length)
        {
            MatchStarting();
        }
        // Starts an automatic report before the match, if there is the right number of players
        else if (MatchStart == null && connections.Count >= MatchMaxPlayersStart)
        {
            MatchStart = StartCoroutine(MatchStartCoroutine());
        }
    }

    /// <summary>
    /// Called when a player prefab is deleted.
    /// </summary>
    /// <param name="conn">NetworkConnection component</param>
    /// <param name="player">NetworkIdentity component</param>
    public override void OnServerRemovePlayer(NetworkConnection conn, NetworkIdentity player)
    {
        GameObject PlayerObject = playerObjectsExists.Find(x => x == player.gameObject);

        if (PlayerObject != null)
            playerObjectsExists.Remove(PlayerObject);

        base.OnServerRemovePlayer(conn, player);
    }

    /// <summary>
    /// Called when a player is disconnected.
    /// </summary>
    /// <param name="conn">NetworkConnection component</param>
    public override void OnServerDisconnect(NetworkConnection conn)
    {
        if (connections.Count == 0)
            MatchStartTimerStop();

        connections.Remove(conn);

        base.OnServerDisconnect(conn);
    }

    /// <summary>
    /// Adds a player to an existing match.
    /// </summary>
    /// <param name="conn">NetworkConnection component</param>
    private void AddToMatch(NetworkConnection conn)
    {
        GameObject[] Spawnpoints = GameObject.FindGameObjectsWithTag("Spawnpoint");

        foreach (var PlayerPrefab in playerObjects)
        {
            if (!playerObjectsExists.Exists(x => x == PlayerPrefab))
            {
                Transform Spawnpoint = Spawnpoints[Random.Range(0, Spawnpoints.Length)].transform;

                CreatePlayerPrefab(conn, PlayerPrefab, Spawnpoint);

                break;
            }
        }
    }

    /// <summary>
    /// Starts a match with the distribution of all existing players.
    /// </summary>
    private void MatchStarting()
    {
        MatchStartTimerStop();

        GameObject[] Spawnpoints = GameObject.FindGameObjectsWithTag("Spawnpoint");

        int index = 0;
        foreach(var conn in connections)
        {
            Transform Spawnpoint = Spawnpoints[Random.Range(0, Spawnpoints.Length)].transform;

            GameObject PlayerPrefab = playerObjects[index];
            CreatePlayerPrefab(conn, PlayerPrefab, Spawnpoint);

            index++;
        }
    }

    /// <summary>
    /// Creates a player prefab in a specific position.
    /// </summary>
    /// <param name="conn">NetworkConnection component</param>
    /// <param name="PlayerPrefab">Player prefab</param>
    /// <param name="Spawnpoint">Transform spawnpoint</param>
    private void CreatePlayerPrefab(NetworkConnection conn, GameObject PlayerPrefab, Transform Spawnpoint)
    {
        GameObject player = Instantiate(PlayerPrefab, Spawnpoint.position, Spawnpoint.rotation);
        playerObjectsExists.Add(player);

        NetworkServer.AddPlayerForConnection(conn, player);

        Debug.Log(conn.connectionId + " - " + PlayerPrefab.name);
    }

    /// <summary>
    /// Stops a timer to automatically start a match.
    /// </summary>
    private void MatchStartTimerStop()
    {
        if (MatchStart != null)
        {
            StopCoroutine(MatchStart);
            MatchStart = null;
        }
    }

    /// <summary>
    /// Timer to automatically start a match.
    /// </summary>
    /// <returns>Coroutine process</returns>
    private IEnumerator MatchStartCoroutine()
    {
        while(true)
        {
            yield return new WaitForSeconds(MatchStartDelay);

            MatchStarting();

            yield break; 
        }
    }
}
