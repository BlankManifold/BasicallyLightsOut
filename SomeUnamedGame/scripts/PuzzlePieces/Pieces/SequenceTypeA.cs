using Godot;
using GArrayInt = Godot.Collections.Array<int>;

namespace PuzzlePieces
{

    public partial class SequenceTypeA : Node2D
    {
        #region Members 
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
        #endregion

        #region Signals and delegates
        [Signal]
        public delegate void SolvedEventHandler();
        [Signal]
        public delegate void MovedEventHandler();
        [Signal]
        public delegate void AddPieceEventHandler(int id);
        #endregion


        public override void _Ready()
        {

        }


        #region Public methods
        public void InitSequence(GArrayInt startConfiguration, int targetColorId)
        {
            _numberOfPieces = startConfiguration.Count;
            _targetColorId = targetColorId;
            _code = Globals.Utilities.CreateBinaryCode(startConfiguration);

            _numberOfSolvedPieces = 0;

            _currentConfiguration.Resize(_numberOfPieces);
            for (int i = 0; i < _numberOfPieces; i++)
                _currentConfiguration[i] = startConfiguration[i];
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
            piece.Flipping += OnBasePieceFlipping;
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
                EmitSignal(SignalName.AddPiece, id);
                return true;
            }

        }
        public void Flip(int id, bool isSetup = false) => _piecesDict[id].Flip(isSetup);
        #endregion

        #region Private Methods
        private bool IsSolved() => (_numberOfPieces == _numberOfSolvedPieces);
        #endregion

        


        #region Event to signals
        public void OnBasePieceFlipping(int id, int colorId, bool isSetup)
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
                EmitSignal(SignalName.Moved);
                if (IsSolved())
                    EmitSignal(SignalName.Solved);
            }
        }
        #endregion

    }
}