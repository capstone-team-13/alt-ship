using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Application = EE.AMVCC.Application;

public class GameFlow : MonoBehaviour
{
    public void GameEnd()
    {
        Application.Instance.Push(new GameCommand.GameEnd(Time.time));
    }
}