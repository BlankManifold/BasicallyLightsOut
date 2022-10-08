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
        private int[] _currentConfiguration;
        private int _numberOfSolvedPieces = 0;

        [Signal]
        delegate void Solved();
        [Signal]
        delegate void Moved();



        public override void _Ready()
        {
            Managers.PuzzleManager puzzleManager = GetParent<Managers.PuzzleManager>();
            Connect(nameof(Solved), puzzleManager, "_on_SequenceTypeA_Solved");
            Connect(nameof(Moved), puzzleManager, "_on_SequenceTypeA_Moved");
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
        public void SetStartConfiguration(int[] startConfiguration)
        {
            _currentConfiguration = (int[])startConfiguration.Clone();
        }
        public void CreateFromCreationSequence(int[] creationSeq, ref int[] startConfiguration)
        {
            foreach (int id in creationSeq)
            {
                _piecesDict[id].Flip(true);
            }

            startConfiguration = (int[])_currentConfiguration.Clone();
        }

        private bool IsSolved()
        {
            return _numberOfPieces == _numberOfSolvedPieces;
        }
        public void Restart(int[] startConfiguration)
        {
            _numberOfSolvedPieces = 0;
            _currentConfiguration = (int[])startConfiguration.Clone();;
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