using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireController : MonoBehaviour
{
    [SerializeField] GameObject firePref;
    [SerializeField] GameObject visualWarning;
    [SerializeField] float timeBetweenSpawn;
    [SerializeField] float timeAfterWarning;
    [SerializeField] float zToSpawn;
    [SerializeField] float yToSpawn;
    [SerializeField] float minXToSpawn;
    [SerializeField] float maxXToSpawn;

    bool spawning = false;
    public void StartSpawning()
    {
        spawning = true;
        StartCoroutine(SpawnFires());
    }

    public void StopSpawning()
    {
        spawning = false;
    }

    IEnumerator SpawnFires()
    {
        while (spawning)
        {
            yield return new WaitForSeconds(timeBetweenSpawn);
            StartCoroutine(SpawnSingleFallingObst());
        }
    }

    IEnumerator SpawnSingleFallingObst()
    {
        float xToSpawn;
        xToSpawn = Random.Range(minXToSpawn, maxXToSpawn);
        GameObject warning = Instantiate(visualWarning, new Vector3(xToSpawn, yToSpawn, zToSpawn), Quaternion.identity);
        yield return new WaitForSeconds(timeAfterWarning);
        Destroy(warning);
        Instantiate(firePref, new Vector3(xToSpawn, yToSpawn, zToSpawn), Quaternion.identity);
    }
}
