using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class PlayerBullet : MonoBehaviour
{
    private ObjectPool<GameObject> playerBulletPool;
    public float speed;
    public string targetTag; //What tag does the target/enemy have?
    public int damage; //How much damage will the bullet deal on the target/enemy?
    IEnumerator DespawnBullet()
    {
        yield return new WaitForSeconds(1);
        KillBullet();
    }
    void KillBullet()
    {
        playerBulletPool.Release(gameObject);
    }
    void Start()
    {
        playerBulletPool = GameObject.Find("Object Pooler").GetComponent<ObjectPoolController>().playerBulletPool;
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
            BossController bossController = other.GetComponent<BossController>();
            bossController.TakeDamage(damage);
            bossController.invincibilityTimer += 0.05f;
            KillBullet();
        }
    }
}
