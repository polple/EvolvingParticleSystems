using UnityEngine;

public class ParticleSystemController : MonoBehaviour
{
    //public GameObject goal;
    //public float distanceTillGoalHeuristic; //sum of shortest and current distance till goal
    //public float shortestDistanceTillGoal;
    //public float CurrentDistanceTillGoal;

    public float fitness = 0;

    public Vector3 direction;

    public Color colour;
    public float startSpeed;
    public float startSize;

    public float rateOverTime;
    public bool HasBurst;
    public bool BurstInterval;


    public void firstGen()
    {
        direction = Quaternion.LookRotation(Random.onUnitSphere).eulerAngles;
        startSpeed = Random.Range(0.1f, 3f);
        startSize = Random.Range(0.05f, 1f);
        colour = new Color(Random.Range(0, 1f), Random.Range(0, 1f), Random.Range(0, 1f), 1f);
        rateOverTime = Random.Range(1f, 25f);

        setAllBasedOnController();
    }

    public void setDirection()
    {
        //set colour of the particles
        var shape = GetComponent<ParticleSystem>().shape;
        shape.rotation = direction;
    }

    public void setColour()
    {
        //set colour of the particles
        //var main = GetComponent<ParticleSystem>().main;
        //main.startColor = colour;
        //set colour of the render material
        GetComponent<Renderer>().material.color = colour;
    }

    public void setStartSpeed()
    {
        //set colour of the particles
        var main = GetComponent<ParticleSystem>().main;
        main.startSpeed = startSpeed;
    }

    public void setStartSize()
    {
        //set colour of the particles
        var main = GetComponent<ParticleSystem>().main;
        main.startSize = startSize;
    }

    public void setRateOverTime()
    {
        //set colour of the particles
        var emi = GetComponent<ParticleSystem>().emission;
        emi.rateOverTime = rateOverTime;
    }


    public void setAllBasedOnController()
    {
        setDirection();
        setColour();
        setStartSpeed();
        setStartSize();
        setRateOverTime();
    }

}
