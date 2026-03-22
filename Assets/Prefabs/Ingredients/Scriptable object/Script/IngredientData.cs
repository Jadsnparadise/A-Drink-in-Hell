using UnityEngine;

[CreateAssetMenu(fileName = "NovoIngrediente", menuName = "Ingrediente")]
public class IngredientData : ScriptableObject
{
    public string ingredientName;
    public GameObject prefab;
    public SpriteRenderer ingredientSprite;
}
