using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStatePanelController : MonoBehaviour
{
    public static GameStatePanelController Instance;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject winRoundPanel;
    [SerializeField] private GameObject creditsPanel;

    [Space]
    [Header("credits config")]
    [SerializeField] private RectTransform creditsText;
    [SerializeField] private float tempoDeSubida = 15f;
    [SerializeField] private float posicaoYFinal = 2000f;

    private bool _gameOver = false;

    private Vector2 posicaoInicialCreditos;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this);
        posicaoInicialCreditos = creditsText.anchoredPosition;
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

    [ContextMenu("Show credits")]
    public void ShowCredits()
    {
        creditsPanel.SetActive(true);

        creditsText.anchoredPosition = posicaoInicialCreditos;

        creditsText.DOAnchorPosY(posicaoYFinal, tempoDeSubida)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                creditsText.gameObject.SetActive(false);
                SceneManager.LoadScene(0);
            });
    }
}
