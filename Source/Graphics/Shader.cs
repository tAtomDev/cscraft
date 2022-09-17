namespace Core;
public class Shader : IDisposable {
    public int ID { get; private set; }
    private ShaderType _type;

    public Shader(string path, ShaderType type) {
        _type = type;
        ID = GL.CreateShader(type);

        string source = File.ReadAllText(path);
        GL.ShaderSource(ID, source);
        GL.CompileShader(ID);

        string log = GL.GetShaderInfoLog(ID);
        if (!String.IsNullOrEmpty(log)) {
            throw new Exception(log);
        }

        Util.Logger.Success($"Created {_type.ToString()} ID {ID} (+)");
    }

    public void Dispose() {
        GL.DeleteShader(ID);
        Util.Logger.Success($"Deleted {_type.ToString()} ID {ID} (-)");
    }
}