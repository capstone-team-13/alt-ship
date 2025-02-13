using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    private enum SceneName
    {
        Start = 0,
        LevelSelection,
        Level1,
        Level2,
        End,
        Undefined
    }

    private static readonly string[] m_sceneNames =
    {
        "Start",
        "Level Selection",
        "Device Registration",
        "Level #2 Asset Implemented",
        "Level #3",
        //"Luka's Greybox",
        "End"
    };

    [SerializeField] private SceneName m_sceneName = SceneName.Undefined;

    public void Load()
    {
#if UNITY_EDITOR
        if (m_sceneName == SceneName.Undefined)
        {
            Debug.LogWarning($"You need to define which scene you want to load. ({gameObject.name})");
        }
#endif
        var sceneIndex = (int)m_sceneName;
        SceneManager.LoadScene(m_sceneNames[sceneIndex]);
    }

    public void MainMenu()
    {
        if (m_sceneName == SceneName.Undefined)
        {
            Debug.LogWarning($"You need to define which scene you want to load. ({gameObject.name})");
        }
        var sceneIndex = (int)m_sceneName;

        SceneManager.LoadScene(m_sceneNames[0]);
    }

    public void LevelLoad()
    {
        var sceneIndex = (int)m_sceneName;

        if (SelectedLevelData.Instance == null) Load();

        int levelNum = SelectedLevelData.Instance.levelChange;

        if (levelNum == 0)
        {
            // For Level One
            SelectedLevelData.Instance.finishedPurpose();
            SceneManager.LoadScene(m_sceneNames[3]);
        }
        else if (levelNum == 1)
        {
            // For Level Two
            SelectedLevelData.Instance.finishedPurpose();
            SceneManager.LoadScene(m_sceneNames[4]);
        }
        else if(levelNum == 2)
        {
            // For Level Three
            SelectedLevelData.Instance.finishedPurpose();
            Load();
        }

    }

    public void CloseGame()
    {
        Application.Quit();
    }
}