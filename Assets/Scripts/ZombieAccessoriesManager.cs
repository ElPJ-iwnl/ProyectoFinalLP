using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieAccessoriesManager : MonoBehaviour
{
    public SpriteRenderer accessoryRenderer;
    public float accessoryHealth = 10f;
    public float accessoryHealthCurrent = 10f;
    public List<Sprite> accessoryStates;

    float divisions = 0;

    void Awake()
    {
        // Auto-asignar SR si no está
        if (accessoryRenderer == null)
            accessoryRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    private void Start()
    {
        
        if (accessoryStates != null && accessoryStates.Count > 0)
            divisions = accessoryHealth / accessoryStates.Count;
        else
            divisions = accessoryHealth; 
    }

    private void Update()
    {
        if (accessoryHealthCurrent <= 0 && accessoryRenderer != null)
        {
            Destroy(accessoryRenderer.gameObject);
        }
    }

    public void TakeDamage(float amnt)
    {
        accessoryHealthCurrent -= amnt;

        if (accessoryRenderer == null || accessoryStates == null || accessoryStates.Count == 0)
            return;

        int index = Mathf.CeilToInt((accessoryHealth - accessoryHealthCurrent) / Mathf.Max(divisions, 0.0001f));
        index = Mathf.Clamp(index, 0, accessoryStates.Count - 1);

        accessoryRenderer.sprite = accessoryStates[index];
        var c = accessoryRenderer.color; 
        c.a = 1f; accessoryRenderer.color = c;
        accessoryRenderer.enabled = true;
    }
}




