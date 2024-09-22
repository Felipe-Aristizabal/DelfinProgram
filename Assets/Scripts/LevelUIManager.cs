using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelUIManager : MonoBehaviour
{
    [SerializeField] private GameObject[] levelNames; // Nombres de las escenas (Tutorial, Level1, Level2, etc.)
    [SerializeField] private ItemManager _itemManager;
    private int currentLevel; // Nivel actual al que ha llegado el jugador
    
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
        for (int i = 0; i < levelNames.Length; i++)
        {
            // Si el nivel está desbloqueado
            if (i <= currentLevel)
            {
                // Apagar el GameObject "Blocked" del nivel
                levelNames[i].transform.Find("Blocked").gameObject.SetActive(false);
            }
            else
            {
                // Encender el GameObject "Blocked" del nivel
                levelNames[i].transform.Find("Blocked").gameObject.SetActive(true);
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
}
