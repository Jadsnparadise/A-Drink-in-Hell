using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Ingredients")]
    [SerializeField] private List<string> requiredIngredients = new List<string>();
    private List<string> collectedIngredients = new List<string>();

    [Header("Drinks")]
    [SerializeField] private List<Drink> drinks = new List<Drink>();
    private Drink currentDrink;

    [SerializeField] private IngredientSpawner spawner;

    [System.Serializable]
    public class Drink
    {
        public string drinkName;
        public List<string> ingredients;
    }


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    void Start()
    {
        StartRound();
    }

    void StartRound()
    {
        collectedIngredients.Clear();

        Debug.Log("=== NOVA RODADA ===");

        SelectRandomDrink();

        spawner.SpawnIngredients(requiredIngredients);
    }

    void SelectRandomDrink()
    {
        if (drinks.Count == 0)
        {
            Debug.LogError("Nenhum drink configurado!");
            return;
        }

        int randomIndex = Random.Range(0, drinks.Count);
        currentDrink = drinks[randomIndex];

        requiredIngredients.Clear();
        requiredIngredients.AddRange(currentDrink.ingredients);

        Debug.Log("Drink selecionado: " + currentDrink.drinkName);

        foreach (string ing in requiredIngredients)
        {
            Debug.Log("Ingrediente: " + ing);
        }
    }

    public string GetCurrentDrinkName()
    {
        return currentDrink != null ? currentDrink.drinkName : "";
    }

    public void CollectIngredient(string ingredient)
    {
        if (requiredIngredients.Contains(ingredient) && !collectedIngredients.Contains(ingredient))
        {
            collectedIngredients.Add(ingredient);
            Debug.Log("Coletou: " + ingredient);

            CheckWin();
        }
    }

    void CheckWin()
    {
        if (collectedIngredients.Count == requiredIngredients.Count)
        {
            Debug.Log("DRINK COMPLETO!");
        }
    }
}