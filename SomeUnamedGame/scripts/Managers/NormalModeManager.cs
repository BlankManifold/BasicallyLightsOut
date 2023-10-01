using Godot;
using GArray = Godot.Collections.Array;
using GArrayInt = Godot.Collections.Array<int>;

namespace Managers
{
    public partial class NormalModeManager : PuzzleManager
    {
        [Export]
        private Resource _puzzleDataRes;
        public Resource PuzzleDataRes { get { return _puzzleDataRes; } set { _puzzleDataRes = value; } }

        public override void _Ready()
        {
            base._Ready();
        }

        public void InitPuzzle(Resource puzzleDataRes, int targetColorId = 0)
        {
            _puzzleDataRes = puzzleDataRes;

            Vector2 frameDimensions = (Vector2)_puzzleDataRes.Get("FrameDimensions");
            GArrayInt scramble = new GArrayInt((GArray)_puzzleDataRes.Get("Scramble")).Duplicate();
            GArrayInt nullIds = new GArrayInt((GArray)_puzzleDataRes.Get("NullIds")).Duplicate();

            base.InitPuzzle(frameDimensions, targetColorId, scramble, nullIds, null);
        }


        public bool ModifyPiece(int id) => _sequence.ModifyPiece(id);
        public GArrayInt CreateNullIds()
        {
            foreach (int id in _sequence.CreateNullIds())
            {
                _nullIds.Add(id);
            }
            return _nullIds;
        }

    }
}