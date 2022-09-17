namespace Core;
public class VAO : IDisposable {
    public int ID { get; private set; } = 0;
    public int[] BufferIDs { get; private set; } = new int[3];
    public int IndicesID;

    private List<Vector3> Positions = new();
    private List<Vector3> UVs = new();
    private List<float> Intensities = new();
    public List<uint> Indices = new();

    public int VertexCount => Positions.Count;
    public int IndicesCount => Indices.Count;

    public VAO() {
        ID = GL.GenVertexArray();
        GL.GenBuffers(BufferIDs.Length, BufferIDs);

        IndicesID = GL.GenBuffer();
    }

    public void Add(Vector3 position) {
        Positions.Add(position);
        Intensities.Add(1.0f);
    }

    public void Add(Vector3 position, Vector3 uvCoords) {
        Positions.Add(position);
        UVs.Add(uvCoords);
        Intensities.Add(1.0f);
    }

    public void Add(Vector3 position, Vector3 uvCoords, float intensity) {
        Positions.Add(position);
        UVs.Add(uvCoords);
        Intensities.Add(intensity);
    }

    public void AddIndices(uint[] indices) => Indices.AddRange(indices);

    public void UploadData() {
        GL.BindVertexArray(this.ID);

        // Vertex Buffer
        GL.BindBuffer(BufferTarget.ArrayBuffer, BufferIDs[0]);
        GL.BufferData(
            BufferTarget.ArrayBuffer, Positions.Count * Vector3.SizeInBytes, 
            Positions.ToArray(), BufferUsageHint.StaticDraw
        );

        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);
        GL.EnableVertexAttribArray(0);

        // UVs Buffer
        GL.BindBuffer(BufferTarget.ArrayBuffer, BufferIDs[1]);
        GL.BufferData(
            BufferTarget.ArrayBuffer, UVs.Count * Vector3.SizeInBytes, 
            UVs.ToArray(), BufferUsageHint.StaticDraw
        );
        GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 0, 0);
        GL.EnableVertexAttribArray(1);

        // Color Intensity Buffer [Util for blocks]
        if (Intensities.Count > 0) {
            GL.BindBuffer(BufferTarget.ArrayBuffer, BufferIDs[2]);
            GL.BufferData(
                BufferTarget.ArrayBuffer, Intensities.Count * sizeof(float), 
                Intensities.ToArray(), BufferUsageHint.StaticDraw
            );
            GL.VertexAttribPointer(5, 1, VertexAttribPointerType.Float, true, 0, 0);
            GL.EnableVertexAttribArray(5);
        }

        // Element Array Buffer
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, IndicesID);
        GL.BufferData(
            BufferTarget.ElementArrayBuffer, Indices.Count * sizeof(uint), 
            Indices.ToArray(), BufferUsageHint.StaticDraw  
        );

        float size = Positions.Count * Vector3.SizeInBytes
            + Intensities.Count * sizeof(float)
            + UVs.Count * Vector2.SizeInBytes
            + Indices.Count * sizeof(uint);
        Util.Logger.Info(
            $"VAO {ID} Size:\n      - {size} Bytes\n      - {(int)(size / 1024.0f)} KBs"
        );
    }
    
    public void Clear() {
        Positions.Clear();
        UVs.Clear();
        Indices.Clear();
        Intensities.Clear();
    }

    public void Draw() {
        GL.BindVertexArray(this.ID);
        GL.DrawElements(BeginMode.Triangles, Indices.Count, DrawElementsType.UnsignedInt, 0);
    }

    public void Dispose() {
        GL.DeleteBuffers(BufferIDs.Length, BufferIDs);
        GL.DeleteBuffer(IndicesID);
        GL.DeleteVertexArray(this.ID);
    }
}