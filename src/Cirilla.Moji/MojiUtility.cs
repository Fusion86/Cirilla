﻿using System;
using System.Drawing;

namespace Cirilla.Moji
{
    public static class MojiUtility
    {
        public static Color GetColor(MojiColor color)
        {
            return color switch
            {
                MojiColor.MOJI_WHITE_DEFAULT => Color.FromArgb(225, 225, 225),
                MojiColor.MOJI_WHITE_SELECTED => Color.FromArgb(255, 255, 255),
                MojiColor.MOJI_WHITE_SELECTED2 => Color.FromArgb(160, 160, 160),
                MojiColor.MOJI_WHITE_DISABLE => Color.FromArgb(110, 110, 110),
                MojiColor.MOJI_WHITE_DEFAULT2 => Color.FromArgb(190, 190, 190),
                MojiColor.MOJI_BLACK_DEFAULT => Color.FromArgb(0, 0, 0),
                MojiColor.MOJI_RED_DEFAULT => Color.FromArgb(195, 35, 35),
                MojiColor.MOJI_RED_SELECTED => Color.FromArgb(230, 50, 50),
                MojiColor.MOJI_RED_SELECTED2 => Color.FromArgb(155, 30, 30),
                MojiColor.MOJI_RED_DISABLE => Color.FromArgb(120, 25, 25),
                MojiColor.MOJI_YELLOW_DEFAULT => Color.FromArgb(220, 175, 25),
                MojiColor.MOJI_YELLOW_SELECTED => Color.FromArgb(245, 205, 50),
                MojiColor.MOJI_YELLOW_SELECTED2 => Color.FromArgb(185, 150, 20),
                MojiColor.MOJI_YELLOW_DISABLE => Color.FromArgb(125, 100, 15),
                MojiColor.MOJI_ORANGE_DEFAULT => Color.FromArgb(220, 120, 25),
                MojiColor.MOJI_ORANGE_SELECTED => Color.FromArgb(245, 140, 30),
                MojiColor.MOJI_ORANGE_SELECTED2 => Color.FromArgb(190, 105, 10),
                MojiColor.MOJI_ORANGE_DISABLE => Color.FromArgb(140, 80, 13),
                MojiColor.MOJI_GREEN_DEFAULT => Color.FromArgb(75, 185, 20),
                MojiColor.MOJI_GREEN_SELECTED => Color.FromArgb(85, 230, 30),
                MojiColor.MOJI_GREEN_SELECTED2 => Color.FromArgb(70, 160, 60),
                MojiColor.MOJI_GREEN_DISABLE => Color.FromArgb(40, 95, 25),
                MojiColor.MOJI_SLGREEN_DEFAULT => Color.FromArgb(177, 217, 173),
                MojiColor.MOJI_SLGREEN_SELECTED => Color.FromArgb(208, 255, 204),
                MojiColor.MOJI_SLGREEN_SELECTED2 => Color.FromArgb(125, 153, 122),
                MojiColor.MOJI_SLGREEN_DISABLE => Color.FromArgb(83, 99, 81),
                MojiColor.MOJI_SLGREEN2_DEFAULT => Color.FromArgb(97, 137, 93),
                MojiColor.MOJI_SLGREEN2_SELECTED => Color.FromArgb(128, 175, 124),
                MojiColor.MOJI_SLGREEN2_SELECTED2 => Color.FromArgb(75, 103, 72),
                MojiColor.MOJI_SLGREEN2_DISABLE => Color.FromArgb(53, 69, 51),
                MojiColor.MOJI_LIGHTBLUE_DEFAULT => Color.FromArgb(80, 200, 230),
                MojiColor.MOJI_LIGHTBLUE_SELECTED2 => Color.FromArgb(80, 160, 180),
                MojiColor.MOJI_LIGHTBLUE_SELECTED => Color.FromArgb(75, 215, 255),
                MojiColor.MOJI_LIGHTBLUE_DISABLE => Color.FromArgb(60, 100, 115),
                MojiColor.MOJI_LIGHTBLUE2_DEFAULT => Color.FromArgb(95, 170, 175),
                MojiColor.MOJI_LIGHTBLUE2_SELECTED2 => Color.FromArgb(85, 150, 155),
                MojiColor.MOJI_LIGHTBLUE2_SELECTED => Color.FromArgb(115, 215, 220),
                MojiColor.MOJI_LIGHTBLUE2_DISABLE => Color.FromArgb(65, 110, 115),
                MojiColor.MOJI_LIGHTGREEN_DEFAULT => Color.FromArgb(45, 165, 95),
                MojiColor.MOJI_LIGHTGREEN_SELECTED => Color.FromArgb(60, 220, 130),
                MojiColor.MOJI_LIGHTGREEN_SELECTED2 => Color.FromArgb(30, 115, 65),
                MojiColor.MOJI_LIGHTGREEN_DISABLE => Color.FromArgb(10, 85, 35),
                MojiColor.MOJI_LIGHTYELLOW_DEFAULT => Color.FromArgb(229, 217, 149),
                MojiColor.MOJI_LIGHTYELLOW_SELECTED => Color.FromArgb(255, 246, 192),
                MojiColor.MOJI_LIGHTYELLOW_SELECTED2 => Color.FromArgb(166, 156, 99),
                MojiColor.MOJI_LIGHTYELLOW_DISABLE => Color.FromArgb(115, 107, 63),
                MojiColor.MOJI_LIGHTYELLOW2_DEFAULT => Color.FromArgb(149, 137, 69),
                MojiColor.MOJI_LIGHTYELLOW2_SELECTED => Color.FromArgb(175, 165, 112),
                MojiColor.MOJI_LIGHTYELLOW2_SELECTED2 => Color.FromArgb(116, 106, 49),
                MojiColor.MOJI_LIGHTYELLOW2_DISABLE => Color.FromArgb(85, 77, 33),
                MojiColor.MOJI_BROWN_DEFAULT => Color.FromArgb(40, 8, 8),
                MojiColor.MOJI_BROWN_SELECTED2 => Color.FromArgb(60, 35, 30),
                MojiColor.MOJI_BROWN_SELECTED => Color.FromArgb(75, 48, 30),
                MojiColor.MOJI_BROWN_DISABLE => Color.FromArgb(30, 20, 20),
                MojiColor.MOJI_YELLOW2_DEFAULT => Color.FromArgb(240, 240, 120),
                MojiColor.MOJI_YELLOW2_SELECTED => Color.FromArgb(255, 255, 203),
                MojiColor.MOJI_YELLOW2_SELECTED2 => Color.FromArgb(100, 85, 45),
                MojiColor.MOJI_YELLOW2_DISABLE => Color.FromArgb(100, 99, 82),
                MojiColor.MOJI_ORENGE2_DEFAULT => Color.FromArgb(235, 145, 95),
                MojiColor.MOJI_ORENGE2_SELECTED => Color.FromArgb(255, 165, 115),
                MojiColor.MOJI_ORENGE2_SELECTED2 => Color.FromArgb(200, 105, 60),
                MojiColor.MOJI_ORENGE2_DISABLE => Color.FromArgb(155, 75, 35),
                MojiColor.MOJI_PURPLE_DEFAULT => Color.FromArgb(155, 110, 220),
                MojiColor.MOJI_PURPLE_SELECTED => Color.FromArgb(185, 152, 245),
                MojiColor.MOJI_PURPLE_SELECTED2 => Color.FromArgb(115, 85, 160),
                MojiColor.MOJI_PURPLE_DISABLE => Color.FromArgb(100, 60, 155),
                MojiColor.MOJI_RED2_DEFAULT => Color.FromArgb(225, 65, 55),
                MojiColor.MOJI_RED2_SELECTED => Color.FromArgb(245, 90, 85),
                MojiColor.MOJI_RED2_SELECTED2 => Color.FromArgb(160, 50, 45),
                MojiColor.MOJI_RED2_DISABLE => Color.FromArgb(130, 35, 30),
                MojiColor.MOJI_BLUE_DEFAULT => Color.FromArgb(0, 128, 255),
                MojiColor.MOJI_BLUE_SELECTED => Color.FromArgb(80, 180, 255),
                MojiColor.MOJI_BLUE_SELECTED2 => Color.FromArgb(0, 68, 248),
                MojiColor.MOJI_BLUE_DISABLE => Color.FromArgb(0, 40, 128),
                MojiColor.MOJI_PALEBLUE_DEFAULT => Color.FromArgb(140, 230, 230),
                MojiColor.MOJI_PALEBLUE_SELECTED => Color.FromArgb(200, 255, 255),
                MojiColor.MOJI_PALEBLUE_SELECTED2 => Color.FromArgb(110, 160, 160),
                MojiColor.MOJI_PALEBLUE_DISABLE => Color.FromArgb(40, 70, 70),
                _ => throw new NotImplementedException(),
            };
        }
    }
}
