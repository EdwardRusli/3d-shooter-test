using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    private PlayerController playerController;
    private GameObject playerCharacter;
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
        playerController.returnBulletToPool(gameObject);
    }
    void OnEnable()
    {
        playerCharacter = GameObject.Find("Player");
        playerController = playerCharacter.GetComponent<PlayerController>();
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
            print("Target has " + bossController.health + " health remaining.");
            bossController.invincibilityTimer += 0.05f;
            KillBullet();
        }
    }
}
