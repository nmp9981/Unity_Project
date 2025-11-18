Vector3 lastFixedPos;
void FixedUpdate() {
    lastFixedPos = transform.position;
}

void Update() {
    Debug.DrawLine(transform.position, lastFixedPos, Color.red);
}
