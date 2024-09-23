using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private ItemManager _itemManager;
    private int currentLevel; // Nivel actual al que ha llegado el jugador

    [SerializeField] private GameObject levelCompletedAnimationGameObject;
    
    private void OnEnable()
    {
        RecipeChecker.OnLevelCompleted += OnLevelCompleted; // Suscribirse al evento
    }

    private void OnDisable()
    {
        RecipeChecker.OnLevelCompleted -= OnLevelCompleted; // Desuscribirse del evento
    }
    
    void Start()
    {
        currentLevel = PlayerPrefs.GetInt("CurrentLevel", 0);
        
        levelCompletedAnimationGameObject.SetActive(false);
    }

    // Método para reiniciar el progreso (opcional)
    public void ResetProgress()
    {
        currentLevel = 0;
        PlayerPrefs.SetInt("CurrentLevel", currentLevel);
        PlayerPrefs.Save();
    }
    
    // Método que se llama cuando se completa un nivel
    public void OnLevelCompleted(GameObject currentLevelObject)
    {
        Debug.Log("¡Nivel completado!");
        levelCompletedAnimationGameObject.SetActive(true);

        PlayerPrefs.SetInt("CurrentLevel", _itemManager.GetCurrentLevel() + 1);
        Debug.Log("Playerprefs seteadas con Level:" + _itemManager.GetCurrentLevel());
        PlayerPrefs.Save();
    }
}
