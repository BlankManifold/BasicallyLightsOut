using Godot;

namespace UIs
{
    public partial class LevelSelectionButton : TextureButton
    {
        private Label _label;

        public override void _Ready()
        {
            _label = GetNode<Label>("Label");
        }

        public void UpdateLabelText(string text)
        {
            _label.Text = text;
        }
        public void UpdateButtonSize(Vector2 size)
        {
           Size = size;
        }

    }
}
