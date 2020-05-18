using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_MainMenu : MonoBehaviour
{
    [Header("Menu Elements")]

    [SerializeField]
    [Tooltip("Server host creation button")]
    // Server host creation button
    private Button Button_Host;

    [SerializeField]
    [Tooltip("Button to connect to the server")]
    // Button to connect to the server
    private Button Button_Join;

    [SerializeField]
    [Tooltip("Button to exit the game")]
    // Button to exit the game
    private Button Button_Exit;

    [Header("Game Maps")]

    [SerializeField]
    [Tooltip("Stores game maps")]
    // Stores game maps
    private string[] GameMaps;

    [Header("Networking Components")]

    [SerializeField]
    [Tooltip("Custom network manager")]
    // Custom network manager
    private NetworkManagerPoniebHunter GameNetworkManager;

    /// <summary>
    /// Called once before the start of the game.
    /// </summary>
    private void Start()
    {
        EventsUiElements();
    }

    /// <summary>
    /// Creates interface element events.
    /// </summary>
    private void EventsUiElements()
    {
        Button_Host.onClick.AddListener(CreateHost);
        Button_Join.onClick.AddListener(JoinTheServer);
        Button_Exit.onClick.AddListener(ExitTheGame);
    }

    /// <summary>
    /// Creates a host and connects to it.
    /// </summary>
    private void CreateHost()
    {
        //StartCoroutine(LoadGameMaps());
        GameNetworkManager.StartHost();
    }

    /// <summary>
    /// Connect to the game server.
    /// </summary>
    private void JoinTheServer()
    {
        GameNetworkManager.StartClient();
    }

    /// <summary>
    /// The procedure for closing the game.
    /// </summary>
    private void ExitTheGame()
    {
        Application.Quit();
    }

    private IEnumerator LoadGameMaps()
    {
        // The Application loads the Scene in the background as the current Scene runs.
        // This is particularly good for creating loading screens.
        // You could also load the Scene by using sceneBuildIndex. In this case Scene2 has
        // a sceneBuildIndex of 1 as shown in Build Settings.

        string GameScene = GameMaps[UnityEngine.Random.Range(0, GameMaps.Length - 1)];
        AsyncOperation AsyncLoad = SceneManager.LoadSceneAsync(GameScene);

        // Wait until the asynchronous scene fully loads
        while (!AsyncLoad.isDone)
        {
            yield return null;
        }
    }
}
