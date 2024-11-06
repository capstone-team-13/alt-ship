using EE.AMVCC;

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
}