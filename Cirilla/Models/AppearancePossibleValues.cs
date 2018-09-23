using System.Collections.Generic;
using System.Windows;

namespace Cirilla.Models
{
    // TODO: Rename this, make not static, etc
    public class AppearancePossibleValues
    {
        public static readonly AppearancePossibleValues Default;

        public List<CharacterObjectTypeWithRect> FaceTypes { get; } = new List<CharacterObjectTypeWithRect>();
        public List<CharacterObjectTypeWithRect> HairTypes { get; } = new List<CharacterObjectTypeWithRect>();

        static AppearancePossibleValues()
        {
            AppearancePossibleValues def = new AppearancePossibleValues();

            // Face types
            int valueStart = 0;
            for (int i = 0; i < 24; i++)
            {
                Int32Rect rect = GetRect(i, 5, 100, 100, 1, 2);
                def.FaceTypes.Add(new CharacterObjectTypeWithRect(valueStart + i, rect));
            }

            // Hair types
            valueStart = 103;
            for (int i = 0; i < 28; i++)
            {
                Int32Rect rect = GetRect(i, 5, 100, 100, 1, 2);
                def.HairTypes.Add(new CharacterObjectTypeWithRect(valueStart + i, rect));
            }

            Default = def;
        }

        private static Int32Rect GetRect(int spriteIndex, int spritesPerRow, int spriteWidth, int spriteHeight, int baseOffset = 0, int spacingBetweenSprites = 0)
        {
            int col = spriteIndex % spritesPerRow;
            int row = spriteIndex / spritesPerRow;

            int x = (col * spriteWidth) + baseOffset + (spacingBetweenSprites * col);
            int y = (row * spriteHeight) + baseOffset + (spacingBetweenSprites * row);

            return new Int32Rect(x, y, spriteWidth, spriteHeight);
        }
    }

    public class CharacterObjectTypeWithRect
    {
        public int Value { get; }
        public Int32Rect Rect { get; }

        public CharacterObjectTypeWithRect(int value, Int32Rect rect)
        {
            Value = value;
            Rect = rect;
        }
    }
}
