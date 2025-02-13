using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectedLevelData : MonoBehaviour
{
    public static SelectedLevelData Instance { get; private set; }

    public int levelChange;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("SelectedLevelData Instance Created");
        }
        else if (Instance == this)
        {
            Debug.Log("Was the instance");
        }
        else
        {
            Debug.Log("Duplicate Instance Destroyed");
            Destroy(gameObject);
        }
    }

    public void levelChosen(int levelNum)
    {
        levelChange = levelNum;
        Debug.Log("Level set to: " + levelChange);
    }

    public void finishedPurpose()
    {
        Destroy(this.gameObject);
    }

}
