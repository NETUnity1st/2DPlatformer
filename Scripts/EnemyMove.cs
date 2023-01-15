using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMove : MonoBehaviour
{
    Rigidbody2D rigid;
    public int nextMove;
    Animator anim;
    SpriteRenderer spriteRenderer;
    CapsuleCollider2D capuslecollider;
    // Start is called before the first frame update
    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        capuslecollider = GetComponent<CapsuleCollider2D>();
        //Think();
        spriteRenderer.flipX = (nextMove == 1);
        Invoke("Think", 5);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //Move
        rigid.velocity = new Vector2(2*nextMove, rigid.velocity.y);
        //지형 체크
        Debug.DrawRay(rigid.position + Vector2.right*(nextMove) * 0.7f + Vector2.down, Vector3.down, new Color(0, 1, 0));
        RaycastHit2D rayHit = Physics2D.Raycast(rigid.position + Vector2.right*(nextMove)*0.7f + Vector2.down*0.5f , Vector3.down, 1, LayerMask.GetMask("Platform"));
        if (rayHit.collider == null)
        {
            CancelInvoke();
            Invoke("Think", 2);
            nextMove = -nextMove;
            rigid.velocity = new Vector2(nextMove, rigid.velocity.y);
            spriteRenderer.flipX = (nextMove == 1);
            Debug.Log("Warning");    
        }
        if(rigid.transform.position.y < -100){
            CancelInvoke();
            DeActive();
        }
    }

    void Think()
    {
        Debug.Log("Thinking..."); 
        nextMove = Random.Range(-1, 2);
        
        //쳐다보는 방향에 대한 코드
        spriteRenderer.flipX = (nextMove == 1);

        float nextThinkTime = Random.Range(2f,5f);
        Invoke("Think", nextThinkTime);
        
    }
    void Update()
    {
        //움직이는 애니메이션
        if (Mathf.Abs(rigid.velocity.x) < 0.3)
            anim.SetBool("IsWalking", false);
        else
            anim.SetBool("IsWalking", true);
    }
    
    public void OnDamaged()
    {
        nextMove = 0;
        spriteRenderer.color = new Color(1,1,1,0.4f);

        spriteRenderer.flipY = true;

        capuslecollider.enabled = false;

        rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);
        Invoke("DeActive",2.0f);
    }
    public void DeActive()
    {
        gameObject.SetActive(false);
    }
}
