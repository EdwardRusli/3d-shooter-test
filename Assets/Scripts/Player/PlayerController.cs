using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using TMPro;
public class PlayerController : MonoBehaviour
{
    //Controls
    private float horizontalInput;
    private float verticalInput;
    private KeyCode fireButton = KeyCode.Space;
    private KeyCode focusButton = KeyCode.LeftShift;
    private bool focusButtonHeld;

    //Movement
    private float horizontalSpeed;
    private float verticalSpeed;
    private bool isMoving;

    //Mouse Aim
    private float mouseX;

    [Header("References")]
    //References
    [SerializeField] private GameObject crosshairSprite;
    private Rigidbody rb;
    [SerializeField] private GameObject objectPoolerObject;
    private ObjectPool<GameObject> playerBulletPool;
    [SerializeField] private GameObject playerCamera;
    private MeshRenderer meshRenderer;
    private ObjectPoolController objectPoolController;
    [SerializeField] private GameObject deathParticleSystem;
    [SerializeField] private Material normalMaterial;
    [SerializeField] private Material invincibleMaterial;
    private ParticleSystem thrusterParticleSystem;
    public GameObject statistics;

    [Header("Gameplay Settings")]
    public float mouseSensitivity;

    [Header("Player Settings")]
    public float baseMoveSpeed;
    private float moveSpeed;
    #region Health, Damage, and Invincibility
    public float health;
    [Tooltip("Maximum health. Has to match base health")] public float maxHealth;
    [Tooltip("Base maximum health before any modifiers are applied. Has to match maximum health")] public float baseHealth;
    [Tooltip("Seconds between each regeneration")] public float regenerationInterval;
    [Tooltip("Amount to regenerate each regeneration")] public float regenerationAmount;
    public float shield;
    [Tooltip("Maximum shield. Has to match base shield")] public float maxShield;
    [Tooltip("Base maximum shield before any modifiers are applied. Has to match maximum shield")] public float baseShield;
    [Tooltip("Seconds between each regeneration")] public float shieldRegenerationInterval;
    [Tooltip("Amount to regenerate each regeneration")] public float shieldRegenerationAmount;
    [Tooltip("How many seconds of not taking damage before regeneration starts?")] public float regenerationDelay;

    [HideInInspector] public float invincibilityTimer;
    private bool invincible;

    private float secondsSinceTakeDamage;
    IEnumerator HealthRegenerationHandler()
    {
        while (true)
        {
            if(health < maxHealth && secondsSinceTakeDamage >= regenerationDelay)
            {
                health += regenerationAmount;
                yield return new WaitForSeconds(regenerationInterval);
            }
            else{ yield return new WaitForFixedUpdate(); }
        }
    }
    IEnumerator ShieldRegenerationHandler()
    {
        while (true)
        {
            if(health >= maxHealth && shield < maxShield && secondsSinceTakeDamage >= regenerationDelay)
            {
                shield += shieldRegenerationAmount;
                yield return new WaitForSeconds(shieldRegenerationInterval);
            }
            else{ yield return new WaitForFixedUpdate(); }
        }
    }
    IEnumerator InvincibilityHandler()
    {
        while (true)
        {
            if (invincibilityTimer > 0)
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
    public void TakeDamage(float damageValue)
    {
        if (!invincible)
        {
            secondsSinceTakeDamage = 0;
            if(shield > 0)
            {
                shield -= damageValue;
            }
            else
            {
                health -= damageValue;
            }

            if(shield < 0)
            {
                health += shield;
                shield = 0;
            }
        }
    }
    #endregion
    #region Attack and Bullet Management
    public float attackDamage;
    public float attackInterval;
    IEnumerator FireShot()
    {
        while (Input.GetKey(fireButton))
        {
            PlayerStandardShot.damage = attackDamage;
            objectPoolController.newPlayerBulletPosition = transform.position;
            objectPoolController.newPlayerBulletRotation = transform.rotation;
            playerBulletPool.Get();
            yield return new WaitForSeconds(attackInterval);
        }
    }
    #endregion
    #region Item Management
    private Dictionary<string, int> currentItems = new Dictionary<string, int>();

    public void AddItem(ItemData itemData)
        {
        if (!currentItems.ContainsKey(itemData.name)) { currentItems.Add(itemData.name, 0); }
        currentItems[itemData.name] = currentItems[itemData.name] + 1;
        UpdateStats();
        }

    public void UpdateStats()
    {
        try { maxHealth = baseHealth * (1+(0.1f*currentItems["HealthItem"])); }
        catch { maxHealth = baseHealth; }
        statistics.GetComponent<TextMeshProUGUI>().text = string.Format(@"Statistics

Max HP: {0}
Max Shield: {1}
Regeneration Interval: {2}
Regeneration Amount: {3}
Shield Regeneration Interval: {4}
Shield Regeneration Amount: {5}
Attack Damage: {6}
Attack Interval: {7}", maxHealth, maxShield, regenerationInterval, regenerationAmount, shieldRegenerationInterval, shieldRegenerationAmount, attackDamage, attackInterval);
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
        UpdateStats();
        rb = GetComponent<Rigidbody>();
        objectPoolController = objectPoolerObject.GetComponent<ObjectPoolController>();
        thrusterParticleSystem = GetComponent<ParticleSystem>();
        playerBulletPool = objectPoolController.playerStandardShotPool;
        meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.material = normalMaterial;
        Cursor.lockState = CursorLockMode.Locked;
        StartCoroutine(InvincibilityHandler());
        StartCoroutine(HealthRegenerationHandler());
        StartCoroutine(ShieldRegenerationHandler());
    }
    

    void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
        mouseX = Input.GetAxis("Mouse X");

        if(horizontalInput != 0 || verticalInput != 0) { isMoving = true; } else { isMoving = false; }


        //Player Camera
        playerCamera.transform.Rotate(0, mouseX * mouseSensitivity, 0);
        playerCamera.transform.position = transform.position;

        //Fire Bullet
        if (Input.GetKeyDown(fireButton)) { StartCoroutine(FireShot()); }
        //Focus
        if(Input.GetKey(focusButton)) { focusButtonHeld = true; } else { focusButtonHeld = false; }
    }



    void FixedUpdate()
    {
        horizontalSpeed = horizontalInput * moveSpeed;
        verticalSpeed = verticalInput * moveSpeed;

        if (focusButtonHeld) { transform.rotation = playerCamera.transform.rotation; moveSpeed = baseMoveSpeed * 0.5f; }
        else 
        { 
            if (isMoving)
            { 
                transform.rotation = Quaternion.Euler(0, -1 * Mathf.Rad2Deg * Mathf.Atan2(rb.velocity.z, rb.velocity.x) + 90, 0);
            } 
            moveSpeed = baseMoveSpeed;
        }
        
        rb.velocity = playerCamera.transform.TransformDirection(new Vector3(horizontalSpeed, 0, verticalSpeed));
        secondsSinceTakeDamage += Time.fixedDeltaTime;
        if (isMoving) { thrusterParticleSystem.enableEmission = true; print("enabled particle system"); } else { thrusterParticleSystem.enableEmission = false; }


        //Check if looking at target
        RaycastHit hit;
        if(Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity) && hit.transform.tag == "Enemy")
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
            Vector3 targetPositionInScreenSpace = Camera.main.WorldToScreenPoint(hit.point);
            crosshairSprite.SetActive(true);
            crosshairSprite.transform.position = targetPositionInScreenSpace;
        }
        else{crosshairSprite.SetActive(false);}

        if(health <= 0) { DestroySelf(); }
    }
}
