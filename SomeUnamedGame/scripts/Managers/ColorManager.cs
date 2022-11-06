using Godot;
using System.Collections.Generic;

namespace Globals
{

    public static class ColorManager
    {
        private static Godot.Collections.Array<Color> _colors = new Godot.Collections.Array<Color>()
        {
            new Color(0f,0f,0f,1f), new Color(1f,1f,1f,1f), new Color(0.5f,0.5f,0.5f,1f)
        };

        public static Godot.Collections.Array<Color> Colors 
        {
            get {return _colors;}
        }

        private static Dictionary<int, int> ColorsRelation = new Dictionary<int, int>()
        {
            {0, 1}, {1, 0}, {2,2}
        };
        
        private static Dictionary<int, float> ColorsValues = new Dictionary<int, float>()
        {
            {0, 0}, {1, 1}, {2, 0.5f}
        };
            
        public static Color Flip(ref int colorId)
        {
            colorId = ColorsRelation[colorId];
            return Colors[colorId];
        }
        
        public static float GetValue(int colorId)
        {
            return ColorsValues[colorId];
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