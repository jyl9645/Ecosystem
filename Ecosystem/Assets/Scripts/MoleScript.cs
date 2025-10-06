using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MoleScript : MonoBehaviour
{
    //stats
    public float hunger = 100;
    public float energy = 100;
    public float health = 100;
    public float age = 0;
    MoleStates state = MoleStates.exploring;

    //stat_update variables
    float hungerStep = 5;
    public float hungerTimer;
    public float hungerLoss = 0.2f;

    //states
    enum MoleStates
    {
        sleeping,
        eating,
        exploring,
        escaping,
        fighting,
        breeding,
        idling,
        dying
    }

    //other vars
    public GameObject home;
    public Vector2 target;

    Queue<GameObject> food_seen = new Queue<GameObject>();
    List<GameObject> enemies_sensed = new List<GameObject>();

    //helper functions
    public void stat_update()
    {
        if (hungerTimer > 0) { hungerTimer -= Time.deltaTime; }
        else { hunger -= hungerLoss; hungerTimer = hungerStep; }

        age += Time.deltaTime;
    }

    public void add_enemy(GameObject enemy)
    {
        enemies_sensed.Add(enemy);
    }

    public void add_food(GameObject food)
    {
        Debug.Log("added");
        food_seen.Enqueue(food);
    }

    public void sense_tunnel(GameObject tunnel)
    {
        target = tunnel.transform.position;
    }

    //state functions
    public void explore()
    {

        if (enemies_sensed.Count != 0)
        {
            state = MoleStates.escaping;
        }

        else
        {
            if (target == Vector2.zero || new Vector2(transform.position.x, transform.position.y) == target)
            {
                target = new Vector2(Random.Range(-8, 8), Random.Range(-4, 4));
            }
            else if ((food_seen.Count == 0) && (enemies_sensed.Count == 0))
            {
                transform.position = Vector2.MoveTowards(transform.position, target, 0.5f);
            }

            if (food_seen.Count != 0)
            {
                target = food_seen.Dequeue().transform.position;
                state = MoleStates.eating;
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
            transform.position = Vector2.MoveTowards(transform.position, target, 0.5f);
        }
    }

    public void escape()
    {

    }

    //game functions
    void Start()
    {
        hungerTimer = hungerStep;
    }

    void Update()
    {
        switch (state)
        {
            case MoleStates.exploring:
                explore();
                break;

            case MoleStates.eating:
                eat();
                break;

            default:
                break;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("berry"))
        {
            Debug.Log("eating");
        }
    }
}
