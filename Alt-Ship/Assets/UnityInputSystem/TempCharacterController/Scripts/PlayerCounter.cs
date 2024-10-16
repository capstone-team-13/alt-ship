using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCounter : MonoBehaviour
{
    public static int playerCount;

    private PlayerInputManager playerInputManager;

    // Start is called before the first frame update
    void Start()
    {
        playerCount = 0;
        playerInputManager = this.GetComponent<PlayerInputManager>();
    }

}
