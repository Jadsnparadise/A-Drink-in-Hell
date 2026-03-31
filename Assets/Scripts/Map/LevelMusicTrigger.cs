using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelMusicTrigger : MonoBehaviour
{
    [SerializeField] private AudioClip levelMusic;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            MusicManager.Instance.EnterLevel(levelMusic);
        }
    }
}
