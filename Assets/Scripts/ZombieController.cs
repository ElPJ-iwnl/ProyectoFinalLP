using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieController : MonoBehaviour
{
    public ZombieScriptableObject thisZombieSO;
    public ZombieAccessoriesManager zombieAccessories;
    public float speed;
    public float health;
    public float handHealth;
    public float currentHealth;
    public GameObject accessory;
    public float accessoryHealth;
    public float damage;
    public float attackInterval;
    GameObject target;
    public bool isAttacking;

    public float damageDelay = 2f;

    // --- NUEVO: misma idea de tolerancia vertical
    [Header("Lane")]
    public float laneTolerance = 0.45f;

    bool isDying;
    bool attackRoutineRunning;

    private void Start()
    {
        speed = thisZombieSO.zombieSpeed;
        health = thisZombieSO.zombieHealth;
        accessoryHealth = thisZombieSO.accessoryHealth;
        damage = thisZombieSO.zombieDamage;
        handHealth = thisZombieSO.zombieHandHealth;
        attackInterval = thisZombieSO.attackInterval;
        currentHealth = health;

        int zLayer = LayerMask.NameToLayer("Zombie");
        if (zLayer >= 0) gameObject.layer = zLayer;
    }

    private void Update()
    {
        if (target == null) isAttacking = false;

        if (!isAttacking && !isDying)
            transform.position += Vector3.left * speed * Time.deltaTime;

        if (currentHealth <= handHealth && transform.childCount > 1)
        {
            Transform hand = transform.GetChild(1);
            var rbH = hand.GetComponent<Rigidbody2D>();
            if (rbH)
            {
                rbH.bodyType = RigidbodyType2D.Dynamic;
                rbH.gravityScale = 1f;
            }
            hand.SetParent(null);
            Destroy(hand.gameObject, 1.5f);
        }

        if (currentHealth <= 0 && transform.childCount > 0)
        {
            isDying = true;

            Transform head = transform.GetChild(0);
            var rb = head.GetComponent<Rigidbody2D>();
            if (rb)
            {
                rb.bodyType = RigidbodyType2D.Dynamic;
                rb.gravityScale = 1f;
            }
            head.SetParent(null);
            Destroy(head.gameObject, 1.5f);

            Destroy(GetComponent<Rigidbody2D>());
            foreach (var c in GetComponents<BoxCollider2D>()) Destroy(c);

            Destroy(gameObject, 2f);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        var plant = collision.GetComponent<PlantManager>();
        if (plant == null) return;

        // SOLO si está en la MISMA FILA (comparando Y)
        if (Mathf.Abs(plant.transform.position.y - transform.position.y) <= laneTolerance)
        {
            isAttacking = true;
            target = plant.gameObject;
            if (!attackRoutineRunning) StartCoroutine(Attack());
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<PlantManager>() != null)
        {
            isAttacking = false;
            target = null;
        }
    }

    IEnumerator Attack()
    {
        attackRoutineRunning = true;
        while (isAttacking && target != null)
        {
            var pm = target.GetComponent<PlantManager>();
            if (pm != null) pm.Damage(damage);
            yield return new WaitForSeconds(attackInterval);
        }
        attackRoutineRunning = false;
    }

    public void DealDamage(float amnt)
    {
        currentHealth -= amnt;
        if (zombieAccessories != null) zombieAccessories.TakeDamage(amnt);
    }
}




