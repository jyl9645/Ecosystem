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
    public float speed = 2f;
    RaptorStates state = RaptorStates.exploring;

    //stat_update variables ----------------------------------------------------------
    float hungerStep = 5;
    public float hungerTimer;
    public float hungerLoss = 2f;

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
    Queue<GameObject> prey_found = new Queue<GameObject>();

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

    public void add_food(GameObject food)
    {
        prey_found.Enqueue(food);
    }

    //state functions
    public void explore()
    {
        if (target == Vector2.zero || (Vector2)transform.position == target)
        {
            target = new Vector2(UnityEngine.Random.Range(-8, 8), UnityEngine.Random.Range(-4, 4));
        }
        else if (prey_found.Count == 0)
        {
            transform.position = Vector2.MoveTowards(transform.position, target, speed * Time.deltaTime);
        }
        if (prey_found.Count != 0)
        {
            target = prey_found.Dequeue().transform.position;
            state = RaptorStates.eating;
        }
    }

    public void eat()
    {
        if (energy > 20)
        {
            transform.position = Vector2.MoveTowards(transform.position, target, speed * Time.deltaTime);
        }
    }

    public void sleep()
    {
        energy += 10 * Time.deltaTime;
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
                Debug.Log("exploring");
                explore();
                break;

            case RaptorStates.eating:
                Debug.Log("eating");
                eat();
                break;

            default:
                break;
        }

        stat_update();
    }
}
