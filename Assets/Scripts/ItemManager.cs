using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ItemManager : MonoBehaviour
{
    public List<LevelItems> levelItems; // Lista de ítems y recetas por nivel
    private RecipeChecker recipeChecker; // Referencia al RecipeChecker
    private Dictionary<(Item, Item), Item> recipeDictionary; // Diccionario de recetas
    private int currentLevel; // Nivel actual
    private string currenLevelName;
    
    [SerializeField] private string[] levelNames; // Nombres de las escenas (Tutorial, Level1, Level2, etc.)
    [SerializeField] private GameObject buttonNextLevel;
    [SerializeField] private GameObject buttonMainMenu;
    
    // Evento que se dispara cuando se completa una combinación de ítems
    public event System.Action OnItemsCombined;

    void Start()
    {
        recipeChecker = FindObjectOfType<RecipeChecker>(); // Encontrar el RecipeChecker en la escena

        UpdateRecipeDictionary();
        ObtainCurrentLevel();

        buttonNextLevel.GetComponent<Button>().onClick.AddListener(LoadNextLevelScene);
        buttonMainMenu.GetComponent<Button>().onClick.AddListener(() => { LoadSpecificScene("MainMenu"); });
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
                if (resultItem.isLastTier)
                {
                    recipeChecker.ReplaceTier3ItemWithTier1(resultItem);
                }
                else
                {
                    Debug.Log("El item no es tier 3, no se hará reemplazo.");
                }
                
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

    private void LoadNextLevelScene()
    {
        switch (currenLevelName)
        {
            case "Tutorial":
                LoadSpecificScene("Level1");
                break;
            case "Level1":
                LoadSpecificScene("Level2");
                break;
            case "Level2":
                LoadSpecificScene("Level3");
                break;
            default:
                Debug.Log("No more Levels");
                break;
        }
    }
    
    private void LoadSpecificScene(string SceneToLoad)
    {
        SceneManager.LoadScene(SceneToLoad);
    }
    
    private void ObtainCurrentLevel()
    {
        string currentScene = SceneManager.GetActiveScene().name;

        switch (currentScene)
        {
            case "Level3":
                currentLevel = 3;
                currenLevelName = "Level3";
                break;
            case "Level2":
                currentLevel = 2;
                currenLevelName = "Level2";
                break;
            case "Level1":
                currentLevel = 1;
                currenLevelName = "Level1";
                break;
            default:
                currentLevel = 0;
                currenLevelName = "Tutorial";
                break;
        }
    }
}
