using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class PlayerStandardShot : MonoBehaviour
{
    public static ObjectPool<GameObject> playerStandardShotPool;
    public float speed;
    public string targetTag; //What tag does the target/enemy have?
    public static float damage;
    IEnumerator DespawnBullet()
    {
        yield return new WaitForSeconds(1);
        KillBullet();
    }
    void KillBullet()
    {
        playerStandardShotPool.Release(gameObject);
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
