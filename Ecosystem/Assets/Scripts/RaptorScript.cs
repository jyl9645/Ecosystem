using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class RaptorScript : MonoBehaviour
{
    //stats --------------------------------------------------------------
    public float hunger = 100;
    public float energy = 100;
    public float health = 100;
    public float age = 0;
    public float death_age = 120;
    public float speed = 5f;
    public float attack = 10f;
    public RaptorStates state = RaptorStates.exploring;

    //stat_update variables ----------------------------------------------------------
    float hungerStep = 3;
    public float hungerTimer;
    public float hungerLoss = 2f;

    public float exploreTimer = 3;
    public float exploreInterval = 3;

    public float energyLoss = 4;

    public float breedTimer = 50;
    public float breedInterval = 50;

    //states --------------------------------------------------------------------------
    public enum RaptorStates
    {
        sleeping,
        eating,
        exploring,
        birthing,
        dying
    }

    //other vars -------------------------------------------------------------------
    public Vector2 target;
    public GameObject prey;
    public GameObject mate;

    public GameObject baby;
    Animator anim;

    //helper functions ------------------------------------------------------
    public void stat_update()
    {
        if (hungerTimer > 0) { hungerTimer -= Time.deltaTime; }
        else { hunger -= hungerLoss; hungerTimer = hungerStep; }

        age += Time.deltaTime;

        if (hunger <= 50)
        {
            state = RaptorStates.exploring;
        }

        if (energy <= 20)
        {
            state = RaptorStates.sleeping;
        }

        if (hunger < 1 || age >= death_age)
        {
            state = RaptorStates.dying;
        }

        if (breedTimer <= 0 && state != RaptorStates.sleeping)
        {
            state = RaptorStates.birthing;
            breedTimer = breedInterval;
        }
        else
        {
            breedTimer -= Time.deltaTime;
        }

        if (exploreTimer < 0)
        {
            exploreTimer = exploreInterval;
        }
        else
        {
            exploreTimer -= Time.deltaTime;
        }

    }

    public void change_target(GameObject food)
    {
        prey = food;
    }

    public GameObject find_closest_mate()
    {
        GameObject closest = null;

        GameObject[] mates = GameObject.FindGameObjectsWithTag("raptor");

        if (mates.Length != 0)
        {
            foreach (GameObject option in mates)
            {
                if (gameObject != option)
                {
                    if (closest == null || Vector2.Distance(gameObject.transform.position, option.transform.position) < Vector2.Distance(gameObject.transform.position, closest.transform.position))
                    {
                        closest = option;
                    }
                } 
            }

            return closest;
        }
        else
        {
            return null;
        }
    }

    //state functions
    public void explore()
    {
        if (target == Vector2.zero || (Vector2)transform.position == target || exploreTimer <= 0)
        {
            target = new Vector2(UnityEngine.Random.Range(-8, 8), UnityEngine.Random.Range(-4, 4));
        }
        else if (prey == null)
        {
            transform.position = Vector2.MoveTowards(transform.position, target, speed * Time.deltaTime);
            energy -= energyLoss * Time.deltaTime;
        }
        if (prey != null)
        {
            state = RaptorStates.eating;
        }
    }

    public void eat()
    {
        if (energy > 20 && prey)
        {
            transform.position = Vector2.MoveTowards(transform.position, prey.transform.position, speed * Time.deltaTime);
            energy -= energyLoss * Time.deltaTime;
            if (prey.GetComponent<MoleScript>().state == MoleScript.MoleStates.sleeping)
            {
                prey = null;
            }
        }
        
        if (prey == null)
        {
            if (energy < 20)
            {
                state = RaptorStates.sleeping;
            }
            else
            {
                state = RaptorStates.exploring;
            }
        }
    }

    public void sleep()
    {
        if (energy >= 100)
        {
            state = RaptorStates.exploring;
        }
        else
        {
            energy += 10 * Time.deltaTime;
        }
    }

    public void dying()
    {
        Destroy(gameObject, 2);
    }

    public void birthing()
    {
        mate = find_closest_mate();
        target = mate.transform.position;
        transform.position = Vector2.MoveTowards(transform.position, target, speed * Time.deltaTime);
    }

    void Start()
    {
        anim = GetComponent<Animator>();
    }
    void Update()
    {
        switch (state)
        {
            case RaptorStates.exploring:
                Debug.Log("Raptor exploring");
                anim.SetTrigger("Walk");
                explore();
                break;

            case RaptorStates.eating:
                Debug.Log("Raptor eating");
                anim.SetTrigger("Eat");
                eat();
                break;

            case RaptorStates.sleeping:
                Debug.Log("Raptor sleeping");
                anim.SetTrigger("Sleep");
                sleep();
                break;

            case RaptorStates.birthing:
                birthing();
                break;

            case RaptorStates.dying:
                anim.SetTrigger("Die");
                dying();
                break;

            default:
                break;
        }

        stat_update();
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("mole") && state == RaptorStates.eating)
        {
            if (collision.collider.GetComponent<MoleScript>().state != MoleScript.MoleStates.sleeping)
            {
                collision.gameObject.GetComponent<MoleScript>().remove_health(attack * Time.deltaTime);
                hunger = Math.Clamp(hunger + 10, 0, 100);
            }
        }

    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("raptor") && state == RaptorStates.birthing)
        {
            state = RaptorStates.exploring;
            collision.collider.GetComponent<RaptorScript>().state = RaptorStates.exploring;
            Instantiate(baby, new Vector2(transform.position.x, transform.position.y - 2), Quaternion.identity);
        }
    }
}
