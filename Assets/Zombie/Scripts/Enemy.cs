using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour {
    public float discoverDistance = 3;
    public float loseTargetDistance = 8;
    GameObject[] targets;
    GameObject target = null;
    NavMeshAgent nav;
    Animator animator;

	// Use this for initialization
	void Awake () {
        animator = GetComponent<Animator>();
        nav = GetComponent<NavMeshAgent>();
        targets = GameObject.FindGameObjectsWithTag("Player");
    }


	// Update is called once per frame
	void Update () {

        //Check target lose
        if (target != null)
        {
            float distance = (target.transform.position - transform.position).magnitude;
            if (distance > loseTargetDistance)
                target = null;
        }

        //Find closest target
        float minDistance = float.MaxValue;
        foreach (GameObject e in targets)
        {
            float distance = (e.transform.position - transform.position).magnitude;
            if (distance <= discoverDistance && distance < minDistance)
            {
                target = e;
            }
        }

        //Update destination
        if (target != null)
        {
            nav.enabled = true;
            nav.SetDestination(target.transform.position);
        }
        else
        {
            nav.enabled = false;
        }
    }

    void FixedUpdate()
    {
        animator.SetFloat("Speed", !nav.enabled || nav.isStopped ? 0.0f : 1.0f);
    }

    void OnTriggerEnter(Collider collider)
    {
        if(collider.gameObject == target)
        {
            animator.SetBool("Attack",true);
        }
    }
    void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject == target)
        {
            animator.SetBool("Attack", false);
        }
    }

    public void Attack()
    {

    }

    public void AttackFinished()
    {
    }
}
