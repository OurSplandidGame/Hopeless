using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharMani : MonoBehaviour
{
    private CharacterController player;
    public float speed;
    public Camera cam;
    private Animator animator;
    Actions action;
    // Use this for initialization
    void Start()
    {
        player = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        action = GetComponent<Actions>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void FixedUpdate()
    {
        if (Input.GetMouseButton(0))
        {
            animator.SetTrigger("Fire1");
        }
        Vector3 goFront = cam.transform.forward;
        goFront.y = 0;
        goFront = Vector3.Normalize(goFront);
        Vector3 goRight = cam.transform.right;
        goRight.y = 0;
        goRight = Vector3.Normalize(goRight);
        Vector3 moveDir = Vector3.Normalize(goRight * Input.GetAxis("Horizontal") + goFront * Input.GetAxis("Vertical"));
        Walk(speed*moveDir);
    }
    
    void Walk(Vector3 velocity)
    {
        //animator.SetFloat("Speed", velocity.magnitude);
        if (velocity.magnitude < 0.001) return;
        player.transform.rotation = Quaternion.LookRotation(velocity);
        player.SimpleMove(velocity);
    }

    void Attack(Vector3 direction)
    {
        animator.SetTrigger("Fire1");
    }

}
