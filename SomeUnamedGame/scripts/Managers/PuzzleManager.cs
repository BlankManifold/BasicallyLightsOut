using Godot;

using PuzzlePieces;
using GArrayInt = Godot.Collections.Array<int>;

namespace Managers
{

    public partial class PuzzleManager : Node2D
    {
        #region Members
        protected SequenceTypeA _sequence;
        protected PackedScene _piecesScene;
        protected int _targetColorId = 0;
        public int TargetColorId { get { return _targetColorId; } }
        protected Vector2 _startPosition = new Vector2(0, 0);
        public Vector2 StartPosition { get { return _startPosition; } }
        protected Vector2 _pieceExtents = new Vector2(100, 100);
        protected Vector2 _separation = new Vector2(1, 1);


        private int _movesCounter = 0;
        public int MovesCounter
        {
            get { return _movesCounter; }
            set
            {
                _movesCounter = value;
                EmitSignal(SignalName.ChangedMovesCounter, _movesCounter);
            }
        }



        [Export]
        protected Vector2 _frameDimensions = new Vector2(0, 0);
        public Vector2 FrameDimensions { get { return _frameDimensions; } }

        [Export]
        protected GArrayInt _startConfiguration;
        public GArrayInt StartConfiguration { get { return _startConfiguration; } set { _startConfiguration = value; } }

        [Export]
        protected GArrayInt _scramble = new GArrayInt() { };
        public GArrayInt Scramble { get { return _scramble; } set { _scramble = value; } }

        [Export]
        protected GArrayInt _nullIds;
        public GArrayInt NullIds { get { return _nullIds; } }

        public GArrayInt CurrentConfiguration { get { return _sequence.CurrentConfiguration; }}
        #endregion

        #region Signals and delegates
        [Signal]
        public delegate void ChangedMovesCounterEventHandler(int movesCounter);
        [Signal]
        public delegate void SolvedEventHandler();

        public delegate Globals.EntropyManager.ResultArray convolutionMethod(GArrayInt A, int[] dims);
        #endregion


        public override void _Ready()
        {
            _piecesScene = ResourceLoader.Load<PackedScene>(Globals.Paths.GetPiecesScenePath(Globals.PiecesType.A));
            
            _sequence = GetNode<SequenceTypeA>("%SequenceTypeA");
            _sequence.AddPiece += OnSequenceTypeAAddPiece;
            _sequence.Solved += OnSequenceTypeASolved;
            _sequence.Moved += OnSequenceTypeAMoved;
        }


        #region Public puzzle main handler-methods
        public void InitPuzzle(Vector2 frameDimension, int targetColorId, GArrayInt scramble, GArrayInt nullIds, GArrayInt configuration = null)
        {
            GlobalPosition = Vector2.Zero;
            MovesCounter = 0;

            _targetColorId = targetColorId;
            _frameDimensions = frameDimension;
            _scramble = scramble ?? new GArrayInt() { };
            _nullIds = nullIds ?? new GArrayInt() { };

            AdjustPiecesSize();

            if (configuration != null)
                _startConfiguration = configuration;
            else
                SetupStartConfiguration();

            _sequence.InitSequence(_startConfiguration, _targetColorId);
        }
        public virtual void CreateSequence(bool fromScramble = true)
        {
            GeneratePieces();

            if (fromScramble)
                _sequence.CreateFromScramble(_scramble, ref _startConfiguration);
        }
        public void Restart()
        {
            MovesCounter = 0;
            _sequence.Restart(_startConfiguration);
        }
        public void Clean()
        {
            MovesCounter = 0;
            _sequence.FillConfiguration();
            _sequence.Restart(_startConfiguration);
        }
        public virtual void ClearAll()
        {
            _scramble.Clear();
            _nullIds.Clear();
            _sequence.ClearAll();
        }
        #endregion

        #region Private and protected methods 
        private void AdjustPiecesSize()
        {
            int pieceExtents = Globals.GridInfoManager.GetMaxTypeAExtents(_frameDimensions, _separation);
            _pieceExtents = new Vector2(pieceExtents, pieceExtents);
            _startPosition = Globals.GridInfoManager.GetStartPosition(_frameDimensions, _pieceExtents, _separation);
        }
        private void AddNeighbours(int id, GArrayInt neighbours)
        {
            if (_startConfiguration[id] != -1)
            {
                neighbours.Add(id);
            }
        }
        protected GArrayInt GetNeighbours(int id)
        {
            GArrayInt neighbours = new GArrayInt() { };
            int[] coords = Globals.Utilities.IdToCoords(id, _frameDimensions);
            int testId = (int)id;

            if (coords[0] > 0)
                AddNeighbours(id - (int)_frameDimensions[1], neighbours);

            if (coords[1] > 0)
                AddNeighbours(id - 1, neighbours);

            if (coords[0] < _frameDimensions[0] - 1)
                AddNeighbours(id + (int)_frameDimensions[1], neighbours);

            if (coords[1] < _frameDimensions[1] - 1)
                AddNeighbours(id + 1, neighbours);

            return neighbours;
        }
        private void SetupStartConfiguration()
        {
            _startConfiguration = new GArrayInt { };
            _startConfiguration.Resize((int)(_frameDimensions[0] * _frameDimensions[1]));
            for (int i = 0; i < _startConfiguration.Count; i++)
            {
                _startConfiguration[i] = 0;
            }

            foreach (int invalidId in _nullIds)
            {
                _startConfiguration[invalidId] = -1;
            }
        }
        protected virtual void GeneratePieces()
        {
            Vector2 position = _startPosition;
            Vector2 shiftx = new Vector2(2 * _pieceExtents[0] + _separation[0], 0);
            Vector2 shifty = new Vector2(0, 2 * _pieceExtents[1] + _separation[1]);

            int id = 0;
            int colorId = 0;

            for (int i = 0; i < _frameDimensions[0]; i++)
            {
                for (int j = 0; j < _frameDimensions[1]; j++)
                {
                    colorId = _startConfiguration[id];
                    if (colorId != -1)
                    {
                        if (colorId == _targetColorId)
                            _sequence.NumberOfSolvedPieces++;

                        position = _startPosition + j * shiftx + i * shifty;

                        BasePiece piece = _piecesScene.Instantiate<BasePiece>();
                        piece.Init(id, colorId, position, 2*_pieceExtents);

                        _sequence.UpdateNeighboursDict(id, GetNeighbours(id));
                        _sequence.AddPieceToSequence(id, piece);
                    }
                    else
                    {
                        _sequence.NumberOfPieces--;
                    }
                    id++;
                }
            }
        }       
        #endregion

        #region Public code and conv methods       
        public void SetBinaryCode(GArrayInt config) => _sequence.Code = Globals.Utilities.CreateBinaryCode(config);
        public string GetBinaryCode() => _sequence.Code;
        public void CreateConvolution(convolutionMethod method)
        {
            Globals.EntropyManager.ResultArray result = method(_sequence.CurrentConfiguration, new int[2] { (int)_frameDimensions[0], (int)_frameDimensions[1] });
            InitPuzzle(result.Dims, _targetColorId, null, null, result.Array);
            CreateSequence(false);
        }
        #endregion

        #region Events to signal
        public void OnSequenceTypeAAddPiece(int id)
        {
            Vector2 pos = Globals.GridInfoManager.FromIdToPos(id, _frameDimensions, _startPosition);
            BasePiece piece = _piecesScene.Instantiate<BasePiece>();
            piece.Init(id, 0, pos, _pieceExtents);

            _sequence.UpdateNeighboursDict(id, GetNeighbours(id));
            _sequence.AddPieceToSequence(id, piece);
        }
        public void OnSequenceTypeASolved()
        {
            _sequence.NumberOfSolvedPieces = 0;
            EmitSignal(SignalName.Solved);
        }
        public void OnSequenceTypeAMoved() => MovesCounter = MovesCounter + 1;
        #endregion
    }
}