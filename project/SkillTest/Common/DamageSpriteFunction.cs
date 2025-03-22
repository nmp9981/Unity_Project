using UnityEngine;

public class DamageSpriteFunction : MonoBehaviour
{
    private void OnEnable()
    {
        Invoke("EraseDamageImage", 1f);
    }
    /// <summary>
    /// 데미지 지우기 : 생성후 1초뒤에 실행
    /// </summary>
    void EraseDamageImage()
    {
        gameObject.SetActive(false);
    }
}
