using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindTarget : MonoBehaviour
{
    public AIAttacker attacker;
    public Collider2D colCheck;
    public Collider2D colAttack;

    private Collider2D target;
    public void OnTriggerEnter2D(Collider2D col){
        if(target != null && col != target)
            return;

        var att = col.gameObject.GetComponentInChildren<Attacker>();
        if(att == null || attacker.hostile == att.hostile) 
            return;

        target = col;
        if(colCheck != null && colCheck.enabled){
            colCheck.enabled = false;
            colAttack.enabled = true;
            attacker.FindTarget(att, true);
        } else {
            colAttack.enabled = false;
            attacker.FindTarget(att);
        }
    }
    
    public void Reset(){
        target = null;
        if(colCheck != null){
            colCheck.enabled = true;
            colAttack.enabled = false;
        } else{
            colAttack.enabled = true;
        }
    }
}
