using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public GameManager gameManager;
    public AudioClip audioJump;
    public AudioClip audioAttack;
    public AudioClip audioDamaged;
    public AudioClip audioItem;
    public AudioClip audioDie;
    public AudioClip audioFinish;
    public AudioClip audioShot;
    public float maxSpeedx;
    Rigidbody2D rigid;
    public SpriteRenderer spriteRenderer;
    Animator anim;
    public float jumpPower;
    BoxCollider2D boxcollider;
    AudioSource audioSource;
    public GameObject LeftBullet;
    public GameObject RightBullet;
    // Start is called before the first frame update
    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        boxcollider = GetComponent<BoxCollider2D>();
        audioSource = GetComponent<AudioSource>();
    }

    public void PlaySound(string action)
    {
        switch (action)
        {
            case "JUMP":
                audioSource.clip = audioJump;
                break;
            case "ATTACK":
                audioSource.clip = audioAttack;
                break;
            case "DAMAGED":
                audioSource.clip = audioDamaged;
                break;
            case "ITEM":
                audioSource.clip = audioItem;
                break;
            case "DIE":
                audioSource.clip = audioDie;
                break;
            case "FINISH":
                audioSource.clip = audioFinish;
                break;
            case "SHOT":
                audioSource.clip = audioShot;
                break;
        }
        audioSource.Play();
    }

    void Update()
    {
        //공격(총알)에 관한 코드
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            if(gameManager.Bullet > -2){
                if(spriteRenderer.flipX){
                    GameObject newItem;
                    newItem = Instantiate(LeftBullet);//총알
                    newItem.transform.position =
                    new Vector3( rigid.transform.position.x ,rigid.transform.position.y);
                    Destroy(newItem , 5.0f);
                    gameManager.GETBullet(-1);
                    PlaySound("SHOT");
                }
                else{
                    GameObject newItem;
                    newItem = Instantiate(RightBullet);//총알
                    newItem.transform.position =
                    new Vector3( rigid.transform.position.x ,rigid.transform.position.y);
                    Destroy(newItem , 5.0f);
                    gameManager.GETBullet(-1);
                    PlaySound("SHOT");
                }
            }
        }
        //점프에 관한 코드
        if (Input.GetButtonDown("Jump") && !(anim.GetBool("IsJumping")))
        {
            rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            anim.SetBool("IsJumping", true);
            PlaySound("JUMP");
        }

        //키를 떼면 잘 멈추게 하는 코드
        if (Input.GetButtonUp("Horizontal"))
        {
            // rigid.velocity = new Vector2(0, rigid.velocity.y);
            rigid.velocity = new Vector2(
                0.1f * rigid.velocity.normalized.x
             , rigid.velocity.y);
        }
        //쳐다보는 방향에 관한 코드
        if (Input.GetButton("Horizontal"))
        {
            spriteRenderer.flipX = (Input.GetAxisRaw("Horizontal") == -1);
        }
        //애니메이션 제어를 위한 코드
        if (Mathf.Abs(rigid.velocity.x) < 0.3)
            anim.SetBool("IsWalking", false);
        else
            anim.SetBool("IsWalking", true);
    }
    void FixedUpdate()
    {
        //move speed
        float h = Input.GetAxisRaw("Horizontal");

        rigid.AddForce(Vector2.right * h, ForceMode2D.Impulse);
        //maxspeed
        if (rigid.velocity.x > maxSpeedx)
            rigid.velocity = new Vector2(maxSpeedx, rigid.velocity.y);
        else if (rigid.velocity.x < -maxSpeedx)
            rigid.velocity = new Vector2(-maxSpeedx, rigid.velocity.y);
        //landing platform
        if (rigid.velocity.y <= 0) //내려가고 있을때만
        {
            Debug.DrawRay(rigid.position, Vector3.down, new Color(0, 1, 0));
            RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, Vector3.down, 1, LayerMask.GetMask("Platform"));
            if (rayHit.collider != null)
            {
                Debug.Log(rayHit.collider.name);
                if (rayHit.distance < 1.0f)
                    anim.SetBool("IsJumping", false);
            }
        }
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Item")
        {
            bool IsBronze = other.gameObject.name.Contains("Bronze");
            bool IsSilver = other.gameObject.name.Contains("Silver");
            bool IsGold = other.gameObject.name.Contains("Gold");
            if (IsBronze)
                gameManager.stagePoint += 50;
            else if (IsSilver)
                gameManager.stagePoint += 100;
            else if (IsGold)
                gameManager.stagePoint += 300;

            gameManager.GETBullet(1);
            other.gameObject.SetActive(false);
            PlaySound("ITEM");
        }
        if (other.gameObject.tag == "Finish")
        {
            PlaySound("FINISH");
            gameManager.NextStage();
        }
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            if (rigid.velocity.y < 0.1 &&
             transform.position.y+0.3 > other.transform.position.y &&
              other.gameObject.name != "Spikes")
            {
                OnAttack(other.transform);
                PlaySound("ATTACK");
            }
            else
            {
                OnDamaged(other.transform.position);
                
            }
        }
    }

    void OnAttack(Transform enemy)
    {
        rigid.AddForce(Vector2.up * 7, ForceMode2D.Impulse);
        gameManager.stagePoint += 200;
        EnemyMove enemyMove = enemy.GetComponent<EnemyMove>();
        enemyMove.OnDamaged();
        // enemyMove.DeActive();
    }
    void OnDamaged(Vector2 targetPos)
    {
        gameManager.healthDown();
        if(gameManager.health == 0)
            PlaySound("DIE");
        else
            PlaySound("DAMAGED");
        //Change Layer
        gameObject.layer = 9; //LayerMask.GetMask("PlayerDamaged");
        //피격시 투명
        spriteRenderer.color = new Color(1, 1, 1, 0.4f);
        //reaction force
        int dirc = transform.position.x - targetPos.x > 0 ? 1 : -1;
        rigid.AddForce(new Vector2(dirc, 1) * 7, ForceMode2D.Impulse);

        anim.SetBool("IsJumping", true);
        anim.SetTrigger("DoDamaged");
        Invoke("offDamaged", 1);
    }
    void offDamaged()
    {
        gameObject.layer = 8;//LayerMask.GetMask("Player");
        spriteRenderer.color = new Color(1, 1, 1, 1);
    }
    public void OnDie()
    {
        spriteRenderer.color = new Color(1, 1, 1, 0.4f);

        spriteRenderer.flipY = true;

        boxcollider.enabled = false;

        rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);
    }
    public void VelocityZero()
    {
        Time.timeScale = 1;
        rigid.velocity = Vector2.zero;
    }
}
