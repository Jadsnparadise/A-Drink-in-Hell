using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStatePanelController : MonoBehaviour
{
    public static GameStatePanelController Instance;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject winRoundPanel;

    private bool _gameOver = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
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
            GameManager.Instance.RestartGame();
            _gameOver = false;
        }
    }

    public void ShowGameOver()
    {
        _gameOver = true;
        gameOverPanel.SetActive(true);
    }

    public void ShowWinPanel()
    {
        winRoundPanel.SetActive(true);
    }
}
