using Unity.VisualScripting;
using UnityEngine;

public class DetectScript : MonoBehaviour
{
    [SerializeField]
    GameObject mole;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("raptor"))
        {
            mole.GetComponent<MoleScript>().add_enemy(collision.gameObject);
        }
        else if (collision.CompareTag("berry"))
        {
            Debug.Log("catch berry");
            mole.GetComponent<MoleScript>().add_food(collision.gameObject);
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("raptor"))
        {
            mole.GetComponent<MoleScript>().remove_enemy(collision.gameObject);
        }
    }
}
