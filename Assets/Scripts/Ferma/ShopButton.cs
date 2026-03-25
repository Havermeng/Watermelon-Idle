using UnityEngine;
using UnityEngine.UI;

public class ShopButton : MonoBehaviour
{
    private Button button;
    
    void Start()
    {
        button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(OpenShop);
        }
    }
    
    void OpenShop()
    {
        if (ShopUI.Instance != null)
        {
            ShopUI.Instance.OpenShop();
        }
    }
}