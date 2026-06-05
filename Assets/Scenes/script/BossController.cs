using System.Collections;
using UnityEngine;

public class BossController : MonoBehaviour
{
    private Animator anim;
    private Transform playerTransform;

    [Header("Attack Timing Settings")]
    [SerializeField] private float timeBetweenAttacks = 15.0f; 
    [SerializeField] private float attackDuration = 1.5f;       

    [Header("Rain Attack Settings")]
    [SerializeField] private GameObject rainDropPrefab; 
    [SerializeField] private int numberOfDrops = 5;      
    [SerializeField] private float delayBetweenDrops = 0.2f; 
    [SerializeField] private float spawnHeight = 5f;     
    [SerializeField] private float horizontalSpread = 4f;  
    
    void Start()
    {
        anim = GetComponent<Animator>();

        GameObject player = GameObject.Find("Artist");
        if (player != null)
            playerTransform = player.transform;
        else
            Debug.LogError("Could not find Artist GameObject in scene!");

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
            yield return new WaitForSeconds(timeBetweenAttacks);

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
            if (rainDropPrefab != null && playerTransform != null)
            {
                float randomX = Random.Range(-horizontalSpread, horizontalSpread);

                Vector3 spawnPosition = new Vector3(
                    playerTransform.position.x + randomX, 
                    playerTransform.position.y + spawnHeight, 
                    transform.position.z
                );

                GameObject newDrop = Instantiate(rainDropPrefab, spawnPosition, Quaternion.identity);
                Destroy(newDrop, 3f);
            }

            yield return new WaitForSeconds(delayBetweenDrops);
        }
    }
}