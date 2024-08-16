using GoRogue.Random;
using SadConsole;
using SadConsole.Input;
using SadRogue.Integration;
using SadRogue.Primitives;
using ShaiRandom.Generators;
using System.Diagnostics;

namespace AsciiGame
{
    internal class MapScreen : ScreenObject
    {
        public bool debug = true;

        public readonly MyGameMap Map;
        public readonly RogueLikeEntity Player;
        public readonly MessageLogConsole MessageLog;
        public readonly GameInfoConsole GameInfo;
        public readonly Console BorderConsole;
        public readonly PopUpConsole PopUp;
       // public readonly Console TurnDisplay;

        public readonly ScreenSurface Border;

        const int MessageLogHeight = 5;
        const int DebugLogHeight = 5;

        public MapScreen(MyGameMap map)
        {
            // Record the map we're rendering
            Map = map;

            // Create a renderer for the map, specifying viewport size.  The value in DefaultRenderer is automatically
            // managed by the map, and renders whenever the map is the active screen.
            //
            // CUSTOMIZATION: Pass in custom fonts/viewport sizes here.
            //
            // CUSTOMIZATION: If you want multiple renderers to render the same map, you can call CreateRenderer and
            // manage them yourself; but you must call the map's RemoveRenderer when you're done with these renderers,
            // and you must add any non-default renderers to the SadConsole screen object hierarchy, IN ADDITION
            // to the map itself.
            Map.DefaultRenderer = Map.CreateRenderer((Program.Width, Program.Height - MessageLogHeight));
            //Map.DefaultRenderer.Font = SadConsole.Game.Instance.LoadFont("./fonts/C64.font");
            // Make the Map (which is also a screen object) a child of this screen.  You MUST have the map as a child
            // of the active screen, even if you are using entirely custom renderers.
            Map.Parent = this;

            // Make sure the map is focused so that it and the entities can receive keyboard input
            Map.IsFocused = true;

            // Generate player, add to map at a random walkable position, and calculate initial FOV
            Player = MapObjectFactory.Player();
            Player.Position = GlobalRandom.DefaultRNG.RandomPosition(Map.WalkabilityView, true);
            Player.Name = "Player";
            Map.AddEntity(Player);
            Player.AllComponents.GetFirst<PlayerFOVController>().CalculateFOV();

            // Center view on player as they move
            Map.DefaultRenderer?.SadComponents.Add(new SadConsole.Components.SurfaceComponentFollowTarget { Target = Player });

            // Create message log
            MessageLog = new MessageLogConsole(Program.Width / 2, MessageLogHeight);
            MessageLog.Parent = this;
            MessageLog.Position = new(0, Program.Height - MessageLogHeight);

            // Create menu pop up
            PopUp = new PopUpConsole(Program.Width / 2, MessageLogHeight);
            PopUp.Parent = this;
            PopUp.Position = new(0, MessageLogHeight + 20);
            PopUp.DefaultBackground = Color.AnsiBlue;
            PopUp.IsVisible = false;

            // Create turn display
            /*TurnDisplay = new TurnDisplayConsole(Program.Width / 2, MessageLogHeight);
            TurnDisplay.Parent = this;
            TurnDisplay.Position = new(MessageLog.Width + 1, Program.Height - MessageLogHeight);*/

            // Create debug log
            GameInfo = new GameInfoConsole(Program.Width / 2, MessageLogHeight);
            GameInfo.Parent = this;
            GameInfo.Position = new(MessageLog.Width + 1, Program.Height - MessageLogHeight);
            GameInfo.IsVisible = debug;

            // Create border between map and UI
            BorderConsole = new Console(Map.Width, Map.Height);
            BorderConsole.Position = Map.Position;
            BorderConsole.Parent = this;
            BorderConsole.DrawBox(new SadRogue.Primitives.Rectangle(Map.Position.X, Map.Position.Y, Program.Width, Program.Height - MessageLogHeight), new ColoredGlyph(Color.Red, Color.Black, '#'));
            BorderConsole.DrawBox(new SadRogue.Primitives.Rectangle(MessageLog.Width, Program.Height - MessageLogHeight, 1, Program.Height - MessageLogHeight), new ColoredGlyph(Color.Red, Color.Black, '|'));

            Border = new ScreenSurface(Program.Width, 1);
            Border.Surface.DrawLine(new Point(60, 5), new Point(66, 20), '$', Color.AnsiBlue, Color.AnsiBlueBright, Mirror.None);
            Border.UseMouse = false;

            //BorderConsole.Children.Add(Border);
        }
    }
}
