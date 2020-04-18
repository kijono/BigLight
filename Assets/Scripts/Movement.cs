using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Movement : MonoBehaviour
{
    protected Rigidbody2D rb;
    protected BoxCollider2D coll;
    public float rayCastLength;

    protected Animator anim;
    public Attacker attacker;

    public float speed;
    public int horizontalMove = 1;
    public LayerMask ground;

    public bool isGround;

    protected SpriteRenderer sprite;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<BoxCollider2D>();
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        
    }

    public void OnTriggerEnter2D(Collider2D col){
        if(col.name == "GameManager" && attacker.isAlive){
            attacker.Die();
        }
    }

    private void FixedUpdate()
    {
        if(coll != null && !coll.isTrigger){
            isGround = 
                rb.velocity.y <= 0.05f && (
                    CheckRayCast(coll.bounds.min) || 
                    CheckRayCast(new Vector2(coll.bounds.max.x, coll.bounds.min.y)));
        }
        
        GroundMovement();
        SwitchAnim();
    }

    protected bool CheckRayCast(Vector2 pos){
        var ret = Physics2D.Raycast(pos, Vector2.down, rayCastLength, ground);
        Debug.DrawLine(pos, pos + new Vector2(0, -rayCastLength), (ret.collider != null) ? Color.red : Color.green);
        return ret.collider != null;
    }

    public bool isStop = false, isForce = false;
    protected virtual void GroundMovement()
    {
        if (horizontalMove != 0)
            transform.localScale = new Vector3(horizontalMove, 1, 1);
        
        if(isStop){
            rb.velocity = Vector2.zero;
        } else if (attacker.isBeHit == 0 && !attacker.IsAttack && !attacker.isFollow){
            rb.velocity = new Vector2(horizontalMove * speed, rb.velocity.y);
        } else {
            if(attacker.isBeHit == 1){
                rb.velocity = new Vector2(attacker.hitForce.x, rb.velocity.y);
            } else if(attacker.IsAttack || attacker.isFollow){
                rb.velocity = new Vector2(attacker.attForce.x * transform.localScale.x, 
                    attacker.attForce.y == 0 ? rb.velocity.y : attacker.attForce.y);
            }
        }
    }

    protected virtual void SwitchAnim()//动画切换
    {
        if(attacker.isBeHit == 0)
            anim.SetFloat("running", Mathf.Abs(rb.velocity.x));
        else
            anim.SetFloat("running", 0f);
    }

    public void NextAttack(){
        attacker.CheckNextAttack();
    }

    public void StopAttack(){
        attacker.StopAttack();
    }
    
    public void BeAttackEnd(){
        attacker.BeAttackEnd();
    }

    public void StopCollision(){
        attacker.StopCollision();
    }
}