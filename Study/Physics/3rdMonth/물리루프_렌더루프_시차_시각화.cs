Vector3 lastFixedPos;
void FixedUpdate() {
    lastFixedPos = transform.position;
}

void Update() {
    Debug.DrawLine(transform.position, lastFixedPos, Color.red);
}

[프레임 시작]
    Input 업데이트
    Update()		    (1)
    LateUpdate()	    (2)

    while(accumulatedTime >= fixedDeltaTime)
    {
        FixedUpdate()   (3)
        Physics Step    (4)
        Collision       (5)
        accumulatedTime -= fixedDeltaTime
    }

    Rendering		    (6)
[프레임 끝]
