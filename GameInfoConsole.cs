using SadConsole;
using SadRogue.Primitives;

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
    }

    public class Info
    {
        public static ulong Turn = 0;
    }
}
