using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardManager : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
{
    public GameObject UI;
    public SlotsManagerCollider colliderName;
    SlotsManagerCollider prevName;

    public PlantCardScriptableObject plantCardScriptableObject;
    public Sprite plantSprite;
    public GameObject plantPrefab;

    public bool isOverCollider = false;
    GameObject plant;
    bool isHoldingPlant;
    GameManager gm;

    void Awake()
    {
        gm = Object.FindFirstObjectByType<GameManager>();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isHoldingPlant || plant == null) return;

        plant.GetComponent<SpriteRenderer>().sprite = plantSprite;

        if (prevName != colliderName || prevName == null)
        {
            if (colliderName == null || !colliderName.isOccupied)
            {
                plant.transform.position = new Vector3(0, 0, -1);
                plant.transform.localPosition = new Vector3(0, 0, -1);
                isOverCollider = false;
                if (prevName != null) prevName.plant = null;
                prevName = colliderName;
            }
        }
        else
        {
            if (!colliderName.isOccupied)
            {
                plant.transform.position = new Vector3(0, 0, -1);
                plant.transform.localPosition = new Vector3(0, 0, -1);
            }
        }

        if (!isOverCollider)
        {
            var p = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            p.z = -1;
            plant.transform.position = p;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (gm != null && gm.SunAmount >= plantCardScriptableObject.cost)
        {
            isHoldingPlant = true;
            Vector3 pos = new Vector3(0, 0, -1);
            plant = Instantiate(plantPrefab, pos, Quaternion.identity);
            var pm = plant.GetComponent<PlantManager>();
            pm.thisSO = plantCardScriptableObject;
            pm.isDragging = true;
            plant.transform.localScale = plantCardScriptableObject.size;
            plant.GetComponent<SpriteRenderer>().sprite = plantSprite;

            var p = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            p.z = -1;
            plant.transform.position = p;
        }
        else
        {
            Debug.Log("Not enough sun!");
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!isHoldingPlant || plant == null) return;

        if (colliderName != null && !colliderName.isOccupied)
        {
            if (gm != null) gm.DeductSun(plantCardScriptableObject.cost);
            isHoldingPlant = false;
            colliderName.isOccupied = true;
            plant.tag = "Untagged";
            plant.transform.SetParent(colliderName.transform);
            plant.transform.position = new Vector3(0, 0, -1);
            plant.transform.localPosition = new Vector3(0, 0, -1);
            plant.AddComponent<BoxCollider2D>();
            var trig = plant.AddComponent<CircleCollider2D>();
            trig.isTrigger = true;

            var pm = plant.GetComponent<PlantManager>();
            pm.isDragging = false;

            // Si la carta es un girasol, añade SunSpawner con fallbacks seguros
            if (plantCardScriptableObject.isSunFlower)
            {
                SunSpawner sunSpawner = plant.AddComponent<SunSpawner>();
                sunSpawner.isSunFlower = true;

                var tmpl = plantCardScriptableObject.sunSpawnerTemplate;
                if (tmpl != null)
                {
                    sunSpawner.minTime = (tmpl.minTime > 0f) ? tmpl.minTime : 5f;
                    sunSpawner.maxTime = (tmpl.maxTime > sunSpawner.minTime) ? tmpl.maxTime : (sunSpawner.minTime + 3f);
                    sunSpawner.sun     = tmpl.sun;
                }

                // Fallback de prefab del sol si el template no lo trae
                if (sunSpawner.sun == null)
                {
                    var globalSpawner = Object.FindFirstObjectByType<SunSpawner>();
                    if (globalSpawner != null && globalSpawner.sun != null)
                        sunSpawner.sun = globalSpawner.sun;
                }

                if (sunSpawner.sun == null)
                    Debug.LogError("[SunSpawner] No hay prefab de 'Sun' asignado. Asignalo en el Scriptable o en el Sun Manager.");
            }
        }
        else
        {
            isHoldingPlant = false;
            Destroy(plant);
        }
    }
}


