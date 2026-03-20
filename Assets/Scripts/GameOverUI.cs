using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverUI : MonoBehaviour
{
    
    [SerializeField] private GameObject panel;
    private bool _gameover = false;

    // Update is called once per frame
    void Update()
    {
        if (_gameover && Input.anyKeyDown)
        {
            Restart();
        }
    }

    public void ShowGameOver()
    {
        _gameover = true;
        panel.SetActive(true);
    }

    void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
