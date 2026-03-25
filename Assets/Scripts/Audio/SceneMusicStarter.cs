using UnityEngine;

public class SceneMusicStarter : MonoBehaviour
{
    public enum SceneMusic { Menu, Game }
    public SceneMusic musicType;

    void Start()
    {
        AudioManager.EnsureExists(); // создаёт AudioManager если нет
        
        if (AudioManager.Instance == null) return;

        if (musicType == SceneMusic.Menu)
            AudioManager.Instance.PlayMusic(AudioManager.Instance.menuMusic);
        else
            AudioManager.Instance.PlayMusic(AudioManager.Instance.gameMusic);
    }
}
