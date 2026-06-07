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

    [Header("Utility Settings")]
    [SerializeField] private float utilityCooldown = 7f;
    [SerializeField] private float utilityDuration = 5f;

    [Header("Super Settings")]
    [SerializeField] private GameObject superProjectilePrefab;
    [SerializeField] private float superCooldown = 15f;
    [SerializeField] private float superAttackDuration = 1.5f;

    private bool isImmune = false;

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
            Debug.LogError("No Animator component found on the Boss GameObject!");
            return;
        }

        StartCoroutine(BasicAttackRoutine());
        StartCoroutine(UtilityRoutine());
        StartCoroutine(SuperRoutine());
    }

    private IEnumerator BasicAttackRoutine()
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

    private IEnumerator UtilityRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(utilityCooldown);
            yield return StartCoroutine(ActivateUtility());
        }
    }

    private IEnumerator ActivateUtility()
    {
        isImmune = true;
        StartCoroutine(FlashGold());
        Debug.Log("Boss utility active - immune for " + utilityDuration + "s");

        yield return new WaitForSeconds(utilityDuration);

        isImmune = false;
        Debug.Log("Boss utility expired");
    }

    private IEnumerator FlashGold()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        Color gold = new Color(1f, 0.84f, 0f);

        sr.color = gold;
        yield return new WaitForSeconds(utilityDuration);
        sr.color = Color.white;
    }

    private IEnumerator SuperRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(superCooldown);
            StartCoroutine(FireSuper());
        }
    }

    private IEnumerator FireSuper()
    {
        anim.SetBool("isSuper", true);
        Debug.Log("FireSuper called, prefab assigned: " + (superProjectilePrefab != null));

        if (superProjectilePrefab != null)
        {
            // Spawn fixed position to the left of the boss
            Vector3 spawnPosition = new Vector3(
                transform.position.x + 6f,
                transform.position.y - 2f,
                transform.position.z
            );
            Debug.Log("Spawning super at: " + spawnPosition);
            GameObject projectile = Instantiate(superProjectilePrefab, spawnPosition, Quaternion.identity);
            Destroy(projectile, superAttackDuration);
        }

        yield return new WaitForSeconds(superAttackDuration);
        anim.SetBool("isSuper", false);
    }

    public bool IsImmune => isImmune;
}