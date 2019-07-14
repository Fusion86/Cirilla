using Cirilla.Core.Structs.Native;

namespace Cirilla.Core.Interfaces
{
    public interface IVertex
    {
        Position Position { get; }
        Normal Normal { get; }
        Tangent Tangent { get; }
        Uv Uv { get; }
    }

    public interface IVertexBaseWeight
    {
        BaseWeight BaseWeight { get; }
    }

    public interface IVertexColor
    {
        ColorRGBA Color { get; }
    }
}
