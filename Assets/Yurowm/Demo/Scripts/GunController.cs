using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour {
    public Transform gunTip;
    public int damage = 20;
    public float interval = 0.4f;
    public float range = 100f;
    
    float timer;
    Ray shootRay;
    RaycastHit target;
    LineRenderer shootLine;
    Light gunLight;
    float flashTime = 0.1f;
    private GameObject user;
    public GameObject User { set { user = value; } get { return user; } }
    // Use this for initialization
    void Awake () {
        shootLine = GetComponent<LineRenderer>();
        gunLight = GetComponent<Light>();
        gunLight.transform.position = gunTip.position;
    }
	
	// Update is called once per frame
	void Update () {
        timer += Time.deltaTime;
        if(timer >= flashTime)
        {
            ShootEffect(false);
        }
	}

    public bool Ready()
    {
        return timer >= interval;
    }

    public void Shoot()
    {
        if (timer >= interval)
        {
            timer = 0;
            ShootEffect(true);
            shootLine.SetPosition(0, gunTip.transform.position);
            shootRay.origin = gunTip.transform.position;
            shootRay.direction = gunTip.transform.forward;
            if (Physics.Raycast(shootRay, out target, range))
            {
                shootLine.SetPosition(1, target.point);
                Character script = target.collider.gameObject.GetComponent<Character>();
                if (script != null)
                {
                    script.TakeDamage(user,1, damage);
                }
            }
            else
            {
                shootLine.SetPosition(1, shootRay.origin + shootRay.direction * range);
            }
        }
    }

    private void ShootEffect(bool enable)
    {
        shootLine.enabled = enable;
        gunLight.enabled = enable;
    }
}
