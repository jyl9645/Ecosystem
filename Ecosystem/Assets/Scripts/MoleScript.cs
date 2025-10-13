using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class MoleScript : MonoBehaviour
{
    //stats --------------------------------------------------------------
    public float hunger = 100;
    public float energy = 100;
    public float health = 100;
    public float age = 0;
    public float death_age = 100;
    public float speed = 2f;
    public MoleStates state = MoleStates.exploring;

    //stat_update variables ----------------------------------------------------------
    float hungerStep = 2;
    public float hungerTimer;
    public float hungerLoss = 5f;

    float energyGain = 5;
    float energyLoss = 4;

    //states --------------------------------------------------------------------------
    public enum MoleStates
    {
        sleeping,
        eating,
        exploring,
        escaping,
        birthing,
        dying
    }

    //other vars -----------------------------------------------------------------------
    public GameObject home;
    public Vector2 target;
    List<GameObject> food_seen = new List<GameObject>();
    List<GameObject> enemies_sensed = new List<GameObject>();
    public SpriteRenderer spriteRenderer;
    public Collider2D circleCollider;

    //helper functions -------------------------------------------------------------------
    public void stat_update()
    {
        if (hungerTimer > 0) { hungerTimer -= Time.deltaTime; }
        else { hunger -= hungerLoss; hungerTimer = hungerStep; }

        age += Time.deltaTime;

        if (hunger <= 50 && state == MoleStates.sleeping)
        {
            state = MoleStates.exploring;
        }

        if ((energy <= 20) && (state != MoleStates.sleeping))
        {
            state = MoleStates.escaping;
        }
    }

    public void add_enemy(GameObject enemy)
    {
        enemies_sensed.Add(enemy);
    }

    public void remove_enemy(GameObject enemy)
    {
        enemies_sensed.Remove(enemy);
    }

    public void add_food(GameObject food)
    {
        Debug.Log("added");
        food_seen.Add(food);
    }

    public GameObject find_closest_tunnel()
    {
        GameObject closest = null;

        GameObject[] tunnels = GameObject.FindGameObjectsWithTag("tunnel");
        foreach (GameObject tunnel in tunnels)
        {
            if (closest == null || Vector2.Distance(gameObject.transform.position, tunnel.transform.position) < Vector2.Distance(gameObject.transform.position, closest.transform.position))
            {
                closest = tunnel;
            }
        }

        return closest;
    }

    public void hide()
    {
        spriteRenderer.enabled = false;
        circleCollider.enabled = false;
    }

    public void appear()
    {
        spriteRenderer.enabled = true;
        circleCollider.enabled = true;
    }

    public void remove_health(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            state = MoleStates.dying;
        }
    }

    //state functions -------------------------------------------------------------------
    public void explore()
    {
        if (enemies_sensed.Count != 0)
        {
            state = MoleStates.escaping;
        }

        else
        {
            if (target == Vector2.zero || (Vector2)transform.position == target || target == (Vector2)home.transform.position)
            {
                target = new Vector2(UnityEngine.Random.Range(-8, 8), UnityEngine.Random.Range(-4, 4));
            }
            else if ((food_seen.Count == 0) && (enemies_sensed.Count == 0))
            {
                transform.position = Vector2.MoveTowards(transform.position, target, speed * Time.deltaTime);
                energy -= energyLoss * Time.deltaTime;
            }

            if (food_seen.Count != 0)
            {
                target = food_seen[0].transform.position;
                state = MoleStates.eating;
            }

            if (enemies_sensed.Count != 0)
            {
                state = MoleStates.escaping;
            }
        }
    }

    public void eat()
    {
        if (enemies_sensed.Count != 0)
        {
            state = MoleStates.escaping;
        }
        else if (food_seen.Count != 0)
        {
            if (food_seen[0])
            {
                target = food_seen[0].transform.position;
                transform.position = Vector2.MoveTowards(transform.position, target, speed * Time.deltaTime);
                energy -= energyLoss * Time.deltaTime;
            }
            else
            {
                food_seen.Remove(food_seen[0]);
            }
        }
        else
        {
            state = MoleStates.exploring;
        }
    }

    public void escape()
    {
        home = find_closest_tunnel();

        if (enemies_sensed.Count == 0)
        {
            state = MoleStates.exploring;
        }

        if (target != (Vector2)home.transform.position)
        {
            target = home.transform.position;
        }
        else
        {
            transform.position = Vector2.MoveTowards(transform.position, target, speed * Time.deltaTime);
            energy -= energyLoss * Time.deltaTime;
        }
    }

    public void sleep()
    {
        if (energy >= 100 && enemies_sensed.Count == 0)
        {
            appear();
            state = MoleStates.exploring;
        }
        else
        {
            energy += energyGain * Time.deltaTime;
        }
    }

    public void birthing()
    {
        if (state == MoleStates.sleeping)
        {
            
        }
    }

    public void dying()
    {
        Destroy(gameObject);
    }

    //game functions ------------------------------------------------------------------------
    void Start()
    {
        hungerTimer = hungerStep;

        spriteRenderer = GetComponent<SpriteRenderer>();
        circleCollider = GetComponent<CircleCollider2D>();
    }

    void Update()
    {
        switch (state)
        {
            case MoleStates.exploring:
                Debug.Log("exploring");
                explore();
                break;

            case MoleStates.eating:
                Debug.Log("eating");
                eat();
                break;

            case MoleStates.escaping:
                Debug.Log("escaping");
                escape();
                break;

            case MoleStates.sleeping:
                Debug.Log("sleeping");
                sleep();
                break;

            case MoleStates.birthing:
                Debug.Log("Birthing");
                break;

            case MoleStates.dying:
                Debug.Log("dying");
                break;

            default:
                break;
        }
        
        if (age >= death_age)
        {
            state = MoleStates.dying;
        }

        stat_update();
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("berry"))
        {
            collision.gameObject.GetComponent<BerryScript>().remove_health();
            if (hunger < 100)
            {
                hunger = Math.Clamp(hunger + 5 * Time.deltaTime, 0, 100);
            }

            else
            {
                state = MoleStates.exploring;
            }
        }

        if (collision.collider.CompareTag("tunnel"))
        {
            if (state == MoleStates.escaping)
            {
                home = collision.collider.gameObject;
                hide();
                state = MoleStates.sleeping;
            }
        }
    }

}
