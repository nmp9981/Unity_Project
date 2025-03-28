// 종횡비
var faceSize = faceData.size;
camera.aspect = faceSize.x / faceSize.y;

// 화각
var positionOffset = Quaternion.Inverse(Room.FaceToRot(faceData.face)) * -eyePointRoomLocal;
var distance = faceData.distance + positionOffset.z;
camera.fieldOfView = 2f * Mathf.Atan2(faceSize.y * 0.5f, distance) * Mathf.Rad2Deg;

// 최대 시야거리 설정
camera.farClipPlane = distance * 1000f;

// 투영면 이동
var shift = new Vector2(positionOffset.x / faceSize.x, positionOffset.y / faceSize.y) * 2f;
var projectionMatrix = camera.projectionMatrix;
projectionMatrix[0, 2] = shift.x;
projectionMatrix[1, 2] = shift.y;

camera.projectionMatrix = projectionMatrix;
