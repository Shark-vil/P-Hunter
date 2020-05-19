using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

public class PlayerTypeMessage : MessageBase
{
    public string name;
}

[System.Serializable]
public class PlayerTypeObjects
{
    public string character;
    public GameObject playerObject;

    public PlayerTypeObjects(string character, GameObject playerObject)
    {
        this.character = character;
        this.playerObject = playerObject;
    }
}