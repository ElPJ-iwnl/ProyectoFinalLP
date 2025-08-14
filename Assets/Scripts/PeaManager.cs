using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeaManager : MonoBehaviour
{
    public float damage;

    private void OnTriggerEnter2D(Collider2D col)
    {
        var zombie = col.GetComponentInParent<ZombieController>();
        if (zombie != null)
        {
            zombie.DealDamage(damage);
            Destroy(gameObject);
        }
    }
}

