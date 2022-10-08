using Godot;
using System.Collections.Generic;

namespace Globals
{

    public static class ColorManager
    {
        private static Godot.Collections.Array<Color> _colors = new Godot.Collections.Array<Color>()
        {
            new Color(0f,0f,0f,1f), new Color(1f,1f,1f,1f) 
        };

        public static Godot.Collections.Array<Color> Colors 
        {
            get {return _colors;}
        }

        private static Dictionary<int, int> ColorsRelation = new Dictionary<int, int>()
        {
            {0, 1}, {1, 0}
        };
            
        public static Color Flip(ref int colorId)
        {
            colorId = ColorsRelation[colorId];
            return Colors[(int)colorId];
        }

        public static int CheckColor(int colorId, int targetColorId)
        {
             if (colorId == targetColorId)
            {
                return 1;
            }
            else
            {
                return -1;
            }
        }
    }
}