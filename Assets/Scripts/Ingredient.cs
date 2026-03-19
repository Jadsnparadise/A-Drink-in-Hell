using UnityEngine;

public class Ingredient : MonoBehaviour
{
    [SerializeField] private string ingredientName;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            GameManager.Instance.CollectIngredient(ingredientName);
            Destroy(gameObject);
        }
    }
}