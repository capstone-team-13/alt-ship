using UnityEngine;

public class SheepDetection : MonoBehaviour
{
    public TentacleDamageDetection parentScript;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Sheep") && parentScript.currentSheep == null && !parentScript.toggle)
        {
            Debug.Log("Sheep detected by SheepDetection Collider");
            parentScript.currentSheep = other.gameObject;
            parentScript.StartCoroutine(parentScript.AttackSheepTime());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == parentScript.currentSheep)
        {
            Debug.Log("Sheep left detection area");
            parentScript.currentSheep = null;
        }
    }
}