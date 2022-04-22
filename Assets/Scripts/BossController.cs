using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class BossController : MonoBehaviour
{
    //References
    private GameObject player;
    private MeshRenderer meshRenderer;
    private ObjectPoolController objectPoolController;
    public GameObject deathParticleSystem;
    public Material normalMaterial;
    public Material invincibleMaterial;
    public GameObject objectPoolerObject;
    public ObjectPool<GameObject> circleBurstBossBulletPool;
    public ObjectPool<GameObject> standardShotBossBulletPool;

    #region Health, Damage, and Invincibility
    public int health;
    public float invincibilityTimer;
    private bool invincible;
    IEnumerator InvincibilityHandler()
    {
        while(true)
        {
            if(invincibilityTimer > 0)
            {
                invincible = true;
                meshRenderer.material = invincibleMaterial;
                invincibilityTimer -= Time.deltaTime;
            }
            else
            {
                invincible = false;
                meshRenderer.material = normalMaterial;
            }
        yield return new WaitForFixedUpdate();
        }
    }
    
    public void TakeDamage(int damageValue)
    {
        if(!invincible)
        {
        health -= damageValue;
        }
    }


    float findYRotationToLookAtTarget(Vector3 targetPosition)
    {
        return((-1 * Mathf.Rad2Deg * Mathf.Atan2(player.transform.position.z-transform.position.z, player.transform.position.x-transform.position.x) + 90));
    }
    #endregion



    #region Attack and Bullet Management
    public float circleAttackCooldownInSeconds;
    public float bulletStreamAttackCooldownInSeconds;
    public float bulletStreamBurstCooldownInSeconds;
    void SetNewProjectileTransform(Vector3 origin, Quaternion orientation)
    {
        objectPoolController.newBossProjectilePosition = origin;
        objectPoolController.newBossProjectileRotation = orientation;
    }


    IEnumerator StartAttacking()
    {
        while(true)
        {
            //Circle Burst Attack
            for (int circleBurstAttackNumber = 0; circleBurstAttackNumber < 3; circleBurstAttackNumber++)
            {
                transform.rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
                for (int i = 0; i < 16; i++)
                {
                    SetNewProjectileTransform(transform.position, Quaternion.Euler(0,(i*22.5f),0)*transform.rotation);
                    circleBurstBossBulletPool.Get();
                }

                yield return new WaitForSeconds(circleAttackCooldownInSeconds);
            }

            //Bullet Stream Attack
            yield return new WaitForSeconds(bulletStreamAttackCooldownInSeconds);
            for (int bulletStreamAttackNumber = 0; bulletStreamAttackNumber < 3; bulletStreamAttackNumber++)
            {
                transform.rotation = Quaternion.Euler(0, findYRotationToLookAtTarget(player.transform.position), 0);
                for (int i = 0; i < 3; i++)
                {
                    SetNewProjectileTransform(transform.position, transform.rotation);
                    standardShotBossBulletPool.Get();
                    yield return new WaitForSeconds(bulletStreamBurstCooldownInSeconds);
                }
                yield return new WaitForSeconds(bulletStreamAttackCooldownInSeconds);
            }

        }
    }

    #endregion
    public void DestroySelf()
    { 
        GameObject deathParticleSystemPrefab = Instantiate(deathParticleSystem, transform.position, Quaternion.identity);
        objectPoolController.bossExists = false;
        Destroy(deathParticleSystemPrefab, 1);
        Destroy(gameObject);
    }

    void Start()
    {
        player = GameObject.FindWithTag("Player");
        meshRenderer = GetComponentInChildren<MeshRenderer>();
        objectPoolController = objectPoolerObject.GetComponent<ObjectPoolController>();
        circleBurstBossBulletPool = objectPoolController.circleBurstBossBulletPool;
        standardShotBossBulletPool = objectPoolController.standardShotBossBulletPool;

        StartCoroutine(InvincibilityHandler());
        StartCoroutine(StartAttacking());
    }
    void Update()
    {

    }
    
    void FixedUpdate()
    {
        if(health <= 0)
        {
            DestroySelf();
        }
    }
}
