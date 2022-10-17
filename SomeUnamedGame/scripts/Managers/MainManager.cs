using Godot;


namespace Managers
{
    class MainManager : Node2D
    {
        [Export]
        private bool _devToolsEnabled = false;
        private PuzzleManager _puzzleManager;
        private UIs.PuzzleUI _puzzleUI;
        private UIs.OptionsUI _optionsUI;
        private UIs.NormalModeMenuUI _normalModeMenu;
        private UIs.MenusTemplate _mainMenu;

        private CanvasLayer _puzzleLayer;

        private Label _debugLabel;


        public override void _Ready()
        {
            Globals.GridInfoManager.InitAreaSize(GetViewport().GetVisibleRect().Size, new Vector3(50, 200, 100), new Vector2(1, 1));
            _puzzleUI = GetNode<UIs.PuzzleUI>("%PuzzleUI");
            _optionsUI = GetNode<UIs.OptionsUI>("%OptionsUI");
            _puzzleLayer = GetNode<CanvasLayer>("PuzzleLayer");
            _mainMenu = GetNode<UIs.MenusTemplate>("%MainMenuUI");
            _normalModeMenu = GetNode<UIs.NormalModeMenuUI>("%NormalModeMenuUI");

            AddPuzzleManager();
            _puzzleUI.Visible = false;
            _normalModeMenu.Visible = false;
            _optionsUI.Visible = false;


            if (_devToolsEnabled)
            {

                _debugLabel = new Label();
                _puzzleLayer.AddChild(_debugLabel);
                _debugLabel.RectScale = new Vector2(2, 2);
                PackedScene puzzleCreationUIScene = ResourceLoader.Load<PackedScene>(Globals.Utilities.GetPuzzleCreationUIScenePath());
                DevTools.PuzzleCreationUI puzzleCreationUI = puzzleCreationUIScene.Instance<DevTools.PuzzleCreationUI>();
                _puzzleLayer.AddChild(puzzleCreationUI);

                puzzleCreationUI.GetNode<DevTools.PuzzleCreationManager>("PuzzleCreationManager").Init(_puzzleManager);
            }
        }

       
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
        public void _on_BackButton_button_down()
        {
            _puzzleManager.Clear();
            _normalModeMenu.Visible = true;
            _puzzleUI.Visible = false;
        }

        public void _on_MainMenuUI_UIButton_down(string name)
        {
            switch (name)
            {
                case "NormalMode":
                    _mainMenu.Visible = false;
                    _normalModeMenu.Visible = true;
                    break;

                case "TimedMode":
                    break;

                case "Options":
                    break;

                default:
                    break;
            }
        }

        public void _on_NormalModeMenuUI_UIButton_down(string name)
        {
            _normalModeMenu.Visible = false;
            _puzzleUI.Visible = true;

            string[] typeAndId = name.Split('_');
            string type = typeAndId[0];
            int id = typeAndId[1].ToInt();

            Resource puzzleDataRes = ResourceLoader.Load(Globals.Utilities.GetPuzzleDataResourcePath(type, id));
            _puzzleManager.UpdatePuzzleResource(puzzleDataRes);
            _puzzleManager.CreateSequence();
        }

    }

}