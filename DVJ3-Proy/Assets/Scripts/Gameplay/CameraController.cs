using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float translationSpeed;
    public float baseZoom;
    public float maxZoom;
    [SerializeField] Vector3 minLimit = Vector3.zero;
    [SerializeField] Vector3 maxLimit = Vector3.zero;
    [SerializeField] List<Transform> players = new List<Transform>();
    float lastDistBetweenPlayers;
    Vector3 stageCenter = new Vector3(-5, 15, -5);
    float zoom;
    Vector3 offset;
    Vector3 minPos;
    Vector3 maxPos;

    public void OnGameplayStart(List<PlayerController> activePlayers)
    {
        for (int i = 0; i < activePlayers.Count; i++)
        {
            players.Add(activePlayers[i].transform);
        }
        zoom = baseZoom;

        minPos = new Vector3(players[0].transform.position.x, 0, players[0].transform.position.z);
        maxPos = new Vector3(players[0].transform.position.x, 0, players[0].transform.position.z);
        for (int i = 1; i < players.Count; i++)
        {
            if (players[i].position.x < minPos.x) minPos.x = players[i].position.x;
            if (players[i].position.x > maxPos.x) maxPos.x = players[i].position.x;
            if (players[i].position.z < minPos.z) minPos.z = players[i].position.z;
            if (players[i].position.z > maxPos.z) maxPos.z = players[i].position.z;
        }
        lastDistBetweenPlayers = (maxPos - minPos).magnitude;
        StartCoroutine(FollowPlayers());
    }

    IEnumerator FollowPlayers()
    {
        float distBetweenPlayers;
        Vector3 targetPos;

        while (true)
        {
            minPos = new Vector3(players[0].transform.position.x, 0, players[0].transform.position.z);
            maxPos = new Vector3(players[0].transform.position.x, 0, players[0].transform.position.z);
            for (int i = 1; i < players.Count; i++)
            {
                if (players[i].position.x < minPos.x) minPos.x = players[i].position.x;
                if (players[i].position.x > maxPos.x) maxPos.x = players[i].position.x;
                if (players[i].position.z < minPos.z) minPos.z = players[i].position.z;
                if (players[i].position.z > maxPos.z) maxPos.z = players[i].position.z;
            }

            Vector3 center = Vector3.Lerp(minPos, maxPos, 0.5f);
            offset = (center - stageCenter);

            distBetweenPlayers = (maxPos - minPos).magnitude;
            zoom += distBetweenPlayers - lastDistBetweenPlayers;

            float clampedZoom = Mathf.Clamp(zoom, maxZoom, 1000);

            targetPos = stageCenter + offset - transform.forward * clampedZoom;
            if (targetPos.x < minLimit.x) targetPos.x = minLimit.x;
            if (targetPos.z < minLimit.z) targetPos.z = minLimit.z;
            if (targetPos.x > maxLimit.x) targetPos.x = maxLimit.x;
            if (targetPos.z > maxLimit.z) targetPos.z = maxLimit.z;

            transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * translationSpeed);

            lastDistBetweenPlayers = distBetweenPlayers;
            yield return new WaitForFixedUpdate();
        }

    }
}
