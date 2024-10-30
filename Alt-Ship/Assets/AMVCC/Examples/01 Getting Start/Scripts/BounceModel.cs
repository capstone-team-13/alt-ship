using UnityEngine;

namespace EE.AMVCC.Examples
{
    public class BounceModel : Model
    {
        public Rigidbody Ball;
        public int Bounces;
        public int BounceToWin;

        public bool GameEnd;
    }
}
