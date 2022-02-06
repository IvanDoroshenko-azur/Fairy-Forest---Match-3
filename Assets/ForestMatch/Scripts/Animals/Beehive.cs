using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Beehive : MonoBehaviour
{
    private HingeJoint2D hing;
    private Animator animCtrl;
    private CircleCollider2D col;
    private Rigidbody2D rig;

    private int stat = 0;
    void Start()
    {
        hing = gameObject.GetComponent<HingeJoint2D>();
        animCtrl = gameObject.GetComponent<Animator>();
        col = gameObject.GetComponent<CircleCollider2D>();
        rig = gameObject.GetComponent<Rigidbody2D>();
    }

   

     void OnMouseDown()
    {
        // Debug.Log("Bee HIt!");
        stat++;
        if (stat == 1)
        {
            rig.AddForce(Vector2.right, ForceMode2D.Impulse);
            animCtrl.SetInteger("statBeehive", 1);
        }
        else if(stat > 1)
            hing.enabled = false;
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        rig.simulated = false;
        col.enabled = false;
        animCtrl.SetInteger("statBeehive", 2);    
    }
}
