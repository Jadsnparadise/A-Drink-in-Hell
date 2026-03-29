using System.Collections.Generic;
using UnityEngine;

public class IngredientsUIController : MonoBehaviour
{
    [Header("UI Setup")]
    [SerializeField] private IngredientUIItem ingredientPrefab;
    [SerializeField] private Transform ingredientsContainer;

    private Dictionary<string, IngredientUIItem> spawnedUIItems = new Dictionary<string, IngredientUIItem>();

    private void OnEnable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnIngredientsGenerated += GenerateUI;
            GameManager.Instance.OnIngredientCollected += UpdateIngredientUI;
        }
    }

    private void OnDisable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnIngredientsGenerated -= GenerateUI;
            GameManager.Instance.OnIngredientCollected -= UpdateIngredientUI;
        }
    }

    private void GenerateUI(List<IngredientData> requiredIngredients)
    {
        foreach (Transform child in ingredientsContainer)
        {
            Destroy(child.gameObject);
        }
        spawnedUIItems.Clear();

        foreach (IngredientData data in requiredIngredients)
        {
            IngredientUIItem newItem = Instantiate(ingredientPrefab, ingredientsContainer);
            newItem.Setup(data.ingredientSprite.sprite);

            spawnedUIItems.Add(data.ingredientName, newItem);
        }
    }

    private void UpdateIngredientUI(string ingredientName)
    {
        if (spawnedUIItems.ContainsKey(ingredientName))
        {
            spawnedUIItems[ingredientName].MarkAsCollected();
        }
    }
}