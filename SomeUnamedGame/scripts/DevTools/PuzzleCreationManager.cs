using Godot;

namespace DevTools
{
    public class PuzzleCreationManager : Node
    {
        private PuzzleCreationUI _puzzleCreationUI;
        private Managers.PuzzleManager _puzzleManager;

        private Vector2 _frameDimensions = new Vector2(0, 0);
        private Vector2 _initialDimensions = new Vector2(0, 0);
        private Vector2 _startPosition;
        private Godot.Collections.Array<int> _startConfiguration;
        private Godot.Collections.Array<int> _scramble = new Godot.Collections.Array<int>() { };
        private Godot.Collections.Array<int> _nullIds = new Godot.Collections.Array<int>() { };
        private Godot.Collections.Array<int> _currentConfiguration;

        private bool _enabled = false;

        enum Status { PLAY = 0, SCRAMBLE = 1, MODIFY_CONFIG = 2 }
        Godot.Collections.Dictionary<Status, string> _statusDict = new Godot.Collections.Dictionary<Status, string>() {
            {Status.PLAY, "PLAY"},{Status.MODIFY_CONFIG, "MODIFY_CONFIG"}, {Status.SCRAMBLE, "SCRAMBLE"} };
        private Status _status = Status.PLAY;


        public void Init(Managers.PuzzleManager puzzleManager)
        {
            _frameDimensions = puzzleManager.FrameDimension;
            _initialDimensions = _frameDimensions;
            _startPosition = puzzleManager.StartPosition;
            _startConfiguration = puzzleManager.StartConfiguration;
            _scramble = puzzleManager.CreationSeq;
            _nullIds = puzzleManager.NullIds;
            _currentConfiguration = puzzleManager.GetCurrentConfiguration();

            _puzzleCreationUI.InitField(_frameDimensions);
            _puzzleCreationUI.UpdateFrameLabel(_frameDimensions);
        }
        public override void _Ready()
        {
            _puzzleCreationUI = GetParent<PuzzleCreationUI>();
            _puzzleManager = (Managers.PuzzleManager)GetTree().GetNodesInGroup("PuzzleManager")[0];
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

            _puzzleManager.CreateSequence(_frameDimensions, null, null, _nullIds);
            _startPosition = _puzzleManager.StartPosition;
            _initialDimensions = _frameDimensions;
        }
        private void ClearAll()
        {
            _scramble = null;
            _nullIds = new Godot.Collections.Array<int>() { };
            _frameDimensions = Vector2.Zero;

            _puzzleCreationUI.UpdateFrameLabel(_frameDimensions);
            _puzzleCreationUI.InitField(_frameDimensions);
            _puzzleManager.CreateSequence(_frameDimensions, null, _scramble, _nullIds);
        }
        private void ModifyConfig(Vector2 clickPosition)
        {
            int id = Globals.GridInfoManager.FromPosToId(clickPosition, _frameDimensions, _startPosition);

            if (id == -1)
                return;

            GetTree().SetInputAsHandled();
            _puzzleManager.ModifyPiece(id);
        }


        public void _on_PuzzleCreation_button_down(string name)
        {
            switch (name)
            {
                case "Clear":
                    ClearAll();
                    break;

                case "Save":
                    break;

                case "Update":
                    UpdateConfiguration();
                    break;

                case "ViewGrid":
                    break;

                default:
                    return;
            }
        }

        public void _on_PuzzleCreationUI_FrameDimChanged(int value, string name)
        {
            switch (name)
            {
                case "RowsField":
                    _frameDimensions[0] = value;
                    break;

                case "ColsField":
                    _frameDimensions[1] = value;
                    break;

                default:
                    return;
            }

            _puzzleCreationUI.UpdateFrameLabel(_frameDimensions);


        }
    }
}