using UnityEngine;

public enum Difficulty { Easy, Medium, Hard }

public class GameSettings : MonoBehaviour
{
    public static GameSettings I;


    public Difficulty difficulty = Difficulty.Easy;

    public float zombieSpeedMultiplier = 1f;   
    public float sunSpawnIntervalMultiplier = 0.5f; 
    public int startingSun = 50;    
    public bool overrideSunTimes = false;
    public float sunMinTime = 3f;
    public float sunMaxTime = 6f;

    public bool overrideZombieSpeed = false;
    public float zombieSpeedAbs = 1.0f;

    public bool overridePeaFireRate = false;
    public float peaFireRateAbs = 1.0f;         
    void Awake()
    {
        if (I != null && I != this) { Destroy(gameObject); return; }
        I = this;
        DontDestroyOnLoad(gameObject);
    }

    public void ApplyEasy()
    {
        difficulty = Difficulty.Easy;

        zombieSpeedMultiplier = 1f;
        sunSpawnIntervalMultiplier = 0.6f;  
        startingSun = 300;                
        overrideSunTimes = false;
        overrideZombieSpeed = false;
        overridePeaFireRate = false;
    }
        public void ApplyMedium()
    {
        difficulty = Difficulty.Medium;

        zombieSpeedMultiplier = 1f;
        sunSpawnIntervalMultiplier = 1f;  
        startingSun = 200;                
        overrideSunTimes = false;
        overrideZombieSpeed = false;
        overridePeaFireRate = false;
    }

    public void ApplyHard()
    {
        difficulty = Difficulty.Hard;
        zombieSpeedMultiplier = 1f;
        sunSpawnIntervalMultiplier = 1.4f;
        startingSun = 150;
        overrideSunTimes = false;
        overrideZombieSpeed = false;
        overridePeaFireRate = false;
    }

}
