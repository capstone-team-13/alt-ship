using NodeCanvas.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_EnemyInstancing : MonoBehaviour
{

    public GameObject enemyModel;
    public float spawnRadius = 10f;
    public Transform ship;

    private bool shipInRange = false;


    private void Update()
    {
        RotateTo();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ship"))
        {

            shipInRange = true;

            if (shipInRange)
            {
                SpawnEnemy();
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Ship"))
        {

            shipInRange = false;

        }
    }

    void SpawnEnemy()
    {
        Vector3 spawnPosition = transform.position;
        spawnPosition.y = 10f;
        enemyModel.SetActive(true);

    }

    void RotateTo()
    {
        if (enemyModel.activeSelf)
        {
            enemyModel.transform.LookAt(ship.position);
        }
    }


}


