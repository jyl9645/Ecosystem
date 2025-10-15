using System;
using System.Collections.Generic;
using UnityEngine;

public class BeetleScript : MonoBehaviour
{
    //stats
    public float hunger = 100;
    public float energy = 100;
    public float health = 100;
    public float age = 0;
    public float death_age = 70;
    public float speed = 2f;
    public float dirt = 0f;
    public BeetleStates state = BeetleStates.exploring;

    //stat_update variables ----------------------------------------------------------
    float hungerStep = 2;
    public float hungerTimer;
    public float hungerLoss = 10f;

    float energyGain = 5;
    float energyLoss = 4;

    public float exploreTimer = 3;
    public float exploreInterval = 3;

    float birthTimer = 30;
    float birthInterval = 30;

    //states --------------------------------------------------------------------------
    public enum BeetleStates
    {
        sleeping,
        eating,
        exploring,
        escaping,
        birthing,
        building,
        dying
    }

    //other stats
    public Vector2 target;
    public List<GameObject> enemies = new List<GameObject>();
    public List<GameObject> food;
    public GameObject tunnelPrefab;
    public GameObject eggs;
    Animator anim;

    //methods
    public void stat_update()
    {
        if (hungerTimer > 0) { hungerTimer -= Time.deltaTime; }
        else { hunger -= hungerLoss; hungerTimer = hungerStep; }

        age += Time.deltaTime;

        if (hunger <= 50 && state == BeetleStates.sleeping)
        {
            state = BeetleStates.exploring;
        }

        if ((energy <= 20) && (state != BeetleStates.sleeping))
        {
            state = BeetleStates.sleeping;
        }

        if (hunger < 1 || age >= death_age)
        {
            state = BeetleStates.dying;
        }

        if (birthTimer <= 0)
        {
            state = BeetleStates.birthing;
        }
        else
        {
            birthTimer -= Time.deltaTime;
        }

        if (dirt >= 40)
        {
            state = BeetleStates.building;
            dirt = 0;
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

    public void add_enemy(GameObject enemy)
    {
        enemies.Add(enemy);
    }

    public void remove_enemy(GameObject enemy)
    {
        enemies.Remove(enemy);
    }

    public void add_food(GameObject fd)
    {
        Debug.Log("added");
        food.Add(fd);
    }

    public void remove_health(float damage)
    {
        health -= damage;
        anim.SetTrigger("Hurt");
        if (health <= 0)
        {
            state = BeetleStates.dying;
        }
    }

    //state functions
    public void explore()
    {
        if (enemies.Count != 0)
        {
            state = BeetleStates.escaping;
        }

        else
        {
            if (target == Vector2.zero || (Vector2)transform.position == target || exploreTimer <= 0)
            {
                target = new Vector2(UnityEngine.Random.Range(-8, 8), UnityEngine.Random.Range(-4, 4));
            }
            else if ((food.Count == 0) && (enemies.Count == 0))
            {
                transform.position = Vector2.MoveTowards(transform.position, target, speed * Time.deltaTime);
                energy -= energyLoss * Time.deltaTime;
            }

            if (food.Count != 0)
            {
                if (food[0])
                {
                    target = food[0].transform.position;
                    state = BeetleStates.eating;
                } 
            }

            if (enemies.Count != 0)
            {
                state = BeetleStates.escaping;
            }
        }
    }

    public void eat()
    {
        if (enemies.Count != 0)
        {
            state = BeetleStates.escaping;
        }
        else if (food.Count != 0)
        {
            if (food[0])
            {
                target = food[0].transform.position;
                transform.position = Vector2.MoveTowards(transform.position, target, speed * Time.deltaTime);
                energy -= energyLoss * Time.deltaTime;
            }
            else
            {
                food.Remove(food[0]);
            }
        }
        else
        {
            state = BeetleStates.exploring;
        }
    }

    public void escape()
    {
        if (enemies.Count == 0)
        {
            state = BeetleStates.exploring;
            return;
        }
        else
        {
            if (enemies[0])
            {
                target = new Vector2(-enemies[0].transform.position.x, -enemies[0].transform.position.y);
                transform.position = Vector2.MoveTowards(transform.position, target, speed * Time.deltaTime);
                energy -= energyLoss * Time.deltaTime;
                return;
            }
        }
    }

    public void sleep()
    {
        if (energy >= 100)
        {
            state = BeetleStates.exploring;
        }
        else
        {
            energy += 10 * Time.deltaTime;
        }
    }

    public void birthing()
    {
        Instantiate(eggs, transform.position, Quaternion.identity);
        birthTimer = birthInterval;

        state = BeetleStates.exploring;
    }

    public void dying()
    {
        Destroy(gameObject, 2);
    }

    public void building()
    {
        Instantiate(tunnelPrefab, new Vector2(transform.position.x, transform.position.y - 1), Quaternion.identity);
        state = BeetleStates.exploring;
    }

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case BeetleStates.exploring:
                Debug.Log("exploring");
                anim.SetTrigger("Walk");
                explore();
                break;

            case BeetleStates.eating:
                Debug.Log("eating");
                anim.SetTrigger("Eat");
                eat();
                break;

            case BeetleStates.escaping:
                Debug.Log("escaping");
                anim.SetTrigger("Walk");
                escape();
                break;

            case BeetleStates.sleeping:
                Debug.Log("sleeping");
                anim.SetTrigger("Sleep");
                sleep();
                break;

            case BeetleStates.birthing:
                Debug.Log("Birthing");
                birthing();
                break;

            case BeetleStates.building:
                building();
                break;
            
            case BeetleStates.dying:
                Debug.Log("dying");
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
        if (collision.collider.CompareTag("berry"))
        {
            collision.gameObject.GetComponent<BerryScript>().remove_health();
            dirt += 4 * Time.deltaTime;
            if (hunger < 100)
            {
                hunger = Math.Clamp(hunger + 5 * Time.deltaTime, 0, 100);
            }

            else
            {
                state = BeetleStates.exploring;
            }
        }

        if (collision.collider.CompareTag("tunnel"))
        {
            collision.gameObject.GetComponent<TunnelScript>().lose_health(20 * Time.deltaTime);
            dirt += 8 * Time.deltaTime;
            if (hunger < 100)
            {
                hunger = Math.Clamp(hunger + 5 * Time.deltaTime, 0, 100);
            }

            else
            {
                state = BeetleStates.exploring;
            }
        }
    }
}
