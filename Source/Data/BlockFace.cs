namespace Core;
internal enum BlockFace {
    Left,
    Right,
    Bottom,
    Top,
    Back,
    Front,
}

internal static class BlockFaceHelper {
    public static readonly BlockFace[] Faces = { 
        BlockFace.Left, BlockFace.Right, BlockFace.Bottom, 
        BlockFace.Top, BlockFace.Back, BlockFace.Front 
    };
    public static int FacesLength => Faces.Length;

    public static Vector3i GetNormal(BlockFace face) {
        switch (face) {
            case BlockFace.Left:
                return new Vector3i(-1, 0, 0);
            case BlockFace.Right:
                return new Vector3i(1, 0, 0);
            case BlockFace.Bottom:
                return new Vector3i(0, -1, 0);
            case BlockFace.Top:
                return new Vector3i(0, 1, 0);
            case BlockFace.Back:
                return new Vector3i(0, 0, -1);
            case BlockFace.Front:
                return new Vector3i(0, 0, 1);
        }

        throw new Exception("Invalid block face");
    }
}