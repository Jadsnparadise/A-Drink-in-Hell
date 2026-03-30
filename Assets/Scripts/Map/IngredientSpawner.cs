using System.Collections.Generic;
using UnityEngine;

public class IngredientSpawner : MonoBehaviour
{

    [SerializeField] private List<IngredientData> ingredientPrefabs;
    [SerializeField] private Transform[] spawnPoints;
    private void Start()
    {
        GameManager.Instance.StartRound(this);
    }

    public void SpawnIngredients(List<IngredientData> requiredIngredients)
    {
        List<IngredientData> shuffledIngredients = new List<IngredientData>(requiredIngredients);

        // Embaralha ingredientes
        Shuffle(shuffledIngredients);

        List<Transform> shuffledSpawnPoints = new List<Transform>(spawnPoints);
        Shuffle(shuffledSpawnPoints);

        // Spawn
        for (int i = 0; i < 3; i++)
        {
            GameObject prefab = GetPrefabByName(shuffledIngredients[i].name);

            if (prefab != null)
            {
                Instantiate(prefab, shuffledSpawnPoints[i].position, Quaternion.identity);
            }
        }
    }

    void Shuffle<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int rand = Random.Range(i, list.Count);
            (list[i], list[rand]) = (list[rand], list[i]);
        }
    }

    GameObject GetPrefabByName(string name)
    {
        foreach (var item in ingredientPrefabs)
        {
            if (item.name == name)
                return item.prefab;
        }

        Debug.LogWarning("Prefab não encontrado: " + name);
        return null;
    }
}