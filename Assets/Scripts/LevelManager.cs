using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private GameObject[] levels; // Array de niveles para seleccionar
    [SerializeField] private GameObject[] levelsPlayable; // Array de niveles jugables
    [SerializeField] private ItemManager _itemManager;
    private int currentLevel; // Nivel actual al que ha llegado el jugador

    private GameObject lastLevelPlayed;
    
    [SerializeField] private GameObject levelCompletedAnimationGameObject;
    
    void Start()
    {
        // Cargar el nivel actual desde PlayerPrefs (por defecto será 0, el tutorial)
        currentLevel = PlayerPrefs.GetInt("CurrentLevel", 0);
        // CompleteLevel(3);
        // ResetProgress();
        UpdateLevelUI();
    }

    // Método para actualizar el UI de los niveles
    void UpdateLevelUI()
    {
        for (int i = 0; i < levels.Length; i++)
        {
            // Si el nivel está desbloqueado
            if (i <= currentLevel)
            {
                // Apagar el GameObject "Blocked" del nivel
                levels[i].transform.Find("Blocked").gameObject.SetActive(false);
            }
            else
            {
                // Encender el GameObject "Blocked" del nivel
                levels[i].transform.Find("Blocked").gameObject.SetActive(true);
            }
        }
    }

    // Método para reiniciar el progreso (opcional)
    public void ResetProgress()
    {
        currentLevel = 0;
        PlayerPrefs.SetInt("CurrentLevel", currentLevel);
        UpdateLevelUI();
    }
    
    public void OnLevelCompleted(GameObject currentLevel)
    {
        Debug.Log("¡Nivel completado!");
        lastLevelPlayed = currentLevel;
        levelCompletedAnimationGameObject.SetActive(true);
    }
    
    // Método para activar el GameObject del nivel actual
    void ActivateCurrentLevel()
    {
        int lastLevelIndex = System.Array.IndexOf(levelsPlayable, lastLevelPlayed);

        for (int i = 0; i < levelsPlayable.Length; i++)
        {
            levelsPlayable[i].SetActive(i == lastLevelIndex);
            // Debug.Log(levelsPlayable[i]);
        }
    }

    
    // Método para avanzar al siguiente nivel
    public void AdvanceToNextLevel()
    {
        if (lastLevelPlayed != null)
        {
            int lastLevelIndex = System.Array.IndexOf(levelsPlayable, lastLevelPlayed);

            if (lastLevelIndex != -1 && lastLevelIndex < levelsPlayable.Length - 1)
            {
                int nextLevelIndex = lastLevelIndex + 1;
                lastLevelPlayed = levelsPlayable[nextLevelIndex];
                PlayerPrefs.SetInt("CurrentLevel", nextLevelIndex);
                PlayerPrefs.Save(); // Guardar los cambios en PlayerPrefs
                _itemManager.SetCurrentLevel(nextLevelIndex);
                UpdateLevelUI();
                ActivateCurrentLevel();

                // Ocultar la animación de nivel completado
                levelCompletedAnimationGameObject.SetActive(false);
            }
            else
            {
                Debug.LogWarning("No more levels to advance.");
            }
        }
        else
        {
            Debug.LogError("lastLevelPlayed is null. Cannot advance to next level.");
        }
    }
}