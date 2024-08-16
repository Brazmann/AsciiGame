using GoRogue.Pathing;
using GoRogue.Random;
using SadRogue.Integration;
using SadRogue.Integration.Components;
using SadRogue.Integration.FieldOfView.Memory;
using SadRogue.Primitives;
using SadRogue.Primitives.GridViews;
using System.Collections.Generic;
using System.Diagnostics;

namespace AsciiGame
{
    /// <summary>
    /// Component for handling the AI of an enemy.
    /// </summary>
    internal class ActorAI : RogueLikeComponentBase<RogueLikeEntity>
    {
        public Actor currentActor { get; set; }
        public bool actorEstablished = false;
        public Point? TargetLastKnownPosition { get; set; }
        public IGridView<bool> WalkabilityView { get; set; }
        public AStar AStarTerrain { get; set; }
        public bool ActionInProgress { get; set; }
        public Dictionary<ulong, Point> MoveQueue; //Key is turn to activate, value is action to take.
        public bool DebugMode = true;
        public bool CanSeeTarget;
        public RogueLikeEntity Target;
        public State CurrentState = State.Idle;
        public ActorAI()
            : base(false, false, false, false)
        {
        }

        public enum State
        {
            Idle,
            Pursuing,
            Searching
        }

        public void TakeTurn()
        {
            if (!actorEstablished)
            {
                currentActor = Parent.CurrentMap.GetEntityAt<Actor>(Parent.Position);
                actorEstablished = true;
            }
            currentActor.FOV.Calculate(Parent.Position, currentActor.FOVRadius, Parent.CurrentMap.DistanceMeasurement);
            Target = Program.GameScreen.Player;
            var bruh = currentActor.FOV.BooleanResultView.Positions();
    
            CanSeeTarget = currentActor.FOV.BooleanResultView[Target.Position];
            if (AStarTerrain == null)
            {
                var map = Parent.CurrentMap;
                var view = new LambdaGridView<bool>(map.Width, map.Height, pos => map.Terrain[pos].IsWalkable);
                AStarTerrain = new AStar(view, Parent.CurrentMap.DistanceMeasurement);
            }
            currentActor.FOV.Calculate(Parent.Position, currentActor.FOVRadius, Parent.CurrentMap.DistanceMeasurement);
            var CurrentTurn = Program.GameScreen.GameInfo.GetTurn();
            if (ActionInProgress == true)
            {
                if (MoveQueue.ContainsKey(CurrentTurn))
                {
                    MoveQueue.TryGetValue(CurrentTurn, out var position);
                    MoveQueue.Remove(CurrentTurn);
                    MoveAction(position);
                    if (MoveQueue.Count == 0)
                    {
                        ActionInProgress = false;
                    }
                }
                else return;
            }
            else
            {
                if (Parent?.CurrentMap == null) return;
                MoveToPlayer();
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
                if(stats.Health <= -100)
                {
                    Explode();
                    Program.GameScreen.MessageLog.AddMessage($"{stats.Name} was gored by {attackerName}");
                } else
                {
                    Parent.CurrentMap.RemoveEntity(Parent);
                    Program.GameScreen.MessageLog.AddMessage($"{stats.Name} was killed by {attackerName}");
                }
                return;
            }
            Program.GameScreen.MessageLog.AddMessage($"{attackerName} attacks {stats.Name} for {amount} damage!");
        }

        public void Explode()
        {
            currentActor.FOV.Calculate(Parent.Position, 2);
            var positions = currentActor.FOV.CurrentFOV;
            foreach (var position in positions)
            {
                var roll = GlobalRandom.DefaultRNG.NextInt(0, 4);
                if (position == Program.GameScreen.Player.Position)
                {

                }
                else if (roll == 3)
                {
                    Debug.WriteLine("3");
                }
                else
                {
                    Item gib = new Item(Color.Pink, '#', true, true, (int)MyGameMap.Layer.Items, 1);
                    switch (roll)
                    {
                        case 0:
                            gib = new Item(Color.Red, '@', true, true, (int)MyGameMap.Layer.Items, 1);
                            break;
                        case 1:
                            gib = new Item(Color.Red, '~', true, true, (int)MyGameMap.Layer.Items, 1);
                            break;
                        case 2:
                            gib = new Item(Color.Red, '*', true, true, (int)MyGameMap.Layer.Items, 1);
                            break;
                    }

                    var terrain = Parent.CurrentMap.GetTerrainAt(position);


                    gib.Appearance.Background = Color.DarkRed;
                    gib.Position = position;
                    
                    Parent.CurrentMap.AddEntity(gib);
                }
            }
            Parent.CurrentMap.RemoveEntity(Parent);
        }

        public Path GetPath(AStar DefinedAStar, Point Position)
        {

                
                //if (PlayerLastKnownPosition == null) return null;
                //if (PlayerLastKnownPosition == Parent.Position) return null; //Stop search if enemy has arrived at last known player position and still doesn't see anything.
                
                return DefinedAStar.ShortestPath(Parent.Position, Position);
            
        }

        /*public Path GetPathTerrainOnly()
        {

            if (!CanSeePlayer)
            {
                currentActor.Appearance.Glyph = 'L'; //debug
                if (playerLastKnownPosition == null) return null;
                if (playerLastKnownPosition == Parent.Position) return null; //Stop search if enemy has arrived at last known player position and still doesn't see anything.
                Debug.WriteLine($"{currentActor.Name} lost sight of target on turn {Info.Turn}! Pursuing last known position {playerLastKnownPosition}!");
                PlayerSpotted = false;
                return AStarTerrain.ShortestPath(Parent.Position, (Point)playerLastKnownPosition);
            }
            else
            {
                currentActor.Appearance.Glyph = 'S'; //debug
                PlayerSpotted = true;
                Debug.WriteLine($"{currentActor.Name} spotted target on turn {Info.Turn}! Pursuing!");
                playerLastKnownPosition = Program.GameScreen.Player.Position;
                return AStarTerrain.ShortestPath(Parent.Position, Program.GameScreen.Player.Position);
            }
        }*/
        #region Actions


        public void MoveToPlayer()
        {
            currentActor.FOV.Calculate(Parent.Position, currentActor.FOVRadius, Parent.CurrentMap.DistanceMeasurement);
           
            Point Target;
            if (!CanSeeTarget)
            {
                currentActor.Appearance.Background = Color.Red;

                if (TargetLastKnownPosition == null) return;
                Target = (Point)TargetLastKnownPosition;
            } else
            {
                currentActor.Appearance.Background = Color.AliceBlue;
                Target = (Point)Program.GameScreen.Player.Position;
                TargetLastKnownPosition = Target;
            }
            var stats = Parent.GoRogueComponents.GetFirstOrDefault<ActorStats>();
            Path path = GetPath(Parent.CurrentMap.AStar, Target);

            if (path == null)
            {
                currentActor.FOV.Calculate(Parent.Position, currentActor.FOVRadius, Parent.CurrentMap.DistanceMeasurement);
                Debug.WriteLine($"{currentActor.Name}: Actors in the way are preventing pathfinding! Trying alternative method.");
                var NewPath = GetPath(AStarTerrain, Target);
                if (NewPath == null) return;
                path = NewPath;
            }
            var CurrentTurn = Program.GameScreen.GameInfo.GetTurn();
            if (path.GetStep(0) == null)
            {
                return;
            }
             var firstPoint = path.GetStep(0);
            var actor = Parent.CurrentMap.GetEntityAt<RogueLikeEntity>(firstPoint);
            if (actor == null)
            {
                //Program.GameScreen.MessageLog.AddMessage($"A {stats.Name} moves {Direction.GetDirection(Parent.Position, firstPoint)}!");
                //ActionQueue.Add(CurrentTurn + 2, MoveAction(firstPoint));
                MoveAction(firstPoint);
                currentActor.FOV.Calculate(Parent.Position, currentActor.FOVRadius, Parent.CurrentMap.DistanceMeasurement);
            }
            else
            {
                if(actor.Layer == (int)MyGameMap.Layer.Items)
                {
                    MoveAction(firstPoint);
                }
                //var point = GetNearestVacantPoint();
                //Parent.Position = point;
                currentActor.FOV.Calculate(Parent.Position, currentActor.FOVRadius, Parent.CurrentMap.DistanceMeasurement);
            }
        }

        public void MoveAction(Point MovePoint)
        {
            Parent.Position = MovePoint;

        }
        #endregion Actions
    }
}
