using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Character : MonoBehaviour
{
    public bool debug = false;
    public bool strolling = false;
    public float strollInterval = 1;
    public float strollRange = 2;
    public float discoverRange = 5;
    public float discoverAngle = 360f;
    public float loseTargetRange = 15;
    public float loseTargetTime = 10;
    public float attackRange = 1.0f;
    public float attackAngle = 90f;
    public float attackInterval = 0.5f;
    public float damage = 10;
    public float maxHealth = 100;
    public float health = 100;
    public float healthRestore = 0;
    public string[] enemyTagList = { };
    public bool IsActive { get { return isActive; } }
    
    
    protected NavMeshAgent nav;
    protected float loseTargetTimer;
    protected float attackTimer;
    protected Vector3 velocity;

    protected List<GameObject> targets;
    protected GameObject target;
    protected Animator animator;
    protected Rigidbody rb;
    protected CapsuleCollider collider;
    protected bool isActive;

    private float timer1s;
    private float strollTimer;
    private Vector3 lastPos;
    //Lock one target from targetList
    void Targeting()
    {
        //Check target lost
        if (target != null)
        {
            Character script = target.GetComponent<Character>();
            if (!script.IsActive || loseTargetTimer >= loseTargetTime)
            {
                loseTargetTimer = 0;
                target = null;
                if(debug) print(tag + ": Lost target");
            }
            else
            {
                float distance = (target.transform.position - transform.position).magnitude;
                if (distance > loseTargetRange)
                    loseTargetTimer += Time.deltaTime;
            }
        }
        //Find closest target
        float minDistance = float.MaxValue;
        foreach (GameObject e in targets)
        {
            Character script = e.GetComponent<Character>();
            if (!script.IsActive) continue;
            float distance = (e.transform.position - transform.position).magnitude;
            float angle = Vector3.Angle(e.transform.position - transform.position, transform.forward);
            if (distance <= discoverRange && distance < minDistance && angle < discoverAngle/2f)
            {
                target = e;
                minDistance = distance;
            }
        }
        if (debug) print(tag + " current target: " + (target != null ? target.tag : "None") + " from list of: " + targets.Count);
    }

    //Update targetList with enemy tags
    void UpdateTargets()
    {
        targets.Clear();
        if (debug) print(tag + ": start update enemy list");
        HashSet <GameObject> dedup = new HashSet<GameObject>();
        foreach (string s in enemyTagList)
        {
            GameObject[] arr = GameObject.FindGameObjectsWithTag(s);
            
            foreach (GameObject e in arr)
            {
                Character script = e.GetComponent<Character>();
                if (dedup.Add(e) && script != null && script.IsActive)
                {
                    targets.Add(e);
                }
            }
        }
        if(debug) print(tag+" targets: " + targets.Count);
    }

    //Follow target when target is locked
    void FollowTarget()
    {
        if (nav != null)
        {
            if (target != null)
            {
                float distance = (target.transform.position - transform.position).magnitude;
                if(distance >= attackRange * 0.85f)
                {
                    nav.SetDestination(target.transform.position);
                }
                else
                {
                    nav.ResetPath();
                    transform.forward = Vector3.Lerp(transform.forward, target.transform.position - transform.position, 2f * Time.deltaTime);
                }
            }
            else
            {
                if (strolling)
                {
                    Strolling();
                }
                else
                {
                    nav.ResetPath();
                }
                
            }
                
        }
            
    }

    // Use this for initialization
    protected virtual void Awake () {
        targets = new List<GameObject>();
        rb = GetComponent<Rigidbody>();
        collider = GetComponent<CapsuleCollider>();
        timer1s = 0;
        velocity = new Vector3(0,0,0);
        lastPos = transform.position;
        animator = GetComponent<Animator>();
        nav = GetComponent<NavMeshAgent>();
        isActive = true;
        UpdateTargets();
    }
	
	// Update is called once per frame
	protected virtual void Update () {
        
        if (!isActive) return;
        velocity = (transform.position - lastPos) / Time.deltaTime;
        lastPos = transform.position;
        timer1s += Time.deltaTime;

        //Restore health
        if(health < maxHealth)
        {
            health += healthRestore * Time.deltaTime;
            health = health > maxHealth ? maxHealth : health;
        }

        //Update targetList every 1 second(Time consuming method)
        if (timer1s >= 1)
        {
            timer1s = 0;
            UpdateTargets();
        }

        Targeting();
        if(nav != null)
        {
            FollowTarget();
            Attack();
        }
        
    }
    void FixedUpdate()
    {
       // if (debug) print(tag + " speed: " + velocity.magnitude);
        AnimMove(velocity.magnitude);
    }
    protected virtual void Strolling()
    {
        if (nav == null) return;
        strollTimer += Time.deltaTime;
        if(strollTimer > strollInterval)
        {
            if (debug) print(tag + " speed: " + velocity.magnitude);
            float x = Random.Range(-strollRange, strollRange);
            float y = Random.Range(-strollRange, strollRange);
            Vector3 dest = transform.position;
            dest.x += x;
            dest.z += y;
            nav.SetDestination(dest);
            strollTimer = Random.Range(0, strollInterval/2);
        }
    }

    public virtual void TakeDamage(GameObject attacker, int type, float amount)
    {
        if (!isActive) return;
        foreach(string e in enemyTagList)
        {
            if(e == attacker.tag)
            {
                target = attacker;
            }
        }
        AnimHurt();
        health -= amount;
        if(health <= 0)
        {
            Die();
        }
        
    }

    protected virtual void Die()
    {
        if (!isActive) return;
        if (nav != null) nav.enabled = false;
        targets.Clear();
        target = null;
        if(collider != null) collider.enabled = false;
        isActive = false;
        AnimDie();
    }

    protected virtual void Attack()
    {
        attackTimer += Time.deltaTime;
        if (target != null && attackTimer >= attackInterval)
        {
            float distance = (target.transform.position - transform.position).magnitude;
            float angle = Vector3.Angle(target.transform.position - transform.position, transform.forward);
            if (distance <= attackRange && angle < attackAngle/2f)
            {
                AnimAttack();
                attackTimer = 0;
                //print("Attack" + target.tag);
            }
        }
        if (attackTimer >= attackInterval) attackTimer = attackInterval; 
    }

    //Animation Event
    protected virtual void DamageTarget() 
    {
        if (!isActive) return;
        if (target != null)
        {
            float distance = (target.transform.position - transform.position).magnitude;
            if (distance <= attackRange)
            {
                Character script = target.GetComponent<Character>();
                script.TakeDamage(this.gameObject, 1, damage);
            }
        }
    }



    protected virtual void AnimMove(float speed)
    {

    }

    protected virtual void AnimAttack()
    {

    }

    protected virtual void AnimHurt()
    {

    }

    protected virtual void AnimDie()
    {

    }
}
