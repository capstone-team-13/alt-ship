using UnityEngine;

namespace EE.AMVCC.Examples
{
    public class BallCommand : Command
    {
        public class LogCommand : BallCommand
        {
            public string Message { get; private set; }

            public LogCommand(string message)
            {
                Message = message;
            }
        }

        public class BounceCommand : BallCommand
        {
            public Vector3 Force { get; private set; }

            public BounceCommand(Vector3 force)
            {
                Force = force;
            }
        }
    }
}