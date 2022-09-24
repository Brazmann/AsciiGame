using SadConsole;
using SadRogue.Primitives;

namespace AsciiGame
{
    /// <summary>
    /// A very basic SadConsole Console subclass that acts as a game message log.
    /// </summary>
    public class TurnDisplayConsole : Console
    {
        public static ulong Turn = 0;

        public TurnDisplayConsole(int width, int height)
            : base(width, height)
        {
            Initialize();
        }

        public TurnDisplayConsole(int width, int height, int bufferWidth, int bufferHeight)
            : base(width, height, bufferWidth, bufferHeight)
        {
            Initialize();
        }

        public TurnDisplayConsole(ICellSurface surface, IFont font = null, Point? fontSize = null)
            : base(surface, font, fontSize)
        {
            Initialize();
        }

        private void Initialize()
        {

        }

        public void DoTurn(ulong turns)
        {
            Cursor.Print($"Turn: {Turn}");
        }
    }
}
