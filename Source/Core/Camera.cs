namespace Core;
public class Camera {
    public Vector3 Position = Vector3.Zero;

    public float Sensitivity = 0.60f;
    public float FOV = 80.0f;

    private float pitch = 0.0f;
    private float yaw = -90.0f;

    public Vector3 Direction => new Vector3(
        (float)Math.Cos(MathHelper.DegreesToRadians(yaw)) * (float)Math.Cos(MathHelper.DegreesToRadians(pitch)), 
        (float)Math.Sin(MathHelper.DegreesToRadians(pitch)), 
        (float)Math.Sin(MathHelper.DegreesToRadians(yaw)) * (float)Math.Cos(MathHelper.DegreesToRadians(pitch))
    );
    public Vector3 FrontDirection => Vector3.Normalize(Direction);
    
    public Matrix4 GetMatrix() {
        return Matrix4.LookAt(
            Position, Position + FrontDirection, new Vector3(0.0f, 1.0f, 0.0f)
        );
    }

    public void Rotate(float pitch, float yaw) {
        this.pitch = Math.Clamp(this.pitch + pitch, -89.0f, 89.0f);
        this.yaw = (this.yaw + yaw) % 360.0f;
    }
}