using Godot;

namespace DevTools
{
    public partial class PuzzleCreationUI : Control
    {
        // private Label _statusLabel;
        // private Label _frameLabel;
        // private Label _scrambleLabel;
        // private FileDialog _saveDialog;
        // private FileDialog _loadDialog;
        // private PuzzleCreationManager _creationManager;

        // public override void _Ready()
        // {
        //     Visible = false;
        //     _creationManager = GetNode<PuzzleCreationManager>("PuzzleCreationManager");
        //     _statusLabel = GetNode<Label>("StatusLabel");
        //     _frameLabel = GetNode<Label>("FrameLabel");
        //     _scrambleLabel = GetNode<Label>("ScrambleLabel");
        //     _saveDialog = GetNode<FileDialog>("SaveDialog");
        //     _loadDialog = GetNode<FileDialog>("LoadDialog");
        //     _saveDialog.CurrentDir = "res://puzzleResource";
        //     _loadDialog.CurrentDir = "res://puzzleResource";

        //     foreach (Button button in GetNode("Buttons").GetChildren())
        //     {
        //         string targetMethod = "_on_PuzzleCreation_button_down";
        //         button.Connect("button_down", _creationManager, targetMethod, new Godot.Collections.Array { button.Name });
        //     }
        // }

        // public void UpdateStatus(string status)
        // {
        //     _statusLabel.Text = "Status:" + status;
        // }
        // public void UpdateFrameLabel(Vector2 frameDimension)
        // {
        //     _frameLabel.Text = $"Framedim: {frameDimension}";
        // }
        // public void UpdateScrambleLabel(Godot.Collections.Array<int> scramble)
        // {
        //     _scrambleLabel.Text = $"Scramble: {scramble}";
        // }
        // public void InitField(Vector2 frameDimension)
        // {
        //     ((SpinBox)FindNode("RowsField")).Value = frameDimension[0];
        //     ((SpinBox)FindNode("ColsField")).Value = frameDimension[1];
        // }
        // public void OpenSaveDialog()
        // {
        //     _saveDialog.Popup_();
        // }
        // public void OpenLoadDialog()
        // {
        //     _loadDialog.Popup_();
        // }

        // public void _on_RowsField_value_changed(float value)
        // {
        //     _creationManager.FrameDimensions = new Vector2(value, _creationManager.FrameDimensions[1]);
        //     UpdateFrameLabel(_creationManager.FrameDimensions);
        // }
        // public void _on_ColsField_value_changed(float value)
        // {
        //     _creationManager.FrameDimensions = new Vector2(_creationManager.FrameDimensions[0],value);
        //     UpdateFrameLabel(_creationManager.FrameDimensions);
        // }
        // public void _on_SaveDialog_confirmed()
        // {
        //     _creationManager.Saved(_saveDialog.CurrentPath);
        // }
        // public void _on_LoadDialog_confirmed()
        // {
        //     _creationManager.Loaded(_loadDialog.CurrentPath);
        // }
        // public void _on_SaveDialog_popup_hide()
        // {
        //     _creationManager.Active();
        //     _saveDialog.CurrentDir = "res://puzzleResource";
        // }
        // public void _on_LoadDialog_popup_hide()
        // {
        //     _creationManager.Active();
        //     _loadDialog.CurrentDir = "res://puzzleResource";
        // }

    }
}
