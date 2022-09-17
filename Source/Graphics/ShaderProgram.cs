namespace Core;
public class ShaderProgram : IDisposable {
    public int ID { get; private set; }

    public ShaderProgram(string vertexShaderPath, string fragmentShaderPath)
        : this(
            new Shader(vertexShaderPath, ShaderType.VertexShader), 
            new Shader(fragmentShaderPath, ShaderType.FragmentShader)
        ) 
    {
    }

    public ShaderProgram(Shader vertexShader, Shader fragmentShader) {
        ID = GL.CreateProgram();
        GL.AttachShader(ID, vertexShader.ID);
        GL.AttachShader(ID, fragmentShader.ID);
        GL.LinkProgram(ID);

        vertexShader.Dispose();
        fragmentShader.Dispose();

        string log = GL.GetProgramInfoLog(ID);
        if (!String.IsNullOrEmpty(log)) {
            throw new Exception(log);
        }

        Util.Logger.Success2($"Created ShaderProgram ID {ID} (+)");
    }

    public ShaderProgram() {
        ID = 0;
    }

    public void UseProgram() {
        GL.UseProgram(ID);
    }

    public void Dispose() {
        GL.DeleteProgram(ID);
        Util.Logger.Success2($"Deleted ShaderProgram ID {ID} (-)");
    }
    
    // Uniform 1
    public void SetUniform1(string uniform, double value) {
        int location = GL.GetUniformLocation(ID, uniform);
        GL.Uniform1(location, value);
    }

    public void SetUniform1(string uniform, float value) {
        int location = GL.GetUniformLocation(ID, uniform);
        GL.Uniform1(location, value);
    }

    public void SetUniform1(string uniform, int value) {
        int location = GL.GetUniformLocation(ID, uniform);
        GL.Uniform1(location, value);
    }

    // Uniform 2
    public void SetUniform2(string uniform, double x, double y) {
        int location = GL.GetUniformLocation(ID, uniform);
        GL.Uniform2(location, x, y);
    }

    public void SetUniform2(string uniform, Vector2 vector) {
        int location = GL.GetUniformLocation(ID, uniform);
        GL.Uniform2(location, vector.X, vector.Y);
    }

    // Uniform 3
    public void SetUniform3(string uniform, double x, double y, double z) {
        int location = GL.GetUniformLocation(ID, uniform);
        GL.Uniform3(location, x, y, z);
    }

    public void SetUniform3(string uniform, Vector3 vector) {
        int location = GL.GetUniformLocation(ID, uniform);
        GL.Uniform3(location, vector.X, vector.Y, vector.Z);
    }

    // Uniform 4
    public void SetUniform4(string uniform, double x, double y, double z, double w) {
        int location = GL.GetUniformLocation(ID, uniform);
        GL.Uniform4(location, x, y, z, w);
    }

    public void SetUniform4(string uniform, Vector4 vector) {
        int location = GL.GetUniformLocation(ID, uniform);
        GL.Uniform4(location, vector.X, vector.Y, vector.Z, vector.W);
    }

    // Uniform Matrix 4
    public void SetUniformMatrix4(string uniform, bool transpose, Matrix4 matrix) {
        int location = GL.GetUniformLocation(ID, uniform);
        GL.UniformMatrix4(location, transpose, ref matrix);
    }

    public void SetUniformMatrix4(string uniform, Matrix4 matrix) {
        int location = GL.GetUniformLocation(ID, uniform);
        GL.UniformMatrix4(location, false, ref matrix);
    }
}