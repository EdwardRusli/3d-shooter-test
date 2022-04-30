using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class BossCircleBurstShot : MonoBehaviour
{
    public static ObjectPool<GameObject> bossCircleBurstShotPool;
    public float speed;
    public string targetTag; //What tag does the target/enemy have?
    public static float damage;
    IEnumerator DespawnBullet()
    {
        yield return new WaitForSeconds(5);
        KillBullet();
    }
    void KillBullet()
    {
        bossCircleBurstShotPool.Release(gameObject);
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
