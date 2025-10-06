using UnityEngine;

public class BushScript : MonoBehaviour
{
    float timer = 20;
    float cooldown = 20;
    float range = 3;

    [SerializeField]
    GameObject berry;
    public void grow()
    {
        Vector2 pos = new Vector2(Random.Range(transform.position.x - range, transform.position.x + range), Random.Range(transform.position.y - range, transform.position.y + range));
        GameObject new_berry = Instantiate(berry, pos, Quaternion.identity);
    }

    void Update()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
        }
        else
        {
            grow();
            timer = cooldown;
        }
    }
}
