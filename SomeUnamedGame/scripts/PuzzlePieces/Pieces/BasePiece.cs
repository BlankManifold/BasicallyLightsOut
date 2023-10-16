using Godot;

namespace PuzzlePieces
{

    public partial class BasePiece: Sprite2D, IFlippable
    {
        public bool IsFlipped {get;} = false;
        public int ColorId {get; set;} = 0;
        protected int _id;
        protected bool _active = true;
        public bool IsActive {get {return _active;} set {_active = value;}}

        [Signal]
        public delegate void FlippingEventHandler(int id, int colorId, bool isSetup = false);


        public virtual void Init(int id, int colorId, Vector2 position, Vector2 __size)
        {
            _id = id;
            ColorId = colorId;
            GlobalPosition = position;
        }
        public override void _Ready()
        {
            Modulate = Globals.ColorManager.Colors[ColorId];
            Flipping += GetParent<SequenceTypeA>().OnBasePieceFlipping;
        }

        
        public virtual void Flip(bool isSetup = false)
        {
            int colorId = ColorId;
            Modulate = Globals.ColorManager.Flip(ref colorId);
            ColorId = colorId;
            
            EmitSignal(SignalName.Flipping, _id, ColorId, isSetup);

            return;
        } 
        public virtual int SelfFlip()
        {
            int colorId = ColorId;
            Modulate = Globals.ColorManager.Flip(ref colorId);
            ColorId = colorId;
            
            return ColorId;
        }
        public void SetColor(int colorId)
        {
            ColorId = colorId;
            Modulate = Globals.ColorManager.Colors[colorId];
        }


    }
}