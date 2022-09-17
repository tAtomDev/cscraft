namespace Core;
internal class Chunk : IDisposable {
    private readonly byte[,,] blockIds = new byte[
        (int)Global.CHUNK_SIZE.X, 
        (int)Global.CHUNK_SIZE.Y, 
        (int)Global.CHUNK_SIZE.Z
    ];
    private readonly World world;
    public readonly Vector3i position;
    
    public bool ShouldRegenerateMesh = true;
    private VAO? mesh = null;

    public Chunk(World world, Vector3i chunkPosition) {
        this.world = world;
        this.position = chunkPosition;
    }

    public bool IsOutOfBounds(int x, int y, int z) {
        return (
            x < 0 || x >= Global.CHUNK_SIZE.X ||
            y < 0 || y >= Global.CHUNK_SIZE.Y ||
            z < 0 || z >= Global.CHUNK_SIZE.Z
        );
    }
    public bool IsWithinBounds(int x, int y, int z) => !IsOutOfBounds(x, y, z);

    public void SetBlock(int x, int y, int z, Block block) => SetBlock(x, y, z, BlockList.GetBlockID(block));
    public void SetBlock(Vector3i position, Block block) => SetBlock(position.X, position.Y, position.Z, block);
    public void SetBlock(Vector3i position, byte id) => SetBlock(position.X, position.Y, position.Z, id);
    public void SetBlock(int x, int y, int z, byte id) {
        if (IsOutOfBounds(x, y, z)) return;
        ShouldRegenerateMesh = true;
        
        blockIds[x, y, z] = id;
    }

    public Block GetBlock(int x, int y, int z) {
        if (IsOutOfBounds(x, y, z)) return BlockType.AIR;

        return BlockList.GetBlock(blockIds[x, y, z]);
    }
    public Block GetBlock(Vector3i position) => GetBlock(position.X, position.Y, position.Z);

    public Block GetBlockFromWorldPosition(int x, int y, int z) {
        var pos = World.WorldToChunkPosition(x, y, z);
        return GetBlock(pos.X, pos.Y, pos.Z);
    }

    public Block? GetBlockFromWorldPosition(Vector3i pos) => GetBlockFromWorldPosition(pos.X, pos.Y, pos.Z);

    public void GenerateMesh() {
        if (mesh is null) mesh = new VAO();
        
        mesh.Clear();
        AddBlocksToVAO(mesh, world);
        mesh.UploadData();
    }

    public void Draw() {
        if (mesh is not null) mesh.Draw();
    }

    public void Generate() {
        for (var x = 0; x < Global.CHUNK_SIZE.X; x++)
        for (var z = 0; z < Global.CHUNK_SIZE.Z; z++) {
            int height = (int)(Global.CHUNK_SIZE.Y / 2) + Random.Shared.Next(-1, 1);
            for (var y = 0; y < Global.CHUNK_SIZE.Y; y++) {
                SetBlock(x, y, z, y == height ? BlockType.GRASS : y < height ? BlockType.DIRT : BlockType.AIR);
            }
        }
    }

    public void Dispose() {
        if (mesh is null) return;
        mesh.Dispose();
        mesh = null;
    }

    private void AddBlocksToVAO(VAO vao, World world) {
        for (var x = 0; x < Global.CHUNK_SIZE.X; x++)
        for (var y = 0; y < Global.CHUNK_SIZE.Y; y++)
        for (var z = 0; z < Global.CHUNK_SIZE.Z; z++) {
            ChunkMeshBuilder.AddBlock(
                world, 
                position * (Vector3i)Global.CHUNK_SIZE + new Vector3i(x, y, z), 
                x, y, z, 
                GetBlock(x, y, z), 
                vao
            );
        }
    }
}