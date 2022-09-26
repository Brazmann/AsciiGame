/*using GoRogue.FOV;
using GoRogue.GameFramework;
using SadRogue.Integration;
using SadRogue.Primitives;
using System.Diagnostics;

class Actor : RogueLikeEntity
{
    public IFOV FOV { get; set; }
    public int FOVRadius { get; }

    public Actor(Color color, int glyph, bool walkable, bool transparent, int layer, int fovRadius)
        : base(color, glyph, walkable, transparent, layer)
    {
        FOVRadius = fovRadius;

        AddedToMap += OnAddedToMap;
        RemovedFromMap += OnRemovedFromMap;
        Moved += OnMoved;
    }

    private void OnAddedToMap(object? s, GameObjectCurrentMapChanged e)
    {
        FOV = new RecursiveShadowcastingFOV(e.Map.TransparencyView);
    }
    private void OnRemovedFromMap(object? s, GameObjectCurrentMapChanged e)
    {
        FOV = null;
    }

    private void OnMoved(object? s, GameObjectPropertyChanged<Point> e)
    {
        //Debug.WriteLine("Hey kid get outta here!");
        //if (CurrentMap != null)
        //FOV.Calculate(Position, FOVRadius, CurrentMap.DistanceMeasurement);
    }
}*/