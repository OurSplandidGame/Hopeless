using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spider : Character {

    protected override void AnimMove(float speed)
    {
        animator.SetFloat("Speed", speed);
    }

    protected override void AnimAttack()
    {
        animator.SetTrigger("Attack");
    }
    protected override void AnimDie()
    {
        animator.SetTrigger("Die");
    }
}
