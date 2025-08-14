using UnityEngine;

public class SunSpawner : MonoBehaviour
{
    public bool isSunFlower;

    public float minTime;
    public float maxTime;

    public GameObject sun;
    public Vector2 minPos;
    public Vector2 maxPos;

    float timer;

    float NextInterval()
    {
        if (GameSettings.I != null && GameSettings.I.overrideSunTimes)
            return Mathf.Max(0.1f, Random.Range(GameSettings.I.sunMinTime, GameSettings.I.sunMaxTime));

        // defaults seguros si vinieron 0
        float min = (minTime > 0f) ? minTime : 5f;
        float max = (maxTime > min) ? maxTime : (min + 3f);
        return Random.Range(min, max);
    }

    Vector3 NextWorldPos()
    {
        if (isSunFlower)
        {
            // nace justo encima del girasol
            return new Vector3(transform.position.x, transform.position.y, -2f);
        }
        else
        {
            // caída aleatoria desde el cielo (si usas este modo)
            return new Vector3(
                Random.Range(minPos.x, maxPos.x),
                Random.Range(minPos.y, maxPos.y),
                -1f
            );
        }
    }

    void OnEnable()
    {
        timer = NextInterval();
        if (timer <= 0f) timer = 1f;
    }

    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            if (sun == null)
            {
                Debug.LogError("[SunSpawner] Prefab 'Sun' no asignado.");
                timer = NextInterval();
                return;
            }

            Vector3 pos = NextWorldPos();
            GameObject s = Instantiate(sun, pos, Quaternion.identity);

            if (isSunFlower)
            {
                // El sol del girasol se queda quieto
                var rb = s.GetComponent<Rigidbody2D>();
                if (rb) Destroy(rb);
            }

            timer = NextInterval();
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        // Si el spawner es un girasol, NO borres los soles que toquen su trigger
        if (isSunFlower) return;

        if (col.CompareTag("Sun"))
            Destroy(col.gameObject);
    }
}
