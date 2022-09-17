using Game;

namespace Core;
internal class Game : IDisposable {
    private Application window;

    ShaderProgram chunkShader = new ShaderProgram();
    TextureArray atlas;
    World world;
    PlayerController player = new PlayerController();

    bool firstMouseMovement = true;
    bool debug = false;
    public Game(Application app) {
        window = app;

        chunkShader = new ShaderProgram("Shaders/chunk_shader.vert", "Shaders/chunk_shader.frag");

        var rand = new Random();
        world = new();

        player.Camera.Position.Y = (Global.CHUNK_SIZE.Y);
        player.Camera.Rotate(-110, 150);

        world.GenerateChunksAroundPosition(player.Camera.Position);

        const int atlasSize = 16;
        atlas = new TextureArray(atlasSize, atlasSize, 3);
        atlas.SetTexture(0, new TextureData("Assets/grass_side.png", atlasSize, atlasSize));
        atlas.SetTexture(1, new TextureData("Assets/grass_top.png", atlasSize, atlasSize));
        atlas.SetTexture(2, new TextureData("Assets/dirt.png", atlasSize, atlasSize));
        atlas.GenerateMipmaps();
        atlas.Use();
    }

    public void Dispose() {
        chunkShader.Dispose();
        atlas.Dispose();
    }

    Vector2 LastMousePosition = Vector2.Zero;
    public void OnMouseMove(MouseMoveEventArgs e) {
        if (window.IsCursorVisible || !window.IsFocused) return;

        if (firstMouseMovement) {
            firstMouseMovement = false;
            LastMousePosition = e.Position;
        }

        float deltaX = (e.X - LastMousePosition.X) * player.Camera.Sensitivity;
        float deltaY = (LastMousePosition.Y - e.Y) * player.Camera.Sensitivity;

        player.Camera.Rotate(deltaY, deltaX);
        LastMousePosition = e.Position;
    }

    public bool IsKeyDown(Keys key) => window.IsKeyDown(key);
    public bool IsKeyPressed(Keys key) => window.IsKeyPressed(key);
    public bool IsKeyReleased(Keys key) => window.IsKeyReleased(key);

    public bool IsMouseButtonDown(MouseButton button) => window.IsMouseButtonDown(button);
    public bool IsMouseButtonPressed(MouseButton button) => window.IsMouseButtonPressed(button);
    public bool IsMouseButtonReleased(MouseButton button) => window.IsMouseButtonReleased(button);

    public void Update(float dt) {
        if (!window.IsFocused) {
            firstMouseMovement = true;
            window.SetMouseVisibility(true);
        }

        if (
            (window.IsMouseButtonPressed(MouseButton.Left) && window.IsCursorVisible) ||
            (window.IsKeyPressed(Keys.Escape) && !window.IsCursorVisible)
        ) {
            window.SetMouseVisibility(!window.IsCursorVisible);
            firstMouseMovement = true;
        }

        if (IsKeyPressed(Keys.F1)) {
            debug = !debug;
        } else if (IsKeyPressed(Keys.F11)) {
            window.WindowState = window.IsFullscreen 
                ? WindowState.Normal
                : WindowState.Fullscreen;
        }

        player.Update(this, world, dt);

        world.GenerateChunksAroundPosition(player.Camera.Position);
        world.Update();
        world.DeleteUnnecessaryCache(player.Camera.Position);
    }

    private void PreDraw() {
        if (debug) {
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
        } else {
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
        }
    }

    public void Draw() {
        PreDraw();

        chunkShader.UseProgram();

        world.Render(chunkShader);
    }

    private Matrix4 GetProjectionMatrix() {
        return Matrix4.CreatePerspectiveFieldOfView(
            MathHelper.DegreesToRadians(player.Camera.FOV), 
            ((float)window.ClientSize.X / (float)window.ClientSize.Y),
            0.01f, 10000.0f
        );
    }

    public void UpdateMatrices() {
        Matrix4 model = Matrix4.Identity;
        Matrix4 view = player.Camera.GetMatrix();
        Matrix4 projection = GetProjectionMatrix();

        chunkShader.SetUniformMatrix4("uProjection", projection);
        chunkShader.SetUniformMatrix4("uModel", model);
        chunkShader.SetUniformMatrix4("uView", view);
    }
}