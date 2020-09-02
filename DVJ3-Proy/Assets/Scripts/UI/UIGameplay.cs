using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIGameplay : MonoBehaviour
{
    public TextMeshProUGUI[] HPText;
    Player[] player;
    int[] playersHP = { 0, 0, 0, 0 }; //implementar esto despues asi no setea la vida todos los frames
    
    void Start()
    {
        player = FindObjectsOfType<Player>();
        
        for (int i = 0; i < player.Length; i++)
        {
            HPText[player[i].playerNumber - 1].gameObject.SetActive(true);
            HPText[player[i].playerNumber - 1].text="P"+ (player[i].playerNumber - 1)+ " HP: " + player[i].hp;
        }
    }

    void Update()
    {
        for (int i = 0; i < player.Length; i++)
        {
            HPText[player[i].playerNumber - 1].text = "P"+ player[i].playerNumber + " HP: " + player[i].hp;
        }
        
    }
}
