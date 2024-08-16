using SadConsole;
using SadConsole.Input;
using SadRogue.Primitives;
using System.Diagnostics;

namespace AsciiGame
{
    /// <summary>
    /// A very basic SadConsole Console subclass that acts as a game message log.
    /// </summary>
    public class GameInfoConsole : Console
    {

        public GameInfoConsole(int width, int height)
            : base(width, height)
        {
            Initialize();
        }

        public GameInfoConsole(int width, int height, int bufferWidth, int bufferHeight)
            : base(width, height, bufferWidth, bufferHeight)
        {
            Initialize();
        }

        public GameInfoConsole(ICellSurface surface, IFont font = null, Point? fontSize = null)
            : base(surface, font, fontSize)
        {
            Initialize();
        }

        private void Initialize()
        {
            UpdateGameInfoDisplay();
        }

        public void UpdateGameInfoDisplay()
        {
            Cursor.Position = new Point(0, 0);
            Cursor.Print($"Turn: {Info.Turn}");
        }

        public void IncrementTurn()
        {
            Info.Turn++;
            UpdateGameInfoDisplay();
        }
        public ulong GetTurn()
        {
            return Info.Turn;
        }
        public override bool ProcessKeyboard(Keyboard keyboard)
        {
            if(keyboard.KeysDown.Count > 0)
            {
                //Debug.WriteLine($"{keyboard.KeysDown[0].Key}");
                Cursor.Position = new Point(0, 0);
                Cursor.Print($"Turn: {Info.Turn} {keyboard.KeysDown[0].Key}______________________________________");
            }

            
            return base.ProcessKeyboard(keyboard);
        }
    }

    public class Info
    {
        public static ulong Turn = 0;
    }
}
