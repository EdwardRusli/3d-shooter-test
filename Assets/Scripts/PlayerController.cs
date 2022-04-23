using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
public class PlayerController : MonoBehaviour
{
    //Controls
    private float horizontalInput;
    private float verticalInput;
    private KeyCode fireButton = KeyCode.Space;

    //Movement
    private float horizontalSpeed;
    private float verticalSpeed;
    public float speed;

    //Mouse Aim
    private float mouseX;
    public float mouseSensitivity;
    public GameObject crosshairSprite;

    //References
    private Rigidbody rb;
    public GameObject objectPoolerObject;
    private ObjectPool<GameObject> playerBulletPool;
    private MeshRenderer meshRenderer;
    private ObjectPoolController objectPoolController;
    public GameObject deathParticleSystem;
    public Material normalMaterial;
    public Material invincibleMaterial;


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
    #endregion
    #region Attack and Bullet Management
    public float fireCooldownInSeconds;
    IEnumerator fireBullet()
    {
        while(Input.GetKey(fireButton))
        {
            objectPoolController.newPlayerBulletPosition = transform.position;
            objectPoolController.newPlayerBulletRotation = transform.rotation;
            playerBulletPool.Get();
            yield return new WaitForSeconds(fireCooldownInSeconds);
        }
    }
    public void returnBulletToPool(GameObject i)
    {
        playerBulletPool.Release(i);
    }
    #endregion
    public void DestroySelf()
    { 
        GameObject deathParticleSystemPrefab = Instantiate(deathParticleSystem, transform.position, Quaternion.identity);
        objectPoolController.playerExists = false;
        Destroy(deathParticleSystemPrefab, 1);
        Destroy(gameObject);
    }




    void Start()
    {
        rb = GetComponent<Rigidbody>();
        objectPoolController = objectPoolerObject.GetComponent<ObjectPoolController>();
        playerBulletPool = objectPoolController.playerBulletPool;
        meshRenderer = GetComponentInChildren<MeshRenderer>();
        meshRenderer.material = normalMaterial;
        Cursor.lockState = CursorLockMode.Locked;
        StartCoroutine(InvincibilityHandler()); 
    }
    

    void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
        mouseX = Input.GetAxis("Mouse X");

        //Fire Bullet
        if(Input.GetKeyDown(fireButton))
        {
            StartCoroutine(fireBullet());
        }


    }


    void LateUpdate()
    {
    }



    void FixedUpdate()
    {
        horizontalSpeed = horizontalInput * speed;
        verticalSpeed = verticalInput * speed;
        rb.velocity = transform.TransformDirection(new Vector3(horizontalSpeed, 0, verticalSpeed));

        //Mouse Look
        transform.Rotate(0, mouseX * mouseSensitivity, 0);

        //Check if looking at target
        RaycastHit hit;
        if(Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity) && hit.transform.tag == "Enemy")
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
            Vector3 targetPositionInScreenSpace = Camera.main.WorldToScreenPoint(hit.point);
            crosshairSprite.SetActive(true);
            crosshairSprite.transform.position = targetPositionInScreenSpace;
        }
        else
        {
            crosshairSprite.SetActive(false);
        }

        if(health <= 0)
        {
            DestroySelf();
        }
    }
}
