using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SunSystem : MonoBehaviour
{
    public int SunValue;
    GameManager gm;

    void Awake()
    {
        gm = Object.FindFirstObjectByType<GameManager>();
    }

    private void OnMouseDown()
    {
        if (gm != null)
        {
            gm.AddSun(SunValue);
            Destroy(gameObject);
        }
    }
}
