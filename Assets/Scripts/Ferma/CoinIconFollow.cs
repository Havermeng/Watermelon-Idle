using UnityEngine;
using TMPro;

public class CoinIconFollow : MonoBehaviour
{
    public TextMeshProUGUI coinText;
    public RectTransform iconRect;
    public float spacing = 5f;

    void LateUpdate()
    {
        RectTransform textRect = (RectTransform)coinText.transform;
        float textWidth = coinText.preferredWidth;

        // Учитываем pivot текста чтобы найти правый край
        float rightEdge = textRect.anchoredPosition.x
        + textWidth * (1f - textRect.pivot.x);

        iconRect.anchoredPosition = new Vector2(
        rightEdge + spacing,
        iconRect.anchoredPosition.y
        );
    }
}
