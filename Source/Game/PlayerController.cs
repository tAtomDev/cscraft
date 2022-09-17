using Core;

namespace Game;
class PlayerController {
    public Camera Camera = new Camera();
    private Vector3 Forward => new Vector3(Camera.FrontDirection.X, 0, Camera.FrontDirection.Z);

    public void Update(Core.Game game, World world, float dt) {
        BlockRaytraceResult? raytrace = world.BlockRaytrace(Camera.Position, Camera.Direction, 30);

        float deltaSpeed = (dt * 20.0f);
        if (game.IsKeyDown(Keys.W)) Camera.Position += deltaSpeed * Forward;
        if (game.IsKeyDown(Keys.S)) Camera.Position += -deltaSpeed * Forward;

        if (game.IsKeyDown(Keys.A)) 
            Camera.Position += (-deltaSpeed * Vector3.Cross(Forward, Vector3.UnitY));
        if (game.IsKeyDown(Keys.D)) 
            Camera.Position += (deltaSpeed * Vector3.Cross(Forward, Vector3.UnitY));


        if (game.IsKeyDown(Keys.Space)) Camera.Position += 0.5f * deltaSpeed * Vector3.UnitY;
        if (game.IsKeyDown(Keys.LeftShift)) Camera.Position -= 0.5f * deltaSpeed * Vector3.UnitY;
        if (game.IsKeyDown(Keys.E)) Camera.FOV += 60 * dt;
        if (game.IsKeyDown(Keys.Q)) Camera.FOV -= 60 * dt;

        Camera.FOV = Math.Clamp(Camera.FOV, 1, 175);

        var cameraChunkPosition = World.WorldToChunkPosition(Camera.Position);
        var cameraBlockPosition = World.ChunkToBlockPosition(Camera.Position);
        if (game.IsMouseButtonPressed(MouseButton.Left)) {
            if (raytrace == null) return;

            world.SetBlock(raytrace.BlockPos, BlockType.AIR);
        } else if (game.IsMouseButtonPressed(MouseButton.Middle)) {
            BlockRaytraceResult? result = world.BlockRaytrace(Camera.Position, Camera.Direction, 30);
            if (raytrace == null) return;

            var pos = result.BlockPos + BlockFaceHelper.GetNormal(raytrace.Face);
            if (pos == cameraBlockPosition) return;

            world.SetBlock(raytrace.BlockPos + BlockFaceHelper.GetNormal(raytrace.Face), BlockType.GRASS);
        }
    }
}