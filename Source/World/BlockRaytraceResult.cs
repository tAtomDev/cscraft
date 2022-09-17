namespace Core;
class BlockRaytraceResult {
        public readonly Block Block;
        public readonly BlockFace Face;
        public readonly Vector3i BlockPos;
        public readonly float Distance;
        public readonly Vector3 Point;

        public BlockRaytraceResult(BlockFace face, Block block, Vector3i blockPos, float distance, Vector3 point) {
            Block = block;
            Face = face;
            BlockPos = blockPos;
            Distance = distance;
            Point = point;
        }
    }