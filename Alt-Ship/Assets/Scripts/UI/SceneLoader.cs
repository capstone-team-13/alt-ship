using UnityEngine;
using UnityEngine.SceneManagement;

public enum SceneName
{
    Start = 0,
    DeviceRegistration,
    LevelSelection,
    Level1,
    Level2,
    Level3,
    End,
    Undefined
}

public class SceneLoader : MonoBehaviour
{
    private static readonly string[] m_sceneNames =
    {
        "Start",
        "Device Registration",
        "Level Selection",
        "Level #2 Asset Implemented",
        "Level #3",
        "Level #4",
        //"Luka's Greybox",
        "End"
    };

    [SerializeField] private SceneName m_sceneName = SceneName.Undefined;

    public SceneName SceneName
    {
        get => m_sceneName;
        set => m_sceneName = value;
    }

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
        else if (levelNum == 2)
        {
            // For Level Three
            SelectedLevelData.Instance.finishedPurpose();
            SceneManager.LoadScene(m_sceneNames[5]);

        }
    }

    public void CloseGame()
    {
        Application.Quit();
    }
}