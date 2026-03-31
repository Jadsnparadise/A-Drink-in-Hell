using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverUI : MonoBehaviour
{
    public static GameOverUI Instance;
    [SerializeField] private GameObject panel;
    private bool _gameOver = false;

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this);
    }

    private void Update()
    {
        if (_gameOver && Input.anyKeyDown)
        {
            Restart();
        }
    }

    public void ShowGameOver()
    {
        _gameOver = true;
        panel.SetActive(true);
    }

    private static void Restart()
    {
        GameManager.Instance.RevivePlayer();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
