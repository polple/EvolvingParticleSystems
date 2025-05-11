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
    public float startLifetime;

    public float rateOverTime;
    //public bool hasBurst;
    //public float burstInterval;
    public enum SizeOverTime { Shrinks, NoChange, Grows }
    public SizeOverTime sizeOverTimeVar; //currently sizeOverTime does not mutate and is not counted in similarity score

    public enum MeshType { Cube, Sphere }
    public MeshType meshTypeVar;

    public enum EmissionShape { Cone, Sphere }
    public EmissionShape emissionShapeVar;

    public string DNA = "--ERROR DNA IS BLANK--";

    public void firstGen()
    {
        direction = Quaternion.LookRotation(Random.onUnitSphere).eulerAngles;
        startSpeed = Random.Range(-1.0f, 2f);
        startSize = Random.Range(0.05f, 0.8f);
        startLifetime = Random.Range(0.5f, 8f);
        colour = new Color(Random.Range(0, 1f), Random.Range(0, 1f), Random.Range(0, 1f), 1f);
        rateOverTime = Random.Range(1f, 25f);
        //hasBurst = Random.value < 0.5f;
        //burstInterval = Random.Range(0.1f, 5f);
        sizeOverTimeVar = (SizeOverTime)Random.Range(0, 3);
        meshTypeVar = (MeshType)Random.Range(0, 2);
        emissionShapeVar = (EmissionShape)Random.Range(0, 2);

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

    public void setStartLifetime()
    {
        //set colour of the particles
        var main = GetComponent<ParticleSystem>().main;
        main.startLifetime = startLifetime;
    }

    public void setRateOverTime()
    {
        //set colour of the particles
        var emi = GetComponent<ParticleSystem>().emission;
        emi.rateOverTime = rateOverTime;
    }

    public void setSizeOverTime()
    {
        var sizeOverTime = GetComponent<ParticleSystem>().sizeOverLifetime;
        AnimationCurve curve = new AnimationCurve();
        sizeOverTime.enabled = true;
        switch (sizeOverTimeVar)
        {
            case SizeOverTime.Shrinks:
                curve.AddKey(0.0f, 1.0f);
                curve.AddKey(1.0f, 0.0f);
                break;

            case SizeOverTime.NoChange:
                curve.AddKey(0.0f, 1.0f);
                curve.AddKey(1.0f, 1.0f);
                break;

            case SizeOverTime.Grows:
                curve.AddKey(0.0f, 0.0f);
                curve.AddKey(1.0f, 1.0f);
                break;
        }
        sizeOverTime.size = new ParticleSystem.MinMaxCurve(1.0f, curve);
    }

    public void setMeshType()
    {
        var renderer = GetComponent<ParticleSystemRenderer>();
        switch (meshTypeVar)
        {
            case MeshType.Cube:
                renderer.mesh = getPrimitiveMesh(PrimitiveType.Cube);
                break;

            case MeshType.Sphere:
                renderer.mesh = getPrimitiveMesh(PrimitiveType.Sphere);
                break;
        }
    }

    public void setEmissionShapeType()
    {
        var shape = GetComponent<ParticleSystem>().shape;
        switch (emissionShapeVar)
        {
            case EmissionShape.Cone:
                shape.shapeType = ParticleSystemShapeType.Cone;
                break;

            case EmissionShape.Sphere:
                shape.shapeType = ParticleSystemShapeType.Sphere;
                break;
        }
    }

    private Mesh getPrimitiveMesh(PrimitiveType type)
    {
        GameObject temp = GameObject.CreatePrimitive(type);
        Mesh mesh = temp.GetComponent<MeshFilter>().sharedMesh;
        DestroyImmediate(temp);
        return mesh;
    }


    public void setAllBasedOnController()
    {
        setDirection();
        setColour();
        setStartSpeed();
        setStartSize();
        setStartLifetime();
        setRateOverTime();
        setSizeOverTime();
        setMeshType();
        setEmissionShapeType();

        DNA = direction.ToString() + ";" + colour.ToString() + ";" + startSpeed.ToString() + ";" + startSize.ToString() + ";" + startLifetime.ToString() + ";" + rateOverTime.ToString() + ";" + sizeOverTimeVar.ToString()+ ";" + meshTypeVar.ToString() +";" + emissionShapeVar.ToString(); //set DNA
        FindAnyObjectByType<WriteDataToFile>().WriteToFile(DNA);
    }

}
