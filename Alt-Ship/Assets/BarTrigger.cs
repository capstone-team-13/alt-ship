using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BarTrigger : MonoBehaviour
{
    [SerializeField] private GameObject slider;


    public void toggleSlider()
    {
        slider.SetActive(!slider.activeSelf);
    }

}
