using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Ingredients")]
    [SerializeField] private List<IngredientData> requiredIngredients;
    private List<IngredientData> collectedIngredients = new List<IngredientData>();

    [Header("Drinks")]
    [SerializeField] private List<Drink> drinks;
    private Drink currentDrink;

    [Header("Game State")]
    public bool firstTimeTalkingToSatan;

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

    public event Action<List<IngredientData>> OnIngredientsGenerated;
    public event Action<string> OnIngredientCollected;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);          
            return;
        }

        Instance = this;
        DontDestroyOnLoad(this);
        PlayerHealth = new Health(3);
    }

    private void Start()
    {
        
        firstTimeTalkingToSatan = PlayerPrefs.GetInt("first Time", 0) != 0;
    }

    public void StartRound(IngredientSpawner spawner)
    {
        collectedIngredients.Clear();

        Debug.Log("=== NOVA RODADA ===");

        spawner.SpawnIngredients(requiredIngredients);
        OnIngredientsGenerated?.Invoke(requiredIngredients);
    }

    public void SelectRandomDrink()
    {
        if (drinks.Count == 0)
        {
            Debug.LogError("Nenhum drink configurado!");
            return;
        }

        int randomIndex = UnityEngine.Random.Range(0, drinks.Count);
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

                SfxManager.Instance.GetcollectedIngredientAudio().Play();

                OnIngredientCollected?.Invoke(ingredient);

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
            Invoke(nameof(GoToSatan), 2f);
            MusicManager.Instance.PlayWinMusic();
        }

        
    }

    private void GoToSatan()
    {
        SceneManager.LoadScene(0);
    }

    public void DamagePlayer(int damage)
    {
        PlayerHealth.TakeDamage(damage);
        PlayerController.Instance.BlinkDamageFeedback();
        if (PlayerHealth.IsDead())
            PlayerDied();
    }

    public void HealPlayer(int heal)
    {
        PlayerHealth.Heal(heal);
    }

    public void RevivePlayer()
    {
        PlayerHealth.FullHeal();
    }

    private void PlayerDied()
    {
        gameOverUI.ShowGameOver();
        MusicManager.Instance.PlayGameOverMusic();
    }
}