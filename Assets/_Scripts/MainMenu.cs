using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject mainMenuPanel;
    public GameObject levelSelectPanel;
    public GameObject optionsPanel;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        if (SceneTransition.instance != null)
            StartCoroutine(SceneTransition.instance.FadeIn());
    }

    public void StartGame()
    {
        mainMenuPanel.SetActive(false);
        levelSelectPanel.SetActive(true);
    }

    public void OpenSettings()
    {
        mainMenuPanel.SetActive(false);
        optionsPanel.SetActive(true);
    }

    public void Back()
    {
        levelSelectPanel.SetActive(false);
        optionsPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    public void LoadLevel(int index)
    {
        if (SceneTransition.instance != null)
            SceneTransition.instance.LoadScene(index);
        else
            SceneManager.LoadScene(index);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}