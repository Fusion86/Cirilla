using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Cirilla.Models
{
    // TODO: Rename this, make not static, etc
    public class AppearancePossibleValues
    {
        public static readonly AppearancePossibleValues Male;
        public static readonly AppearancePossibleValues Female;

        public List<CharacterObjectTypeWithRect> FaceTypes { get; } = new List<CharacterObjectTypeWithRect>();
        public List<CharacterObjectTypeWithRect> HairTypes { get; } = new List<CharacterObjectTypeWithRect>();
        public List<CharacterObjectTypeWithRect> EyebrowTypes { get; } = new List<CharacterObjectTypeWithRect>();
        public List<CharacterObjectTypeWithRect> BrowTypes { get; } = new List<CharacterObjectTypeWithRect>();

        static AppearancePossibleValues()
        {
            AppearancePossibleValues female = new AppearancePossibleValues();

            // Face types
            BitmapSource src = new BitmapImage(new Uri(@"L:\MHWMods\154553\chunk0\ui\chara_make\tex\thumb_face01_ID.png"));
            for (int i = 0; i < 24; i++)
            {
                Int32Rect rect = GetRect(i, 5, 100, 100, 1, 2);
                CroppedBitmap img = new CroppedBitmap(src, rect);
                female.FaceTypes.Add(new CharacterObjectTypeWithRect(i, img));
            }

            // Hair types
            src = new BitmapImage(new Uri(@"L:\MHWMods\154553\chunk0\ui\chara_make\tex\thumb_hair01_ID.png"));
            for (int i = 0; i < 28; i++)
            {
                Int32Rect rect = GetRect(i, 5, 100, 100, 1, 2);
                CroppedBitmap img = new CroppedBitmap(src, rect);
                female.HairTypes.Add(new CharacterObjectTypeWithRect(100 + i, img)); // Type starts at 100
            }

            // Eyebrow types
            src = new BitmapImage(new Uri(@"L:\MHWMods\154553\chunk0\ui\chara_make\tex\thumb_brow01_ID.png"));
            for (int i = 0; i < 16; i++)
            {
                Int32Rect rect = GetRect(i, 4, 120, 56, 3, 6);
                CroppedBitmap img = new CroppedBitmap(src, rect);
                female.EyebrowTypes.Add(new CharacterObjectTypeWithRect(i, img));
            }

            // Brow types
            src = new BitmapImage(new Uri(@"L:\MHWMods\154553\chunk0\ui\chara_make\tex\thumb_forehead01_ID.png"));
            for (int i = 0; i < 24; i++)
            {
                Int32Rect rect = GetRect(i, 4, 120, 56, 3, 6);
                CroppedBitmap img = new CroppedBitmap(src, rect);
                female.BrowTypes.Add(new CharacterObjectTypeWithRect(i, img));
            }

            Male = female;
            Female = female;
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
        public BitmapSource Image { get; }

        public CharacterObjectTypeWithRect(int value, BitmapSource image)
        {
            Value = value;
            Image = image;
        }
    }
}
