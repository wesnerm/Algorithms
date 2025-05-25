namespace Algorithms.ComputationalGeometry;

[Flags]
public enum MatrixGroup
{
    Identity = 0x001 | Translation | IsotropicScale | Rotation,
    Translation = 0x002 | LengthPreserving | WindowToViewport,
    IsotropicScale = 0x004 | AnglePreserving | AnisotropicScale, // Anisotropic Scale
    AnisotropicScale = 0x008 | WindowToViewport,
    Rotation = 0x010 | LengthPreserving,
    WindowToViewport = 0x020 | Affine, // AnisotropicScale | Translation,
    LengthPreserving = 0x040 | AnglePreserving, // . Translation | Rotation,
    AnglePreserving = 0x080 | Affine, // . Translation | Rotation | IsotropicScale,
    Affine = 0x100 | Nonsingular, // . Translation | Rotation,
    Nonsingular = 0x200,
}