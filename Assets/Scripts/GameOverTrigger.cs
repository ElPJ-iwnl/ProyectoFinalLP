using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(BoxCollider2D))]
public class GameOverTrigger : MonoBehaviour
{
    [Header("Qué mostrar al perder (elige 1)")]
    [Tooltip("Arrastra aquí TU sprite de Game Over (asset del Project).")]
    [SerializeField] private Sprite gameOverSpriteDirect;     

    [Tooltip("O arrastra un GameObject de la jerarquía que tenga UI Image o SpriteRenderer.")]
    [SerializeField] private GameObject gameOverUI;           

    [Header("Tamaño en pantalla")]
    [Range(0.1f, 1f)] public float imageScreenPercent = 0.75f; 
    [Range(0.1f, 1f)] public float spriteScreenPercent = 0.75f;

    [Header("Fallback (si no asignas nada)")]
    [Tooltip("Ruta dentro de Resources sin extensión. Ej: 'GameOver' => Assets/Resources/GameOver.png")]
    [SerializeField] private string gameOverSpritePath = "GameOver";

    [Header("Flujo")]
    [SerializeField] private float delayBeforeMenu = 2f;
    [SerializeField] private string menuSceneName = "MenuPrincipal";

    private bool triggered;

    private Image uiImage;
    private SpriteRenderer srImage;
    private Canvas autoCanvas;
    private GameObject autoBg;

    void Reset()
    {
        var bc = GetComponent<BoxCollider2D>();
        bc.isTrigger = true;
    }

    void Awake()
    {
        if (gameOverUI != null)
        {
            uiImage = gameOverUI.GetComponentInChildren<Image>(true);
            if (uiImage == null) srImage = gameOverUI.GetComponentInChildren<SpriteRenderer>(true);

            gameOverUI.SetActive(false);
            if (srImage != null) srImage.enabled = false;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered) return;

        Debug.Log($"[GameOverTrigger] ACTIVADO por {other.name} (tag={other.tag})");
        triggered = true;
        StartCoroutine(GameOverSequence());
    }

    System.Collections.IEnumerator GameOverSequence()
    {
        Sprite chosen = ChooseSprite();

        if (uiImage != null)
        {
            if (chosen != null) uiImage.sprite = chosen;
            gameOverUI.SetActive(true);

            var rt = uiImage.rectTransform;
            rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = Vector2.zero;
            rt.sizeDelta = new Vector2(Screen.width * imageScreenPercent,
                                       Screen.height * imageScreenPercent);
            uiImage.preserveAspect = true;

            yield return StartCoroutine(FadeImageUnscaled(uiImage, 0f, 1f, 0.25f));
        }
        else if (srImage != null)
        {
            if (chosen != null) srImage.sprite = chosen;
            CenterAndScaleSpriteRenderer(srImage, spriteScreenPercent);
            gameOverUI.SetActive(true);
            srImage.enabled = true;
        }

        else
        {
            autoCanvas = CreateOverlayCanvas();


            autoBg = new GameObject("GameOverBG", typeof(Image));
            autoBg.transform.SetParent(autoCanvas.transform, false);
            var bImg = autoBg.GetComponent<Image>();
            bImg.color = new Color(0f, 0f, 0f, 0.65f);
            var brt = bImg.rectTransform; brt.anchorMin = Vector2.zero; brt.anchorMax = Vector2.one;
            brt.offsetMin = Vector2.zero; brt.offsetMax = Vector2.zero;


            var imgGO = new GameObject("GameOverImage", typeof(Image));
            imgGO.transform.SetParent(autoCanvas.transform, false);
            var img = imgGO.GetComponent<Image>();
            img.preserveAspect = true;

            if (chosen == null) chosen = Resources.Load<Sprite>(gameOverSpritePath);

            if (chosen != null)
            {
                img.sprite = chosen;
                var rt = img.rectTransform;
                rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
                rt.anchoredPosition = Vector2.zero;
                rt.sizeDelta = new Vector2(Screen.width * imageScreenPercent,
                                           Screen.height * imageScreenPercent);
            }

            yield return StartCoroutine(FadeImageUnscaled(img, 0f, 1f, 0.25f));
        }

        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(delayBeforeMenu);
        Time.timeScale = 1f;
        SceneManager.LoadScene(menuSceneName);
    }

    private Sprite ChooseSprite()
    {
        if (gameOverSpriteDirect != null) return gameOverSpriteDirect;         
        if (uiImage != null && uiImage.sprite != null) return uiImage.sprite;   
        if (srImage != null && srImage.sprite != null) return srImage.sprite; 
        return Resources.Load<Sprite>(gameOverSpritePath);                      
    }

    private Canvas CreateOverlayCanvas()
    {
        var go = new GameObject("AutoGameOverCanvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        var canvas = go.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 10000;
        var scaler = go.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        return canvas;
    }

    private System.Collections.IEnumerator FadeImageUnscaled(Image img, float from, float to, float dur)
    {
        if (img == null) yield break;
        Color c = img.color; c.a = from; img.color = c;
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

    private void CenterAndScaleSpriteRenderer(SpriteRenderer sr, float percent)
    {
        var cam = Camera.main;
        if (cam == null || sr.sprite == null) return;

        Vector3 center = cam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, -cam.transform.position.z));
        center.z = 0f; sr.transform.position = center;

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

