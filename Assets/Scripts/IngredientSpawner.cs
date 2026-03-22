using System.Collections.Generic;
using UnityEngine;

public class IngredientSpawner : MonoBehaviour
{

    [SerializeField] private List<IngredientData> ingredientPrefabs;
    [SerializeField] private Transform[] spawnPoints;

    public void SpawnIngredients(List<IngredientData> requiredIngredients)
    {
        List<IngredientData> pool = new List<IngredientData>();

        // Garante pelo menos 1 de cada
        pool.AddRange(requiredIngredients);

        // Preenche o resto aleatoriamente
        while (pool.Count < spawnPoints.Length)
        {
            IngredientData random = requiredIngredients[Random.Range(0, requiredIngredients.Count)];
            pool.Add(random);
        }

        // Embaralha
        for (int i = 0; i < pool.Count; i++)
        {
            int rand = Random.Range(i, pool.Count);
            (pool[i], pool[rand]) = (pool[rand], pool[i]);
        }

        // Spawn
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            GameObject prefab = GetPrefabByName(pool[i].name);

            if (prefab != null)
            {
                Instantiate(prefab, spawnPoints[i].position, Quaternion.identity);
            }
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