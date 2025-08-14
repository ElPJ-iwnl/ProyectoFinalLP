using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieManager : MonoBehaviour
{
    public ZombieScriptableObject[] zombieScriptableObjects;
    public ZombieScriptableObject[] easyZombies;  
    public ZombieScriptableObject[] hardZombies; 
    public ZombieScriptableObject[] mediumZombies;   
    public ZombieScriptableObject selectedSO;
    public float timeInterval;
    public bool randomizeTimes;
    public float minTime;
    public float maxTime;
    public Transform[] columns;
    public int selectedColumns;

    private void Start()
    {
        StartCoroutine(ZombieSpawn());
    }

    public IEnumerator ZombieSpawn()
    {
       timeInterval = randomizeTimes ? Random.Range(minTime, maxTime) : timeInterval;
        yield return new WaitForSeconds(timeInterval);


        ZombieScriptableObject[] pool = zombieScriptableObjects; 

            if (GameSettings.I != null)
            {
                if (GameSettings.I.difficulty == Difficulty.Easy)
                {
                    if (easyZombies != null && easyZombies.Length > 0)
                        pool = easyZombies;
                }
                else if (GameSettings.I.difficulty == Difficulty.Medium)
                {
                    if (mediumZombies != null && mediumZombies.Length > 0)
                    {
                        pool = mediumZombies;
                    }
                    else
                    {

                        bool hasEasy = easyZombies != null && easyZombies.Length > 0;
                        bool hasHard = hardZombies != null && hardZombies.Length > 0;

                        if (hasEasy && hasHard)
                        {
                            float r = Random.value; 
                            pool = (r < 0.7f) ? easyZombies : hardZombies;
                        }
                        else if (hasEasy) pool = easyZombies;
                        else if (hasHard) pool = hardZombies;
                    }
                }
                else 
                {
                    bool hasEasy = easyZombies != null && easyZombies.Length > 0;
                    bool hasHard = hardZombies != null && hardZombies.Length > 0;

                    if (hasEasy && hasHard)
                    {
                        var list = new List<ZombieScriptableObject>(easyZombies.Length + hardZombies.Length);
                        list.AddRange(easyZombies);
                        list.AddRange(hardZombies);
                        pool = list.ToArray();
                    }
                    else if (hasHard) pool = hardZombies;
                    else if (hasEasy) pool = easyZombies;
                }
            }
if (pool == null || pool.Length == 0) pool = zombieScriptableObjects;

        Debug.Log($"[ZombieManager] Dificultad={ (GameSettings.I ? GameSettings.I.difficulty.ToString() : "NULL") } | Pool={pool.Length} | Ejemplo={(pool.Length>0 ? pool[0].name : "VACIO")}");
        selectedSO = pool[Random.Range(0, pool.Length)];

        int columnID = Random.Range(0, columns.Length);
        GameObject zombie = Instantiate(selectedSO.zombieDefault, columns[columnID]);

        var zc = zombie.GetComponent<ZombieController>();
        zc.thisZombieSO = selectedSO;

        zombie.transform.SetParent(columns[columnID]);
        zombie.transform.position = new Vector3(0, 0, -1);
        zombie.transform.localPosition = new Vector3(0, 0, -1);

        if (selectedSO.zombieAccessory != null)
        {
            GameObject accessory = Instantiate(selectedSO.zombieAccessory, zombie.transform);
            zc.accessory = accessory;
            zc.zombieAccessories = accessory.GetComponent<ZombieAccessoriesManager>();
            zc.zombieAccessories.accessoryHealth = selectedSO.accessoryHealth;
            zc.zombieAccessories.accessoryHealthCurrent = selectedSO.accessoryHealth;
        }

        StartCoroutine(ZombieSpawn());
    }
} 