using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Attacker
{
    void Start(){
        UpdateHp();
    }

    private new void Update(){
        if(!isAlive) return;

        base.Update();
        if(isBeHit != 0) 
            return;

        if (Input.GetButtonDown("Fire1")) {
            if(!IsAttack){
                StartAttack(0);
            } else {
                CacheNextAttack();
            }
        }
    }
    
    public override void BeAttack(Attacker attacker, AttackData attData){
        if(isBeHit != 0) return;
        base.BeAttack(attacker, attData);
    }

    public override void Die(){
        base.Die();
        
        anim.SetTrigger("hit");
        rb.velocity = new Vector2(0, 15f);
        StartCoroutine(GameOver());
    }
    
    private IEnumerator GameOver(){
        yield return new WaitForSeconds(0.7f);
        GameManager.instance.GameOver();
    }

    private List<GameObject> listHp = new List<GameObject>();
    public GameObject lightObj;
    public RectTransform ui;
    protected override void UpdateHp(){
        if(listHp.Count < hp){
            var off = hp - listHp.Count;
            for(int i = 0; i < off; ++i){
                var obj = Instantiate(lightObj);
                obj.transform.SetParent(ui);
                listHp.Add(obj);
            }
            return;
        }

        for(int i = listHp.Count - 1; i >= 0; --i)
            listHp[i].gameObject.SetActive(hp > i);
    }
}
