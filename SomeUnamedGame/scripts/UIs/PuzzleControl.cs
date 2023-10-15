using Godot;

namespace UIs
{
    public partial class PuzzleControl : Control
    {
        public void OnPuzzleUIChangeVisible(bool visible)
        {
            Visible = visible;
        }
    }
}
