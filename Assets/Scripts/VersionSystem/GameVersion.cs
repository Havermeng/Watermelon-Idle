using UnityEngine;

/// <summary>
/// Структура данных для хранения информации о версии игры.
/// Содержит номер версии и список изменений.
/// </summary>
[System.Serializable]
public class GameVersion
{
    public string version;
    [TextArea(3, 10)] public string changes;
}