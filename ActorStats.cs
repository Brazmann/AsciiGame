using System.Linq;
using GoRogue.GameFramework;
using SadRogue.Integration;
using SadRogue.Integration.Components;
using SadRogue.Primitives;

namespace AsciiGame
{
    /// <summary>
    /// Simple component that moves its parent toward the player if the player is visible. It demonstrates the basic
    /// usage of the integration library's component system, as well as basic AStar pathfinding.
    /// </summary>
    internal class ActorStats : RogueLikeComponentBase<RogueLikeEntity>
    {
        public string Name { get; set; }
        public char Character { get; set; }
        public int Health { get; set; }
        public int ArmorClass { get; set; }
        public string Description { get; set; }
        public ActorStats()
            : base(false, false, false, false)
        { }

        }
}
