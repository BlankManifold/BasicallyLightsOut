using Godot;


namespace PuzzlePieces
{

    public class TypeA : BasePiece
    {
        private CollisionShape2D _shape;


        [Export]
        private Vector2 _extents = new Vector2(100, 100);
        public Vector2 Extents { get { return _extents; } }

        public override void Init(int id, int colorId, Vector2 position, Vector2 extents)
        {
            base.Init(id, colorId, position, extents);
            _extents = extents;
        }
        public override void _Ready()
        {
            base._Ready();
            _shape = GetNode<CollisionShape2D>("Area2D/Shape");
            RectangleShape2D rectangleShape2D = (RectangleShape2D)(_shape.Shape);
            rectangleShape2D.Extents = _extents;

            ColorRect _rect = GetNode<ColorRect>("ColorRect");
            _rect.RectSize = 2 * _extents;
            _rect.RectPosition = - _extents;
        }
        public void _on_Area2D_input_event(Object _, InputEvent @event, int __)
        {
            if (@event is InputEventMouseButton mousebutton && mousebutton.ButtonIndex == 1 && mousebutton.IsPressed())
            {
                Flip();
                mousebutton.Dispose();
            }

            @event.Dispose();
        }
    }
}
