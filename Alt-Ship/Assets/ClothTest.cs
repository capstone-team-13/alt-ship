using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClothTest : MonoBehaviour
{

    public float currentHeight;
    private float lastHeight;

    void Update()
    {
        if(lastHeight != currentHeight)
        {
            ResizeSail(currentHeight);
            lastHeight = currentHeight;
        }
    }

    void ResizeSail(float heightFactor)
    {
        heightFactor = Mathf.Clamp(heightFactor, 0.2f, 1f);

        Cloth sailCloth = GetComponent<Cloth>();
        sailCloth.enabled = false; // Temporarily disable cloth physics

        transform.localScale = new Vector3(1, heightFactor, 1); // Resize the sail
        RefreshCloth();
        Invoke(nameof(ReEnableCloth), 0.1f); // Re-enable after a short delay
    }

    void ReEnableCloth()
    {
        GetComponent<Cloth>().enabled = true;
    }

    void RefreshCloth()
    {
        Cloth sailCloth = GetComponent<Cloth>();
        if (sailCloth != null)
        {
            ClothSkinningCoefficient[] constraints = sailCloth.coefficients;
            for (int i = 0; i < constraints.Length; i++)
            {
                constraints[i].maxDistance *= transform.localScale.y; // Scale constraint distances
            }
            sailCloth.coefficients = constraints;
        }
    }
}
