using Godot;

namespace Managers
{
    public class TimedModeManager : PuzzleManager
    {
        private delegate void ScrambleGeneratorDel(TimedModeManager timedPuzzle);


        private int _dim;
        public int Dim { get { return _dim; } }
        private ScrambleGeneratorDel _scrambleGenerator;
        private ScrambleManager _scrambleManager = new ScrambleManager();



        public override void _Ready()
        {
            base._Ready();
            _dim = (int)_frameDimensions[0];
        }



        public void InitPuzzle(int dim)
        {
            _dim = dim;
            InitPuzzle(new Vector2(dim, dim), 0, null, null, null);
            UpdateScrambleGenerator();
            GeneratePieces();
        }
        public override void CreateSequence(bool fromScramble = true)
        {
            Clean();
            _scrambleGenerator(this);
            _sequence.CreateFromConfiguration(_startConfiguration);
        }



        protected override void GeneratePieces()
        {
            Vector2 position = _startPosition;
            Vector2 shiftx = new Vector2(2 * _pieceExtents[0] + _separation[0], 0);
            Vector2 shifty = new Vector2(0, 2 * _pieceExtents[1] + _separation[1]);

            int id = 0, colorId = 0;

            for (int i = 0; i < _frameDimensions[0]; i++)
            {
                for (int j = 0; j < _frameDimensions[1]; j++)
                {
                    colorId = _startConfiguration[id];

                    if (colorId == _targetColorId)
                        _sequence.NumberOfSolvedPieces++;

                    position = _startPosition + j * shiftx + i * shifty;

                    PuzzlePieces.BasePiece piece = _piecesScene.Instance<PuzzlePieces.BasePiece>();
                    piece.Init(id, colorId, position, _pieceExtents);

                    _sequence.UpdateNeighboursDict(id, GetNeighbours(id));
                    _sequence.AddPieceToSequence(id, piece);

                    id++;
                }
            }

        }
        private void UpdateScrambleGenerator()
        {
            switch (_dim)
            {
                case 4:
                    _scrambleManager.Range = new int[] {1,1};
                    _scrambleGenerator = _scrambleManager.CreateRandomScrambleRC;
                    break;
                case 5:
                    _scrambleManager.Range = new int[] {1,1};
                    _scrambleGenerator = _scrambleManager.CreateRandomScramble;
                    break;
                default:
                    _scrambleManager.Range = new int[] {2,2};
                    _scrambleGenerator = _scrambleManager.CreateRandomScramble;
                    break;
            }
        }



    }
}