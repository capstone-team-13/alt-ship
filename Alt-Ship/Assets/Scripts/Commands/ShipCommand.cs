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

    public class HealthUpdate : ShipCommand
    {
        public int CurrentHealth;

        public HealthUpdate(int currentHealth)
        {
            CurrentHealth = currentHealth;
        }
    }

    public class Damage : ShipCommand
    {
        public int Value;

        public Damage(int value)
        {
            Value = value;
        }
    }
}