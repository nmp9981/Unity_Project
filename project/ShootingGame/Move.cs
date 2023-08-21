using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
    [SerializeField] Animator moveAnim;
    // Start is called before the first frame update
    void Awake()
    {
        moveAnim = GetComponent<Animator>();
        this.transform.position = new Vector2(0, -3);
    }

    // Update is called once per frame
    void Update()
    {
        MovePlayer();
    }
    private void MovePlayer()
    {
        float h = Input.GetAxisRaw("Horizontal");//좌우 즉시 이동
        
        float v= Input.GetAxisRaw("Vertical");//상하 즉시 이동

        Vector2 CurPos = transform.position;
        Vector2 deltaPos = new Vector2(h, v) * GamaManager.Instance.PlayerSpeed * Time.deltaTime;
        this.transform.position = CurPos + deltaPos;//최종 위치

        if(Input.GetButtonUp("Horizontal") || Input.GetButtonDown("Horizontal"))
        {
            moveAnim.SetInteger("Move", (int)h);
        }
    }
}
