using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class SceneLoader : Singleton<SceneLoader>
{
    [Header("Loading Screen")]
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private Slider progressSlider;
    [SerializeField] private TextMeshProUGUI progressText;
    [SerializeField] private float minLoadingTime = 0.5f;

    private AsyncOperation currentOperation;

    protected override void Awake()
    {
        base.Awake();
        if (loadingScreen != null)
            loadingScreen.SetActive(false);
    }

    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneAsync(sceneName));
    }

    public void LoadScene(int sceneIndex)
    {
        StartCoroutine(LoadSceneAsync(sceneIndex));
    }

    IEnumerator LoadSceneAsync(string sceneName)
    {
        if (loadingScreen != null)
        {
            loadingScreen.SetActive(true);
            if (progressSlider != null) progressSlider.value = 0f;
            if (progressText != null) progressText.text = "0%";
        }

        float startTime = Time.time;

        currentOperation = SceneManager.LoadSceneAsync(sceneName);
        currentOperation.allowSceneActivation = false;

        float progress = 0f;
        while (!currentOperation.isDone)
        {
            progress = Mathf.Clamp01(currentOperation.progress / 0.9f);

            if (progressSlider != null) progressSlider.value = progress;
            if (progressText != null) progressText.text = Mathf.RoundToInt(progress * 100) + "%";

            if (progress >= 1f)
            {
                float elapsed = Time.time - startTime;
                if (elapsed < minLoadingTime)
                    yield return new WaitForSeconds(minLoadingTime - elapsed);

                currentOperation.allowSceneActivation = true;
            }

            yield return null;
        }
    }

    IEnumerator LoadSceneAsync(int sceneIndex)
    {
        if (loadingScreen != null)
        {
            loadingScreen.SetActive(true);
            if (progressSlider != null) progressSlider.value = 0f;
            if (progressText != null) progressText.text = "0%";
        }

        float startTime = Time.time;

        currentOperation = SceneManager.LoadSceneAsync(sceneIndex);
        currentOperation.allowSceneActivation = false;

        float progress = 0f;
        while (!currentOperation.isDone)
        {
            progress = Mathf.Clamp01(currentOperation.progress / 0.9f);

            if (progressSlider != null) progressSlider.value = progress;
            if (progressText != null) progressText.text = Mathf.RoundToInt(progress * 100) + "%";

            if (progress >= 1f)
            {
                float elapsed = Time.time - startTime;
                if (elapsed < minLoadingTime)
                    yield return new WaitForSeconds(minLoadingTime - elapsed);

                currentOperation.allowSceneActivation = true;
            }

            yield return null;
        }
    }

    public float GetProgress()
    {
        return currentOperation != null ? currentOperation.progress : 0f;
    }

    public bool IsLoading()
    {
        return currentOperation != null && !currentOperation.isDone;
    }
}
