namespace Core;
internal class World : IDisposable {
    public static Vector3i WorldToChunkPosition(Vector3 pos) => WorldToChunkPosition(
        (int)pos.X, (int)pos.Y, (int)pos.Z
    );
    public static Vector3i WorldToChunkPosition(int x, int y, int z) => new Vector3i(
        x < 0 ? (x + 1) / (int)Global.CHUNK_SIZE.X - 1 : x / (int)Global.CHUNK_SIZE.X,
        y < 0 ? (y + 1) / (int)Global.CHUNK_SIZE.Y - 1 : y / (int)Global.CHUNK_SIZE.Y,
        z < 0 ? (z + 1) / (int)Global.CHUNK_SIZE.Z - 1 : z / (int)Global.CHUNK_SIZE.Z
    );

    public static Vector3i ChunkToBlockPosition(Vector3 pos) => ChunkToBlockPosition(
        (int)pos.X, (int)pos.Y, (int)pos.Z
    );
    public static Vector3i ChunkToBlockPosition(int x, int y, int z) => new Vector3i(
            x < 0 ? (x + 1) % (int)Global.CHUNK_SIZE.X + (int)Global.CHUNK_SIZE.X - 1 : x % (int)Global.CHUNK_SIZE.X,
            y < 0 ? (y + 1) % (int)Global.CHUNK_SIZE.Y + (int)Global.CHUNK_SIZE.Y - 1 : y % (int)Global.CHUNK_SIZE.Y,
            z < 0 ? (z + 1) % (int)Global.CHUNK_SIZE.Z + (int)Global.CHUNK_SIZE.Z - 1 : z % (int)Global.CHUNK_SIZE.Z
        );
    public readonly Dictionary<Vector3i, Chunk> Chunks = new();
    public readonly Dictionary<Vector3i, Chunk> Cache = new();
    public readonly List<Vector3i> UpdateList = new();
    public readonly List<Vector3i> LoadList = new();
    public readonly List<Vector3i> UnloadList = new();

    public World() {
    }

    public Chunk? GetChunk(int x, int y, int z) => GetChunk(new Vector3i(x, y, z));
    public Chunk? GetChunk(Vector3i position) {
        return Chunks.TryGetValue(position, out Chunk? chunk)
            ? chunk : null;
    }

    public void SetBlock(int x, int y, int z, Block block) => SetBlock(x, y, z, BlockList.GetBlockID(block));
    public void SetBlock(Vector3i position, byte id) => SetBlock(position.X, position.Y, position.Z, id);
    public void SetBlock(Vector3i position, Block block) => SetBlock(position.X, position.Y, position.Z, block);
    public void SetBlock(int x, int y, int z, byte id) {
        var chunkPos = WorldToChunkPosition(x, y, z);
        var blockPos = ChunkToBlockPosition(x, y, z);

        if (Chunks.TryGetValue(chunkPos, out Chunk? chunk)) {
            chunk!.SetBlock(blockPos, id);
            UpdateList.Add(chunkPos);
        } else return;

        UpdateNeighboursChunks(x, y, z);
    }
    public void RegenerateNeighboursChunks(Vector3i chunkPosition) {
        for (int i = -1; i <= 1; i++)
        for (int j = -1; j <= 1; j++)
        for (int k = -1; k <= 1; k++) {
            Chunk? chunk = GetChunk(chunkPosition + new Vector3i(i, j, k));
            if (chunk is not null) {
                UpdateList.Add(chunk.position);
            }
        }
    }
    
    public void UpdateNeighboursChunks(int x, int y, int z) {
        var chunkPos = WorldToChunkPosition(x, y, z);
        var blockPos = ChunkToBlockPosition(x, y, z);

        if (
            blockPos.X == 0 || blockPos.X == Global.CHUNK_SIZE.X - 1
        ) {
            Chunk? a = GetChunk(chunkPos.X - 1, chunkPos.Y, chunkPos.Z);
            Chunk? b = GetChunk(chunkPos.X + 1, chunkPos.Y, chunkPos.Z);
            if (a is not null) UpdateList.Add(a.position);
            if (b is not null) UpdateList.Add(b.position);
        }
        
        if (
            blockPos.Y == 0 || blockPos.Y == Global.CHUNK_SIZE.Y - 1
        ) {
            Chunk? a = GetChunk(chunkPos.X, chunkPos.Y - 1, chunkPos.Z);
            Chunk? b = GetChunk(chunkPos.X, chunkPos.Y + 1, chunkPos.Z);
            if (a is not null) UpdateList.Add(a.position);
            if (b is not null) UpdateList.Add(b.position);
        }

        if (
            blockPos.Z == 0 || blockPos.Z == Global.CHUNK_SIZE.X - 1
        ) {
            Chunk? a = GetChunk(chunkPos.X, chunkPos.Y, chunkPos.Z + 1);
            Chunk? b = GetChunk(chunkPos.X, chunkPos.Y, chunkPos.Z - 1);
            if (a is not null) UpdateList.Add(a.position);
            if (b is not null) UpdateList.Add(b.position);
        }
    }

    public Block GetBlock(Vector3i position) => GetBlock(position.X, position.Y, position.Z);
    public Block GetBlock(int x, int y, int z) {
        var chunkPos = WorldToChunkPosition(x, y, z);
        var blockPos = ChunkToBlockPosition(x, y, z);

        return Chunks.TryGetValue(chunkPos, out Chunk? chunk)
            ? chunk.GetBlock(blockPos)
            : BlockType.AIR;
    }

    public bool IsTransparentAt(Vector3i position) => IsTransparentAt(position.X, position.Y, position.Z);
    public bool IsTransparentAt(int x, int y, int z) {
        Vector3i chunkPos = WorldToChunkPosition(x, y, z);
        return GetBlock(x, y, z) == BlockType.AIR 
            && GetChunk(chunkPos) != null;
    }

    public void Update() {
        if (LoadList.Count != 0) {
            var pos = LoadList.First();
            LoadList.Remove(pos);
            GenerateChunk(pos);
        }

        if (UnloadList.Count != 0) {
            var pos = UnloadList.First();
            UnloadList.Remove(pos);
            UnloadChunk(pos);
        }

        if (UpdateList.Count != 0) {
            foreach (var pos in UpdateList) {
                Chunk? chunk = GetChunk(pos);
                if (chunk is null) continue; 

                chunk.GenerateMesh();
            }
            UpdateList.Clear();
        }
    }

    public void GenerateChunk(Vector3i position) {
        if (Cache.TryGetValue(position, out Chunk? cachedChunk)) {
            Chunks.TryAdd(position, cachedChunk);
            Cache.Remove(position);
            return;
        }
        var chunk = new Chunk(this, position);
        chunk.Generate();

        Chunks.TryAdd(position, chunk);
        RegenerateNeighboursChunks(position);
    }

    public void UnloadChunk(Vector3i position) {
        var chunk = GetChunk(position);
        if (chunk is null) return;

        Cache.Add(position, chunk);
        Chunks.Remove(position, out var _);
        RegenerateNeighboursChunks(position);
    }

    public void DeleteUnnecessaryCache(Vector3 worldPosition) {
        if (Cache.Count < 10) return;

        var pos = WorldToChunkPosition(worldPosition);
        foreach (var entry in Cache) {
            if (((Vector3)(pos - entry.Key)).LengthFast > 5f) {
                entry.Value.Dispose();
                Cache.Remove(entry.Key);
                continue;
            }
        }
    }

    const int renderDistance = 8;
    public void GenerateChunksAroundPosition(Vector3 worldPos) {
        worldPos.Y = 0;
        Vector3i position = WorldToChunkPosition(worldPos);
        List<Vector3i> unloadChunkPositions = new();
        for (var i = -renderDistance; i < renderDistance; i++)
        for (var j = -renderDistance; j < renderDistance; j++)  {
            Vector3i chunkPosition = position + new Vector3i(i, 0, j);
            if (LoadList.Contains(chunkPosition)) {
                continue;
            } 

            Chunk? chunkAt = GetChunk(chunkPosition);
            
            if (chunkAt is null) LoadList.Add(chunkPosition);
            else {
                unloadChunkPositions.Add(chunkPosition);
            }
        }

        foreach (var entry in Chunks) {
            if (unloadChunkPositions.Contains(entry.Key)) {
                continue;
            }
            if (LoadList.Contains(entry.Key)) {
                LoadList.Remove(entry.Key);
                continue;
            }
            if (!UnloadList.Contains(entry.Key)) {
                UnloadList.Add(entry.Key);
            }
        }
    }

    public void Render(ShaderProgram shader) {
        WorldRenderer.RenderWorld(this, shader);
    }

    public void Dispose() {
        foreach (var entry in Chunks) {
            entry.Value.Dispose();
        }   

        Chunks.Clear();
    }

    public BlockRaytraceResult BlockRaytrace(Vector3 position, Vector3 direction, float range) {
        const float epsilon = -1e-6f;

        direction.NormalizeFast();
        Vector3i start = (Vector3i)(position - direction * 0.5f);
        Vector3i end = (Vector3i)(position + direction * (range + 0.5f)); 

        var minX = Math.Min(start.X, end.X) - 1;
        var minY = Math.Min(start.Y, end.Y) - 1;
        var minZ = Math.Min(start.Z, end.Z) - 1;

        var maxX = Math.Max(start.X, end.X) + 1;
        var maxY = Math.Max(start.Y, end.Y) + 1;
        var maxZ = Math.Max(start.Z, end.Z) + 1;

        BlockRaytraceResult? result = null;
        for (var x = minX; x <= maxX; x++)
        for (var y = minY; y <= maxY; y++)
        for (var z = minZ; z <= maxZ; z++) {
            var block = GetBlock(x, y, z);
            if (block == BlockType.AIR) continue;

            var translation = Vector3.Zero;
            var scale = Vector3.One;
            foreach (var face in BlockFaceHelper.Faces) {
                var normal = BlockFaceHelper.GetNormal(face);
                var divisor = Vector3.Dot(normal, direction);

                // Ignore back faces
                if (divisor >= epsilon) continue;

                var planeNormal = normal * normal;
                var blockPos = new Vector3(x, y, z) + translation;
                var blockSize = new Vector3(0.5f) * scale;
                var d = -(Vector3.Dot(blockPos, planeNormal) + Vector3.Dot(blockSize, normal));
                var numerator = Vector3.Dot(planeNormal, position) + d;
                var distance = Math.Abs(-numerator / divisor);
                var point = position + distance * direction;
                
                if (
                    point.X < x + translation.X - blockSize.X + epsilon || 
                    point.X > x + translation.X + blockSize.X - epsilon || 
                    point.Y < y + translation.Y - blockSize.Y + epsilon || 
                    point.Y > y + translation.Y + blockSize.Y - epsilon || 
                    point.Z < z + translation.Z - blockSize.Z + epsilon || 
                    point.Z > z + translation.Z + blockSize.Z - epsilon
                ) {
                    continue;
                }

                if (distance <= range && (result is null || result.Distance > distance)) {
                    result = new BlockRaytraceResult(face, block, new Vector3i(x, y, z), distance, point);
                }
            }
        }
        
        return result!;
    }
}