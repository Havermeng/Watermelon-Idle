using UnityEngine;

public class GameReadyBridge : MonoBehaviour
{
    private static bool called = false;

    void Start()
    {
        if (called) return;
        called = true;
        
#if UNITY_WEBGL && !UNITY_EDITOR
        TryInvokeGameReady();
#endif
        
        Debug.Log("GameReadyBridge initialized");
    }

    void TryInvokeGameReady()
    {
        try
        {
            var yaGamesType = System.Type.GetType("YaGames.YaGames, YaGames");
            if (yaGamesType != null)
            {
                var method = yaGamesType.GetMethod("GameReady");
                method?.Invoke(null, null);
                Debug.Log("GameReady API called (YaGames)");
                return;
            }

            var sdkType = System.Type.GetType("YandexGamesSdk.YandexGamesSDK, YandexGamesSdk");
            if (sdkType != null)
            {
                var method = sdkType.GetMethod("GameReady");
                method?.Invoke(null, null);
                Debug.Log("GameReady API called (YandexGamesSdk)");
                return;
            }

            Debug.Log("GameReady API not found - normal for non-Yandex platform");
        }
        catch (System.Exception e)
        {
            Debug.Log("GameReady API error: " + e.Message);
        }
    }

    public static void ResetState()
    {
        called = false;
    }
}
