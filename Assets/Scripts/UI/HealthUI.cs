using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    [SerializeField] private Image[] hearts;
    [SerializeField] private Sprite fullHeart;
    [SerializeField] private Sprite emptyHeart;
    
    // Update is called once per frame
    void Update()
    {
        UpdateHearts();
    }

    void UpdateHearts()
    {
        var health = GameManager.Instance.PlayerHealth.Current;
        for(int i = 0; i < hearts.Length; i++)
            hearts[i].sprite = i < health ? fullHeart : emptyHeart;
    }
}
