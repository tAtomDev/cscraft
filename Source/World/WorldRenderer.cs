namespace Core;
internal static class WorldRenderer {
    public static void RenderWorld(World world, ShaderProgram shaderProgram) {
        foreach(var entry in world.Chunks) {
            var matrix = Matrix4.CreateTranslation(
                entry.Key * (Global.CHUNK_SIZE)
            );
            shaderProgram.SetUniformMatrix4("uModel", matrix);
            
            entry.Value.Draw();
        }
    }
}