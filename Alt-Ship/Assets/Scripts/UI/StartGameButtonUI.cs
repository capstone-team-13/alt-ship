using UnityEngine;

public class StartGameButtonUI : MonoBehaviour
{
    [SerializeField] private SceneLoader m_sceneLoader;

    public void StartGame()
    {
        if (PlayerDeviceManager.Instance == null) m_sceneLoader.SceneName = SceneName.DeviceRegistration;
        else m_sceneLoader.SceneName = SceneName.LevelSelection;
    }
}