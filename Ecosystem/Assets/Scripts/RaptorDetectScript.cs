using UnityEngine;

public class RaptorDetectScript : MonoBehaviour
{

    [SerializeField]
    GameObject raptor;

    GameObject prey;

    void Start()
    {
        prey = raptor.GetComponent<RaptorScript>().prey;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("mole"))
        {
            raptor.GetComponent<RaptorScript>().change_target(collision.gameObject);
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision == prey)
        {
            raptor.GetComponent<RaptorScript>().change_target(null);
        }
    }

}
