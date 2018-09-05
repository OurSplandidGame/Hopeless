using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animator))]
public class Player : MonoBehaviour
{

    public Transform rightGunBone;
    public Transform leftGunBone;
    public Arsenal[] arsenal;
    public float speed;
    public Camera cam;

    private Actions action;
    private CharacterController player;
    private Animator animator;
    private GunController gun;
    private int curWeapon;

    void Awake()
    {
        player = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        action = GetComponent<Actions>();
        if (arsenal.Length > 0)
            SetArsenal(arsenal[1].name);
        curWeapon = 1;
    }

    void FixedUpdate()
    {

        if (Input.GetMouseButton(0))
        {
            animator.SetBool("Squat", false);
            animator.SetFloat("Speed", 0f);
            animator.SetBool("Aiming", true);
            if (gun.Ready())
                animator.SetTrigger("Attack");
        }
        else
        {
            //animator.ResetTrigger("Attack");
            animator.SetBool("Aiming", false);
        }

        Vector3 goFront = cam.transform.forward;
        goFront.y = 0;
        goFront = Vector3.Normalize(goFront);
        Vector3 goRight = cam.transform.right;
        goRight.y = 0;
        goRight = Vector3.Normalize(goRight);
        Vector3 moveDir = Vector3.Normalize(goRight * Input.GetAxis("Horizontal") + goFront * Input.GetAxis("Vertical"));
        Move(speed * moveDir);
    }

    void Move(Vector3 velocity)
    {
        animator.SetFloat("Speed", velocity.magnitude);
        if (velocity.magnitude < 0.001) return;
        animator.SetBool("Aiming", false);
        player.transform.rotation = Quaternion.LookRotation(velocity);
        player.SimpleMove(velocity);
    }

    void Attack()
    {
        gun.Shoot();
        animator.ResetTrigger("Attack");
    }

    public void SetArsenal(string name)
    {
        foreach (Arsenal hand in arsenal)
        {
            if (hand.name == name)
            {
                if (rightGunBone.childCount > 0)
                    Destroy(rightGunBone.GetChild(0).gameObject);
                if (leftGunBone.childCount > 0)
                    Destroy(leftGunBone.GetChild(0).gameObject);
                if (hand.rightGun != null)
                {
                    GameObject newRightGun = (GameObject)Instantiate(hand.rightGun);
                    newRightGun.transform.parent = rightGunBone;
                    newRightGun.transform.localPosition = Vector3.zero;
                    newRightGun.transform.localRotation = Quaternion.Euler(90, 0, 0);
                }
                if (hand.leftGun != null)
                {
                    GameObject newLeftGun = (GameObject)Instantiate(hand.leftGun);
                    newLeftGun.transform.parent = leftGunBone;
                    newLeftGun.transform.localPosition = Vector3.zero;
                    newLeftGun.transform.localRotation = Quaternion.Euler(90, 0, 0);
                }
                animator.runtimeAnimatorController = hand.controller;
                gun = rightGunBone.GetChild(0).gameObject.GetComponent<GunController>();
                return;
            }
        }
    }

    [System.Serializable]
    public struct Arsenal
    {
        public string name;
        public GameObject rightGun;
        public GameObject leftGun;
        public RuntimeAnimatorController controller;
    }
}
