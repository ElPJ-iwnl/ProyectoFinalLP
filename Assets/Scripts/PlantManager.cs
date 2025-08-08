using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantManager : MonoBehaviour
{
    public PlantCardScriptableObject thisSO;
    public Transform shootPoint;
    public GameObject Bullet;
    public float health;
    public float damage;
    public float range;
    public float speed;
    public LayerMask zombieLayer;
    public float fireRate;

    public bool isMine;
    public float growDuration;
    public GameObject explosion;
    public Sprite grownSprite;
    public float blinkingRate;
    public Sprite[] states;
    public int stateCount;
    public bool isGrown;

    public bool isDragging = true;

    private void Start()
    {
        health = thisSO.health;
        damage = thisSO.damage;
        range = thisSO.range;
        speed = thisSO.speed;
        Bullet = thisSO.Bullet;
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

    private void Update()
    {
        if (health <= 0)
        {
            //Dead
            Destroy(this.gameObject);
        }
    }

    public IEnumerator Attack()
    {
        yield return new WaitUntil(() => !isDragging);
        yield return new WaitForSeconds(fireRate);
        if (speed > 0)
        {
            RaycastHit2D hit = Physics2D.Raycast(shootPoint.position, shootPoint.right, range, zombieLayer) ;
            Debug.DrawRay(shootPoint.position, shootPoint.right, Color.red);
            if (hit)
            {
                if (hit.transform.tag == "Zombie")
                {
                    Debug.Log("Hit zombie");
                    GameObject bullet = Instantiate(Bullet, shootPoint.transform.position, Quaternion.identity);
                    bullet.GetComponent<PeaManager>().damage = damage;
                    bullet.GetComponent<Rigidbody2D>().linearVelocity = transform.right * speed;
                }
            }
            StartCoroutine(Attack());
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
        this.GetComponent<SpriteRenderer>().sprite = states[stateCount];
        yield return new WaitForSeconds(blinkingRate);
        stateCount = isGrown ? stateCount == 2 ? 3 : 2 : stateCount == 1 ? 0 : 1;
        StartCoroutine(blink());
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (isMine && isGrown)
        {
            if (collision.gameObject.tag == "Zombie")
            {
                collision.GetComponent<ZombieController>().DealDamage(damage);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (isMine && isGrown)
        {
            if (collision.gameObject.tag == "Zombie")
            {
                Instantiate(explosion, this.transform);
                Destroy(this.gameObject);
            }
        }
    }

    public void Damage(float amnt)
    {
        health -= amnt;
    }
}
