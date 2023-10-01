using Godot;

public partial class PuzzleControl : Control
{
    public void _on_PuzzleUI_ChangeVisible(bool visible)
    {
        Visible = visible;
    }
}
