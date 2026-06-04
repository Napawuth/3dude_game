using UnityEngine;

public class FormatProjectile : MonoBehaviour
{
    public float damage = 24f;
    public float speed = 10f;
    public float maxRange = 15f;

    private Vector3 spawnPosition;
    private int direction;

    public void SetDirection(int dir)
    {
        direction = dir;
    }

    void Start()
    {
        spawnPosition = transform.position;
    }

    void Update()
    {
        // Move in direction of player's facing
        transform.position += new Vector3(direction * speed * Time.deltaTime, 0f, 0f);

        // Destroy once it exceeds max range
        if (Vector3.Distance(transform.position, spawnPosition) >= maxRange)
            Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Boss"))
        {
            EnemyHealth bossHealth = other.GetComponent<EnemyHealth>();
            if (bossHealth != null)
                bossHealth.TakeDamage(damage);

            Destroy(gameObject);
        }

        // Destroy on hitting platform or ground
        if (other.CompareTag("Ground"))
            Destroy(gameObject);
    }
}