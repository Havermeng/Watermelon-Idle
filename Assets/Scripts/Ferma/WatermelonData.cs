using UnityEngine;

[System.Serializable]
public class WatermelonData
{
    public string name;        // Русское название
    public string nameEn;      // Английское название
    public int unlockCost;
    public float growTime;
    public int harvestValue;
    public Sprite seedSprite;
    public Sprite sproutSprite;
    public Sprite watermelonSprite;
    
    // Возвращает название на нужном языке
    public string GetName()
    {
        if (LocalizationManager.Instance != null && 
            LocalizationManager.Instance.GetCurrentLanguage() == "en" &&
            !string.IsNullOrEmpty(nameEn))
            return nameEn;
        return name;
    }
}