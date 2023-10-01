using Godot;


namespace PuzzlePieces
{

    public partial class TypeA : BasePiece
    {
        private CollisionShape2D _shape;


        [Export]
        private Vector2 _extents = new Vector2(100, 100);
        public Vector2 Size { get { return _extents; } }

        public override void Init(int id, int colorId, Vector2 position, Vector2 extents)
        {
            base.Init(id, colorId, position, extents);
            _extents = extents;
        }
        public override void _Ready()
        {
            base._Ready();
            _shape = GetNode<CollisionShape2D>("Area2D/Shape3D");
            RectangleShape2D rectangleShape2D = (RectangleShape2D)(_shape.Shape3D);
            rectangleShape2D.Size = _extents;

            ColorRect _rect = GetNode<ColorRect>("ColorRect");
            _rect.Size = 2 * _extents;
            _rect.Position = - _extents;
        }
        public void _on_Area2D_input_event(object _, InputEvent @event, int __)
        {
            if (@event is InputEventMouseButton mousebutton && mousebutton.ButtonIndex == MouseButton.Left && mousebutton.IsPressed())
            {
                Flip();
                mousebutton.Dispose();
            }

            @event.Dispose();
        }
    }
}
