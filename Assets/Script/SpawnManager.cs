using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager instance;
    public Transform[] spawnPoint;
    // Start is called before the first frame update

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        //foreach (Transform t in spawnPoint)
        //{
        //    t.gameObject.SetActive(false);
        //}
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Transform GetSpawnPoint()
    {
        return spawnPoint[Random.Range(0, spawnPoint.Length)];
    }
}
