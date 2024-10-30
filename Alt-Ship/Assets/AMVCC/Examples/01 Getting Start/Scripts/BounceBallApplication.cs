namespace EE.AMVCC.Examples
{
    public class BounceBallApplication : Application<BounceModel, BounceView, BounceController>
    {
        public static class Events
        {
            public static class Game
            {
                private const string CATEGORY = "game.";
                public const string Start = CATEGORY + "start";
                public const string End = CATEGORY + "end";
            }

            public static class Ball
            {
                private const string CATEGORY = "ball.";
                public const string HitGround = CATEGORY + "hitground";
            }
        }
    }
}
