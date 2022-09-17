namespace Core;
public class Program {
    public static void Main() {
        Util.Logger.LogMainInfo("Initializing application");
        using (Application app = new Application()) {
            app.Run();
        }
        Util.Logger.LogMainInfo("Closing application");
    }
}