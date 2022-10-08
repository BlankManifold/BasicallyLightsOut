using Godot;


namespace UIs
{
    class PuzzleUI: Control
    {
        private Label _movesLabel;
        public override void _Ready()
        {
            _movesLabel = GetNode<Label>("MovesLabel");
        }
        public void _on_PuzzleManager_ChangedMovesCounter(int moveCounter)
        {
            _movesLabel.Text = moveCounter.ToString();
        }
    }
}