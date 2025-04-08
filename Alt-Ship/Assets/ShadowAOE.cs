using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowAOE : MonoBehaviour
{
    [SerializeField] private NewTentacleSystem newTentacleSystem;

    private void OnEnable()
    {
        this.gameObject.transform.localScale = new Vector3(newTentacleSystem.outerDistanceInstant * 2, 1f, newTentacleSystem.outerDistanceInstant * 2);
    }


}
