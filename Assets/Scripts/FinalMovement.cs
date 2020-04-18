using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FinalMovement : Movement
{
    public bool isJump, isDashing;
    public float jumpForce, dashForce;
    public float dashDuration, dashCoolDown;
    private float dashTime;
    public float shadowDuration, shadowInter;

    bool jumpPressed;
    int jumpCount;

    public AudioSource source;
    public AudioClip dashClip;
    public AudioClip jumpClip;
    
    void Update()
    {
        if (Input.GetButtonDown("Jump") && jumpCount > 0) {
            jumpPressed = true;
        }
        
        bool canDash = !attacker.IsAttack && attacker.isBeHit == 0;
        if(!isDashing) {
            if(!canDash) return;
            if (dashTime == 0f && Input.GetButtonDown("Dash")) {
                isDashing = true;
                dashTime = Time.time + dashDuration;
                shadowTime = Time.time + shadowInter;
                
                source.Stop();
                source.clip = dashClip;
                source.Play();
            } else if (Time.time >= dashTime){
                dashTime = 0f;
            }
        } else {
            if(Time.time >= dashTime){
                isDashing = false;
                dashTime = Time.time + dashCoolDown;
            } else if(Time.time >= shadowTime) {
                shadowTime = Time.time + shadowInter;
                if(canDash)
                    DashShadow.CreateShadow(sprite.sprite, transform, shadowDuration);
            }
        }
    }

    public new void OnTriggerEnter2D(Collider2D col){
        base.OnTriggerEnter2D(col);
        if(col.name == "winZone"){
            Destroy(col.gameObject);
            GameManager.instance.Win();
        }
    }
    
    void FixedUpdate()
    {
        isGround = 
            rb.velocity.y <= 0.05f && (
                CheckRayCast(coll.bounds.min) || 
                CheckRayCast(new Vector2(coll.bounds.max.x, coll.bounds.min.y)));

        if(!isDashing)
            GroundMovement();
        
        if(!attacker.IsAttack){
            Jump();
            Dashing();
            SwitchAnim();
        }
    }

    void Jump()//跳跃
    {
        if (isGround){
            jumpCount = 2;//可跳跃数量
            isJump = false;
        }
        
        if (jumpPressed) {
            if(jumpCount == 2){
                if(!isGround)
                    jumpCount = 1;
                isJump = true;
            }
            
            if(!isJump || jumpCount == 0) return;
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            jumpCount--;
            jumpPressed = false;
            isGround = false;
            
            source.Stop();
            source.clip = jumpClip;
            source.Play();
        }
    }

    protected override void GroundMovement()
    {
        horizontalMove = (int)Input.GetAxisRaw("Horizontal");//只返回-1，0，1
        base.GroundMovement();
    }

    private float shadowTime = 0f;
    void Dashing(){
        if(isDashing)
            rb.velocity = new Vector2(dashForce * transform.localScale.x, rb.velocity.y);
    }

    protected override void SwitchAnim()//动画切换
    {
        anim.SetBool("falling", false);
        anim.SetBool("jumping", false);
        if (!isGround && rb.velocity.y > 0){
            anim.SetBool("jumping", true);
        } else if (!isGround && rb.velocity.y < 0) {
            anim.SetBool("falling", true);
        }
        
        if(isDashing)
            return;
        
        anim.SetFloat("running", Mathf.Abs(rb.velocity.x));
    }
}

public class DashShadow : MonoBehaviour {
    private const int CapNum = 5;
    private static Queue<DashShadow> pool = new Queue<DashShadow>();
    public static void Purge(){
        pool.Clear();
    }
    
    public static void CreateShadow(Sprite sp, Transform transform, float duration){
        DashShadow sha = null;
        if(pool.Count == 0){
            var obj = new GameObject("shadow");
            sha = obj.AddComponent<DashShadow>();
        } else {
            sha = pool.Dequeue();
        }

        sha.transform.position = transform.position;
        sha.transform.localScale = transform.localScale;
        sha.UpdateSprite(sp, duration);
        sha.gameObject.SetActive(true);
    }

    public static void RestoreShadow(DashShadow shadow){
        shadow.gameObject.SetActive(false);
        pool.Enqueue(shadow);
    }

    private float ctime, duration;
    private SpriteRenderer spr = null;
    public void UpdateSprite(Sprite sp, float duration){
        if(this.spr == null)
            this.spr = gameObject.AddComponent<SpriteRenderer>();
        
        this.spr.sprite = sp;
        this.spr.sortingOrder = 1;
        this.ctime = this.duration = duration;
    }

    float tmp = 0f;
    void Update(){
        if(ctime > 0){
            ctime -= Time.deltaTime;
            var color = this.spr.color;
            if(ctime <= 0){
                color.a = 1;
                this.spr.color = color;
                RestoreShadow(this);
            } else {
                color.a = Mathf.SmoothDamp(color.a, 0f, ref tmp, duration);
                this.spr.color = color;
            }
        }
    }
}