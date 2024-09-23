using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class RecipeChecker : MonoBehaviour
{
    public static event System.Action<GameObject> OnLevelCompleted;
    
    private ItemManager itemManager;
    private Dictionary<string, bool> recipeCompletionStatus; // Para rastrear el estado de las recetas
    [SerializeField] private LevelManager _levelManager;

    private Transform gridTransform;
    
    void Start()
    {
        Debug.Log("RecipeChecker Start");
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
        
        // DebugRecipeCompletionStatus();
    }

    public void InitializeRecipeCompletionStatus()
    {
        Debug.Log("Initializing recipe completion status");
        
        recipeCompletionStatus = new Dictionary<string, bool>();
        gridTransform = transform.parent.parent.GetChild(0).transform;

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
        // DebugRecipeCompletionStatus();
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

                    if (item.isLastTier)
                    {
                        if (CheckLevelCompletion() || item.itemName == "Magic_Necklace")
                        {
                            _levelManager.OnLevelCompleted(transform.parent.parent.gameObject);
                            transform.parent.parent.gameObject.SetActive(false);

                            // Limpiar el diccionario de recetas al completar el nivel
                            recipeCompletionStatus.Clear();
                            OnLevelCompleted?.Invoke(transform.parent.parent.gameObject);
                        }
                    }

                    // Debuggear el estado del diccionario después de actualizar
                    // DebugRecipeCompletionStatus();
                }
            }
        }
    }

    private void ActivateRecipeCheck(string itemName)
    {
        string unifiedItemName = GetUnifiedNecklaceName(itemName);

        foreach (Transform recipe in transform)
        {
            Transform recipItemsTransform = FindChildByName(recipe, "RecipeItems");
            Transform resultTransform = FindChildByName(recipItemsTransform, "Result");
            if (resultTransform != null)
            {
                UnityEngine.UI.Image imageComponent = resultTransform.GetComponent<UnityEngine.UI.Image>();
                if (imageComponent != null && imageComponent.sprite != null && GetUnifiedNecklaceName(imageComponent.sprite.name) == unifiedItemName)
                {
                    FindChildByName(recipe, "Check").gameObject.SetActive(true);
                    break;
                }
            }
        }
    }

    private string GetUnifiedNecklaceName(string itemName1, string itemName2 = "")
    {
        if (itemName1.StartsWith("Necklace") || itemName2.StartsWith("Necklace"))
        {
            return "Multiple_Necklace";
        }
        return itemName1; // Retorna el nombre original si no es un collar
    }
    
    private bool CheckLevelCompletion()
    {
        foreach (var status in recipeCompletionStatus.Values)
        {
            if (!status)
            {
                // Si alguna receta no está completada, el nivel no está completo
                // Debug.Log("No está completo");
                return false;
            }
        }

        // Debug.Log("Completamos");
        _levelManager.OnLevelCompleted(transform.parent.parent.gameObject);
        transform.parent.parent.gameObject.SetActive(false);

        // Limpiar el diccionario de recetas al completar el nivel
        recipeCompletionStatus.Clear();

        // Debug.Log("Level Complete!");
        return true;
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

    public void ReplaceTier3ItemWithTier1(Item tier3Item)
    {
        if (tier3Item != null)
        {
            StartCoroutine(WaitAndReplace(tier3Item));
        }
    }

    private IEnumerator WaitAndReplace(Item tier3Item)
    {
        yield return new WaitForSeconds(2);

        Item leastFrequentTier1Item = GetLeastFrequentTier1Item();

        if (leastFrequentTier1Item != null)
        {
            ReplaceItemInGrid(tier3Item, leastFrequentTier1Item);
        } else { Debug.Log("No se encontró el item que menos aparece en el grid"); }
    }

    private Item GetLeastFrequentTier1Item()
    {
        Dictionary<Item, int> itemCounts = new Dictionary<Item, int>();

        foreach (Item tier1Item in itemManager.GetItemsForCurrentLevel())
        {
            itemCounts[tier1Item] = 0;
        }

        // Contar cuántas veces aparece cada ítem de tier 1 en el grid
        foreach (Transform slot in gridTransform)
        {
            ItemComponent itemComponent = slot.GetComponentInChildren<ItemComponent>();
            if (itemComponent != null && itemComponent.item != null && itemCounts.ContainsKey(itemComponent.item))
            {
                itemCounts[itemComponent.item]++;
            }
        }

        // Mostrar el conteo de cada ítem
        foreach (var itemCount in itemCounts)
        {
            Debug.Log($"Item: {itemCount.Key.itemName}, Count: {itemCount.Value}");
        }

        Item leastFrequentItem = null;
        int minCount = int.MaxValue;

        // Encontrar el ítem con el conteo más bajo
        foreach (var itemCount in itemCounts)
        {
            if (itemCount.Value < minCount)
            {
                minCount = itemCount.Value;
                leastFrequentItem = itemCount.Key;
            }
        }

        // Mostrar cuál es el ítem menos frecuente
        if (leastFrequentItem != null)
        {
            Debug.Log($"Least Frequent Item: {leastFrequentItem.itemName}, Count: {minCount}");
        }
        else
        {
            Debug.Log("No tier 1 items found in the grid.");
        }

        return leastFrequentItem;
    }


    private void ReplaceItemInGrid(Item oldItem, Item newItem)
    {
        foreach (Transform slot in gridTransform)
        {
            ItemComponent itemComponent = slot.GetComponentInChildren<ItemComponent>();
            if (itemComponent != null && itemComponent.item == oldItem)
            {
                itemComponent.item = newItem;
                slot.GetChild(0).GetComponent<UnityEngine.UI.Image>().sprite = newItem.sprite;
                break;
            } 
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
