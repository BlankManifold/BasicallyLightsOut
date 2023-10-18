using Godot;

public partial class PuzzleDataRes : Resource
{

    [Export]
    public int Id;
    [Export]
    public Vector2 FrameDimensions;
    [Export]
    public Godot.Collections.Array<int> Scramble;
    [Export]
    public Godot.Collections.Array<int> NullIds;
    
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
    public void Init(int id, Vector2 frameDimensions, Godot.Collections.Array<int> scramble, Godot.Collections.Array<int> nullIds)
    {
        Id = id;
        FrameDimensions = frameDimensions;
        Scramble = scramble;
        NullIds =  nullIds;
    }   
}

