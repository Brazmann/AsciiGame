using GoRogue.FOV;
using GoRogue.GameFramework;
using GoRogue.Pathing;
using SadRogue.Integration;
using SadRogue.Integration.Components;
using SadRogue.Primitives;
using System;
using System.Diagnostics;
using System.Linq;

namespace AsciiGame
{
    /// <summary>
    /// Simple component that moves its parent toward the player if the player is visible. It demonstrates the basic
    /// usage of the integration library's component system, as well as basic AStar pathfinding.
    /// </summary>
    internal class DemoEnemyAI : RogueLikeComponentBase<RogueLikeEntity>
    {
        public Actor currentActor { get; set; }
        public bool actorEstablished = false;
        public Point? playerLastKnownPosition { get; set;}

        public DemoEnemyAI()
            : base(false, false, false, false)
        {
            
        }

        public void TakeTurn()
        {
            if (Parent?.CurrentMap == null) return;
            if (actorEstablished == false)
            {
                currentActor = Parent.CurrentMap.GetEntityAt<Actor>(Parent.Position);
                actorEstablished = true;
            }
            currentActor.FOV.Calculate(Parent.Position, currentActor.FOVRadius, Parent.CurrentMap.DistanceMeasurement); //IDK if I want to keep calculating it's FOV on it's turn, but it's looking like that's how it's gotta be.
            var stats = Parent.GoRogueComponents.GetFirstOrDefault<ActorStats>();
            Path path = GetPath();
            if (path == null) return;

                var firstPoint = path.GetStep(0);
                if (Parent.CanMove(firstPoint))
                {
                    //Program.GameScreen.MessageLog.AddMessage($"A {stats.Name} moves {Direction.GetDirection(Parent.Position, firstPoint)}!");
                    Parent.Position = firstPoint;
                }
        }

        public void TakeDamage(string attackerName, int amount)
        {
            var stats = Parent.GoRogueComponents.GetFirstOrDefault<ActorStats>();
            stats.Health -= amount;
            Debug.WriteLine(stats.Health);
            if (stats.Health <= 0)
            {
                Parent.CurrentMap.RemoveEntity(Parent);
                Program.GameScreen.MessageLog.AddMessage($"{stats.Name} was killed by {attackerName}");
                return;
            }
            Program.GameScreen.MessageLog.AddMessage($"{attackerName} attacks {stats.Name} for {amount} damage!");
        }

        public Path GetPath()
        {
            if (!currentActor.FOV.CurrentFOV.Contains(Program.GameScreen.Player.Position))
            {
                if (playerLastKnownPosition == null) return null;
                if (playerLastKnownPosition == Parent.Position) return null;
                Debug.WriteLine($"Pursuing last known position {playerLastKnownPosition}");
                return Parent.CurrentMap.AStar.ShortestPath(Parent.Position, (Point)playerLastKnownPosition);
            }
            else
            {
                playerLastKnownPosition = Program.GameScreen.Player.Position;
                return Parent.CurrentMap.AStar.ShortestPath(Parent.Position, Program.GameScreen.Player.Position);
            }
        }
    }
}
