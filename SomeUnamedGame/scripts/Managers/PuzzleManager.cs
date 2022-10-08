using Godot;
using PuzzlePieces;
using System;

namespace Managers
{
    public class PuzzleManager : Node
    {
        private SequenceTypeA _sequence;
        private Globals.PiecesType _piecesType = Globals.PiecesType.A;
        private PackedScene _piecesScene;
        private int _targetColorId = 0;
        private Vector2 _startPosition = new Vector2(200, 200);
        private Vector2 _pieceExtents = new Vector2(100, 100);


        private int _movesCounter = 0;
        public int MovesCounter
        {
            get {return _movesCounter;}
            set {
                _movesCounter = value;
                EmitSignal(nameof(ChangedMovesCounter),_movesCounter);
                }
        }


        [Export]
        private int[] _frameDimension;
        [Export]
        private int[] _startConfiguration;
        [Export]
        private int[] _creationSeq;
        [Export]
        private int[] _nullIds;

        [Signal]
        delegate void ChangedMovesCounter(int movesCounter);



        public void InitSequence(int[] frameDimension, int targetColorId = 0, int[] configuration = null, int[] creationSeq = null, int[] nullIds = null)
        {
            _targetColorId = targetColorId;
            _frameDimension = frameDimension;

            if (configuration != null)
            {
                _startConfiguration = configuration;
            }
            else
            {

                _startConfiguration = new int[] { };
                Array.Resize(ref _startConfiguration, _frameDimension[0] * _frameDimension[1]);
                for (int i = 0; i < _startConfiguration.Length; i++)
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
            Vector2 shiftx = new Vector2(2 * _pieceExtents[0] + 5, 0);
            Vector2 shifty = new Vector2(0, 2 * _pieceExtents[1] + 5);

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
                        AddChild(piece);

                        _sequence.UpdateNeighboursDict(id, GetNeighbours(id));
                        _sequence.UpdatePieceDict(id, piece);
                    }
                    id++;
                }
            }
            _sequence.UpdateNumberOfPieces(numberOfPieces);

            if (creationSeq != null)
            {
                _creationSeq = creationSeq;
                _sequence.CreateFromCreationSequence(_creationSeq, ref _startConfiguration);
            }


        }
        public override void _Ready()
        {
            _piecesScene = ResourceLoader.Load<PackedScene>(Globals.Utilities.GetPiecesScenePath(_piecesType));
            _sequence = GetNode<SequenceTypeA>("SequenceTypeA");
            InitSequence(_frameDimension, _targetColorId, null, _creationSeq, _nullIds);
        }



        private Godot.Collections.Array<int> GetNeighbours(int id)
        {
            Godot.Collections.Array<int> neighbours = new Godot.Collections.Array<int>() { };
            int[] coords = Globals.Utilities.IdToCoords(id, _frameDimension);
            int testId = (int)id;

            if (coords[0] > 0)
            {
                AddNeighbours(id - _frameDimension[1], neighbours);
            }

            if (coords[1] > 0)
            {
                AddNeighbours(id - 1, neighbours);
            }

            if (coords[0] < _frameDimension[0] - 1)
            {
                AddNeighbours(id + _frameDimension[1], neighbours);
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



        public void _on_SequenceTypeA_Solved()
        {
            GD.Print("Solved in " + _movesCounter + " moves");
        }
        public void _on_SequenceTypeA_Moved()
        {
            MovesCounter = MovesCounter+1;
        }

    }
}