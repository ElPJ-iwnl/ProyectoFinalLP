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
    {
        return Random.Range(GameSettings.I.sunMinTime, GameSettings.I.sunMaxTime);
    }

    return Random.Range(minTime, maxTime);
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
    }

    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
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
        if (col.CompareTag("Sun"))
            Destroy(col.gameObject);
    }
}
