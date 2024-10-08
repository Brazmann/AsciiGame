﻿using GoRogue.GameFramework;
using SadRogue.Integration;
using SadRogue.Integration.Keybindings;
using SadRogue.Primitives;
using System.Diagnostics;

namespace AsciiGame
{
    /// <summary>
    /// Subclass of the integration library's keybindings component that moves enemies as appropriate when the player
    /// moves.
    /// </summary>
    /// <remarks>
    /// CUSTOMIZATION: Components can also be attached to maps, so the code for calling TakeTurn on all entities could
    /// be moved to a map component as well so that it is more re-usable by code that doesn't pertain to movement.
    /// </remarks>
    internal class CustomPlayerKeybindingsComponent : PlayerKeybindingsComponent
    {
        
        protected override void MotionHandler(Direction direction)
        {
            
            var actor = Parent.CurrentMap.GetEntityAt<Actor>(new Point(Parent.Position.X + direction.DeltaX, Parent.Position.Y + direction.DeltaY));

            if (actor != null)
            {
                int PlayerDamage = 2000;
                //Debug.WriteLine($"Transparency of tile: {actor.CurrentMap.TransparencyView[Parent.Position]}");
                if (actor.Layer == (int)MyGameMap.Layer.Monsters) //Check target tile is monster
                {
                    var ai = actor.GoRogueComponents.GetFirstOrDefault<ActorAI>();
                    ai.TakeDamage("Player", PlayerDamage);
                    Program.GameScreen.GameInfo.IncrementTurn();
                    TakeAllTurns();
                    return;
                }
                else
                {
                    return;
                }
            } else if (!Parent!.CanMoveIn(direction)){
                return;
            }
            PrintNearbyEntities();
            Program.GameScreen.GameInfo.IncrementTurn();
            var bruh = true;

            Parent.Position += direction;
            //Parent.Position += direction;
            //Parent.Position += direction;
            TakeAllTurns();
        }
        public void PrintNearbyEntities()
        {
            Actor checkEntity = Parent.CurrentMap.GetEntityAt<Actor>(Parent.Position.X + 1, Parent.Position.Y);
            if (checkEntity == null) return;
            //Program.GameScreen.MessageLog.AddMessage(checkEntity.Name);
        }

        public void TakeAllTurns()
        {
            foreach (var entity in Parent.CurrentMap!.Entities.Items)
            {
                var ai = entity.GoRogueComponents.GetFirstOrDefault<ActorAI>();
                ai?.TakeTurn();
            }
        }

    }
}
