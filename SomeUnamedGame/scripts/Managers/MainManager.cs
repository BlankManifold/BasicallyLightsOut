using Godot;


namespace Managers
{
    class MainManager : Node2D
    {
        private PuzzleManager _puzzleManager;
        private UIs.PuzzleUI _puzzleUI;
        private UIs.OptionsUI _optionsUI;

        private CanvasLayer _puzzleLayer;

        private Label _debugLabel;


        public override void _Ready()
        {
            Globals.GridInfoManager.InitAreaSize(GetViewport().GetVisibleRect().Size, new Vector3(50, 100, 100));
            _puzzleUI = GetNode<UIs.PuzzleUI>("%PuzzleUI");
            _optionsUI = GetNode<UIs.OptionsUI>("%OptionsUI");
            _puzzleLayer = GetNode<CanvasLayer>("PuzzleLayer");

            AddPuzzleManager();

            _debugLabel = new Label();
            _puzzleLayer.AddChild(_debugLabel);
            _debugLabel.RectScale = new Vector2(2,2);
        }

        // public override void _PhysicsProcess(float delta)
        // {
        //     _debugLabel.Text = $"Size: {GetViewport().Size} \n OverriderSize: {GetViewport().GetSizeOverride()} \n";
        //     _debugLabel.Text += $"VisibleSize: {GetViewport().GetVisibleRect().Size} \n MousePos: {GetGlobalMousePosition()} \n";
        //     _debugLabel.Text += $"WindowSize: {OS.WindowSize} \n";
            
        // }
        private void AddPuzzleManager()
        {
            PackedScene puzzleManagerScene = ResourceLoader.Load<PackedScene>(Globals.Utilities.GetPuzzleMenagerScenePath());
            _puzzleManager = puzzleManagerScene.Instance<PuzzleManager>();
            GetNode("%PuzzleControl").AddChild(_puzzleManager);
            _puzzleManager.Connect("ChangedMovesCounter", _puzzleUI, "_on_PuzzleManager_ChangedMovesCounter");
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