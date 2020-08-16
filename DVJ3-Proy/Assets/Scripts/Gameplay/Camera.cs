using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera : MonoBehaviour
{
    List <Transform> players;
    void Start()
    {
        Player[] activePlayers;
        activePlayers = FindObjectsOfType<Player>();
        for (int i = 0; i < activePlayers.Length; i++)
        {
            players.Add(activePlayers[i].transform);
        }
    }

    //void FixedUpdate()
    //{
    //    Vector3 newPos;
    //    for (int i = 0; i < players.Count; i++)
    //    { 
    //        
    //    }
    //}
}
