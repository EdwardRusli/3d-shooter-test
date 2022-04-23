using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class BossStandardShotBullet : MonoBehaviour
{
    private ObjectPool<GameObject> standardShotBossBulletPool;
    public float speed;
    public string targetTag; //What tag does the target/enemy have?
    public int damage; //How much damage will the bullet deal on the target/enemy?
    IEnumerator DespawnBullet()
    {
        yield return new WaitForSeconds(5);
        KillBullet();
    }
    void KillBullet()
    {
        standardShotBossBulletPool.Release(gameObject);
    }
    void Start()
    {
        standardShotBossBulletPool = GameObject.Find("Object Pooler").GetComponent<ObjectPoolController>().standardShotBossBulletPool;
    }
    void OnEnable()
    {
        
        StartCoroutine(DespawnBullet());
    }

    void FixedUpdate()
    {
        transform.Translate(Vector3.forward * speed);
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.tag == targetTag)
        {
            PlayerController playerController = other.GetComponent<PlayerController>();
            playerController.TakeDamage(damage);
            playerController.invincibilityTimer += 0.05f;
            KillBullet();
        }
    }
}
