using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public TMP_Text sunDisp;
    public int startingSunAmnt = 50;
    public int SunAmount = 0;

    void Start()
    {
        // Si hay GameSettings, usar su valor inicial
        if (GameSettings.I != null) startingSunAmnt = GameSettings.I.startingSun;
        AddSun(startingSunAmnt);
    }

    public void AddSun(int amnt)
    {
        SunAmount += amnt;
        if (sunDisp) sunDisp.text = "" + SunAmount;
    }

    public void DeductSun(int amnt)
    {
        SunAmount -= amnt;
        if (sunDisp) sunDisp.text = "" + SunAmount;
    }
}


