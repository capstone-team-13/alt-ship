using EE.AMVCC;

public class GameCommand : Command
{
    public class GameStart : GameCommand
    {
        public float Time { get; private set; }

        public GameStart(float time)
        {
            Time = time;
        }
    }

    public class GameEnd : GameCommand
    {
        public float Time { get; private set; }

        public GameEnd(float time)
        {
            Time = time;
        }
    }
}