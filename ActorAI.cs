using GoRogue.FOV;
using GoRogue.GameFramework;
using GoRogue.Pathing;
using SadRogue.Integration;
using SadRogue.Integration.Components;
using SadRogue.Primitives;
using SadRogue.Primitives.GridViews;
using System;
using System.Diagnostics;
using System.Linq;

namespace AsciiGame
{
    /// <summary>
    /// Component for handling the AI of an enemy.
    /// </summary>
    internal class ActorAI : RogueLikeComponentBase<RogueLikeEntity>
    {
        public Actor currentActor { get; set; }
        public bool actorEstablished = false;
        public Point? playerLastKnownPosition { get; set;}
        public IGridView<bool> WalkabilityView { get; set;}

        public ActorAI()
            : base(false, false, false, false)
        {
            
        }

        public void TakeTurn()
        {
            var bruh = WalkabilityView;
            if (Parent?.CurrentMap == null) return;
            if (actorEstablished == false)
            {
                currentActor = Parent.CurrentMap.GetEntityAt<Actor>(Parent.Position);
                actorEstablished = true;
            }
            currentActor.FOV.Calculate(Parent.Position, currentActor.FOVRadius, Parent.CurrentMap.DistanceMeasurement);
            var stats = Parent.GoRogueComponents.GetFirstOrDefault<ActorStats>();
            Path path = GetPath();
            
            if (path == null)
            {
                Debug.WriteLine("Path is null!");
                return;
            }

                var firstPoint = path.GetStep(0);
                var actor = Parent.CurrentMap.GetEntityAt<RogueLikeEntity>(firstPoint);
            if (actor == null)
                {
                    //Program.GameScreen.MessageLog.AddMessage($"A {stats.Name} moves {Direction.GetDirection(Parent.Position, firstPoint)}!");
                    Parent.Position = firstPoint;
                currentActor.FOV.Calculate(Parent.Position, currentActor.FOVRadius, Parent.CurrentMap.DistanceMeasurement);
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
            if (!currentActor.FOV.BooleanResultView[Program.GameScreen.Player.Position])
            {
                if (playerLastKnownPosition == null) return null;
                if (playerLastKnownPosition == Parent.Position) return null; //Stop search if enemy has arrived at last known player position and still doesn't see anything.
                Debug.WriteLine($"{currentActor.Name} lost sight of target on turn {Info.Turn}! Pursuing last known position {playerLastKnownPosition}!");
                
                return Parent.CurrentMap.AStar.ShortestPath(Parent.Position, (Point)playerLastKnownPosition);
            }
            else
            {
                Debug.WriteLine($"{currentActor.Name} spotted target on turn {Info.Turn}! Pursuing!");
                playerLastKnownPosition = Program.GameScreen.Player.Position;
                return Parent.CurrentMap.AStar.ShortestPath(Parent.Position, Program.GameScreen.Player.Position);
            }
        }
    }
    internal class AIstar : AStar
    {
        public AIstar(IGridView<bool> WalkabilityView, Distance DistanceMeasurement)
    : base(WalkabilityView, DistanceMeasurement)
        {

        }
    }

}
