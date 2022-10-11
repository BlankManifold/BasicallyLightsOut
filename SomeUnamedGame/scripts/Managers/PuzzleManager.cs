using Godot;
using PuzzlePieces;
using System;

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
        private Vector2 _frameDimension = new Vector2(0, 0);
        public Vector2 FrameDimension { get { return _frameDimension; } }

        [Export]
        private Godot.Collections.Array<int> _startConfiguration;
        public Godot.Collections.Array<int> StartConfiguration { get { return _startConfiguration; } }

        [Export]
        private Godot.Collections.Array<int> _scramble;
        public Godot.Collections.Array<int> CreationSeq { get { return _scramble; } }

        [Export]
        private Godot.Collections.Array<int> _nullIds;
        public Godot.Collections.Array<int> NullIds { get { return _nullIds; } }

        [Signal]
        delegate void ChangedMovesCounter(int movesCounter);



        public void InitSequence(Vector2 frameDimension, int targetColorId = 0, Godot.Collections.Array<int> configuration = null, Godot.Collections.Array<int> scramble = null, Godot.Collections.Array<int> nullIds = null)
        {
            GlobalPosition = Vector2.Zero;

            _targetColorId = targetColorId;
            _frameDimension = frameDimension;

            if (configuration != null)
            {
                _startConfiguration = configuration;
            }
            else
            {

                _startConfiguration = new Godot.Collections.Array<int> { };
                _startConfiguration.Resize((int)(_frameDimension[0] * _frameDimension[1]));
                for (int i = 0; i < _startConfiguration.Count; i++)
                {
                    _startConfiguration[i] = 0;
                }

                _nullIds = nullIds;
                foreach (int invalidId in _nullIds)
                {
                    _startConfiguration[invalidId] = -1;
                }
            }

            _sequence.SetStartConfiguration(_startConfiguration);

            Vector2 position = _startPosition;
            Vector2 shiftx = new Vector2(2 * _pieceExtents[0] + _separation[0], 0);
            Vector2 shifty = new Vector2(0, 2 * _pieceExtents[1] + _separation[1]);

            int id = 0;
            int colorId = 0;
            int numberOfPieces = 0;

            for (int i = 0; i < frameDimension[0]; i++)
            {
                for (int j = 0; j < frameDimension[1]; j++)
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

            if (scramble != null)
            {
                _scramble = scramble;
                _sequence.CreateFromCreationSequence(_scramble, ref _startConfiguration);
            }


        }
        public override void _Ready()
        {
            _piecesScene = ResourceLoader.Load<PackedScene>(Globals.Utilities.GetPiecesScenePath(_piecesType));
            _sequence = GetNode<SequenceTypeA>("SequenceTypeA");

            int pieceExtents = Globals.GridInfoManager.GetMaxTypeAExtents(_frameDimension, _separation);
            _pieceExtents = new Vector2(pieceExtents, pieceExtents);
            _startPosition = Globals.GridInfoManager.GetStartPosition(_frameDimension, _pieceExtents, _separation);

            InitSequence(_frameDimension, _targetColorId, null, _scramble, _nullIds);
        }



        private Godot.Collections.Array<int> GetNeighbours(int id)
        {
            Godot.Collections.Array<int> neighbours = new Godot.Collections.Array<int>() { };
            int[] coords = Globals.Utilities.IdToCoords(id, _frameDimension);
            int testId = (int)id;

            if (coords[0] > 0)
            {
                AddNeighbours(id - (int)_frameDimension[1], neighbours);
            }

            if (coords[1] > 0)
            {
                AddNeighbours(id - 1, neighbours);
            }

            if (coords[0] < _frameDimension[0] - 1)
            {
                AddNeighbours(id + (int)_frameDimension[1], neighbours);
            }

            if (coords[1] < _frameDimension[1] - 1)
            {
                AddNeighbours(id + 1, neighbours);
            }

            return neighbours;
        }
        private void AddNeighbours(int id, Godot.Collections.Array<int> neighbours)
        {
            if (_startConfiguration[id] != -1)
            {
                neighbours.Add(id);
            }
        }
        public void Restart()
        {
            MovesCounter = 0;
            _sequence.Restart(_startConfiguration);

        }
        public void CreateSequence(Vector2 frameDimension, Godot.Collections.Array<int> configuration, Godot.Collections.Array<int> scramble, Godot.Collections.Array<int> nullIds)
        {
            MovesCounter = 0;
            _sequence.ClearAll();

            int pieceExtents = Globals.GridInfoManager.GetMaxTypeAExtents(frameDimension, _separation);
            _pieceExtents = new Vector2(pieceExtents, pieceExtents);
            _startPosition = Globals.GridInfoManager.GetStartPosition(frameDimension, _pieceExtents, _separation);

            InitSequence(frameDimension, _targetColorId, null, scramble, nullIds);
        }

        public Godot.Collections.Array<int> CreateNullIds()
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

        public Godot.Collections.Array<int> GetCurrentConfiguration()
        {
            return _sequence.CurrentConfiguration;
        }


        public void _on_SequenceTypeA_AddPiece(int id)
        {
            Vector2 pos = Globals.GridInfoManager.FromIdToPos(id, _frameDimension, _startPosition);
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