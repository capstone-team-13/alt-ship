using EE.AMVCC;
using UnityEngine;

public class PlayerCommand : Command
{
    public PlayerModel Player;

    public PlayerCommand(PlayerModel player)
    {
        Player = player;
    }

    public class Move : PlayerCommand
    {
        public Vector3 Input;

        public Move(PlayerModel player, Vector3 input) : base(player)
        {
            Input = input;
        }
    }

    public class ChangeSpeed : PlayerCommand
    {
        public float Speed;

        public ChangeSpeed(PlayerModel player, float speed) : base(player)
        {
            Speed = speed;
        }
    }

    public class Grab : PlayerCommand
    {
        public Grab(PlayerModel player) : base(player)
        {
        }
    }

    public class Shoot : PlayerCommand
    {
        public Shoot(PlayerModel player) : base(player)
        {
        }
    }

    public class Dead : PlayerCommand
    {
        public Dead(PlayerModel player) : base(player)
        {
        }
    }
}