using System.Collections.Generic;
using System;
using Godot;

namespace PuzzlePieces
{

    public class SequenceTypeA : Node2D
    {
        private int _id = 0;
        private int _numberOfPieces = 0;
        private int _targetColorId = 0;

        private Godot.Collections.Dictionary<int, Godot.Collections.Array<int>> _neighboursDict = new Godot.Collections.Dictionary<int, Godot.Collections.Array<int>>() { };
        private Godot.Collections.Dictionary<int, BasePiece> _piecesDict = new Godot.Collections.Dictionary<int, BasePiece>() { };
        private Godot.Collections.Array<int> _currentConfiguration;
        public Godot.Collections.Array<int> CurrentConfiguration { get { return _currentConfiguration; } }
        private int _numberOfSolvedPieces = 0;

        [Signal]
        delegate void Solved();
        [Signal]
        delegate void Moved();
        [Signal]
        delegate void AddPiece(int id);



        public override void _Ready()
        {
            Managers.PuzzleManager puzzleManager = GetParent<Managers.PuzzleManager>();
            Connect(nameof(Solved), puzzleManager, "_on_SequenceTypeA_Solved");
            Connect(nameof(Moved), puzzleManager, "_on_SequenceTypeA_Moved");
            Connect(nameof(AddPiece), puzzleManager, "_on_SequenceTypeA_AddPiece");
        }



        public void UpdateNeighboursDict(int id, Godot.Collections.Array<int> neighbours)
        {
            _neighboursDict.Add(id, neighbours);
        }
        public void UpdatePieceDict(int id, BasePiece piece)
        {
            _piecesDict.Add(id, piece);
        }
        public void UpdateNumberOfSolvedPieces(bool solved = true)
        {
            if (solved)
            {
                _numberOfSolvedPieces++;
                return;
            }

            _numberOfSolvedPieces--;
        }
        public void UpdateNumberOfPieces(int numberOfPieces)
        {
            _numberOfPieces = numberOfPieces;
        }
        public void SetStartConfiguration(Godot.Collections.Array<int> startConfiguration)
        {
            _currentConfiguration = (Godot.Collections.Array<int>)startConfiguration.Duplicate();
        }
        public void CreateFromCreationSequence(Godot.Collections.Array<int> scramble, ref Godot.Collections.Array<int> startConfiguration)
        {
            foreach (int id in scramble)
            {
                _piecesDict[id].Flip(true);
            }

            startConfiguration = (Godot.Collections.Array<int>)_currentConfiguration.Duplicate();
        }
        public void ClearAll()
        {
            foreach (Node piece in GetChildren())
            {
                piece.QueueFree();
            }

            _neighboursDict.Clear();
            _piecesDict.Clear();
            _numberOfSolvedPieces = 0;
            _numberOfPieces = 0;
        }
        public bool ModifyPiece(int id)
        {
            if (_piecesDict.ContainsKey(id))
            {
                BasePiece piece = _piecesDict[id];

                float alpha = 0f;
                if (!piece.IsActive)
                    alpha = 1f;

                _piecesDict[id].Modulate = new Color(0f, 0f, 0f, alpha);
                piece.IsActive = !piece.IsActive;

                return piece.IsActive;
            }
            else
            {
                EmitSignal(nameof(AddPiece),id);
                return true;
            }

        }
        public Godot.Collections.Array<int> CreateNullIds()
        {
            Godot.Collections.Array<int> nullIds = new Godot.Collections.Array<int>() { };
            foreach (int keys in _piecesDict.Keys)
            {
                if (!_piecesDict[keys].IsActive)
                {
                    nullIds.Add(keys);
                }
            }

            return nullIds;
        }
        private bool IsSolved()
        {
            return _numberOfPieces == _numberOfSolvedPieces;
        }
        public void Restart(Godot.Collections.Array<int> startConfiguration)
        {
            _numberOfSolvedPieces = 0;
            _currentConfiguration = (Godot.Collections.Array<int>)startConfiguration.Duplicate();
            foreach (int id in _piecesDict.Keys)
            {
                _piecesDict[id].SetColor(startConfiguration[id]);
            }
        }


        public void _on_BasePiece_Flipping(int id, int colorId, bool isSetup)
        {
            _currentConfiguration[id] = colorId;
            _numberOfSolvedPieces += Globals.ColorManager.CheckColor(colorId, _targetColorId);

            foreach (int neighbourId in _neighboursDict[id])
            {
                colorId = _piecesDict[neighbourId].SelfFlip();
                _currentConfiguration[neighbourId] = colorId;
                _numberOfSolvedPieces += Globals.ColorManager.CheckColor(colorId, _targetColorId);
            }

            if (!isSetup)
                EmitSignal(nameof(Moved));

            if (IsSolved())
                EmitSignal(nameof(Solved));
        }

    }
}