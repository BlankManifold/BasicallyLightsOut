using Godot;
using GArrayInt = Godot.Collections.Array<int>;

namespace Managers
{
    public partial class NormalModeManager : PuzzleManager
    {
        [Export]
        private PuzzleDataRes _puzzleRes;
        public PuzzleDataRes PuzzleRes
        {
            get { return _puzzleRes; }
            set { _puzzleRes = value; }
        }

        public override void _Ready()
        {
            base._Ready();
        }

        public void InitPuzzle(PuzzleDataRes puzzleRes, int targetColorId = 0)
        {
            _puzzleRes = puzzleRes;

            Vector2 frameDimensions = _puzzleRes.FrameDimensions;
            GArrayInt scramble = _puzzleRes.Scramble.Duplicate();
            GArrayInt nullIds = _puzzleRes.NullIds.Duplicate();

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