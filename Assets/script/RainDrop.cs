using UnityEngine;

public class RainDrop : MonoBehaviour
{
    public float damage = 18f;
    // Mode A: Runs instantly when hitting a solid 3D surface
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "Artist")
        {
            PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
            if (playerHealth != null)
                playerHealth.TakeDamage(damage);
        }
        Destroy(gameObject);
        // This will instantly delete the drop when it touches ANY object
            // (This completely bypasses tag mistakes, layer bugs, and typos)
            Destroy(gameObject);
    }

    // Mode B: Runs instantly if your floor is set to "Is Trigger"
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Artist")
        {
            PlayerHealth playerHealth = other.gameObject.GetComponent<PlayerHealth>();
            if (playerHealth != null)
                playerHealth.TakeDamage(damage);
        }
        Destroy(gameObject);
    }
}