using Godot;

partial class PuzzleDataRes : Resource
{

    [Export]
    int Id;
    [Export]
    Vector2 FrameDimensions;
    [Export]
    Godot.Collections.Array<int> Scramble;
    [Export]
    Godot.Collections.Array<int> NullIds;
    
    public PuzzleDataRes()
    {
        Id = 0;
        FrameDimensions = new Vector2(0, 0);
        Scramble = new Godot.Collections.Array<int>();
        NullIds =  new Godot.Collections.Array<int>();
    }
   
    public PuzzleDataRes(int id, Vector2 frameDimensions, Godot.Collections.Array<int> scramble, Godot.Collections.Array<int> nullIds)
    {
        Id = id;
        FrameDimensions = frameDimensions;
        Scramble = scramble;
        NullIds =  nullIds;
    }   
}

