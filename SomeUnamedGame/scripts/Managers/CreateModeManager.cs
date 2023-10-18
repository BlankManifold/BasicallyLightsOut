using Godot;
using GArrayInt = Godot.Collections.Array<int>;

namespace Managers
{
    public partial class CreateModeManager : NormalModeManager
    {
        #region Members
        private UIs.CreateModeUI _createModeUI;
        private Vector2 _initialDimensions = new Vector2(0, 0);
        
        // C'è gia in sequence come member di PuzzleMananger
        private GArrayInt _currentConfiguration;


        private bool _enabled = false;
        private Status _status = Status.PLAY;


        enum Status { PLAY, SCRAMBLE, MODIFY_CONFIG }
        Godot.Collections.Dictionary<Status, string> _statusDict = new Godot.Collections.Dictionary<Status, string>()
        {
            {Status.PLAY, "PLAY"},
            {Status.MODIFY_CONFIG, "MODIFY_CONFIG"},
            {Status.SCRAMBLE, "SCRAMBLE"}
        };
        #endregion

        #region Godot funcs override
        public override void _Ready()
        {
            _createModeUI = GetParent<UIs.CreateModeUI>();
            _puzzleRes = new PuzzleDataRes();

            foreach (Button button in _createModeUI.GetNode("%Buttons").GetChildren())
            {
                button.ButtonDown += () => OnCreateModeUIButtonDown(button.Name);
            }

            _createModeUI.SpinBoxValueChanged += OnCreateModeUISpinBoxValueChanged;
            _createModeUI.SaveDiagolInteraction += OnCreateModeUISaveDialogInteraction;
            _createModeUI.LoadDiagolInteraction += OnCreateModeUILoadDialogInteraction;
        }
        public override void _Input(InputEvent @event)
        {
            if (!_enabled)
            {
                if (@event is InputEventKey keyEvent && keyEvent.IsPressed() && keyEvent.Keycode == Key.D)
                {
                    _enabled = true;
                    _createModeUI.Visible = true;
                    keyEvent.Dispose();
                    return;
                }
                @event.Dispose();
                return;
            }
            if (_enabled)
            {
                if (@event is InputEventKey keyEvent && keyEvent.IsPressed())
                {
                    switch (keyEvent.Keycode)
                    {
                        case Key.D:
                            _enabled = !_enabled;
                            _createModeUI.Visible = _enabled;
                            break;

                        case Key.C:
                            UpdateStatus(Status.SCRAMBLE);
                            break;

                        case Key.M:
                            UpdateStatus(Status.MODIFY_CONFIG);
                            break;

                        case Key.P:
                            UpdateStatus(Status.PLAY);
                            break;

                        default:
                            break;

                    }

                    _createModeUI.UpdateStatus(_statusDict[_status]);
                    keyEvent.Dispose();
                    return;
                }

                if (_status != Status.PLAY)
                {
                    if (@event is InputEventMouseButton mousebutton && mousebutton.IsPressed())
                    {
                        switch (_status)
                        {
                            case Status.MODIFY_CONFIG:
                                ModifyConfig(mousebutton.Position);
                                break;

                            case Status.SCRAMBLE:
                                ModifyScramble(mousebutton.Position);
                                break;


                            default:
                                break;
                        }
                        mousebutton.Dispose();
                        return;
                    }
                    @event.Dispose();
                    return;
                }

                @event.Dispose();
                return;
            }

            @event.Dispose();
            return;
        }
        #endregion

        #region Public Methods
        public void InitPuzzle(Vector2 frameDimension, int targetColorId)
        {
            InitPuzzle(frameDimension, targetColorId, null, null, null);
            _initialDimensions = _frameDimensions;
            _currentConfiguration =  _sequence.CurrentConfiguration;

            _createModeUI.InitField(_frameDimensions);
            _createModeUI.UpdateFrameLabel(_frameDimensions);
        }
        public override void ClearAll()
        {
            _scramble.Clear();
            _nullIds = new GArrayInt() { };
            _frameDimensions = Vector2.Zero;

            InitPuzzle(_frameDimensions, _targetColorId);

            _createModeUI.UpdateFrameLabel(_frameDimensions);
            _createModeUI.InitField(_frameDimensions);
        }
        public void Active()
        {
            _enabled = true;
            _status = Status.PLAY;
        }
        public void Saved(string filePath)
        {
            _puzzleRes.Init(0, _frameDimensions, _scramble, _nullIds);
            Error err = ResourceSaver.Save(_puzzleRes, filePath);
            Active();
        }
        public void Loaded(string filePath)
        {
            ClearAll();

            _puzzleRes = ResourceLoader.Load<PuzzleDataRes>(filePath);
            _frameDimensions = _puzzleRes.FrameDimensions;
            _scramble = _puzzleRes.Scramble.Duplicate();
            _nullIds = _puzzleRes.NullIds.Duplicate();

            UpdateConfiguration();
            _createModeUI.UpdateFrameLabel(_frameDimensions);
            _createModeUI.UpdateScrambleLabel(_scramble);

            Active();
        }
        #endregion

        #region Private Methods
        private void UpdateStatus(Status status)
        {
            if (_status != status)
                _status = status;
            else
                _status = Status.PLAY;
        }
        private void UpdateConfiguration()
        {
            if (_initialDimensions != _frameDimensions)
            {
                _nullIds.Clear();
            }
            else
            {
                _nullIds = CreateNullIds();
            }

            InitPuzzle(_frameDimensions, _targetColorId, _scramble, _nullIds, null);
            _initialDimensions = _frameDimensions;

            UpdateStatus(Status.PLAY);
            _createModeUI.UpdateStatus(_statusDict[_status]);
        }
        private void ClearThis()
        {
            _scramble.Clear();
            _createModeUI.UpdateScrambleLabel(_scramble);
            Restart();
        }
        private void ModifyConfig(Vector2 clickPosition)
        {
            int id = Globals.GridInfoManager.FromPosToId(clickPosition, _frameDimensions, _startPosition);

            if (id == -1)
                return;

            GetViewport().SetInputAsHandled();
            ModifyPiece(id);
        }
        private void ModifyScramble(Vector2 clickPosition)
        {
            int id = Globals.GridInfoManager.FromPosToId(clickPosition, _frameDimensions, _startPosition);

            if (id == -1) return;

            GetViewport().SetInputAsHandled();
            _scramble.Add(id);
            _sequence.Flip(id, true);
            _createModeUI.UpdateScrambleLabel(_scramble);
        }
        private void SavePuzzleResource()
        {
            _enabled = false;
            _createModeUI.OpenSaveDialog();
        }
        private void LoadPuzzleResource()
        {
            _enabled = false;
            _createModeUI.OpenLoadDialog();
        }
        #endregion
 
        #region Event to signals
        public void OnCreateModeUIButtonDown(string name)
        {
            switch (name)
            {
                case "Clear":
                    ClearAll();
                    break;

                case "Save":
                    SavePuzzleResource();
                    break;

                case "Load":
                    LoadPuzzleResource();
                    break;

                case "Update":
                    UpdateConfiguration();
                    break;

                case "Reset":
                    ClearThis();
                    break;

                default:
                    return;
            }
        }
        public void OnCreateModeUISpinBoxValueChanged(string direction, double value)
        {
            if (direction == "row")
                _frameDimensions = new Vector2((int)value, _frameDimensions[1]);
            else if (direction == "col")
                _frameDimensions = new Vector2(_frameDimensions[0],(int)value);

            _createModeUI.UpdateFrameLabel(_frameDimensions);
        }
        public void OnCreateModeUISaveDialogInteraction(string interaction, string path)
        {
            if (interaction == "confirmed")
                Saved(path);
            else if (interaction == "hide")
                Active();
        }
        public void OnCreateModeUILoadDialogInteraction(string interaction, string path)
        {
            if (interaction == "confirmed")
                Loaded(path);
            else if (interaction == "hide")
                Active();
        }
        #endregion
    }
}