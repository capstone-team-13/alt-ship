using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewTentacleAttack : MonoBehaviour
{
    [Header("Attack Mode")]
    [SerializeField] private bool left;
    [SerializeField] private bool right;

    [Header("Variables")]
    [SerializeField] float moveDuration;

    [Header("References")]
    [SerializeField] private GameObject ship;
    [SerializeField] private GameObject endLocation;
    [SerializeField] private SailMovementSystem sailMovementSystem;
    [SerializeField] private SailFunction sailFunction;
    [SerializeField] private NewTentacleCombat newTentacleCombat;

    [Header("Gizmo Toggle")]
    [SerializeField] private bool toggle;

    private bool isMoving = false;

    private void Awake()
    {
        GameObject obj = GameObject.FindGameObjectWithTag("ShipParent");
        if (obj != null)
        {
            ship = obj;

            sailMovementSystem = obj.GetComponent<SailMovementSystem>();
            sailFunction = obj.GetComponentInChildren<SailFunction>();
            newTentacleCombat = obj.GetComponent<NewTentacleCombat>();
        }
    }


private IEnumerator MoveToEnd()
    {
        yield return new WaitForSeconds(1f);

        isMoving = true;

        Vector3 startPos = ship.transform.position;
        Quaternion startRot = ship.transform.rotation;

        float elapsedTime = 0f;

        while(elapsedTime < moveDuration)
        {
            float t = elapsedTime / moveDuration;
            t = Mathf.SmoothStep(0, 1, t);

            ship.transform.position = Vector3.Lerp(startPos, endLocation.transform.position, t);
            ship.transform.rotation = Quaternion.Slerp(startRot, endLocation.transform.rotation, t);

            elapsedTime += Time.deltaTime;
            yield return null;

        }

        ship.transform.position = endLocation.transform.position;
        ship.transform.rotation = endLocation.transform.rotation;

        newTentacleCombat.InitiateAttack(left, right);

        isMoving = false;

    }


    private void OnDrawGizmos()
    {
        // Preview where the ship will go
        if (toggle)
        {
            Matrix4x4 originalMatrix = Gizmos.matrix;

            Gizmos.matrix = Matrix4x4.TRS(endLocation.transform.position, endLocation.transform.rotation, Vector3.one);

            Gizmos.color = Color.green;

            Gizmos.DrawWireCube(Vector3.zero, new Vector3(17f, 5f, 50f));

            Gizmos.DrawWireSphere(new Vector3(Vector3.zero.x, Vector3.zero.y, Vector3.zero.z + 15f), 5f);

            if (right)
            {
                Gizmos.color = Color.red;

                Gizmos.DrawWireSphere(new Vector3(Vector3.zero.x + 37f, Vector3.zero.y, Vector3.zero.z - 2.5f), 3.5f);
            }

            if (left)
            {
                Gizmos.color = Color.blue;

                Gizmos.DrawWireSphere(new Vector3(Vector3.zero.x - 37f, Vector3.zero.y, Vector3.zero.z - 2.5f), 3.5f);
            }

            Gizmos.matrix = originalMatrix;
        }
    }

     public void StartAttack()
    {
        if(!isMoving && ship != null)
        {
            sailFunction.disableSail();
            sailMovementSystem.DisableSailing();
            StartCoroutine(MoveToEnd());

        }
    }

    public void StartGrab()
    {
        newTentacleCombat.InitiateMovement(left, right);
    }

}
