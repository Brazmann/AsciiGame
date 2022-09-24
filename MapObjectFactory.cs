using SadRogue.Integration;
using SadRogue.Integration.FieldOfView.Memory;
using SadRogue.Integration.Keybindings;
using SadRogue.Primitives;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using GoRogue.FOV;

namespace AsciiGame
{
    /// <summary>
    /// Simple class with some static functions for creating map objects.
    /// </summary>
    /// <remarks>
    /// CUSTOMIZATION:  This demonstrates how to create objects based on "composition"; using components.  The integration
    /// library offers a robust component system that integrates both SadConsole's and GoRogue's components into one
    /// interface to support this.  You can either add more functions to create more objects, or remove this and
    /// implement the "factory" system in the GoRogue.Factories namespace, which provides a more robust interface for it.
    ///
    /// Note that SadConsole components cannot be attached directly to `RogueLikeCell` or `MemoryAwareRogueLikeCell`
    /// instances for reasons pertaining to performance.
    ///
    /// Alternatively, you can remove this system and choose to use inheritance to create your objects instead - the
    /// integration library also supports creating subclasses or RogueLikeCell and RogueLikeEntity.
    /// </remarks>
    internal static class MapObjectFactory
    {
        public static string gamePath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        public static Dictionary<string, Color> Colors = new Dictionary<string, Color>()
        {
            {"bruh", Color.Green }
        };
        public static MemoryAwareRogueLikeCell Grass(Point position)
        {
            var rnd = new Random();

            switch (rnd.Next(0, 13))
            {
                case 0:
                case 1:
                case 2:
                case 3:
                    return new(position, Color.Green, Color.TransparentBlack, ',', (int)MyGameMap.Layer.Terrain);
                case 4:
                case 5:
                case 6:
                case 7:
                    return new(position, Color.Green, Color.TransparentBlack, '.', (int)MyGameMap.Layer.Terrain);
                case 8:
                case 9:
                case 10:
                case 11:
                    return new(position, Color.Green, Color.TransparentBlack, 39, (int)MyGameMap.Layer.Terrain);
                case 12:
                    //return new(position, Color.AnsiGreen, Color.TransparentBlack, 'T', (int)MyGameMap.Layer.Terrain, false, false);
                    return new(position, Color.Green, Color.TransparentBlack, '.', (int)MyGameMap.Layer.Terrain);
                default:
                    return new(position, Color.HotPink, Color.AnsiBlack, '#', (int)MyGameMap.Layer.Terrain);
            }

            /*if(rnd.Next(0, 2) == 1){
                return new(position, Color.Green, Color.Black, '.', (int)MyGameMap.Layer.Terrain);
            } else
            {
                return new(position, Color.White, Color.Black, '.', (int)MyGameMap.Layer.Terrain);
            }*/
        }

        public static MemoryAwareRogueLikeCell Wall(Point position)
            => new(position, Color.White, Color.Black, '#', (int)MyGameMap.Layer.Terrain, false, false);

        public static RogueLikeEntity Player()
        {
            // Create entity with appropriate attributes
            var player = new RogueLikeEntity('@', false, layer: (int)MyGameMap.Layer.Monsters);
            player.Name = "Player";
            // Add component for controlling player movement via keyboard.
            var motionControl = new CustomPlayerKeybindingsComponent();
            motionControl.SetMotions(PlayerKeybindingsComponent.ArrowMotions);
            motionControl.SetMotions(PlayerKeybindingsComponent.NumPadAllMotions);
            player.AllComponents.Add(motionControl);
            // Add component for updating map's player FOV as they move
            player.AllComponents.Add(new PlayerFOVController());

            return player;
        }

        public static string monsterJson = File.ReadAllText(@$"{gamePath}\data\json\Monsters\Monsters.json");
        public static RogueLikeEntity Enemy()
        {

            var monsters = JsonConvert.DeserializeObject<Dictionary<string, Monster>>(monsterJson).ToList(); 
            var rnd = new Random();
            var roll = rnd.Next(0, monsters.Count);

            var ID = monsters[roll].Key;
            var Name = monsters[roll].Value.Name;
            var Character = monsters[roll].Value.Character;
            var MonsterColor = monsters[roll].Value.Color;
            var Health = monsters[roll].Value.Health;
            var ArmorClass = monsters[roll].Value.ArmorClass;
            var Description = monsters[roll].Value.Description;

            var ParsedColor = ColorExtensions2.FromName(MonsterColor);
            var Transparency = monsters[roll].Value.Transparent;

            var enemy = new Actor(ParsedColor, Character, false, Transparency, layer: (int)MyGameMap.Layer.Monsters, 100);
            enemy.AllComponents.Add(new ActorStats());
            var stats = enemy.GoRogueComponents.GetFirstOrDefault<ActorStats>();
            enemy.Name = ID;
            stats.Name = Name;
            stats.Character = Character;
            stats.Health = Health;
            stats.ArmorClass = ArmorClass;
            stats.Description = Description;

            enemy.AllComponents.Add(new EnemyAI());
            
              
            return enemy;
        }
    }

    public class Monster
    {
        public string Name { get; set; }
        public char Character { get; set; }
        public string Color { get; set; }
        public bool Transparent { get; set;}
        public int Health { get; set; }
        public int ArmorClass { get; set; }
        public string Description { get; set; }
    }
    public class Monsters
    {
        public List<Monster> monster { get; set; }
    }
}
