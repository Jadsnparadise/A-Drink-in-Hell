using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    public static bool isPaused;

    private InputSystemActions _input;

    void Awake()
    {
        // Criar e registrar o listener aqui para que o input seja escutado
        // mesmo que o GameObject seja desativado posteriormente.
        _input = new InputSystemActions();
        _input.Player.Pause.performed += OnPause;
        _input.Player.Enable();
    }

    void Start()
    {
        gameObject.SetActive(false);
    }

    void OnDestroy()
    {
        // Limpar subscriptions ao destruir para evitar memory leaks.
        if (_input != null)
        {
            _input.Player.Pause.performed -= OnPause;
            _input.Player.Disable();
            _input = null;
        }
    }

    private void OnPause(InputAction.CallbackContext context)
    {
        if (isPaused)
            ResumeGame();
        else
            PauseGame();
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;
        isPaused = true;
        gameObject.SetActive(true);
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        isPaused = false;
        gameObject.SetActive(false);
    }

    public void Restart()
    {
        GameManager.Instance.RestartGame();
    }
}