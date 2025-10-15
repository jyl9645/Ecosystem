using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class TunnelScript : MonoBehaviour
{
    //stats
    public float health = 100;
    public float breed_timer = 50;
    public List<GameObject> inhabitants;
    public GameObject baby;
    public Collider2D collider;

    //methods
    public void add_inhabit(GameObject mole)
    {
        inhabitants.Add(mole);
    }

    public void remove_inhabit(GameObject mole)
    {
        inhabitants.Remove(mole);
    }

    public void lose_health(float dmg)
    {
        health -= dmg;
        if (health <= 0)
        {
            collider.enabled = false;
            foreach (GameObject inhabitant in inhabitants)
            {
                inhabitant.GetComponent<MoleScript>().state = MoleScript.MoleStates.exploring;
            }
            Destroy(gameObject);
        }
    }

    public void breed()
    {
        if (inhabitants.Count >= 1)
        {
            if (inhabitants[0])
            {
                inhabitants[0].GetComponent<MoleScript>().state = MoleScript.MoleStates.birthing;
            }
            Instantiate(baby, position:new Vector2(transform.position.x, transform.position.y - 1), Quaternion.identity);
            breed_timer = 80;
        }   
    }

    void Update()
    {
        breed_timer -= Time.deltaTime;
    }
}
