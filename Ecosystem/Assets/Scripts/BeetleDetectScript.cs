using UnityEngine;

public class BeetleDetectScript : MonoBehaviour
{
    [SerializeField]
    GameObject beetle;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("mole"))
        {
            beetle.GetComponent<BeetleScript>().add_enemy(collision.gameObject);
        }
        else if (collision.CompareTag("berry") || collision.CompareTag("tunnel"))
        {
            beetle.GetComponent<BeetleScript>().add_food(collision.gameObject);
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("raptor"))
        {
            beetle.GetComponent<BeetleScript>().remove_enemy(collision.gameObject);
        }
    }
}
