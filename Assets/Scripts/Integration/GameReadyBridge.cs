using UnityEngine;

public static class GameReadyBridge
{
    private static bool initialized = false;

    public static void Initialize()
    {
        if (initialized) return;
        initialized = true;
        
#if UNITY_WEBGL && !UNITY_EDITOR
        TryInvokeGameReady();
#endif
    }

    public static void GameReady()
    {
        Initialize();
        
#if UNITY_WEBGL && !UNITY_EDITOR
        TryInvokeGameReady();
#endif
    }

    private static void TryInvokeGameReady()
    {
        try
        {
            var yaGamesType = System.Type.GetType("YaGames.YaGames, YaGames");
            if (yaGamesType != null)
            {
                var gameReadyMethod = yaGamesType.GetMethod("GameReady");
                gameReadyMethod?.Invoke(null, null);
                Debug.Log("GameReady API called successfully");
                return;
            }

            var gameReadyEngineType = System.Type.GetType("YandexGamesSdk.YandexGamesSDK, YandexGamesSdk");
            if (gameReadyEngineType != null)
            {
                var gameReadyMethod = gameReadyEngineType.GetMethod("GameReady");
                gameReadyMethod?.Invoke(null, null);
                Debug.Log("GameReady API called successfully (YandexGamesSdk)");
                return;
            }

            var ysType = System.Type.GetType("YandexSDK, YandexSDK");
            if (ysType != null)
            {
                var method = ysType.GetMethod("GameReady");
                method?.Invoke(null, null);
                Debug.Log("GameReady API called successfully (YandexSDK)");
                return;
            }

            Debug.Log("GameReady API not found - this is normal if not running on Yandex Games");
        }
        catch (System.Exception e)
        {
            Debug.Log("GameReady API call failed: " + e.Message);
        }
    }

    public static void GameStarted()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        try
        {
            var yaGamesType = System.Type.GetType("YaGames.YaGames, YaGames");
            if (yaGamesType != null)
            {
                var method = yaGamesType.GetMethod("GameStarted");
                method?.Invoke(null, null);
            }
        }
        catch { }
#endif
    }
}
