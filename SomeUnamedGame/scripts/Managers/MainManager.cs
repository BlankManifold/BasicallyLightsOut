using Godot;


namespace Managers
{
    class MainManager : Control
    {
        private PuzzleManager _puzzleManager;
        private UIs.PuzzleUI _puzzleUI;
        private UIs.OptionsUI _optionsUI;


        public override void _Ready()
        {
            _puzzleManager = GetNode<PuzzleManager>("PuzzleManager");
            _puzzleUI = GetNode<UIs.PuzzleUI>("PuzzleUI");
            _optionsUI = GetNode<UIs.OptionsUI>("OptionsUI");
        }

        public void _on_RestartButton_button_down()
        {
            _puzzleManager.Restart();
        }

        public void _on_OptionsButton_button_down()
        {

        }
    }
    
}