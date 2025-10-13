using UnityEngine;

public class BeetleScript : MonoBehaviour
{
    //stats
    public float hunger = 100;
    public float energy = 100;
    public float health = 100;
    public float age = 0;
    public float speed = 2f;
    public BeetleStates state = BeetleStates.exploring;

    //stat_update variables ----------------------------------------------------------
    float hungerStep = 2;
    public float hungerTimer;
    public float hungerLoss = 5f;

    float energyGain = 5;
    float energyLoss = 4;

    //states --------------------------------------------------------------------------
    public enum BeetleStates
    {
        sleeping,
        eating,
        exploring,
        escaping,
        birthing,
        dying
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
