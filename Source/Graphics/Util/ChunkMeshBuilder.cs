namespace Core;
internal static class ChunkMeshBuilder {
    private static readonly Vector3[] FacePositions = {
        // Left
        new Vector3(-0.5f, +0.5f, -0.5f), new Vector3(-0.5f, +0.5f, +0.5f),
        new Vector3(-0.5f, -0.5f, -0.5f), new Vector3(-0.5f, -0.5f, +0.5f),
        // Right
        new Vector3(+0.5f, +0.5f, +0.5f), new Vector3(+0.5f, +0.5f, -0.5f),
        new Vector3(+0.5f, -0.5f, +0.5f), new Vector3(+0.5f, -0.5f, -0.5f),
        // Bottom
        new Vector3(-0.5f, -0.5f, +0.5f), new Vector3(+0.5f, -0.5f, +0.5f),
        new Vector3(-0.5f, -0.5f, -0.5f), new Vector3(+0.5f, -0.5f, -0.5f),
        // Top
        new Vector3(-0.5f, +0.5f, -0.5f), new Vector3(+0.5f, +0.5f, -0.5f),
        new Vector3(-0.5f, +0.5f, +0.5f), new Vector3(+0.5f, +0.5f, +0.5f),
        // Back
        new Vector3(+0.5f, +0.5f, -0.5f), new Vector3(-0.5f, +0.5f, -0.5f),
        new Vector3(+0.5f, -0.5f, -0.5f), new Vector3(-0.5f, -0.5f, -0.5f),
        // Front
        new Vector3(-0.5f, +0.5f, +0.5f), new Vector3(+0.5f, +0.5f, +0.5f),
        new Vector3(-0.5f, -0.5f, +0.5f), new Vector3(+0.5f, -0.5f, +0.5f)
    };

    private static readonly Vector2[] FaceUVs = {
        new Vector2(0, 0), new Vector2(1, 0),
        new Vector2(0, 1), new Vector2(1, 1)
    };

    private static readonly float[] FaceColorIntensity = {
        0.60f, // Left
        0.60f, // Right
        0.40f, // Bottom
        1.0f,  // Top
        0.80f, // Back
        0.80f, // Front
    };

    private static readonly uint[] FaceIndices = {2, 1, 0, 2, 3, 1};

    public static void AddBlock(World world, Vector3i worldOffset, int x, int y, int z, Block block, VAO vao) {
        if (block == BlockType.AIR) return;

        foreach (var face in BlockFaceHelper.Faces) {
            var faceNormal = BlockFaceHelper.GetNormal(face);
            if (world.IsTransparentAt(worldOffset + faceNormal)) {
                AddFace(x, y, z, face, vao, block);
            }
        }
    }

    public static void AddFace(int x, int y, int z, BlockFace face, VAO vao, Block block) {
        int faceId = (int)face;
        int indicesOffset = vao.VertexCount;
        uint textureId = block.GetFaceTexture(face);

        for (var i = 0; i < 4; i++) {
            vao.Add(
                (FacePositions[faceId * 4 + i] + new Vector3(x, y, z)),
                new Vector3(FaceUVs[i].X, FaceUVs[i].Y, textureId),
                FaceColorIntensity[faceId]
            );
        }

        var newIndices = new uint[FaceIndices.Length];
        for (var i = 0; i < newIndices.Length; i++) {
            newIndices[i] = (uint)(FaceIndices[i] + indicesOffset);
        }

        vao.AddIndices(newIndices);
    }
}