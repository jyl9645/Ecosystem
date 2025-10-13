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
    public float speed = 5f;
    public float attack = 10f;
    RaptorStates state = RaptorStates.exploring;

    //stat_update variables ----------------------------------------------------------
    float hungerStep = 5;
    public float hungerTimer;
    public float hungerLoss = 2f;

    public float smooth = 0.9f;
    private Vector2 velocity = Vector2.zero;

    //states --------------------------------------------------------------------------
    enum RaptorStates
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

    //helper functions ------------------------------------------------------
    public void stat_update()
    {
        if (hungerTimer > 0) { hungerTimer -= Time.deltaTime; }
        else { hunger -= hungerLoss; hungerTimer = hungerStep; }

        age += Time.deltaTime;

        if (hunger <= 50 || state == RaptorStates.sleeping)
        {
            state = RaptorStates.exploring;
        }

        if (energy <= 20)
        {
            state = RaptorStates.sleeping;
        }
    }

    public void change_target(GameObject food)
    {
        prey = food;
    }

    //state functions
    public void explore()
    {
        if (target == Vector2.zero || (Vector2)transform.position == target)
        {
            target = new Vector2(UnityEngine.Random.Range(-8, 8), UnityEngine.Random.Range(-4, 4));
        }
        else if (prey == null)
        {
            transform.position = Vector2.SmoothDamp(transform.position, target, ref velocity, smooth);
        }
        if (prey != null)
        {
            state = RaptorStates.eating;
        }
    }

    public void eat()
    {
        if (energy > 20)
        {
            transform.position = Vector2.SmoothDamp(transform.position, prey.transform.position, ref velocity, smooth);
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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case RaptorStates.exploring:
                Debug.Log("Raptor exploring");
                explore();
                break;

            case RaptorStates.eating:
                Debug.Log("Raptor eating");
                eat();
                break;

            case RaptorStates.sleeping:
                Debug.Log("Raptor sleeping");
                sleep();
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
            collision.gameObject.GetComponent<MoleScript>().remove_health(attack * Time.deltaTime);
        }
    }
}
