using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRespawner : MonoBehaviour
{
    private void OnTriggerEnter(Collider entity)
    {
        if (entity.gameObject.tag == "Player")
        {
            GameObject[] Spawnpoints = GameObject.FindGameObjectsWithTag("Spawnpoint");
            Transform Spawnpoint = Spawnpoints[Random.Range(0, Spawnpoints.Length)].transform;
            entity.gameObject.transform.position = Spawnpoint.position;
        }
        else
        {
            Destroy(entity.gameObject);
        }
    }
}
