using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Ingredients")]
    [SerializeField] private List<IngredientData> requiredIngredients = new List<IngredientData>();
    private List<IngredientData> collectedIngredients = new List<IngredientData>();

    [Header("Drinks")]
    [SerializeField] private List<Drink> drinks;
    private Drink currentDrink;

    [SerializeField] private IngredientSpawner spawner;

    [System.Serializable]
    public class Drink
    {
        public string drinkName;
        public List<IngredientData> ingredients;
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

        foreach (IngredientData ing in requiredIngredients)
        {
            Debug.Log("Ingrediente: " + ing.name);
        }
    }

    public string GetCurrentDrinkName()
    {
        return currentDrink != null ? currentDrink.drinkName : "";
    }

    public void CollectIngredient(string ingredient)
    {
        foreach (IngredientData requiredIngredient in requiredIngredients)
        {
            if (requiredIngredient.ingredientName == ingredient && !collectedIngredients.Contains(requiredIngredient)) 
            {
                collectedIngredients.Add(requiredIngredient);

                Debug.Log("Coletou: " + ingredient);

                CheckWin();
                break;
            }
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