using System.Collections.Generic;
using Godot;

namespace PuzzlePieces
{

    public class BasePiece: Sprite, IFlippable
    {
        public bool IsFlipped {get;} = false;
        public int ColorId {get; set;} = 0;
        protected int _id;

        [Signal]
        delegate void Flipping(int id, int colorId, bool isSetup = false);

        public virtual void Init(int id, int colorId, Vector2 position, Vector2 _)
        {
            _id = id;
            ColorId = colorId;
            GlobalPosition = position;
        }

        public override void _Ready()
        {
            Modulate = Globals.ColorManager.Colors[ColorId];
            SequenceTypeA sequence = (SequenceTypeA)GetTree().GetNodesInGroup("sequenceTypeA")[0];
            Connect(nameof(Flipping), sequence, "_on_BasePiece_Flipping");
        }

        public virtual void Flip(bool isSetup = false)
        {
            int colorId = ColorId;
            Modulate = Globals.ColorManager.Flip(ref colorId);
            ColorId = colorId;
            
            EmitSignal(nameof(Flipping), _id, ColorId, isSetup);

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