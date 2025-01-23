using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DeviceRegister : MonoBehaviour
{
    #region Editor API

    [SerializeField] private PlayerInput m_playerPrefab;

    [SerializeField] private Transform[] m_spawnPoints;

    // Old Data Structure
    [SerializeField] private CameraController m_cameraController;

    #endregion

    #region Unity Callbacks

    [UsedImplicitly]
    private void OnValidate()
    {
        if (m_playerPrefab == null)
        {
            string errorMessage = $"{nameof(m_playerPrefab)} is not assigned in the Inspector. " +
                                  "Please assign a valid PlayerInput prefab to proceed.";
            Debug.LogError(errorMessage);
            throw new UnityException(errorMessage);
        }

        if (m_spawnPoints == null || m_spawnPoints.Length == 0)
        {
            string errorMessage = "Spawn points are not set or are empty. " +
                                  "Please assign at least one valid spawn point in the Inspector to proceed.";
            Debug.LogError(errorMessage);
            throw new UnityException(errorMessage);
        }
    }

    [UsedImplicitly]
    private void Start()
    {
        __M_SpawnPlayers();

        if (PlayerDeviceManager.Instance != null)
        {
            PlayerDeviceManager.Instance.DisableHandler();

            try
            {
                __M_SyncDataToOldStructure();
            }
            catch (Exception exception)
            {
                Debug.LogError($"Exception occurred: {exception.Message}");
            }
            finally
            {
                DestroyImmediate(gameObject);
            }
        }
        else
        {
            string errorMessage = $"{nameof(PlayerDeviceManager)} instance is null. " +
                                  "Please ensure the game is started from the Start Scene to ensure all controllers are registered correctly.";
            throw new UnityException(errorMessage);
        }
    }

    #endregion

    #region Internal

    private const uint PLAYER_COUNT = 2;
    private PlayerInput[] m_players = new PlayerInput[PLAYER_COUNT];

    private void __M_SpawnPlayers()
    {
        int spawnPointId = 0;
        for (int id = 0; id < PLAYER_COUNT; id++)
        {
            PlayerInput player = __M_CreatePlayer(id, spawnPointId);
            if (player == null) continue;

            __M_AssignPlayerName(player, id);
            __M_AssignPlayerDevice(player, id);
            __M_AssignPlayerModel(player, id);
            __M_AssignToPlayerArray(player, id);

            spawnPointId = __M_GetNextSpawnPointId(spawnPointId);
        }
    }

    private PlayerInput __M_CreatePlayer(int id, int spawnPointId)
    {
        PlayerInput player = Instantiate(m_playerPrefab, m_spawnPoints[spawnPointId].position, Quaternion.identity);
        if (player == null)
        {
            Debug.LogError($"Failed to instantiate player prefab for player #{id + 1}.");
        }

        return player;
    }

    private static void __M_AssignPlayerName(PlayerInput player, int id)
    {
        player.name = "Player #" + (id + 1);
    }

    private void __M_AssignPlayerDevice(PlayerInput player, int id)
    {
        InputDevice device = PlayerDeviceManager.Instance.GetDeviceByPlayerId(id);
        player.SwitchCurrentControlScheme(device);

        Debug.Log($"Device assigned to player {player.playerIndex}: {device.displayName}");
    }

    private static void __M_AssignPlayerModel(PlayerInput player, int id)
    {
        var playerController = player.GetComponent<PlayerController>();
        if (playerController != null) playerController.SetUpPlayerModel(id);
        else throw new Exception($"PlayerController component not found on GameObject: {player.name}");
    }

    private void __M_AssignToPlayerArray(PlayerInput player, int id)
    {
        if (m_players.Length > id)
        {
            m_players[id] = player;
        }
        else
        {
            Debug.LogError($"Index {id} out of bounds for m_players array.");
        }
    }

    private int __M_GetNextSpawnPointId(int currentSpawnPointId)
    {
        return (currentSpawnPointId + 1) % m_spawnPoints.Length;
    }

    private void __M_SyncDataToOldStructure()
    {
        foreach (PlayerInput player in m_players)
        {
            Debug.Log($"Player Name {player.name}");
            m_cameraController.AddPlayer(player);
        }

        m_cameraController.PlayerCount = m_players.Length;
    }

    #endregion
}