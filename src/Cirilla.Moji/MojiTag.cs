using System;

namespace Cirilla.Moji
{
    public interface IMojiTag
    {
        MojiType Type { get; }
    }

    public class MojiSizeTag : IMojiTag
    {
        public MojiSizeTag(int size)
        {
            Size = size;
        }

        public int Size { get; }
        public MojiType Type => MojiType.SIZE;

        public static MojiSizeTag Parse(string parameter, int row, int col)
        {
            if (int.TryParse(parameter, out int size))
                return new MojiSizeTag(size);
            else
                throw new MojiParserException($"Couldn't convert '{parameter}' to an integer. Line: {row}   Char: {col}");
        }
    }

    public class MojiStyleTag : IMojiTag
    {
        public MojiStyleTag(MojiColor color)
        {
            Color = color;
        }

        public MojiColor Color { get; }
        public MojiType Type => MojiType.STYL;

        public static MojiStyleTag Parse(string parameter, int row, int col)
        {
            if (Enum.TryParse<MojiColor>(parameter, out var color))
                return new MojiStyleTag(color);
            else
                throw new MojiParserException($"Unknown color '{parameter}'. Line: {row}   Char: {col}");
        }
    }

    public class MojiIconTag : IMojiTag
    {
        public MojiIcon Icon { get; }
        public MojiType Type => MojiType.ICON;

        public static MojiIconTag Parse(string parameter, int row, int col)
        {
            //if (Enum.TryParse<MojiColor>(parameter, out var color))
            //    return new MojiStyleTag(color);
            //else
            //    throw new MojiParserException($"Unknown color '{parameter}'. Line: {row}   Char: {col}");

            return new MojiIconTag();
        }
    }
}
