using System.Threading;
using UnityEngine;

public class EggScript : MonoBehaviour
{

    [SerializeField]
    GameObject bug;

    float timer = 10;

    // Update is called once per frame
    void Update()
    {
        if (timer <= 0)
        {
            Instantiate(bug, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
        else
        {
            timer -= Time.deltaTime;
        }
    }
}
