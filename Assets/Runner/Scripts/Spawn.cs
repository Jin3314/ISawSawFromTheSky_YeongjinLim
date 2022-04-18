using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawn : MonoBehaviour
{
    public GameObject Saw;
    public float startTime = 1;
    public float repeatTime = 0.5f;

    // Start is called before the first frame update
    private void Start()
    {
        InvokeRepeating("SawSpawn", startTime, repeatTime);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void SawSpawn()
    {
        float xPos = Random.Range(60, -60);
        float zPos = Random.Range(60, -60);
        Vector3 SawPos = new Vector3(xPos, 60, zPos);
        Instantiate(Saw, SawPos, Saw.transform.rotation);
    }
}
