using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class EscapeMenu : MonoBehaviour
{
    public static EscapeMenu instance;

    public PlayerMovement script;

    public GameObject escapePanel;
    public GameObject optionsPanel;
    private bool isOpen = false;

    PlayerControls controls;
    InputAction escapeAction;

    private void Awake()
    {
        controls = new PlayerControls();
        escapeAction = controls.Player.Escape;

        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void OnEnable()
    {
        controls.Player.Enable();
        escapeAction.started += OnEscapePressed;
    }

    private void OnDisable()
    {
        escapeAction.started -= OnEscapePressed;
        controls.Player.Disable();
    }

    private void OnEscapePressed(InputAction.CallbackContext context)
    {
        Debug.Log("AE & NT");
        if (IsAllowedInCurrentScene())
        {
            Toggle();
        }
    }

    private bool IsAllowedInCurrentScene()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;

        if (currentSceneName == "MainMenu")
        {
            return false;
        }
        return true;
    }

    public void Toggle()
    {
        isOpen = !isOpen;
        escapePanel.SetActive(isOpen);
        Time.timeScale = isOpen ? 0f : 1f;

        Cursor.lockState = isOpen ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = isOpen ? true : false;

        if (script == null)
        {
            script = FindFirstObjectByType<PlayerMovement>();
        }

        if (script != null)
        {
            script.enabled = !isOpen;
        }
    }

    public void OpenSettings()
    {
        escapePanel.SetActive(false);
        optionsPanel.SetActive(true);
    }

    public void Back()
    {
        escapePanel.SetActive(true);
        optionsPanel.SetActive(false);
    }

    public void Resume()
    {
        isOpen = false;
        escapePanel.SetActive(false);
        Time.timeScale = 1f;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        if (script == null)
        {
            script = FindFirstObjectByType<PlayerMovement>();
        }

        if (script != null)
        {
            script.enabled = true;
        }
    }

    public void GoToMainMenu()
    {
        Resume();
        if (SceneTransition.instance != null) SceneTransition.instance.LoadScene(0);
        else SceneManager.LoadScene(0);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}