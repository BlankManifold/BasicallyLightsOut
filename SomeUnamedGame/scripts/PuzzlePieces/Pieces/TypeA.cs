using Godot;


namespace PuzzlePieces
{

    public partial class TypeA : BasePiece
    {
        private CollisionShape2D _shape;


        [Export]
        private Vector2 _size = new Vector2(100, 100);
        public Vector2 Size { get { return _size; } }

        public override void Init(int id, int colorId, Vector2 position, Vector2 size)
        {
            base.Init(id, colorId, position, size);
            _size = size;
        }
        public override void _Ready()
        {
            base._Ready();
            _shape = GetNode<CollisionShape2D>("%CollisionShape2D");
            RectangleShape2D rectangleShape2D = (RectangleShape2D)_shape.Shape;
            rectangleShape2D.Size = _size;

            ColorRect _rect = GetNode<ColorRect>("ColorRect");
            _rect.Size = _size;
            _rect.Position = - _size/2;
        }
        public void _on_area_2d_input_event(Node _, InputEvent @event, int __)
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
