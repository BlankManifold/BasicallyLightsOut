using Godot;

namespace DevTools
{
    public class PuzzleCreationUI : Control
    {
        private Label _statusLabel;
        private Label _frameLabel;

        [Signal]
        delegate void FrameDimChanged(int value, string name);

        public override void _Ready()
        {
            Visible = false;
            PuzzleCreationManager creationManager = GetNode<PuzzleCreationManager>("PuzzleCreationManager");
            _statusLabel = GetNode<Label>("StatusLabel");
            _frameLabel = GetNode<Label>("FrameLabel");

            foreach (Button button in GetNode("Buttons").GetChildren())
            {
                string targetMethod = $"_on_PuzzleCreation_button_down";
                button.Connect("button_down", creationManager, targetMethod, new Godot.Collections.Array { button.Name });
            }

            Connect("FrameDimChanged", creationManager, "_on_PuzzleCreationUI_FrameDimChanged");
        }

        public void UpdateStatus(string status)
        {
            _statusLabel.Text = "Status:" + status;
        }
        public void UpdateFrameLabel(Vector2 frameDimension)
        {
            _frameLabel.Text = $"Framedim: {frameDimension}";
        }
        public void InitField(Vector2 frameDimension)
        {
            ((SpinBox)FindNode("RowsField")).Value = frameDimension[0];
            ((SpinBox)FindNode("ColsField")).Value = frameDimension[1];
        }

        public void _on_RowsField_value_changed(float value)
        {
            EmitSignal(nameof(FrameDimChanged), (int)value, "RowsField");
        }
        public void _on_ColsField_value_changed(float value)
        {
            EmitSignal(nameof(FrameDimChanged), (int)value, "ColsField");
        }


    }
}
