using SadConsole;
using System.Diagnostics;

namespace AsciiGame
{
    /// <summary>
    /// ***********README***********
    ///
    /// The provided code is a simple template to demonstrate some integration library features and set up some boilerplate
    /// for you.  Feel free to use, or delete, any of it that you want; it shows one way of doing things, not the only way!
    ///
    /// The code contains a few comments beginning with "CUSTOMIZATION:", which show you some common points to modify in
    /// order to accomplish some common tasks.  The tags by no means represent a comprehensive guide to everything you
    /// might want to modify; they're simply designed to provide a "quick-start" guide that can help you accomplish some
    /// common tasks.
    /// </summary>
    internal static class Program
    {
        // Window width/height
        public const int Width = 128;
        public const int Height = 64;

        // Map width/height
        private const int MapWidth = 256;
        private const int MapHeight = 128;

        public static MapScreen GameScreen;

        public static ulong Seed = 1;

        private static void Main()
        {
            ShaiRandom.Serializer.RegisterShaiRandomDefaultTags();
            GoRogue.Random.GlobalRandom.DefaultRNG.Seed(1);
            Debug.WriteLine($"Seed: {GoRogue.Random.GlobalRandom.DefaultRNG.StringSerialize()}");
            Game.Create(Width, Height, "./fonts/C64.font");
            Game.Instance.OnStart = Init;
            Game.Instance.Run();
            Game.Instance.Dispose();
            
        }

        private static void Init()
        {
            // Generate a dungeon map
            var map = MapFactory.GenerateDungeonMap(MapWidth, MapHeight);

            // Create a MapScreen and set it as the active screen so that it processes input and renders itself.
            //GameScreen = new MapScreen(map);
            //GameHost.Instance.Screen = GameScreen;
            ChangeToMapScreen(map);

            // Destroy the default starting console that SadConsole created automatically because we're not using it.
            GameHost.Instance.DestroyDefaultStartingConsole();
        }

        public static void ChangeToMapScreen(MyGameMap map)
        {
            GameScreen = new MapScreen(map);
            GameHost.Instance.Screen = GameScreen;
        }
    }
}
