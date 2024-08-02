using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public List<LevelItems> levelItems; // Lista de ítems y recetas por nivel
    private RecipeChecker recipeChecker; // Referencia al RecipeChecker
    private Dictionary<(Item, Item), Item> recipeDictionary; // Diccionario de recetas
    private int currentLevel; // Nivel actual

    // Evento que se dispara cuando se completa una combinación de ítems
    public event System.Action OnItemsCombined;

    void Start()
    {
        recipeChecker = FindObjectOfType<RecipeChecker>(); // Encontrar el RecipeChecker en la escena
        SetCurrentLevel(currentLevel); // Inicializar con el nivel actual
    }

    // Método para combinar dos ítems
    public Item CombineItems(Item item1, Item item2)
    {
        // Iterar a través de todas las recetas en el diccionario
        foreach (var recipe in recipeDictionary)
        {
            if ((recipe.Key.Item1.itemName == item1.itemName && recipe.Key.Item2.itemName == item2.itemName) || 
                (recipe.Key.Item1.itemName == item2.itemName && recipe.Key.Item2.itemName == item1.itemName))
            {
                Item resultItem = recipe.Value;
                resultItem.isUsedInRecipe = true; // Marcar el ítem como utilizado en una receta

                // Disparar el evento cuando se combina un ítem
                OnItemsCombined?.Invoke();

                return resultItem; // Devolver el ítem resultante si la combinación es válida
            }
        }

        return null; // Devolver null si la combinación no es válida
    }

    // Método para obtener los ítems disponibles para el nivel actual
    public List<Item> GetItemsForCurrentLevel()
    {
        foreach (var levelItem in levelItems)
        {
            if (levelItem.level == currentLevel)
            {
                return levelItem.items;
            }
        }
        return new List<Item>(); // Devolver una lista vacía si no hay ítems para el nivel dado
    }
    
    // Método para obtener los resultados de las recetas del nivel actual
    public List<Item> GetResultOfRecipesForCurrentLevel()
    {
        List<Item> resultItems = new List<Item>();

        foreach (var levelItem in levelItems)
        {
            if (levelItem.level == currentLevel)
            {
                foreach (var recipe in levelItem.recipes)
                {
                    resultItems.Add(recipe.result);
                }
                break;
            }
        }

        return resultItems;
    }

    // Método para establecer el nivel actual y actualizar recetas e ítems
    public void SetCurrentLevel(int level)
    {
        currentLevel = level;
        UpdateRecipeChecker();
        
        UpdateRecipeDictionary();
    }

    // Actualizar el diccionario de recetas basado en el nivel actual
    private void UpdateRecipeDictionary()
    {
        recipeDictionary = new Dictionary<(Item, Item), Item>();

        foreach (var levelItem in levelItems)
        {
            if (levelItem.level == currentLevel)
            {
                foreach (var recipe in levelItem.recipes)
                {
                    recipeDictionary[(recipe.item1, recipe.item2)] = recipe.result;
                    recipeDictionary[(recipe.item2, recipe.item1)] = recipe.result;
                }
                break;
            }
        }
    }

    public void UpdateRecipeChecker()
    {
        recipeChecker = FindObjectOfType<RecipeChecker>();
        
        // Reinicializar el estado de las recetas en el RecipeChecker
        if (recipeChecker != null)
        {
            recipeChecker.InitializeRecipeCompletionStatus();
        }
    }
}
