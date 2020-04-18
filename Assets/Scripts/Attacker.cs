using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AttackData{
    public Vector2 hitForce, attForce;
    public float timeScale, timeScaleDur, shakeTime;
    public int att = 1;
}

public class Attacker : MonoBehaviour
{
    public BoxCollider2D attackColl = null;
    public BoxCollider2D hitColl = null;
    public List<AttackData> attackDatas;
    public int hp, hostile = 1;
    public bool isTouchAttack = false;

    protected AttackData attData = null;
    public Animator anim = null;
    public Rigidbody2D rb = null;
    public Movement mv = null;

    public int nextAttId = -1, nextAttId1 = -1;
    public void CacheNextAttack(){
        if(nextAttId1 != -1) return;
        
        var nId = (nextAttId != -1) ? (nextAttId + 1) : (attId + 1);
        if(nId >= 0 && nId <= attackDatas.Count - 1){
            if(nextAttId == -1) nextAttId = nId;
            else nextAttId1 = nId;

            if(doNext) {
                StartAttack(nextAttId);
            }
        } else {
            nextAttId1 = -1;
            if(nextAttId == -1)
                doNext = false;
        }
    }

    private bool doNext = false;    
    public bool CheckNextAttack(){
        if(nextAttId != -1){
            StartAttack(nextAttId);
            return true;
        } else {
            doNext = true;
            return false;
        }
    }

    public int attId = -1;
    public void StartAttack(int id){
        if(attId != -1)
            anim.ResetTrigger("att" + attId);

        attId = id;
        doNext = false;
        nextAttId = -1;
        if(nextAttId1 != -1){
            nextAttId = nextAttId1;
            nextAttId1 = -1;
        }

        attData = attackDatas[id];
        anim.ResetTrigger("hit");
        anim.ResetTrigger("idle");
        anim.SetTrigger("att" + attId);
        
        attForce.x = attData.attForce.x;
        attackColl.enabled = false;
        attackColl.enabled = true;
    }

    public void StopCollision(){
        if(isTouchAttack) 
            return;

        attackColl.enabled = false;
    }

    public virtual void StopAttack(bool force = false){
        if(isTouchAttack && !force) return;

        attId = -1;
        attData = null;
        attackColl.enabled = false;

        if(!CheckNextAttack()){
            anim.SetTrigger("idle");
        }
    }
    
    public bool IsAttack{ get { return attId >= 0; }}
    public bool isFollow = false;

    public void OnTriggerEnter2D(Collider2D col){
        if(attId >= 0 && col != hitColl && col.name != "attack"){
            var att = col.gameObject.GetComponentInChildren<Attacker>();
            if(att != null && att.isAlive && att.hostile != hostile){
                DoAttack(att);
            }
        }
    }

    protected virtual void DoAttack(Attacker att){
        att.BeAttack(this, attData);
    }

    public int isBeHit = 0;
    public Vector2 hitForce = Vector2.zero;
    public Vector2 attForce = Vector2.zero;
    public virtual void BeAttack(Attacker attacker, AttackData attData){
        if(!isAlive)
            return;
        
        if(IsAttack){
            anim.ResetTrigger("att" + attId);
            StopAttack(true);
        }

        anim.ResetTrigger("idle");
        anim.ResetTrigger("hit");
        anim.SetTrigger("hit");
        
        if(attData.timeScale > 0){
            GameManager.instance.SetTimeScale(attData.timeScale, attData.timeScaleDur);
        }

        if(attData.shakeTime > 0){
            CameraManager.instance.SetShake(attData.shakeTime);
        }

        hp -= attData.att;
        UpdateHp();

        if(hp <= 0) {
            Die();
        } else {
            isBeHit = attData.hitForce.y > 0 ? 2 : 1;
            int dir = (int)attacker.transform.lossyScale.x;

            hitForce.y = attData.hitForce.y;
            hitForce.x = attData.hitForce.x * dir;
            rb.velocity = hitForce;
        }
    }

    protected virtual void UpdateHp(){

    }

    public virtual void BeAttackEnd(){
        if(isBeHit == 1 || hitColl.isTrigger){
            isBeHit = 0;
            anim.ResetTrigger("hit");
            anim.SetTrigger("idle");
        } else if(isBeHit != 0){
            isBeHit = 3;
        }
    }

    public bool isAlive = true;
    public virtual void Die(){
        isAlive = false;
    }

    protected void Update()
    {
        if(isBeHit == 3 && mv.isGround){
            isBeHit = 0;
            anim.SetTrigger("idle");
        }
    }
}
