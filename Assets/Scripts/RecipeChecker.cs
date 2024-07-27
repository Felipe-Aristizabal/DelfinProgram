using System.Collections.Generic;
using UnityEngine;

public class RecipeChecker : MonoBehaviour
{
    private ItemManager itemManager;
    private Dictionary<string, bool> recipeCompletionStatus; // Para rastrear el estado de las recetas
    [SerializeField] private LevelManager _levelManager;
    
    void Start()
    {
        itemManager = FindObjectOfType<ItemManager>();
        InitializeRecipeCompletionStatus();

        // Suscribirse al evento de combinación de ítems
        if (itemManager != null)
        {
            itemManager.OnItemsCombined += CheckRecipeCompletion;
        }
        else
        {
            Debug.LogError("ItemManager not found.");
        }
        
        DebugRecipeCompletionStatus();
    }

    public void InitializeRecipeCompletionStatus()
    {
        recipeCompletionStatus = new Dictionary<string, bool>();

        foreach (Transform recipe in transform)
        {
            Transform recipItemsTransform = FindChildByName(recipe, "RecipeItems");
            Transform resultTransform = FindChildByName(recipItemsTransform, "Result");

            if (resultTransform != null)
            {
                UnityEngine.UI.Image imageComponent = resultTransform.GetComponent<UnityEngine.UI.Image>();
                if (imageComponent != null && imageComponent.sprite != null)
                {
                    string resultItemName = imageComponent.sprite.name;
                    FindChildByName(recipe, "Check").gameObject.SetActive(false);
                    if (!recipeCompletionStatus.ContainsKey(resultItemName))
                    {
                        recipeCompletionStatus[resultItemName] = false;
                    }
                }
                else
                {
                    Debug.LogError("Image component or sprite not found on Result object.");
                }
            }
            else
            {
                Debug.LogError("Result Transform not found in recipe.");
            }
        }
    }
    
    void CheckRecipeCompletion()
    {
        // Obtener la lista de ítems utilizados en recetas
        List<Item> items = itemManager.GetResultOfRecipesForCurrentLevel();

        foreach (Item item in items)
        {
            // Debug.Log(item.itemName);
            if (item.isUsedInRecipe)
            {
                if (!recipeCompletionStatus.ContainsKey(item.itemName))
                {
                    recipeCompletionStatus[item.itemName] = false;
                }

                if (!recipeCompletionStatus[item.itemName])
                {
                    // Actualizar el estado de la receta como completada
                    recipeCompletionStatus[item.itemName] = true;

                    // Buscar y activar el objeto "Check" en el recetario
                    ActivateRecipeCheck(item.itemName);

                    // Verificar si el nivel está completo
                    CheckLevelCompletion();

                    // Debuggear el estado del diccionario después de actualizar
                    // DebugRecipeCompletionStatus();
                }
            }
        }
    }

    private void ActivateRecipeCheck(string itemName)
    {
        foreach (Transform recipe in transform)
        {
            Transform recipItemsTransform = FindChildByName(recipe, "RecipeItems");
            Transform resultTransform = FindChildByName(recipItemsTransform, "Result");
            if (resultTransform != null)
            {
                UnityEngine.UI.Image imageComponent = resultTransform.GetComponent<UnityEngine.UI.Image>();
                if (imageComponent != null && imageComponent.sprite != null && imageComponent.sprite.name == itemName)
                {
                    FindChildByName(recipe, "Check").gameObject.SetActive(true);
                    break;
                }
            }
        }
    }

    private void CheckLevelCompletion()
    {
        foreach (var status in recipeCompletionStatus.Values)
        {
            if (!status)
            {
                // Si alguna receta no está completada, el nivel no está completo
                return;
            }
        }

        _levelManager.OnLevelCompleted(transform.parent.parent.gameObject);
        transform.parent.parent.gameObject.SetActive(false);
        
        // Limpiar el diccionario de recetas al completar el nivel
        recipeCompletionStatus.Clear();
        
        // Debug.Log("Level Complete!");
    }

    private Transform FindChildByName(Transform parent, string nameToMatch)
    {
        foreach (Transform child in parent)
        {
            if (child.name == nameToMatch)
            {
                return child;
            }
        }
        return null;
    }

    private void DebugRecipeCompletionStatus()
    {
        Debug.Log("Recipe Completion Status:");
        foreach (var entry in recipeCompletionStatus)
        {
            Debug.Log($"Item: {entry.Key}, Completed: {entry.Value}");
        }
    }

    private void OnDestroy()
    {
        if (itemManager != null)
        {
            itemManager.OnItemsCombined -= CheckRecipeCompletion;
        }
    }
}
