

using System.Diagnostics;
using System.ComponentModel;

namespace Core;
public class Application : GameWindow {
    public bool IsCursorVisible { get; private set; } = true;

    Game game;

    public Application() : base(
        new GameWindowSettings() {
            //RenderFrequency = 60.0f,
            //UpdateFrequency = 60.0f,
        }, 
        new NativeWindowSettings() { 
            Title = "Game",
            Size = new OpenTK.Mathematics.Vector2i(600, 600),
            APIVersion = Version.Parse("4.1.0"),
            //DepthBits = 2,
            //NumberOfSamples = 4,
        }
    ) {
        game = new Game(this);
        SetMouseVisibility(true);

        StartGameLoop();

        // End of game loop
        game.Dispose();
    }

    public void SetMouseVisibility(bool visible) {
        IsCursorVisible = visible;

        CursorVisible = visible;
        CursorGrabbed = !visible;
    }

    DateTime time1 = DateTime.Now;
    DateTime time2 = DateTime.Now;

    Stopwatch watch = new Stopwatch();
    Stopwatch watch2 = new Stopwatch();

    public bool running = true;
    private void StartGameLoop() {
        watch.Start();
        watch2.Start();
        while (running) {
            ProcessEvents();
            time2 = DateTime.Now;
            float deltaTime = (time2.Ticks - time1.Ticks) / 10000000f; 

            game.Update(deltaTime);
            game.UpdateMatrices();
            DrawGame();

            time1 = time2;

            var ms = watch.ElapsedMilliseconds;
            watch.Restart();

            if (watch2.ElapsedMilliseconds > 100) {
                watch2.Restart();
                Title = $"Game (Frame time: {ms}ms)";
            }
        }
    }

    protected override void OnClosing(CancelEventArgs e) {
        running = false;
        base.OnClosing(e);
    }

    protected override void OnMouseMove(MouseMoveEventArgs e) {
        game.OnMouseMove(e);

        base.OnMouseMove(e);
    }


    private void DrawGame() {
        GL.Enable(EnableCap.DepthTest);
        GL.Enable(EnableCap.CullFace);
        //GL.Enable(EnableCap.Multisample);
        GL.Enable(EnableCap.Blend);
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);
        GL.ClearColor(Color.Cyan);

        game.UpdateMatrices();
        game.Draw();

        Context.SwapBuffers();
    }

    protected override void OnResize(ResizeEventArgs e) {
        GL.Viewport(0, 0, e.Width, e.Height);

        base.OnResize(e);
    }
}