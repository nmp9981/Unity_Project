public class TransformUtility
{
    
}

public class Transform2D
{
    public Vec2 position;
    public float rotation;
    public Vec2 scale = VectorMathUtils.OneVector2D();

    public Mat3 LocalToWorld { get; set; }
    public Mat3 WorldToLocal { get; set; }

    bool dirty = true;

    public void SetDirty()
    {
        dirty = true;
    }

    public void UpdateMatrices()
    {
        if (!dirty) return;

        Mat3 T = MatrixUtility.Translate(position);
        Mat3 R = MatrixUtility.Rotate(rotation);
        Mat3 S = MatrixUtility.Scale(scale);

        LocalToWorld = T * R * S;

        // 구조적 역행렬 (Gauss-Jordan 금지)
        Mat3 invS = MatrixUtility.Scale(new Vec2(
            1.0f / scale.x,
            1.0f / scale.y
        ));
        Mat3 invR = MatrixUtility.Rotate(-rotation);
        Mat3 invT = MatrixUtility.Translate((-1f)*position);

        WorldToLocal = invS * invR * invT;

        dirty = false;
    }
}
