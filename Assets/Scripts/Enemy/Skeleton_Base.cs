using UnityEngine;

public class Skeleton_Base : MonoBehaviour
{
    private Animator animator;
    public bool isDead = false;
    private bool hasdied = false;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (isDead && !hasdied)
        {
            Die();
        }
    }

    void Die()
    {
        hasdied = true;
        animator.applyRootMotion = true;
        animator.SetTrigger("Died");
    }
}