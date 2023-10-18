using Godot;

namespace UIs
{
    public partial class CreateModeUI : Control
    {
        private Label _statusLabel;
        private Label _frameLabel;
        private Label _scrambleLabel;
        private FileDialog _saveDialog;
        private FileDialog _loadDialog;

        public enum DialogInteraction
        {
            CONFIRMED, HIDE,
        }

        [Signal]
        public delegate void SaveDiagolInteractionEventHandler(string intercationType, string path = null);
        [Signal]
        public delegate void LoadDiagolInteractionEventHandler(string intercationType, string path = null);
        [Signal]
        public delegate void SpinBoxValueChangedEventHandler(string direction, double value);

        public override void _Ready()
        {
            Visible = false;
            _statusLabel = GetNode<Label>("%StatusLabel");
            _frameLabel = GetNode<Label>("%FrameLabel");
            _scrambleLabel = GetNode<Label>("%ScrambleLabel");
            _saveDialog = GetNode<FileDialog>("%SaveDialog");
            _loadDialog = GetNode<FileDialog>("%LoadDialog");
            _saveDialog.CurrentDir = "res://puzzleResource";
            _loadDialog.CurrentDir = "res://puzzleResource";

            _saveDialog.Confirmed += OnSaveDialogConfirmed;
            _loadDialog.Confirmed += OnLoadDialogConfirmed;
            _saveDialog.Canceled += OnSaveDialogHide;
            _loadDialog.Canceled += OnSaveDialogHide;
            GetNode<SpinBox>("%RowsSpinBox").ValueChanged += OnRowsSpinBoxValueChanged;
            GetNode<SpinBox>("%ColsSpinBox").ValueChanged += OnColsSpinBoxValueChanged;
        }

        public void UpdateStatus(string status)
        {
            _statusLabel.Text = "Status:" + status;
        }
        public void UpdateFrameLabel(Vector2 frameDimension)
        {
            _frameLabel.Text = $"Framedim: {frameDimension}";
        }
        public void UpdateScrambleLabel(Godot.Collections.Array<int> scramble)
        {
            _scrambleLabel.Text = $"Scramble: {scramble}";
        }
        public void InitField(Vector2 frameDimension)
        {
            GetNode<SpinBox>("%RowsSpinBox").Value = frameDimension[0];
            GetNode<SpinBox>("%ColsSpinBox").Value = frameDimension[1];
        }
        public void OpenSaveDialog()
        {
            _saveDialog.Popup();
        }
        public void OpenLoadDialog()
        {
            _loadDialog.Popup();
        }


        #region Event to signals
        public void OnRowsSpinBoxValueChanged(double value)
        {
            EmitSignal(SignalName.SpinBoxValueChanged, "row", value);
        }
        public void OnColsSpinBoxValueChanged(double value)
        {
            EmitSignal(SignalName.SpinBoxValueChanged, "col", value);
        }
        public void OnSaveDialogConfirmed()
        {
            EmitSignal(SignalName.SaveDiagolInteraction, "confirmed", _saveDialog.CurrentPath);
        }
        public void OnLoadDialogConfirmed()
        {
            EmitSignal(SignalName.LoadDiagolInteraction, "confirmed", _saveDialog.CurrentPath);
        }
        public void OnSaveDialogHide()
        {
            EmitSignal(SignalName.SaveDiagolInteraction, "hide");
            _saveDialog.CurrentDir = "res://puzzleResource";
        }
        public void OnLoadDialogHide()
        {
            EmitSignal(SignalName.LoadDiagolInteraction, "hide");
            _loadDialog.CurrentDir = "res://puzzleResource";
        }
        #endregion
    }
}
