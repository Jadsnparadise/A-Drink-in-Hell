using System;
using System.Collections.Generic;
using UnityEditor;
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
    [HideInInspector] public bool firstTimeTalkingToSatan;
    
    [Header("Health")]
    [SerializeField] private int maxHealth = 3;
    [SerializeField] private float damageCooldown = 3;
    
    private float _damageCooldownTimer;

    [System.Serializable]
    public class Drink
    {
        public string drinkName;
        public List<IngredientData> ingredients;
    }
    
    [Header("Health")]
    public Health PlayerHealth;

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
        PlayerHealth = new Health(maxHealth);
        _damageCooldownTimer = 0;
    }

    private void Start()
    {
        firstTimeTalkingToSatan = PlayerPrefs.GetInt("first Time", 0) != 0;
    }

    public void StartRound(IngredientSpawner spawner)
    {
        collectedIngredients.Clear();
        Time.timeScale = 1f; // Despausa o jogo

        Debug.Log("=== NOVA RODADA ===");

        MusicManager.Instance.StopGameOverMusic();

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

    public void DamagePlayer(int damage, bool ignoreCooldown = false)
    {
        if (!(_damageCooldownTimer + damageCooldown <= Time.time) && !ignoreCooldown) return;
        
        PlayerHealth.TakeDamage(damage);
        PlayerController.Instance.BlinkDamageFeedback();
        _damageCooldownTimer = Time.time;
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
        PlayerController.SetIgnoreEnemyLayerCollision(false);
    }

    private static void PlayerDied()
    {
        MusicManager.Instance.PlayGameOverMusic();
        
        var controller = PlayerController.Instance;
        controller.Die();
    }

    public void ShowGameOver()
    {
        Time.timeScale = 0f; // Pausa o jogo
        GameOverUI.Instance.ShowGameOver();
    }

    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }

    public void QuitGame()
    {
        Application.Quit();
        #if UNITY_EDITOR
                EditorApplication.isPlaying = false;
        #endif
    }
}