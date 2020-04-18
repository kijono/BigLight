using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIAttacker : Attacker
{
    public Transform limitLeft, limitRight;
    private Vector3 limitl, limitr;
    void Start(){
        if(limitLeft != null){
            limitl = limitLeft.position;
            limitr = limitRight.position;
        }
    }

    protected override void DoAttack(Attacker att){
        att.BeAttack(this, attData);
        if(isTouchAttack)
            StopAttack();
    }

    public override void BeAttackEnd(){
        base.BeAttackEnd();

        if(moveType == 2){
            DoStop();
        }
    }

    public override void Die(){
        isBeHit = 0;
        base.Die();

        Destroy(mv.gameObject);
    }

    protected new void Update()
    {
        base.Update();
        if(isBeHit == 0)
            DoAI();
    }

    protected void DoAI(){
        DoMove();
    }

    public int moveType = 1;
    public FindTarget findTarget;

    private float stopTime = 0f;
    protected virtual void DoMove(){
        if(moveType == 1){
            var ps = mv.transform.position;
            if(ps.x < limitl.x || ps.x > limitr.x){
                mv.horizontalMove *= -1;
                mv.transform.localScale = new Vector3(mv.horizontalMove, 1, 1);

                ps.x = ps.x < limitl.x ? limitl.x : limitr.x;
                mv.transform.position = ps;

                findTarget.Reset();
                StopAttack(true);
            }
        } else if(moveType == 2) {
            if(mv.isStop){
                if(Time.time >= stopTime){
                    DoBack();
                }
            } else if(isFollow){
                if(stopTime > 0){
                    if(Time.time >= stopTime){
                        stopTime = 0f;
                        isFollow = false;

                        findTarget.Reset();
                        findTarget.enabled = true;
                    }
                } else {
                    MoveToTarget(mv.speed, ref attForce.x, ref attForce.y);
                }
            } else {
                if(transform.position.y < targetPos.y){
                    DoStop();
                }
            }
        }
    }

    public float stopDuration = 0.5f;
    private void DoStop(){
        StopAttack(true);
                    
        mv.isStop = true;
        findTarget.enabled = false;
        stopTime = Time.time + stopDuration;
    }

    public float backTime = 1f;
    public float backSpeed = 10f;
    private void DoBack(){
        mv.isStop = false;

        isFollow = true;
        attForce.x = 0f; attForce.y = backSpeed;
        stopTime = Time.time + backTime;
    }

    private Attacker target = null;
    private Vector3 targetPos = Vector3.zero;

    public Transform targetPosobj;
    public virtual void FindTarget(Attacker tar, bool isCheck = false){
        if(moveType == 1){
            target = tar;
            StartAttack(0);
        } else if(moveType == 2) {
            target = tar;
            isFollow = isCheck;
            
            if(!isCheck){
                StartAttack(0);
                MoveToTarget(attData.attForce.x, ref attForce.x, ref attForce.y);
            } else {
                MoveToTarget(mv.speed, ref attForce.x, ref attForce.y);
            }
        }
    }
    
    private void MoveToTarget(float speed, ref float vx, ref float vy){
        targetPos = target.transform.position;
        if(targetPosobj != null)
            targetPosobj.position = targetPos;

        if(transform.position.x < targetPos.x){
            mv.horizontalMove = 1;
        } else if(transform.position.x > targetPos.x){
            mv.horizontalMove = -1;
        }
        
        var off = transform.position - targetPos;
        var dis = Vector3.Distance(targetPos, transform.position);
        vx = speed * Mathf.Abs(off.x / dis);
        vy = -speed * Mathf.Abs(off.y / dis);
    }
}
