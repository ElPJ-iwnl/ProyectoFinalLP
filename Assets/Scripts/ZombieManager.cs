using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieManager : MonoBehaviour
{
    [Header("Pools")]
    public ZombieScriptableObject[] zombieScriptableObjects; // opcional/base
    public ZombieScriptableObject[] easyZombies;             // Fácil
    public ZombieScriptableObject[] hardZombies;             // Difícil (bucket, etc.)
    public ZombieScriptableObject[] mediumZombies;           // Medio (cone)

    [Header("Spawn")]
    public float timeInterval = 5f;
    public bool randomizeTimes = true;
    public float minTime = 3f;
    public float maxTime = 8f;
    [Tooltip("Un Transform por FILA (1..5 de arriba a abajo)")]
    public Transform[] columns;

    ZombieScriptableObject selectedSO;

    void Start()
    {
        StartCoroutine(ZombieSpawn());
    }

    IEnumerator ZombieSpawn()
    {
        float wait = randomizeTimes ? Random.Range(minTime, maxTime) : timeInterval;
        yield return new WaitForSeconds(wait);

        // -------- seleccionar pool según dificultad --------
        ZombieScriptableObject[] pool = null;

        if (GameSettings.I != null)
        {
            switch (GameSettings.I.difficulty)
            {
                case Difficulty.Easy:
                    pool = NonEmpty(easyZombies);
                    break;

                case Difficulty.Medium:
                    pool = NonEmpty(mediumZombies); // pon aquí SOLO el de cono si quieres
                    break;

                case Difficulty.Hard:
                    pool = MergeNonEmpty(easyZombies, mediumZombies, hardZombies); // todos
                    break;
            }
        }

        // Fallbacks
        if (pool == null || pool.Length == 0) pool = NonEmpty(zombieScriptableObjects);
        if (pool == null || pool.Length == 0) pool = MergeNonEmpty(easyZombies, mediumZombies, hardZombies);
        if (pool == null || pool.Length == 0) yield break;

        // -------- instanciar --------
        selectedSO = pool[Random.Range(0, pool.Length)];

        if (columns == null || columns.Length == 0)
        {
            Debug.LogError("[ZombieManager] Columns vacío: asigna 5 transforms (1..5).");
            yield break;
        }

        int columnID = Random.Range(0, columns.Length);
        Transform lane = columns[columnID];
        if (lane == null)
        {
            Debug.LogError($"[ZombieManager] columns[{columnID}] es NULL.");
            yield break;
        }

        // instancia como hijo y resetea local
        GameObject zombie = Instantiate(selectedSO.zombieDefault, lane);
        var zc = zombie.GetComponent<ZombieController>();
        zc.thisZombieSO = selectedSO;

        zombie.transform.localPosition = new Vector3(0, 0, -1f);

        // ------ accesorio: parent a la CABEZA y ordenar delante ------
        if (selectedSO.zombieAccessory != null)
        {
            GameObject acc = Instantiate(selectedSO.zombieAccessory);
            acc.SetActive(true);

            // cabeza = child(0) según tu ZombieController
            Transform head = (zombie.transform.childCount > 0) ? zombie.transform.GetChild(0) : zombie.transform;

            // parent y mantener local del prefab
            acc.transform.SetParent(head, false);

            // igualar sorting layer/orden para que se vea delante
            var headSR = head.GetComponentInChildren<SpriteRenderer>();
            var accSR  = acc.GetComponentInChildren<SpriteRenderer>();
            if (headSR != null && accSR != null)
            {
                accSR.sortingLayerID = headSR.sortingLayerID;
                accSR.sortingOrder   = headSR.sortingOrder + 1; // delante de la cabeza
                accSR.enabled        = true;
                var accColor = accSR.color; accColor.a = 1f; accSR.color = accColor;
            }

            // configurar el manager del accesorio (auto-asign si falta)
            var zam = acc.GetComponent<ZombieAccessoriesManager>();
            zc.accessory = acc;
            zc.zombieAccessories = zam;

            if (zam != null)
            {
                if (zam.accessoryRenderer == null)
                    zam.accessoryRenderer = accSR; // auto
                zam.accessoryHealth = selectedSO.accessoryHealth;
                zam.accessoryHealthCurrent = selectedSO.accessoryHealth;
            }
        }

        // LOG para verificar
        Debug.Log($"[ZombieManager] Diff={ (GameSettings.I ? GameSettings.I.difficulty.ToString() : "NULL") } | " +
                  $"Spawned='{selectedSO.name}' | Acc={(selectedSO.zombieAccessory ? selectedSO.zombieAccessory.name : "None")} | " +
                  $"Lane={columnID + 1}");

        StartCoroutine(ZombieSpawn());
    }

    // ---------- helpers ----------
    static ZombieScriptableObject[] NonEmpty(ZombieScriptableObject[] arr)
        => (arr != null && arr.Length > 0) ? arr : null;

    static ZombieScriptableObject[] MergeNonEmpty(params ZombieScriptableObject[][] arrays)
    {
        List<ZombieScriptableObject> list = new List<ZombieScriptableObject>();
        foreach (var a in arrays)
            if (a != null && a.Length > 0) list.AddRange(a);
        return list.Count > 0 ? list.ToArray() : null;
    }
}
