using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameTimer : MonoBehaviour
{
    [Header("Win (puede ser UI Image o SpriteRenderer)")]
    public GameObject winUI;                
    [Range(0.1f, 1f)] public float imageScreenPercent = 0.6f;   
    [Range(0.1f, 1f)] public float spriteScreenPercent = 0.6f; 

    [Header("Navegación")]
    public string menuSceneName = "MenuPrincipal";
    public float waitBeforeMenu = 2f;

    [Header("Tiempo para ganar")]
    public float winTimeOverride = -1f; 

    [Header("Fallback si no asignas Win UI")]
    public string winSpritePath = "win"; 

    float timer, targetTime;
    bool winning;

    Image uiImage;                
    SpriteRenderer srImage;       
    Canvas autoCanvas;            
    GameObject autoBg;

    void Start()
    {
        if (winTimeOverride > 0f) targetTime = winTimeOverride;
        else if (GameSettings.I != null) targetTime = GameSettings.I.winTimeSeconds;
        else targetTime = 30f;

        timer = 0f;

        if (winUI != null)
        {
            uiImage = winUI.GetComponentInChildren<Image>(true);
            if (uiImage == null) srImage = winUI.GetComponentInChildren<SpriteRenderer>(true);

            winUI.SetActive(false);
            if (srImage != null) srImage.enabled = false;
        }
    }

    void Update()
    {
        if (winning) return;
        timer += Time.deltaTime;
        if (timer >= targetTime)
        {
            StartCoroutine(WinSequence());
            winning = true;
        }
    }

    System.Collections.IEnumerator WinSequence()
    {
        if (uiImage != null)
        {
            winUI.SetActive(true);

            var rt = uiImage.rectTransform;
            rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = Vector2.zero;
            float w = Screen.width  * imageScreenPercent;
            float h = Screen.height * imageScreenPercent;
            rt.sizeDelta = new Vector2(w, h);
            uiImage.preserveAspect = true;

            yield return StartCoroutine(FadeImageUnscaled(uiImage, 0f, 1f, 0.3f));
        }
        else if (srImage != null)
        {
            CenterAndScaleSpriteRenderer(srImage, spriteScreenPercent);
            if (winUI != null) winUI.SetActive(true);
            srImage.enabled = true;
        }
        else
        {
            autoCanvas = CreateOverlayCanvas();

            autoBg = new GameObject("WinBG", typeof(Image));
            autoBg.transform.SetParent(autoCanvas.transform, false);
            var bImg = autoBg.GetComponent<Image>();
            bImg.color = new Color(0f, 0f, 0f, 0.65f);
            var brt = bImg.rectTransform;
            brt.anchorMin = Vector2.zero; brt.anchorMax = Vector2.one;
            brt.offsetMin = Vector2.zero; brt.offsetMax = Vector2.zero;

            var imgGO = new GameObject("WinImage", typeof(Image));
            imgGO.transform.SetParent(autoCanvas.transform, false);
            var img = imgGO.GetComponent<Image>();
            img.preserveAspect = true;

            Sprite s = Resources.Load<Sprite>(winSpritePath);
            if (s != null)
            {
                img.sprite = s;
                var rt = img.rectTransform;
                rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
                rt.anchoredPosition = Vector2.zero;
                rt.sizeDelta = new Vector2(Screen.width * imageScreenPercent,
                                           Screen.height * imageScreenPercent);
            }

            yield return StartCoroutine(FadeImageUnscaled(img, 0f, 1f, 0.3f));
        }

        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(waitBeforeMenu);
        Time.timeScale = 1f;
        SceneManager.LoadScene(menuSceneName);
    }

    Canvas CreateOverlayCanvas()
    {
        var canvasGO = new GameObject("AutoWinCanvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        var canvas = canvasGO.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 10000;
        var scaler = canvasGO.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        return canvas;
    }

    System.Collections.IEnumerator FadeImageUnscaled(Image img, float from, float to, float dur)
    {
        if (img == null) yield break;
        Color c = img.color;
        c.a = from;
        img.color = c;
        float t = 0f;
        while (t < dur)
        {
            t += Time.unscaledDeltaTime;
            c.a = Mathf.Lerp(from, to, t / dur);
            img.color = c;
            yield return null;
        }
        c.a = to; img.color = c;
    }

    void CenterAndScaleSpriteRenderer(SpriteRenderer sr, float percent)
    {
        var cam = Camera.main;
        if (cam == null || sr.sprite == null) return;

        Vector3 center = cam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, -cam.transform.position.z));
        center.z = 0f;
        sr.transform.position = center;


        float worldH = 2f * cam.orthographicSize * percent;    
        float worldW = worldH * cam.aspect;                    

        Vector2 baseSize = sr.sprite.bounds.size;
        Vector3 currentScale = sr.transform.localScale;
        baseSize = new Vector2(baseSize.x / Mathf.Max(currentScale.x, 1e-5f),
                               baseSize.y / Mathf.Max(currentScale.y, 1e-5f));

        float scaleX = worldW / baseSize.x;
        float scaleY = worldH / baseSize.y;
        float finalScale = Mathf.Min(scaleX, scaleY) * 0.95f; 

        sr.transform.localScale = new Vector3(finalScale, finalScale, 1f);

        try { sr.sortingLayerName = "UI"; } catch { }
        sr.sortingOrder = 10000;

        var anim = sr.GetComponent<Animator>();
        if (anim) anim.enabled = false;
    }
}
