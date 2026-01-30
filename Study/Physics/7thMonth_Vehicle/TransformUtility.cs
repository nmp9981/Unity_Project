public class TransformUtility
{
    
}
public class Transform3D
{
    // ===== 데이터 =====
    Vec3 _position;
    Mat3 _rotation; // orthonormal (R * R^T = I)
    Vec3 _scale = VectorMathUtils.OneVector3D();

    // ===== 접근자 (자동 dirty) =====
    public Vec3 position
    {
        get => _position;
        set { _position = value; dirty = true; }
    }

    public Mat3 rotation
    {
        get => _rotation;
        set { _rotation = value; dirty = true; }
    }
    public Vec3 scale
    {
        get => _scale;
        set { _scale = value; dirty = true; }
    }

    // ===== 행렬 =====
    public Mat4 LocalToWorld { get; set; }


    public Mat4 WorldToLocal { get; private set; }

    bool dirty = true;

    // ===== 수동 dirty =====
    public void SetDirty() => dirty = true;

    // ===== 갱신 =====
    public void UpdateMatrices()
    {
        if (!dirty) return;

        // Local → World
        LocalToWorld = MatrixUtility.TR(_position, _rotation);

        // World → Local (구조적 역행렬)
        WorldToLocal = MatrixUtility.InverseTR(_position, _rotation);

        dirty = false;
    }
    public Vec3 Up
    {
        get { return rotation * VectorMathUtils.UpVector3D(); }
    }

    public Vec3 Forward
    {
        get { return rotation * VectorMathUtils.FrontVector3D(); }
    }

    public Vec3 Right
    {
        get { return rotation * VectorMathUtils.RightVector3D(); }
    }
}
public class Transform2D
{
    // ===== 데이터 =====
    Vec2 _position;
    float _rotation; // rad
    Vec2 _scale = VectorMathUtils.OneVector2D();

    // ===== 접근자 =====
    public Vec2 position
    {
        get => _position;
        set { _position = value; dirty = true; }
    }

    public float rotation
    {
        get => _rotation;
        set { _rotation = value; dirty = true; }
    }

    public Vec2 scale
    {
        get => _scale;
        set { _scale = value; dirty = true; }
    }

    // ===== 행렬 =====
    public Mat3 LocalToWorld { get; private set; }
    public Mat3 WorldToLocal { get; private set; }

    bool dirty = true;

    public void SetDirty() => dirty = true;

    // ===== 갱신 =====
    public void UpdateMatrices()
    {
        if (!dirty) return;

        // Local → World
        Mat3 T = MatrixUtility.Translate(_position);
        Mat3 R = MatrixUtility.Rotate(_rotation);
        Mat3 S = MatrixUtility.Scale(_scale);

        LocalToWorld = T * R * S;

        // ===== 구조적 역행렬 =====
        float invX = MathUtility.Abs(_scale.x) < MathUtility.EPSILON ? 0.0f : 1.0f / _scale.x;
        float invY = MathUtility.Abs(_scale.y) < MathUtility.EPSILON ? 0.0f : 1.0f / _scale.y;

        Mat3 invS = MatrixUtility.Scale(new Vec2(invX, invY));
        Mat3 invR = MatrixUtility.Rotate(-_rotation);
        Mat3 invT = MatrixUtility.Translate((-1f)*_position);

        WorldToLocal = invS * invR * invT;

        dirty = false;
    }
    public Vec2 Up
    {
        get { return rotation * VectorMathUtils.UpVector2D(); }
    }
    public Vec2 Right
    {
        get { return rotation * VectorMathUtils.RightVector2D(); }
    }
}
