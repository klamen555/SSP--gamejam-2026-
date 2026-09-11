using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneTransition : MonoBehaviour
{
    public static SceneTransition instance;
    public Image fadePanel;
    public float fadeDuration = 1f;
    public bool fadeInOnStart = true;

    private void Awake()
    {
        Debug.Log("SceneTransition Awake, instance is: " + (instance == null ? "null" : "exists"));
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public void LoadScene(int index)
    {
        StartCoroutine(FadeAndLoad(index));
    }

    private IEnumerator FadeAndLoad(int index)
    {
        yield return StartCoroutine(FadeOut());
        SceneManager.LoadScene(index);
    }

    public IEnumerator FadeIn()
    {
        float elapsed = 0f;
        Color c = fadePanel.color;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            c.a = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            fadePanel.color = c;
            yield return null;
        }
        c.a = 0f;
        fadePanel.color = c;
    }

    private IEnumerator FadeOut()
    {
        float elapsed = 0f;
        Color c = fadePanel.color;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            c.a = Mathf.Lerp(0f, 1f, elapsed / fadeDuration);
            fadePanel.color = c;
            yield return null;
        }
        c.a = 1f;
        fadePanel.color = c;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // put the index of scenes that should not be faded into
        int[] noFadeScenes = { };
        if (System.Array.IndexOf(noFadeScenes, scene.buildIndex) >= 0) return;
        StartCoroutine(FadeIn());
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}