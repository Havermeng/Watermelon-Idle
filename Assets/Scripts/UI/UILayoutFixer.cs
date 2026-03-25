using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(Canvas))]
public class UILayoutFixer : MonoBehaviour
{
    [SerializeField] private bool fixAnchors = true;
    [SerializeField] private bool fixSafeArea = true;
    [SerializeField] private bool disableStaticRaycasts = true;

    private RectTransform rectTransform;
    private Canvas canvas;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponent<Canvas>();
    }

    void Start()
    {
        if (fixAnchors)
            FixAnchors();

        if (fixSafeArea)
            ApplySafeArea();

        if (disableStaticRaycasts)
            DisableStaticRaycasts();
    }

    void FixAnchors()
    {
        if (rectTransform == null) return;

        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        rectTransform.anchoredPosition = Vector2.zero;
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;
    }

    void ApplySafeArea()
    {
        if (rectTransform == null) return;

        Rect safeArea = Screen.safeArea;
        if (safeArea.width == 0 || safeArea.height == 0) return;

        Vector2 anchorMin = safeArea.position;
        Vector2 anchorMax = safeArea.position + safeArea.size;

        Vector2 sizeDelta = rectTransform.sizeDelta;
        Vector2 anchorSize = new Vector2(Screen.width, Screen.height);

        anchorMin.x /= anchorSize.x;
        anchorMin.y /= anchorSize.y;
        anchorMax.x /= anchorSize.x;
        anchorMax.y /= anchorSize.y;

        rectTransform.anchorMin = anchorMin;
        rectTransform.anchorMax = anchorMax;
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;
    }

    void DisableStaticRaycasts()
    {
        if (canvas == null) return;

        Graphic[] graphics = GetComponentsInChildren<Graphic>(true);
        foreach (Graphic graphic in graphics)
        {
            if (graphic is Image || graphic is TextMeshProUGUI)
            {
                if (!IsInteractiveElement(graphic.gameObject))
                {
                    graphic.raycastTarget = false;
                }
            }
        }
    }

    bool IsInteractiveElement(GameObject go)
    {
        return go.GetComponent<Button>() != null ||
               go.GetComponent<Toggle>() != null ||
               go.GetComponent<Slider>() != null ||
               go.GetComponent<InputField>() != null ||
               go.GetComponent<Dropdown>() != null ||
               go.GetComponent<TMP_Dropdown>() != null ||
               go.GetComponent<TMP_InputField>() != null;
    }

    void OnRectTransformDimensionsChange()
    {
        if (fixSafeArea)
            ApplySafeArea();
    }
}
