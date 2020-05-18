using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
        
    }

    /// <summary>
    /// Connect to the game server.
    /// </summary>
    private void JoinTheServer()
    {

    }

    /// <summary>
    /// The procedure for closing the game.
    /// </summary>
    private void ExitTheGame()
    {
        Application.Quit();
    }
}
