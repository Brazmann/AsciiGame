using GoRogue.GameFramework;
using SadRogue.Integration;
using SadRogue.Integration.Keybindings;
using SadRogue.Primitives;
using System;
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
            if (!Parent!.CanMoveIn(direction))
            {
                var entity = Parent.CurrentMap.GetEntityAt<RogueLikeEntity>(new Point(Parent.Position.X + direction.DeltaX, Parent.Position.Y + direction.DeltaY));
                if (entity == null) return;
                if (entity.Layer == 1)
                {
                    Parent.CurrentMap.RemoveEntity(entity);
                    Program.GameScreen.MessageLog.AddMessage($"Player fucking killed {entity.Name}!");
                } else
                {
                    return;
                }
            }
            PrintNearbyEntities();
            Program.GameScreen.GameInfo.IncrementTurn();
            Parent.Position += direction;

            foreach (var entity in Parent.CurrentMap!.Entities.Items)
            {
                var ai = entity.GoRogueComponents.GetFirstOrDefault<DemoEnemyAI>();
                ai?.TakeTurn();
            }
        }
        public void PrintNearbyEntities()
        {
                RogueLikeEntity checkEntity = Parent.CurrentMap.GetEntityAt<RogueLikeEntity>(Parent.Position.X + 1, Parent.Position.Y);
                if(checkEntity == null) return;
                Program.GameScreen.MessageLog.AddMessage(checkEntity.Name);
        }
    }
}
