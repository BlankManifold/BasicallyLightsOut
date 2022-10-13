using Godot;
using System;

using PuzzlePieces;
using GArrayInt = Godot.Collections.Array<int>;
using GArray = Godot.Collections.Array;

namespace Managers
{
    public class PuzzleManager : Node2D
    {
        private SequenceTypeA _sequence;
        private Globals.PiecesType _piecesType = Globals.PiecesType.A;
        private PackedScene _piecesScene;
        private int _targetColorId = 0;
        private Vector2 _startPosition = new Vector2(0, 0);
        public Vector2 StartPosition { get { return _startPosition; } }
        private Vector2 _pieceExtents = new Vector2(100, 100);
        private Vector2 _separation = new Vector2(1, 1);


        private int _movesCounter = 0;
        public int MovesCounter
        {
            get { return _movesCounter; }
            set
            {
                _movesCounter = value;
                EmitSignal(nameof(ChangedMovesCounter), _movesCounter);
            }
        }

        [Export]
        private Resource _puzzleDataRes;

        [Export]
        private Vector2 _frameDimensions = new Vector2(0, 0);
        public Vector2 FrameDimensions { get { return _frameDimensions; } }

        [Export]
        private GArrayInt _startConfiguration;
        public GArrayInt StartConfiguration { get { return _startConfiguration; } }

        [Export]
        private GArrayInt _scramble = new GArrayInt() { };
        public GArrayInt Scramble { get { return _scramble; } }

        [Export]
        private GArrayInt _nullIds;
        public GArrayInt NullIds { get { return _nullIds; } }


        [Signal]
        delegate void ChangedMovesCounter(int movesCounter);


        public void InitSequence(int targetColorId)
        {
            GlobalPosition = Vector2.Zero;
            _targetColorId = targetColorId;

            _frameDimensions = (Vector2)_puzzleDataRes.Get("FrameDimensions");
            _scramble = new GArrayInt((GArray)_puzzleDataRes.Get("Scramble"));
            _nullIds = new GArrayInt((GArray)_puzzleDataRes.Get("NullIds"));

            SetUpBaseConfiguration();
            _sequence.SetStartConfiguration(_startConfiguration);
            GeneratePieces();
            GenerateScrambledConfig();
        }
        public void InitSequence(Vector2 frameDimension, int targetColorId, GArrayInt configuration, GArrayInt scramble, GArrayInt nullIds)
        {
            GlobalPosition = Vector2.Zero;

            _targetColorId = targetColorId;
            _frameDimensions = frameDimension;
            _scramble = scramble;
            _nullIds = nullIds;

            if (configuration != null)
            {
                _startConfiguration = configuration;
            }
            else
            {
                SetUpBaseConfiguration();
            }

            _sequence.SetStartConfiguration(_startConfiguration);
            GeneratePieces();
            GenerateScrambledConfig();
        }
        public override void _Ready()
        {
            _piecesScene = ResourceLoader.Load<PackedScene>(Globals.Utilities.GetPiecesScenePath(_piecesType));
            _sequence = GetNode<SequenceTypeA>("SequenceTypeA");

            int pieceExtents = Globals.GridInfoManager.GetMaxTypeAExtents(_frameDimensions, _separation);
            _pieceExtents = new Vector2(pieceExtents, pieceExtents);
            _startPosition = Globals.GridInfoManager.GetStartPosition(_frameDimensions, _pieceExtents, _separation);

            InitSequence(_frameDimensions, _targetColorId, null, _scramble, _nullIds);
        }



        public void Restart(GArrayInt scramble = null)
        {
            MovesCounter = 0;
            if (scramble != null)
            {
                _sequence.FillConfiguration(0);
                _sequence.CreateFromCreationSequence(scramble, ref _startConfiguration);
                return;
            }
            _sequence.Restart(_startConfiguration);
        }
        public void CreateSequence(Vector2 frameDimensions, GArrayInt configuration, GArrayInt scramble, GArrayInt nullIds)
        {
            MovesCounter = 0;
            _sequence.ClearAll();

            int pieceExtents = Globals.GridInfoManager.GetMaxTypeAExtents(frameDimensions, _separation);
            _pieceExtents = new Vector2(pieceExtents, pieceExtents);
            _startPosition = Globals.GridInfoManager.GetStartPosition(frameDimensions, _pieceExtents, _separation);

            InitSequence(frameDimensions, _targetColorId, null, scramble, nullIds);
        }
        public void CreateSequence()
        {
            MovesCounter = 0;
            _sequence.ClearAll();

            Vector2 frameDimensions = (Vector2)_puzzleDataRes.Get("FrameDimensions");

            int pieceExtents = Globals.GridInfoManager.GetMaxTypeAExtents(frameDimensions, _separation);
            _pieceExtents = new Vector2(pieceExtents, pieceExtents);
            _startPosition = Globals.GridInfoManager.GetStartPosition(frameDimensions, _pieceExtents, _separation);

            InitSequence(_targetColorId);
        }
        public GArrayInt CreateNullIds()
        {
            foreach (int id in _sequence.CreateNullIds())
            {
                _nullIds.Add(id);
            }
            return _nullIds;
        }
        public bool ModifyPiece(int id)
        {
            return _sequence.ModifyPiece(id);
        }
        public void FlipManually(int id)
        {
            _sequence.FlipManually(id);
        }
        public GArrayInt GetCurrentConfiguration()
        {
            return _sequence.CurrentConfiguration;
        }
        public void UpdatePuzzleResource(Resource puzzleDataRes)
        {
            _puzzleDataRes = puzzleDataRes;
        }

        private void AddNeighbours(int id, GArrayInt neighbours)
        {
            if (_startConfiguration[id] != -1)
            {
                neighbours.Add(id);
            }
        }
        private GArrayInt GetNeighbours(int id)
        {
            GArrayInt neighbours = new GArrayInt() { };
            int[] coords = Globals.Utilities.IdToCoords(id, _frameDimensions);
            int testId = (int)id;

            if (coords[0] > 0)
            {
                AddNeighbours(id - (int)_frameDimensions[1], neighbours);
            }

            if (coords[1] > 0)
            {
                AddNeighbours(id - 1, neighbours);
            }

            if (coords[0] < _frameDimensions[0] - 1)
            {
                AddNeighbours(id + (int)_frameDimensions[1], neighbours);
            }

            if (coords[1] < _frameDimensions[1] - 1)
            {
                AddNeighbours(id + 1, neighbours);
            }

            return neighbours;
        }
        private void SetUpBaseConfiguration()
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
        private void GeneratePieces()
        {
            Vector2 position = _startPosition;
            Vector2 shiftx = new Vector2(2 * _pieceExtents[0] + _separation[0], 0);
            Vector2 shifty = new Vector2(0, 2 * _pieceExtents[1] + _separation[1]);

            int id = 0;
            int colorId = 0;
            int numberOfPieces = 0;

            for (int i = 0; i < _frameDimensions[0]; i++)
            {
                for (int j = 0; j < _frameDimensions[1]; j++)
                {
                    colorId = _startConfiguration[id];
                    if (colorId != -1)
                    {
                        numberOfPieces++;
                        if (colorId == _targetColorId)
                            _sequence.UpdateNumberOfSolvedPieces();

                        position = _startPosition + j * shiftx + i * shifty;

                        BasePiece piece = _piecesScene.Instance<BasePiece>();
                        piece.Init(id, colorId, position, _pieceExtents);

                        _sequence.AddChild(piece);
                        _sequence.UpdateNeighboursDict(id, GetNeighbours(id));
                        _sequence.UpdatePieceDict(id, piece);
                    }
                    id++;
                }
            }
            _sequence.UpdateNumberOfPieces(numberOfPieces);

        }
        private void GenerateScrambledConfig()
        {
            if (_scramble != null)
            {
                _sequence.CreateFromCreationSequence(_scramble, ref _startConfiguration);
            }
        }



        public void _on_SequenceTypeA_AddPiece(int id)
        {
            Vector2 pos = Globals.GridInfoManager.FromIdToPos(id, _frameDimensions, _startPosition);
            BasePiece piece = _piecesScene.Instance<BasePiece>();
            piece.Init(id, 0, pos, _pieceExtents);

            _sequence.AddChild(piece);
            _sequence.UpdateNeighboursDict(id, GetNeighbours(id));
            _sequence.UpdatePieceDict(id, piece);
        }
        public void _on_SequenceTypeA_Solved()
        {
            GD.Print("Solved in " + _movesCounter + " moves");
        }
        public void _on_SequenceTypeA_Moved()
        {
            MovesCounter = MovesCounter + 1;
        }

    }
}