using UnityEngine;

public class BerryScript : MonoBehaviour
{
    //stats
    public float health = 100;

    [SerializeField]
    Animator animator;

    //functions
    public void remove_health()
    {
        health -= 10 * Time.deltaTime;
        animator.SetTrigger("hit");
    }

    void Update()
    {
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }
}
