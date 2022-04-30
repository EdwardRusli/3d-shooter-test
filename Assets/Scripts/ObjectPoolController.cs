using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ObjectPoolController : MonoBehaviour
{
    //Player References
    public ObjectPool<GameObject> playerStandardShotPool;
    public GameObject playerBulletPrefab;
    //Player Properties
    public Vector3 newPlayerBulletPosition;
    public Quaternion newPlayerBulletRotation;
    public bool playerExists = true;



//Boss
    public Vector3 newBossProjectilePosition;
    public Quaternion newBossProjectileRotation;
    public bool bossExists = true;

    //Circle Burst Attack
    public ObjectPool<GameObject> bossCircleBurstShotPool;
    public GameObject circleBurstBossBulletPrefab;

    
    //Standard Shot
    public ObjectPool<GameObject> bossStandardShotPool;
    public GameObject standardShotBossBulletPrefab;

    

    void Awake()
    {
        playerStandardShotPool = new ObjectPool<GameObject>(
            createFunc: () => Instantiate(playerBulletPrefab),
            actionOnGet: (obj) => {obj.SetActive(true); obj.transform.position = newPlayerBulletPosition; obj.transform.rotation = newPlayerBulletRotation;},
            actionOnRelease: (obj) => obj.SetActive(false),
            actionOnDestroy: (obj) => Destroy(obj),
            collectionCheck: false,
            defaultCapacity: 10,
            maxSize: 10);
        bossCircleBurstShotPool = new ObjectPool<GameObject>(
            createFunc: () => Instantiate(circleBurstBossBulletPrefab),
            actionOnGet: (obj) => {obj.SetActive(true); obj.transform.position = newBossProjectilePosition; obj.transform.rotation = newBossProjectileRotation;},
            actionOnRelease: (obj) => obj.SetActive(false),
            actionOnDestroy: (obj) => Destroy(obj),
            collectionCheck: false,
            defaultCapacity: 10,
            maxSize: 10);
        bossStandardShotPool = new ObjectPool<GameObject>(
            createFunc: () => Instantiate(standardShotBossBulletPrefab),
            actionOnGet: (obj) => {obj.SetActive(true); obj.transform.position = newBossProjectilePosition; obj.transform.rotation = newBossProjectileRotation;},
            actionOnRelease: (obj) => obj.SetActive(false),
            actionOnDestroy: (obj) => Destroy(obj),
            collectionCheck: false,
            defaultCapacity: 10,
            maxSize: 10);

        PlayerStandardShot.playerStandardShotPool = playerStandardShotPool;
        BossCircleBurstShot.bossCircleBurstShotPool = bossCircleBurstShotPool;
        BossStandardShot.bossStandardShotPool = bossStandardShotPool;
    }
}
