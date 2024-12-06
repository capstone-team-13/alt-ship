using UnityEngine;

[RequireComponent(typeof(ShipModel))]
public class Damage : MonoBehaviour

{
    public int damageAmount = 1;
    public float damageInterval = 10f;

    private float timeSinceAttack = 0f;
    private bool shipInCollider = false;

    private ShipModel m_ship;

    private void Start()
    {
        m_ship = GetComponent<ShipModel>();
    }

    private void Update()
    {
        if (shipInCollider)
        {

            timeSinceAttack += Time.deltaTime;


            if (timeSinceAttack >= damageInterval)
            {
                DealDamageToShip();

                timeSinceAttack = 0f;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Ship"))
        {
            shipInCollider = true;

            DealDamageToShip();
        }
    }

    private void OnTriggerExit(Collider other)
    {

        if (other.CompareTag("Ship"))
        {
            shipInCollider = false;

            timeSinceAttack = 0f;
        }
    }

    private void DealDamageToShip()
    {


        if (m_ship.Health != 0)
        {
            // ship.Health.TakeDamage(damageAmount);
        }
    }

    public void SetupAttack()
    {
        // call animation here
    }

}