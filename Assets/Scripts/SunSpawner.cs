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


        float min = (minTime > 0f) ? minTime : 5f;
        float max = (maxTime > min) ? maxTime : (min + 3f);
        return Random.Range(min, max);
    }

    Vector3 NextWorldPos()
    {
        if (isSunFlower)
        {
            return new Vector3(transform.position.x, transform.position.y, -2f);
        }
        else
        {
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

                var rb = s.GetComponent<Rigidbody2D>();
                if (rb) Destroy(rb);
            }

            timer = NextInterval();
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {

        if (isSunFlower) return;

        if (col.CompareTag("Sun"))
            Destroy(col.gameObject);
    }
}
