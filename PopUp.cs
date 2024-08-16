using SadConsole;
using SadConsole.Input;
using SadRogue.Primitives;
using System.Diagnostics;

namespace AsciiGame
{
    /// <summary>
    /// A very basic SadConsole Console subclass that acts as a game message log.
    /// </summary>
    public class PopUpConsole : Console
    {

        public PopUpConsole(int width, int height)
            : base(width, height)
        {
            Initialize();
        }

        public PopUpConsole(int width, int height, int bufferWidth, int bufferHeight)
            : base(width, height, bufferWidth, bufferHeight)
        {
            Initialize();
        }

        public PopUpConsole(ICellSurface surface, IFont font = null, Point? fontSize = null)
            : base(surface, font, fontSize)
        {
            Initialize();
        }

        private void Initialize()
        {
            
        }

        public void ToggleVisibility()
        {
            IsVisible = !IsVisible;
        }
    }
}
