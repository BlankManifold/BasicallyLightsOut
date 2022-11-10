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


        private Godot.Collections.Dictionary<string, bool> _badConfig4x4Dict = new Godot.Collections.Dictionary<string, bool>() { };

        [Signal]
        delegate void ChangedMovesCounter(int movesCounter);
        [Signal]
        delegate void Solved();

        public delegate Globals.EntropyManager.ResultArray convolutionMethod(GArrayInt A, int[] dims);


        public void InitSequence(int targetColorId)
        {
            GlobalPosition = Vector2.Zero;
            _targetColorId = targetColorId;

            _frameDimensions = (Vector2)_puzzleDataRes.Get("FrameDimensions");
            _scramble = new GArrayInt((GArray)_puzzleDataRes.Get("Scramble")).Duplicate();
            _nullIds = new GArrayInt((GArray)_puzzleDataRes.Get("NullIds")).Duplicate();

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


        public void Clear()
        {
            if (_scramble != null)
                _scramble.Clear();

            if (_nullIds != null)
                _nullIds.Clear();

            _sequence.ClearAll();
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

            InitSequence(frameDimensions, _targetColorId, configuration, scramble, nullIds);
        }
        public void CreateRandomSequence(Vector2 frameDimensions)
        {
            MovesCounter = 0;
            Clear();

            int pieceExtents = Globals.GridInfoManager.GetMaxTypeAExtents(frameDimensions, _separation);
            _pieceExtents = new Vector2(pieceExtents, pieceExtents);
            _startPosition = Globals.GridInfoManager.GetStartPosition(frameDimensions, _pieceExtents, _separation);

            CreateRandomScramble2(frameDimensions);
            InitSequence(frameDimensions, _targetColorId, null, _scramble, null);
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
        public void CreateNewRandomSequence()
        {
            MovesCounter = 0;
            _sequence.FillConfiguration();
            CreateRandomScramble2(_frameDimensions);
            GenerateScrambledConfig();
        }
        public void CreateConvolution(convolutionMethod method)
        {
            Globals.EntropyManager.ResultArray result = method(_sequence.CurrentConfiguration, new int[2] { (int)_frameDimensions[0], (int)_frameDimensions[1] });
            CreateSequence(result.Dims, result.Array, null, null);
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
        private GArrayInt GetNeighbours(int id, Vector2 frameDimensions)
        {
            GArrayInt neighbours = new GArrayInt() { };
            int[] coords = Globals.Utilities.IdToCoords(id, frameDimensions);
            int testId = (int)id;

            if (coords[0] > 0)
            {
                neighbours.Add(id - (int)frameDimensions[1]);
            }

            if (coords[1] > 0)
            {
                neighbours.Add(id - 1);
            }

            if (coords[0] < frameDimensions[0] - 1)
            {
                neighbours.Add(id + (int)frameDimensions[1]);
            }

            if (coords[1] < frameDimensions[1] - 1)
            {
                neighbours.Add(id + 1);
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

            if (_nullIds != null)
            {
                foreach (int invalidId in _nullIds)
                {
                    _startConfiguration[invalidId] = -1;
                }
            }
            else
            {
                _nullIds = new GArrayInt() { };
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

            // GD.Print("Primary: " + Globals.SymmetriesManager.MainDiagonalCheck(_startConfiguration, _frameDimensions).ToString());
            // GD.Print("Secondary: " +  Globals.SymmetriesManager.SecondaryDiagonalCheck(_startConfiguration, _frameDimensions).ToString());
            // GD.Print("Vertical: " +  Globals.SymmetriesManager.VerticalCheck(_startConfiguration, _frameDimensions).ToString());
            // GD.Print("Horizontal: " +  Globals.SymmetriesManager.HorizontalCheck(_startConfiguration, _frameDimensions).ToString());
        }
        private void CreateRandomScramble(Vector2 frameDimensions)
        {

            GArrayInt testConfig = new GArrayInt() { };
            GArrayInt testConfigScrambled = new GArrayInt() { };
            GArrayInt prepScramble = new GArrayInt() { };
            GArrayInt prepTestConfig = new GArrayInt() { };


            int numberOfPieces = (int)(frameDimensions[0] * frameDimensions[1]);

            testConfig.Resize(numberOfPieces);
            testConfigScrambled.Resize(numberOfPieces);
            prepScramble.Resize(numberOfPieces);
            prepTestConfig.Resize(numberOfPieces);
            for (int i = 0; i < numberOfPieces; i++)
            {
                prepScramble[i] = i;
                prepTestConfig[i] = 1;
            }

            GArrayInt toBeRemoved = new GArrayInt() { numberOfPieces - 1, numberOfPieces - (int)frameDimensions[1], (int)frameDimensions[1] - 1, 0 };
            int toBeRemovedLimit = 2;

            for (int i = 0; i < toBeRemovedLimit; i++)
            {
                toBeRemoved.RemoveAt(Globals.RandomManager.rnd.Next(0, toBeRemoved.Count));
            }

            foreach (int idx in toBeRemoved)
            {
                prepScramble.RemoveAt(idx);
                prepTestConfig[idx] = 0;
            }


            int toBeRemovedLength = (int)(numberOfPieces) / 2 - toBeRemoved.Count;
            int scrambleLenght = (int)(numberOfPieces) / 2;
            //GArrayInt test = new GArrayInt() {0,1,1,0,1,0,0,1,0,1,1,0,0,1,1,0};

            do
            {
                _scramble = prepScramble.Duplicate();
                testConfig = prepTestConfig.Duplicate();

                int idToRemove;

                for (int i = 0; i < toBeRemovedLength; i++)
                {
                    idToRemove = Globals.RandomManager.rnd.Next(0, _scramble.Count);
                    testConfig[_scramble[idToRemove]] = 0;
                    _scramble.RemoveAt(idToRemove);
                }

                for (int i = 0; i < numberOfPieces; i++)
                {
                    int cont = testConfig[i];
                    foreach (int id in GetNeighbours(i, frameDimensions))
                    {
                        cont += testConfig[id];
                    }

                    if (cont % 2 == 0)
                        testConfigScrambled[i] = 0;
                    else
                        testConfigScrambled[i] = 1;
                }

            }
            while (
                    //false
                    //Globals.SymmetriesManager.VerticalCheckScramble(test, frameDimensions,scrambleLenght)
                    Globals.SymmetriesManager.MainDiagonalCheck(testConfigScrambled, frameDimensions) ||
                    Globals.SymmetriesManager.MainDiagonalCheckScramble(testConfig, frameDimensions, scrambleLenght) ||
                    Globals.SymmetriesManager.VerticalCheck(testConfigScrambled, frameDimensions) ||
                    Globals.SymmetriesManager.HorizontalCheck(testConfigScrambled, frameDimensions) ||
                    Globals.SymmetriesManager.SecondaryDiagonalCheck(testConfigScrambled, frameDimensions) ||
                    Globals.SymmetriesManager.SecondaryDiagonalCheckScramble(testConfig, frameDimensions, scrambleLenght)
                );

            GetNode<Label>("%TestRoba").Text = $"{_scramble}";

        }
        private void CreateRandomScramble2(Vector2 frameDimensions)
        {
            if (_scramble != null)
                _scramble = new GArrayInt() { };
            else
                _scramble.Clear();

            GArrayInt testConfig = new GArrayInt() { };
            GArrayInt testConfigScrambled = new GArrayInt() { };


            int numberOfPieces = (int)(frameDimensions[0] * frameDimensions[1]);

            testConfig.Resize(numberOfPieces);
            testConfigScrambled.Resize(numberOfPieces);
            for (int i = 0; i < numberOfPieces; i++)
                testConfig[i] = 0;


            // GArrayInt toBeRemoved = new GArrayInt() { numberOfPieces - 1, numberOfPieces - (int)frameDimensions[1], (int)frameDimensions[1] - 1, 0 };
            // int toBeRemovedLimit = 0;
            // for (int i = 0; i < toBeRemovedLimit; i++)
            //     toBeRemoved.RemoveAt(Globals.RandomManager.rnd.Next(0, toBeRemoved.Count));

            // int toBeRemovedLength = (int)(numberOfPieces) / 2 - toBeRemoved.Count;
            int scrambleLenght = (int)(numberOfPieces) / 2;

            do
            {
                _scramble.Clear();
                for (int i = 0; i < numberOfPieces; i++)
                {
                    testConfig[i] = 0;
                }


                for (int row = 0; row < _frameDimensions[0]; row++)
                {
                    int col = Globals.RandomManager.rnd.Next(0, (int)frameDimensions[1]);
                    int id = Globals.Utilities.CoordsToId(row, col, frameDimensions);

                    _scramble.Add(id);
                    testConfig[id] = 1;
                }

                for (int col = 0; col < frameDimensions[1]; col++)
                {
                    int cont = 0;
                    int id, row;
                    do
                    {
                        row = Globals.RandomManager.rnd.Next(0, (int)frameDimensions[0]);
                        id = Globals.Utilities.CoordsToId(row, col, frameDimensions);
                        cont++;
                    } while (_scramble.Contains(id) && cont < frameDimensions[1]);

                    _scramble.Add(id);
                    testConfig[id] = 1;
                }

                for (int i = 0; i < numberOfPieces; i++)
                {
                    int cont = testConfig[i];
                    foreach (int id in GetNeighbours(i, frameDimensions))
                    {
                        cont += testConfig[id];
                    }

                    if (cont % 2 == 0)
                        testConfigScrambled[i] = 0;
                    else
                        testConfigScrambled[i] = 1;
                }

            }
            while (
                    IsBadConfig(frameDimensions,GetBinaryCode(testConfigScrambled)) ||
                    // false
                    //Globals.SymmetriesManager.VerticalCheckScramble(test, frameDimensions,scrambleLenght)
                    Globals.SymmetriesManager.MainDiagonalCheckScramble(testConfig, frameDimensions, scrambleLenght, 1f) ||
                    Globals.SymmetriesManager.SecondaryDiagonalCheckScramble(testConfig, frameDimensions, scrambleLenght, 1f) ||
                    Globals.SymmetriesManager.MainDiagonalCheck(testConfigScrambled, frameDimensions, 1f) ||
                    Globals.SymmetriesManager.SecondaryDiagonalCheck(testConfigScrambled, frameDimensions, 1f) ||
                    Globals.SymmetriesManager.VerticalCheck(testConfigScrambled, frameDimensions, 1f) ||
                    Globals.SymmetriesManager.HorizontalCheck(testConfigScrambled, frameDimensions, 1f)
                );

            GetNode<Label>("%TestRoba").Text = $"{_scramble}";

        }
        private void LoadBadConfig()
        {
            File file = new File();
            file.Open(Globals.Paths.BadConfig4x4path, File.ModeFlags.Read);
            while (!file.EofReached())
            {
                string line = file.GetLine();
                _badConfig4x4Dict[line] = true;
            }
            file.Close();
        }
        private bool IsBadConfig(Vector2 frameDimensions, string code)
        {
            switch (frameDimensions[0])
            {
                case 4:
                    return _badConfig4x4Dict.ContainsKey(code);
            }
            return true;
        }
        public string GetBinaryCode()
        {
            string code = "";
            foreach (int bit in _sequence.CurrentConfiguration)
            {
                code += bit.ToString();
            }
            return code;
        }
        public string GetBinaryCode(GArrayInt config)
        {
            string code = "";
            foreach (int bit in config)
            {
                code += bit.ToString();
            }
            return code;
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
            _sequence.ResetNumberOfSolvedPieces();
            EmitSignal(nameof(Solved));
        }
        public void _on_SequenceTypeA_Moved()
        {
            MovesCounter = MovesCounter + 1;
        }

    }
}