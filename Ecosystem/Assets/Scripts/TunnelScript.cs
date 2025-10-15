using System.Collections.Generic;
using Unity.VisualScripting;
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
    public Animator anim;

    //methods
    public void add_inhabit(GameObject mole)
    {
        inhabitants.Add(mole);
    }

    public void remove_inhabit(GameObject mole)
    {
        inhabitants.Remove(mole);
    }

    public void tunnel_sleep()
    {
        anim.SetTrigger("Sleep");
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
                remove_inhabit(inhabitant);
            }
            anim.SetTrigger("Break");
            Destroy(gameObject, 1);
        }
    }

    public void breed()
    {
        if (inhabitants.Count >= 1)
        {
            anim.SetTrigger("Breed");
            if (inhabitants[0])
            {
                inhabitants[0].GetComponent<MoleScript>().state = MoleScript.MoleStates.birthing;
            }
            Instantiate(baby, position: new Vector2(transform.position.x, transform.position.y - 1), Quaternion.identity);
            breed_timer = 80;
        }
    }

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        breed_timer -= Time.deltaTime;

        if (breed_timer <= 0)
        {
            breed();
        }
    }

}
