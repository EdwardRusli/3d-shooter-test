using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossCircleBurstBullet : MonoBehaviour
{
    private BossController bossController;
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
        bossController.circleBurstBossBulletPool.Release(gameObject);
    }
    void OnEnable()
    {
        bossController = GameObject.Find("Boss").GetComponent<BossController>();
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
            print("Player has " + playerController.health + " health remaining.");
            playerController.invincibilityTimer += 0.05f;
            KillBullet();
        }
    }
}