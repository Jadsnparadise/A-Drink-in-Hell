using UnityEngine;

public class SfxManager : MonoBehaviour
{
    public static SfxManager Instance;

    [Header("Ingredients")]
    [SerializeField] private AudioSource collectIngredient;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this);
    }

    public AudioSource GetcollectedIngredientAudio()
    {
        return collectIngredient;
    }
}
