using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    
    [Header("Health")]
    public Health PlayerHealth;
    
    [Header("Panels")]
    [SerializeField] private GameOverUI gameOverUI;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        PlayerHealth = new Health(3);
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

    public void DamagePlayer(int damage)
    {
        PlayerHealth.TakeDamage(damage);

        if (PlayerHealth.IsDead())
            PlayerDied();
    }

    private void PlayerDied()
    {
        gameOverUI.ShowGameOver();
    }
}