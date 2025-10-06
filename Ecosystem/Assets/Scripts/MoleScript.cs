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
    public float speed = 2f;
    MoleStates state = MoleStates.exploring;

    //stat_update variables ----------------------------------------------------------
    float hungerStep = 5;
    public float hungerTimer;
    public float hungerLoss = 2f;

    //states --------------------------------------------------------------------------
    enum MoleStates
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
    Queue<GameObject> food_seen = new Queue<GameObject>();
    List<GameObject> enemies_sensed = new List<GameObject>();
    public SpriteRenderer spriteRenderer;
    public Collider2D circleCollider;

    //helper functions -------------------------------------------------------------------
    public void stat_update()
    {
        if (hungerTimer > 0) { hungerTimer -= Time.deltaTime; }
        else { hunger -= hungerLoss; hungerTimer = hungerStep; }

        age += Time.deltaTime;

        if (hunger <= 50 || state == MoleStates.sleeping)
        {
            state = MoleStates.exploring;
        }

        if (energy <= 20)
        {
            state = MoleStates.sleeping;
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
        food_seen.Enqueue(food);
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
            }

            if (food_seen.Count != 0)
            {
                target = food_seen.Dequeue().transform.position;
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
        else
        {
            transform.position = Vector2.MoveTowards(transform.position, target, speed * Time.deltaTime);
        }
    }

    public void escape()
    {
        if (enemies_sensed.Count != 0)
        {
            Debug.Log("heading home");
            target = home.transform.position;
        }
        else
        {
            state = MoleStates.exploring;
        }
        transform.position = Vector2.MoveTowards(transform.position, target, speed * Time.deltaTime);
    }

    public void sleep()
    {
        if (target != (Vector2)home.transform.position)
        {
            target = home.transform.position;
        }
        else
        {
            transform.position = Vector2.MoveTowards(transform.position, target, speed * Time.deltaTime);
        }
    }

    //game functions
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
            if (state == MoleStates.sleeping || state == MoleStates.escaping)
            {
                home = collision.collider.gameObject;
                hide();
                state = MoleStates.sleeping;
                energy += 10 * Time.deltaTime;
            }
        }
    }

}
