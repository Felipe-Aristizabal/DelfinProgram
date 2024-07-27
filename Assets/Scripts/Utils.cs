using UnityEngine;
using UnityEngine.SceneManagement;

public class Utils : MonoBehaviour
{
    [SerializeField] private GameObject mainMenuGameObject;
    [SerializeField] private GameObject uiLevelsToDeactivate;
    [SerializeField] private ItemManager _itemManager;
    
    public void OpenSpecifiedURL(string urlString)
    {
        Application.OpenURL(urlString);
    }

    public void OpenFirstLevel()
    {
        SceneManager.LoadScene("Levels");
    }
    
    public void OpenMenuCanvas(GameObject gameObjectToActivate)
    {
        mainMenuGameObject.SetActive(false);
        gameObjectToActivate.SetActive(true);
    }
    
    public void BackToTheMenu(GameObject gameObjectToDeactivate)
    {
        gameObjectToDeactivate.SetActive(false);
        mainMenuGameObject.SetActive(true);
    }

    public void OpenSpecificLevel(GameObject levelToActivate)
    {
        uiLevelsToDeactivate.gameObject.SetActive(false);
        levelToActivate.SetActive(true);
        _itemManager.SetCurrentLevel(GetIndexLevel(this.gameObject));
    }

    private int GetIndexLevel(GameObject childOfIndex)
    {
        switch (childOfIndex.transform.parent.name)
        {
            case "Tutorial": return 0;
                break;
            case "Level1": return 1;
                break;
            case "Level2": return 2;
                break;
            case "Level3": return 3;
                break;
        }

        return 0;
    }
    
    public void ExitGame()
    {
        Application.Quit();
    }
}
