using Godot;
using GArrayInt = Godot.Collections.Array<int>;

namespace PuzzlePieces
{

    public class SequenceTypeA : Node2D
    {
        private string _code = "";
        public string Code { get { return _code; } set { _code = value; }}
        private int _numberOfPieces = 0;
        public int NumberOfPieces { get { return _numberOfPieces; } set { _numberOfPieces = value;}}
        private int _targetColorId = 0;

        private Godot.Collections.Dictionary<int, GArrayInt> _neighboursDict = new Godot.Collections.Dictionary<int, GArrayInt>() { };
        private Godot.Collections.Dictionary<int, BasePiece> _piecesDict = new Godot.Collections.Dictionary<int, BasePiece>() { };
        private GArrayInt _currentConfiguration = new GArrayInt() { };
        public GArrayInt CurrentConfiguration { get { return _currentConfiguration; }}
        private int _numberOfSolvedPieces = 0;
        public int NumberOfSolvedPieces { get { return _numberOfSolvedPieces;} set { _numberOfSolvedPieces = value;} }

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



        public void InitSequence(Managers.PuzzleManager puzzleManager)
        {
            _numberOfPieces = puzzleManager.StartConfiguration.Count;
            _targetColorId = puzzleManager.TargetColorId;
            _code = Globals.Utilities.CreateBinaryCode(puzzleManager.StartConfiguration);

            _numberOfSolvedPieces = 0;

            _currentConfiguration.Resize(_numberOfPieces);
            for (int i = 0; i < _numberOfPieces; i++)
                _currentConfiguration[i] = puzzleManager.StartConfiguration[i];
        }
        public void CreateFromScramble(GArrayInt scramble, ref GArrayInt startConfiguration)
        {
            foreach (int id in scramble)
                _piecesDict[id].Flip(true);

            for (int i = 0; i < _currentConfiguration.Count; i++)
                startConfiguration[i] = _currentConfiguration[i];
        }
        public void CreateFromConfiguration(GArrayInt config)
        {
            Restart(config);
        }
        public void UpdateNeighboursDict(int id, GArrayInt neighbours) => _neighboursDict.Add(id, neighbours);
        public void AddPieceToSequence(int id, BasePiece piece)
        {
            AddChild(piece);
            _piecesDict.Add(id, piece);
        }
        public void ClearAll()
        {
            foreach (Node piece in GetChildren())
                piece.QueueFree();

            _neighboursDict.Clear();
            _piecesDict.Clear();
            _numberOfSolvedPieces = 0;
            _numberOfPieces = 0;
        }
        public void Restart(GArrayInt startConfiguration)
        {
            _numberOfSolvedPieces = 0;
            for (int i = 0; i < startConfiguration.Count; i++)
                _currentConfiguration[i] = startConfiguration[i];

            foreach (int id in _piecesDict.Keys)
            {
                _piecesDict[id].SetColor(startConfiguration[id]);
                _numberOfSolvedPieces += Globals.ColorManager.CheckColor(startConfiguration[id], _targetColorId);
            }
        }
        public void FillConfiguration(int colorId = 0)
        {
            for (int i = 0; i < _currentConfiguration.Count; i++)
                _currentConfiguration[i] = colorId;

            foreach (int id in _piecesDict.Keys)
                _piecesDict[id].SetColor(colorId);
        }


        private bool IsSolved() => (_numberOfPieces == _numberOfSolvedPieces);


        public GArrayInt CreateNullIds()
        {
            GArrayInt nullIds = new GArrayInt() { };
            foreach (int keys in _piecesDict.Keys)
            {
                if (!_piecesDict[keys].IsActive)
                {
                    nullIds.Add(keys);
                }
            }

            return nullIds;
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
                EmitSignal(nameof(AddPiece), id);
                return true;
            }

        }



        public void _on_BasePiece_Flipping(int id, int colorId, bool isSetup)
        {
            _currentConfiguration[id] = colorId;
            _numberOfSolvedPieces += Globals.ColorManager.CheckColorChanging(colorId, _targetColorId);

            foreach (int neighbourId in _neighboursDict[id])
            {
                colorId = _piecesDict[neighbourId].SelfFlip();
                _currentConfiguration[neighbourId] = colorId;
                _numberOfSolvedPieces += Globals.ColorManager.CheckColorChanging(colorId, _targetColorId);
            }

            if (!isSetup)
            {
                EmitSignal(nameof(Moved));
                if (IsSolved())
                    EmitSignal(nameof(Solved));
            }
        }

    }
}