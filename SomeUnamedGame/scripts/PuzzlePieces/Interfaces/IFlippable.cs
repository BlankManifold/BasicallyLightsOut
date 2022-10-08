
using Godot;

namespace PuzzlePieces
{

    public interface IFlippable
    {
        bool IsFlipped {get;}
        int ColorId {get; set;}
        
        void Flip(bool isSetup = false);
    }
    
}