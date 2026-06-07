using UnityEngine;

public class BossSuperProjectile : MonoBehaviour
{
    public float damage = 130f;
    private bool hasHit = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Artist" && !hasHit)
        {
            hasHit = true;
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
                playerHealth.TakeDamage(damage);

            Debug.Log("Super hit player for " + damage + " damage!");
        }
    }
}