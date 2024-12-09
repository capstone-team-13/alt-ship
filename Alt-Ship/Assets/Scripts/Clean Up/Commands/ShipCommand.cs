using EE.AMVCC;

namespace EE.CU
{
    public class ShipCommand : Command
    {
        public class Steer : ShipCommand
        {
            public float RotationSign;

            public Steer(float rotationSign)
            {
                RotationSign = rotationSign;
            }
        }

        public class Move : ShipCommand
        {
            public float Direction;

            public Move(float direction)
            {
                Direction = direction;
            }
        }
    }
}