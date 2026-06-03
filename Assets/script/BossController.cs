using System.Collections;
using UnityEngine;

public class BossController : MonoBehaviour
{
    private Animator anim;

    [Header("Attack Timing Settings")]
    [SerializeField] private float minTimeBetweenAttacks = 3f; 
    [SerializeField] private float maxTimeBetweenAttacks = 6f; 
    [SerializeField] private float attackDuration = 1.5f;       

    [Header("Rain Attack Settings")]
    [SerializeField] private GameObject rainDropPrefab; 
    [SerializeField] private int numberOfDrops = 5;      
    [SerializeField] private float delayBetweenDrops = 0.2f; 

    [Header("2D Spawn Position Setup")]
    [SerializeField] private float spawnHeight = 5f;     
    [SerializeField] private float horizontalSpread = 4f;  
    
    [Tooltip("Pushes the rain zone forward. Use a negative number if the boss is facing left!")]
    [SerializeField] private float attackXOffset = -3f; // Adjust this to push it in front of the boss

    void Start()
    {
        anim = GetComponent<Animator>();

        if (anim == null)
        {
            Debug.LogError("ERROR: No Animator component found on the Boss GameObject!");
            return;
        }

        StartCoroutine(RandomAttackRoutine());
    }

    private IEnumerator RandomAttackRoutine()
    {
        while (true)
        {
            float randomCooldown = Random.Range(minTimeBetweenAttacks, maxTimeBetweenAttacks);
            yield return new WaitForSeconds(randomCooldown);

            anim.SetBool("isAttack", true);

            StartCoroutine(SpawnRainBurst());

            yield return new WaitForSeconds(attackDuration);

            anim.SetBool("isAttack", false);
        }
    }

    private IEnumerator SpawnRainBurst()
    {
        for (int i = 0; i < numberOfDrops; i++)
        {
            if (rainDropPrefab != null)
            {
                float randomX = Random.Range(-horizontalSpread, horizontalSpread);
                
                // Calculate the center point of the attack by adding the horizontal offset
                float spawnCenterX = transform.position.x + attackXOffset;

                Vector3 finalSpawnPosition = new Vector3(
                    spawnCenterX + randomX, 
                    transform.position.y + spawnHeight, 
                    transform.position.z
                );

                GameObject newDrop = Instantiate(rainDropPrefab, finalSpawnPosition, Quaternion.identity);
                
                Destroy(newDrop, 3f);
            }

            yield return new WaitForSeconds(delayBetweenDrops);
        }
    }
}