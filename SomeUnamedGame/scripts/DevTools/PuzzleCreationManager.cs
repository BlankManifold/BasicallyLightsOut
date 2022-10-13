using Godot;
using GArray = Godot.Collections.Array;
using GArrayInt = Godot.Collections.Array<int>;

namespace DevTools
{
    public class PuzzleCreationManager : Node
    {
        private PuzzleCreationUI _puzzleCreationUI;
        private Managers.PuzzleManager _puzzleManager;
        private Resource _puzzleDataRes;

        private Vector2 _frameDimensions = new Vector2(0, 0);
        public Vector2 FrameDimensions { get { return _frameDimensions; } set { _frameDimensions = value; } }
        private Vector2 _initialDimensions = new Vector2(0, 0);
        private Vector2 _startPosition;
        private GArrayInt _startConfiguration;
        private GArrayInt _scramble = new GArrayInt() { };
        public GArrayInt Scramble { get { return _scramble; } }
        private GArrayInt _nullIds = new GArrayInt() { };
        public GArrayInt NullIds { get { return _nullIds; } }
        private GArrayInt _currentConfiguration;

        private bool _enabled = false;
        private Status _status = Status.PLAY;


        enum Status { PLAY = 0, SCRAMBLE = 1, MODIFY_CONFIG = 2 }
        Godot.Collections.Dictionary<Status, string> _statusDict = new Godot.Collections.Dictionary<Status, string>()
        {
            {Status.PLAY, "PLAY"},
            {Status.MODIFY_CONFIG, "MODIFY_CONFIG"},
            {Status.SCRAMBLE, "SCRAMBLE"}
        };



        public void Init(Managers.PuzzleManager puzzleManager)
        {
            _frameDimensions = puzzleManager.FrameDimensions;
            _initialDimensions = _frameDimensions;
            _startPosition = puzzleManager.StartPosition;
            _startConfiguration = puzzleManager.StartConfiguration;
            //_scramble = puzzleManager.CreationSeq;
            _nullIds = puzzleManager.NullIds;
            _currentConfiguration = puzzleManager.GetCurrentConfiguration();

            _puzzleCreationUI.InitField(_frameDimensions);
            _puzzleCreationUI.UpdateFrameLabel(_frameDimensions);
        }
        public override void _Ready()
        {
            _puzzleCreationUI = GetParent<PuzzleCreationUI>();
            _puzzleManager = (Managers.PuzzleManager)GetTree().GetNodesInGroup("PuzzleManager")[0];

            GDScript puzzleDataScript = (GDScript)GD.Load("res://scripts/DevTools/PuzzleData.gd");
            _puzzleDataRes = (Resource)puzzleDataScript.New();

        }



        public override void _Input(InputEvent @event)
        {
            if (!_enabled)
            {
                if (@event is InputEventKey keyEvent && keyEvent.IsPressed() && keyEvent.Scancode == (uint)KeyList.D)
                {
                    _enabled = true;
                    _puzzleCreationUI.Visible = true;
                    keyEvent.Dispose();
                    return;
                }
            }
            if (_enabled)
            {
                if (@event is InputEventKey keyEvent && keyEvent.IsPressed())
                {
                    switch (keyEvent.Scancode)
                    {
                        case (uint)KeyList.D:
                            _enabled = !_enabled;
                            _puzzleCreationUI.Visible = _enabled;
                            break;

                        case (uint)KeyList.C:
                            UpdateStatus(Status.SCRAMBLE);
                            break;

                        case (uint)KeyList.M:
                            UpdateStatus(Status.MODIFY_CONFIG);
                            break;

                        case (uint)KeyList.P:
                            UpdateStatus(Status.PLAY);
                            break;

                        default:
                            break;

                    }

                    _puzzleCreationUI.UpdateStatus(_statusDict[_status]);
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
                }

                @event.Dispose();
            }

        }



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
                _nullIds = _puzzleManager.CreateNullIds();
            }

            _puzzleManager.CreateSequence(_frameDimensions, null, _scramble, _nullIds);
            _startPosition = _puzzleManager.StartPosition;
            _initialDimensions = _frameDimensions;

            UpdateStatus(Status.PLAY);
            _puzzleCreationUI.UpdateStatus(_statusDict[_status]);
        }
        private void ClearAll()
        {
            _scramble.Clear();
            _nullIds = new GArrayInt() { };
            _frameDimensions = Vector2.Zero;

            _puzzleCreationUI.UpdateFrameLabel(_frameDimensions);
            _puzzleCreationUI.InitField(_frameDimensions);
            _puzzleManager.CreateSequence(_frameDimensions, null, _scramble, _nullIds);
        }
        private void ClearThis()
        {
            _scramble.Clear();
            _puzzleCreationUI.UpdateScrambleLabel(_scramble);
            _puzzleManager.Restart(_scramble);
        }
        private void ModifyConfig(Vector2 clickPosition)
        {
            int id = Globals.GridInfoManager.FromPosToId(clickPosition, _frameDimensions, _startPosition);

            if (id == -1)
                return;

            GetTree().SetInputAsHandled();
            _puzzleManager.ModifyPiece(id);
        }
        private void ModifyScramble(Vector2 clickPosition)
        {
            int id = Globals.GridInfoManager.FromPosToId(clickPosition, _frameDimensions, _startPosition);

            if (id == -1)
                return;

            GetTree().SetInputAsHandled();
            _scramble.Add(id);
            _puzzleManager.FlipManually(id);
            _puzzleCreationUI.UpdateScrambleLabel(_scramble);
        }
        private void SavePuzzleResource()
        {
            _enabled = false;
            _puzzleCreationUI.OpenSaveDialog();
        }
        private void LoadPuzzleResource()
        {
             _enabled = false;
            _puzzleCreationUI.OpenLoadDialog();
        }
        public void Active()
        {
            _enabled = true;
            _status = Status.PLAY;
        }
        public void Saved(string filePath)
        {
            _puzzleDataRes.Call("init", 0, _frameDimensions, _scramble, _nullIds);
            Godot.Error err = ResourceSaver.Save(filePath, _puzzleDataRes);
            Active();
        }
        public void Loaded(string filePath)
        {
            ClearAll();
            
            Resource _puzzleDataRes = ResourceLoader.Load(filePath);
            _frameDimensions = (Vector2)_puzzleDataRes.Get("FrameDimensions");
            _scramble = new GArrayInt((GArray)_puzzleDataRes.Get("Scramble"));
            _nullIds = new GArrayInt((GArray)_puzzleDataRes.Get("NullIds"));

            UpdateConfiguration();
            _puzzleCreationUI.UpdateFrameLabel(_frameDimensions);
            _puzzleCreationUI.UpdateScrambleLabel(_scramble);
            
            Active();
        }
        



        public void _on_PuzzleCreation_button_down(string name)
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

       
    }
}