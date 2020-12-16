using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingObstController : MonoBehaviour
{
    [SerializeField] Vector3 minSpawn;
    [SerializeField] Vector3 maxSpawn;
    [SerializeField] float timeBetweenSpawn;
    [SerializeField] GameObject visualWarning;
    [SerializeField] float timeAfterWarning;
    GameObject prefabToSpawn;

    bool spawning = false;
    public void StartSpawning(GameObject prefab)
    {
        prefabToSpawn = prefab;
        spawning = true;
        StartCoroutine(SpawnFallingObstacles());
    }

    public void StopSpawning()
    {
        spawning = false;
    }

    IEnumerator SpawnFallingObstacles()
    {
        while (true)
        {
            yield return new WaitForSeconds(timeBetweenSpawn);
            StartCoroutine(SpawnSingleFallingObst());
        }
    }

    IEnumerator SpawnSingleFallingObst()
    {
        Vector3 spawnPos;
        spawnPos.x = Random.Range(minSpawn.x, maxSpawn.x);
        spawnPos.z = Random.Range(minSpawn.z, maxSpawn.z);
        GameObject warning = Instantiate(visualWarning, new Vector3(spawnPos.x, minSpawn.y, spawnPos.z),Quaternion.identity);
        yield return new WaitForSeconds(timeAfterWarning);
        Destroy(warning);
        Instantiate(prefabToSpawn, new Vector3(spawnPos.x, maxSpawn.y, spawnPos.z), Quaternion.identity);
    }

}
