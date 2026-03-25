using UnityEngine;
using TMPro;

public class ClickManager : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    private int score = 0;
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (mainCamera == null) return;
            
            Vector2 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

            if (hit.collider != null && hit.collider.gameObject == gameObject)
            {
                score++;
                scoreText.text = score.ToString();
            }
        }
    }
}
