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

    //scores for certain metrics
    public float fireSimilarity = 0;
    public float bubbleSimilarity = 0;

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

    //calculate similarity to fire
    public void calcFieryScore()
    {
        fireSimilarity = 0;
        fireSimilarity = fireSimilarity + CompareValues(startLifetime, 0.5f);
        fireSimilarity = fireSimilarity + CompareValues(rateOverTime, 30f);
        fireSimilarity = fireSimilarity + CompareColors(colour, Color.red);
        fireSimilarity = fireSimilarity + CompareValues(direction.x, -90);
        if (this.emissionShapeVar == EmissionShape.Cone)
            fireSimilarity++;
        if (this.sizeOverTimeVar == SizeOverTime.Shrinks)
            fireSimilarity++;
    }

    //Calculate similarity to bubbles
    public void calcBubbleScore()
    { 
        bubbleSimilarity = 0;
        bubbleSimilarity = bubbleSimilarity + CompareColors(colour, Color.cyan);
        if (this.startLifetime > 1f)
            bubbleSimilarity++;
        if (this.meshTypeVar == MeshType.Sphere)
            bubbleSimilarity++;
        if (this.sizeOverTimeVar == SizeOverTime.Grows)
            bubbleSimilarity++;
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

        calcFieryScore();
        calcBubbleScore();

        DNA = direction.ToString() + ";" + colour.ToString() + ";" + startSpeed.ToString() + ";" + startSize.ToString() + ";" + startLifetime.ToString() + ";" + rateOverTime.ToString() + ";" + sizeOverTimeVar.ToString()+ ";" + meshTypeVar.ToString() +";" + emissionShapeVar.ToString()+";"+fireSimilarity+";"+bubbleSimilarity; //set DNA
        FindAnyObjectByType<WriteDataToFile>().WriteToFile(DNA);
    }



    //------------Comparison Functions----------------//

    //Linear Value Comparison
    public static float CompareValues(float a, float b)
    {
        if (Mathf.Approximately(a, b))
            return 1f;

        float maxPossibleDifference = Mathf.Max(a, b);

        if (maxPossibleDifference == 0)
            return 1f; // Avoid division by zero

        //checking for negative numbers
        //if opposite signs then 0 since even if they are close they will act very different
        if (Mathf.Sign(a) != Mathf.Sign(b))
            return 0f;
        //if both are negative then make both positive and update the max number
        if (Mathf.Sign(a) == -1 && Mathf.Sign(b) == -1)
        {
            a = Mathf.Abs(a);
            b = Mathf.Abs(b);
            maxPossibleDifference = Mathf.Max(a, b);
        }

        float difference = Mathf.Abs(a - b);

        return 1f - (difference / maxPossibleDifference);
    }

    //Colour Value Comparison
    public static float CompareColors(Color a, Color b)
    {
        float rDiff = Mathf.Abs(a.r - b.r);
        float gDiff = Mathf.Abs(a.g - b.g);
        float bDiff = Mathf.Abs(a.b - b.b);
        float aDiff = Mathf.Abs(a.a - b.a);
        float avgDiff = (rDiff + gDiff + bDiff + aDiff) / 4f;
        return 1f - avgDiff;
    }

    //Vector Comparison
    public static float CompareVectors(Vector3 a, Vector3 b)
    {
        if (a == b)
            return 1f;
        else
            return Vector3.Dot(a.normalized, b.normalized) * 0.5f + 0.5f; // Convert [-1,1] to [0,1]
    }
}
