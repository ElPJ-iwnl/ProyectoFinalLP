using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantManager : MonoBehaviour
{
    public PlantCardScriptableObject thisSO;
    public Transform shootPoint;
    public GameObject Bullet;
    public float health, damage, range, speed, fireRate;
    public LayerMask zombieLayer; 

    [Header("Tolerancia vertical para 'misma fila'")]
    public float laneTolerance = 1.0f; 


    public bool isMine;
    public float growDuration;
    public GameObject explosion;
    public Sprite grownSprite;
    public float blinkingRate;
    public Sprite[] states;
    public int stateCount;
    public bool isGrown;

    public bool isDragging = true;

    void Start()
    {
        health  = thisSO.health;
        damage  = thisSO.damage;
        range   = thisSO.range;
        speed   = thisSO.speed;
        Bullet  = thisSO.Bullet;
        zombieLayer = thisSO.zombieLayer;
        fireRate = thisSO.fireRate;

        isMine = thisSO.isMine;
        growDuration = thisSO.growDuration;
        explosion = thisSO.Explosion;
        grownSprite = thisSO.grownSprite;
        blinkingRate = thisSO.blinkingRate;
        states = thisSO.mineStates;

        if (isMine)
        {
            StartCoroutine(mineStateUpdate());
            StartCoroutine(blink());
        }

        StartCoroutine(Attack());
    }

    void Update()
    {
        if (health <= 0) Destroy(gameObject);
    }

    IEnumerator Attack()
    {
        yield return new WaitUntil(() => !isDragging);

        while (true)
        {
            yield return new WaitForSeconds(fireRate);

            if (speed <= 0) continue;

            var zombies = GameObject.FindGameObjectsWithTag("Zombie");
            bool hayObjetivo = false;

            for (int i = 0; i < zombies.Length; i++)
            {
                Transform z = zombies[i].transform;

                if (Mathf.Abs(z.position.y - transform.position.y) > laneTolerance) continue;

                float dx = z.position.x - shootPoint.position.x;
                if (dx > 0f && dx <= range)
                {
                    hayObjetivo = true;
                    break;
                }
            }

            if (hayObjetivo)
            {
                GameObject bullet = Instantiate(Bullet, shootPoint.position, Quaternion.identity);
                var pea = bullet.GetComponent<PeaManager>();
                if (pea) pea.damage = damage;

                var rb = bullet.GetComponent<Rigidbody2D>();
                if (rb)
                {
                    rb.linearVelocity = Vector2.right * speed;
                    rb.linearVelocity       = Vector2.right * speed;
                }
            }
        }
    }

    public IEnumerator mineStateUpdate()
    {
        isGrown = false;
        yield return new WaitUntil(() => !isDragging);
        yield return new WaitForSeconds(growDuration);
        isGrown = true;
    }

    public IEnumerator blink()
    {
        yield return new WaitUntil(() => !isDragging);
        GetComponent<SpriteRenderer>().sprite = states[stateCount];
        yield return new WaitForSeconds(blinkingRate);
        stateCount = isGrown ? (stateCount == 2 ? 3 : 2) : (stateCount == 1 ? 0 : 1);
        StartCoroutine(blink());
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isMine || !isGrown) return;
        if (collision.CompareTag("Zombie"))
            collision.GetComponent<ZombieController>().DealDamage(damage);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!isMine || !isGrown) return;
        if (collision.CompareTag("Zombie"))
        {
            Instantiate(explosion, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }

    public void Damage(float amnt) => health -= amnt;
}



