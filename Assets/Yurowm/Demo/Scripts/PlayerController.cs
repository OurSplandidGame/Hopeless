using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Animator))]
public class PlayerController : Character {

	public Transform rightGunBone;
	public Transform leftGunBone;
	public Arsenal[] arsenal;
    public float speed;
    public Camera cam;

    private Actions action;
    private CharacterController player;
    private GunController gun;
    private bool attacking = false;

    
	protected override void Awake() {
        base.Awake();
        player = GetComponent<CharacterController>();
        action = GetComponent<Actions>();
        if (arsenal.Length > 0)
			SetArsenal (arsenal[1].name);
        }

    void FixedUpdate()
    {
        if(Input.GetMouseButton(0))
        {
            animator.SetBool("Squat", false);
            animator.SetFloat("Speed", 0f);
            animator.SetBool("Aiming", true);
            if (gun.Ready())
            {
                animator.SetTrigger("Attack");
                attacking = true;
            }
                
        }
        else if(!attacking || Input.GetButtonDown("Horizontal") || Input.GetButtonDown("Vertical"))
        {
            attacking = false;
            animator.ResetTrigger("Attack");
            animator.SetBool("Aiming", false);
            Vector3 goFront = cam.transform.forward;
            goFront.y = 0;
            goFront = Vector3.Normalize(goFront);
            Vector3 goRight = cam.transform.right;
            goRight.y = 0;
            goRight = Vector3.Normalize(goRight);
            Vector3 moveDir = Vector3.Normalize(goRight * Input.GetAxis("Horizontal") + goFront * Input.GetAxis("Vertical"));
            Move(speed * moveDir);
        }


    }

    void Move(Vector3 velocity)
    {
        animator.SetFloat("Speed", velocity.magnitude);
        if (velocity.magnitude < 0.001) return;
        animator.SetBool("Aiming", false);
        player.transform.forward =Vector3.Lerp(velocity, player.transform.forward,2.0f*Time.deltaTime);
        player.SimpleMove(velocity);
    }

    void OnShoot()
    {
        if (target != null)
            player.transform.forward = target.transform.position - transform.position;
        gun.Shoot();
        animator.ResetTrigger("Attack");
    }

    void EndShoot()
    {
        attacking = false;
    }

    public void SetArsenal(string name) {
		foreach (Arsenal hand in arsenal) {
			if (hand.name == name) {
				if (rightGunBone.childCount > 0)
					Destroy(rightGunBone.GetChild(0).gameObject);
				if (leftGunBone.childCount > 0)
					Destroy(leftGunBone.GetChild(0).gameObject);
				if (hand.rightGun != null) {
					GameObject newRightGun = (GameObject) Instantiate(hand.rightGun);
					newRightGun.transform.parent = rightGunBone;
					newRightGun.transform.localPosition = Vector3.zero;
					newRightGun.transform.localRotation = Quaternion.Euler(90, 0, 0);
					}
				if (hand.leftGun != null) {
					GameObject newLeftGun = (GameObject) Instantiate(hand.leftGun);
					newLeftGun.transform.parent = leftGunBone;
					newLeftGun.transform.localPosition = Vector3.zero;
					newLeftGun.transform.localRotation = Quaternion.Euler(90, 0, 0);
				}
				animator.runtimeAnimatorController = hand.controller;
                gun = rightGunBone.GetChild(0).gameObject.GetComponent<GunController>();
                gun.User = gameObject;
                return;
				}
		}
	}

	[System.Serializable]
	public struct Arsenal {
		public string name;
		public GameObject rightGun;
		public GameObject leftGun;
		public RuntimeAnimatorController controller;
	}
}
