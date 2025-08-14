using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotsManagerCollider : MonoBehaviour
{
    public GameObject plant;
    public bool isOccupied = false;

    [Header("Fila")]
    public int rowIndex = 0; 

    void OnMouseOver()
    {
        var cards = Object.FindObjectsByType<CardManager>(FindObjectsSortMode.None);
        foreach (var item in cards)
        {
            item.colliderName = this;
            item.isOverCollider = true;
        }

        if (plant == null)
        {
            var dragging = GameObject.FindGameObjectWithTag("Plant");
            if (dragging != null)
            {
                plant = dragging;
                plant.transform.SetParent(transform);
                plant.transform.localPosition = new Vector3(0, 0, -1);
            }
        }
        else
        {
            isOccupied = false;
        }
    }
}


