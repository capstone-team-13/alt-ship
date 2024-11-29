using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    private enum SceneName
    {
        Start = 0,
        LevelSelection,
        Level1,
        Undefined
    }

    private static readonly string[] m_sceneNames =
    {
        "Start",
        "Level Selection",
        "Luka's Greybox"
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

    public void CloseGame()
    {
        Application.Quit();
    }
}
