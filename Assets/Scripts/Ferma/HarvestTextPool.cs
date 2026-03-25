using UnityEngine;
using TMPro;

public class HarvestTextPool : Singleton<HarvestTextPool>
{
    [Header("Pool Settings")]
    [SerializeField] private int initialPoolSize = 10;
    [SerializeField] private GameObject textPrefab;

    private Canvas popupCanvas;
    private RectTransform canvasRect;
    private GameObject textParent;

    protected override void Awake()
    {
        base.Awake();
        CreatePool();
    }

    void CreatePool()
    {
        popupCanvas = GetComponentInParent<Canvas>();
        if (popupCanvas != null)
        {
            canvasRect = popupCanvas.GetComponent<RectTransform>();
            textParent = new GameObject("HarvestTextPool");
            textParent.transform.SetParent(popupCanvas.transform, false);
            RectTransform rt = textParent.AddComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
        }

        CreateInitialPool();
    }

    void CreateInitialPool()
    {
        for (int i = 0; i < initialPoolSize; i++)
        {
            CreateNewTextObject();
        }
    }

    GameObject CreateNewTextObject()
    {
        GameObject go = new GameObject("HarvestText");
        go.SetActive(false);

        if (textParent != null)
            go.transform.SetParent(textParent.transform, false);
        else if (popupCanvas != null)
            go.transform.SetParent(popupCanvas.transform, false);

        RectTransform rt = go.AddComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.sizeDelta = new Vector2(200, 80);

        TextMeshProUGUI tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.fontSize = 72;
        tmp.color = Color.green;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.raycastTarget = false;

        if (TMPro.TMP_Settings.defaultFontAsset != null)
            tmp.font = TMPro.TMP_Settings.defaultFontAsset;

        go.AddComponent<HarvestTextAnim>();

        return go;
    }

    public void Show(Transform targetTransform, int amount)
    {
        GameObject textObj = GetPooledObject();
        if (textObj == null)
        {
            textObj = CreateNewTextObject();
        }

        TextMeshProUGUI tmp = textObj.GetComponent<TextMeshProUGUI>();
        tmp.text = "+" + amount;
        tmp.color = Color.green;

        HarvestTextAnim anim = textObj.GetComponent<HarvestTextAnim>();
        anim.Setup(targetTransform);

        textObj.SetActive(true);
    }

    GameObject GetPooledObject()
    {
        if (textParent == null) return null;

        foreach (Transform child in textParent.transform)
        {
            if (!child.gameObject.activeSelf)
                return child.gameObject;
        }
        return null;
    }
}

public class HarvestTextAnim : MonoBehaviour
{
    private TextMeshProUGUI tmp;
    private RectTransform rt;
    private Vector3 startWorldPos;
    private float duration = 1f;
    private float elapsed = 0f;
    private bool isAnimating = false;

    void Awake()
    {
        tmp = GetComponent<TextMeshProUGUI>();
        rt = GetComponent<RectTransform>();
    }

    public void Setup(Transform target)
    {
        if (target == null || Camera.main == null)
        {
            rt.anchoredPosition = Vector2.zero;
        }
        else
        {
            Vector3 worldPos = target.position + new Vector3(0, 0.5f, 0);
            Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(Camera.main, worldPos);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                transform.parent as RectTransform, screenPos, null, out Vector2 localPos);
            rt.anchoredPosition = localPos;
        }

        elapsed = 0f;
        isAnimating = true;
    }

    void Update()
    {
        if (!isAnimating) return;

        elapsed += Time.deltaTime;
        float t = elapsed / duration;

        if (t >= 1f)
        {
            isAnimating = false;
            gameObject.SetActive(false);
            return;
        }

        rt.anchoredPosition += new Vector2(0, 150f * Time.deltaTime);
        tmp.color = new Color(0.3f, 0.8f, 0.2f, 1f - t);
    }
}
