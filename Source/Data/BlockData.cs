using System.Collections.Generic;
namespace Core;
struct Block {
    public string Name;
    public bool Solid = true;
    public bool Visible = true;
    public Dictionary<BlockFace, uint> Textures = new();
    

    public Block(string name) {
        Name = name;
    }

    public Block(string name, uint defaultTexture) {
        Name = name;

        foreach (var face in BlockFaceHelper.Faces) {
            if (!Textures.TryGetValue(face, out var _)) {
                Textures.Add(face, defaultTexture);
            }
        }
    }

    public uint GetFaceTexture(BlockFace face) {
        return Textures.TryGetValue(face, out uint id)
            ? id 
            : 0;
    }

    public override int GetHashCode() {
        return Name.GetHashCode();
    }

    public override bool Equals(object? obj) {
        return (obj is Block) && ((Block)obj).Name == this.Name;
    }
    public static bool operator!=(Block a, Block b) => !a.Equals(b);
    public static bool operator==(Block a, Block b) => a.Equals(b);
}

internal static class BlockType {
    public static readonly Block AIR = new Block("Air");
    public static readonly Block GRASS = new Block("Grass") { 
        Textures = new() { 
            { BlockFace.Left, 0 }, 
            { BlockFace.Right, 0 }, 
            { BlockFace.Front, 0 }, 
            { BlockFace.Back, 0 }, 
            { BlockFace.Top, 1 }, 
            { BlockFace.Bottom, 2 }, 
        } 
    };
    public static readonly Block DIRT = new Block("Dirt", 2);
}

internal static class BlockList {
    public static readonly Block[] Blocks = {
        BlockType.AIR, BlockType.GRASS, BlockType.DIRT
    };

    public static Block GetBlock(byte id) {
        return Blocks[id];
    }

    public static byte GetBlockID(Block block) {
        return (byte)Array.FindIndex(Blocks, x => x == block);
    }
}