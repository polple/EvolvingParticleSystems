using UnityEngine;
using System.Collections.Generic;
using System.Linq;


public class GeneticAlgorithm : MonoBehaviour
{
    public GameObject IndividualPrefab;
    public int populationSize = 50;

    public float timePassed = 0;
    public float trialTime = 5;

    public List<GameObject> population = new List<GameObject>();

    int generation = 1;
    bool started = false;

    // Start is called before the first frame update
    void Start()
    {
        StartGA();
    }

    public void StartGA()
    {
        for (int i = 0; i < populationSize; i++)
        {
            GameObject individual = Instantiate(IndividualPrefab, this.transform.position, this.transform.rotation);
            individual.transform.Translate(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), 0); //move spawn a bit randomly
            individual.transform.parent = this.transform;
            individual.GetComponent<ParticleSystemController>().firstGen();
            population.Add(individual);
        }
        started = true;

        this.GetComponent<DisplayIndividuals>().putRandomIndividualsOnDisplay();
    }

    // Update is called once per frame
    void Update()
    {
        if (started == true)
        {
            timePassed += Time.deltaTime;
            if (timePassed >= trialTime)
            {
                //BreedNewPopulation();
                timePassed = 0;
            }
        }
    }

    public void SelectPrimeSpeciment(GameObject PrimeIndividualContainer)
    {

        //set distance of each individual to it
        foreach (GameObject individual in population) 
        {
            float fitness = getFitnessBasedOnSimilarity(PrimeIndividualContainer.transform.GetChild(0).gameObject, individual);
            individual.GetComponent<ParticleSystemController>().fitness = fitness;
        }

    }

    public float getFitnessBasedOnSimilarity(GameObject PrimeIndividual, GameObject IndividualBeingChecked)
    {
        //get their controllers
        ParticleSystemController prime = PrimeIndividual.GetComponent<ParticleSystemController>();
        ParticleSystemController indiv = IndividualBeingChecked.GetComponent<ParticleSystemController>();

        //break each feature into a 0 to 1 similarity scrore
        float sizeSimilarity = CompareValues(prime.startSize, indiv.startSize);
        float speedSimilarity = CompareValues(prime.startSpeed, indiv.startSpeed);
        float rateSimilarity = CompareValues(prime.rateOverTime, indiv.rateOverTime);
        float colorSimilarity = CompareColors(prime.colour, indiv.colour);
        float directionSimilarity = CompareVectors(prime.direction, indiv.direction);

        float totalSimilarity = (sizeSimilarity + speedSimilarity + rateSimilarity + colorSimilarity + directionSimilarity);
        return totalSimilarity;
    }

    GameObject Breed(GameObject parent1, GameObject parent2)
    {
        GameObject offspring = Instantiate(IndividualPrefab, this.transform.position, this.transform.rotation);
        offspring.transform.Translate(Random.Range(-3f, 3f), Random.Range(-2f, 2f), Random.Range(-3f, 3f)); //spawn position noise
        offspring.transform.parent = this.transform;
        ParticleSystemController cont = offspring.GetComponent<ParticleSystemController>();

        int rand = Random.Range(0, 100);
        if (rand == 1) //complete mutation 1% chance
        {
            //re-randomise all traits
            cont.firstGen();

        }
        else //Uniform Crossover i.e. for each trait pick one parent over the other
        {
            //Speed to inherit
            if (Random.value > 0.5f) { 
                cont.startSpeed = parent1.GetComponent<ParticleSystemController>().startSpeed; }
            else { 
                cont.startSpeed = parent2.GetComponent<ParticleSystemController>().startSpeed; }
            cont.setStartSpeed();

            //Size to inherit
            if (Random.value > 0.5f) { 
                cont.startSize = parent1.GetComponent<ParticleSystemController>().startSize; }
            else { 
                cont.startSize = parent2.GetComponent<ParticleSystemController>().startSize; }
            cont.setStartSize();

            //Direction to inherit
            if (Random.value > 0.5f) { 
                cont.direction = parent1.GetComponent<ParticleSystemController>().direction; }
            else { 
                cont.direction = parent2.GetComponent<ParticleSystemController>().direction; }
            cont.setDirection();

            //colour to inherit
            if (Random.value > 0.5f) { 
                cont.colour = parent1.GetComponent<ParticleSystemController>().colour; }
            else { 
                cont.colour = parent2.GetComponent<ParticleSystemController>().colour; }
            cont.setColour();

            //RateOverTime to inherit
            if (Random.value > 0.5f) { 
                cont.rateOverTime = parent1.GetComponent<ParticleSystemController>().rateOverTime; }
            else { 
                cont.rateOverTime = parent2.GetComponent<ParticleSystemController>().rateOverTime; }
            cont.setRateOverTime();

        }

        //Potential Smaller Additional Mutation
        if (rand > 90) //smaller mutation with 10% chance
        {
            float directionChange = Random.Range(-1f, 1f);
            float speedChange = Random.Range(-0.2f, 0.6f);
            float sizeChange = Random.Range(-0.2f, 0.6f);
            float colourChange = Random.Range(-1, 1f);
            float rateOverTimeChange = Random.Range(-2, 2f);

            //can alter x,y, and/or z direction
            int pickDir = Random.Range(0, 4);
            if (pickDir == 1) { cont.direction.x = cont.direction.x + directionChange; }
            else if (pickDir == 2) { cont.direction.z = cont.direction.z + directionChange; }
            else if (pickDir == 3) { cont.direction.y = cont.direction.y + directionChange; }

            //partial mutate speed
            if (Random.value > 0.5f)
                cont.startSpeed = cont.startSpeed + speedChange;

            //partial mutate startSize
            if (Random.value > 0.5f)
                cont.startSize = cont.startSize + sizeChange;

            //partial mutate Colour
            if (Random.value > 0.5f)
                cont.colour = new Color(cont.colour.r + colourChange, cont.colour.g + colourChange, cont.colour.b + colourChange, 1f);

            //partial mutate startSize
            if (Random.value > 0.5f)
                cont.rateOverTime = cont.rateOverTime + rateOverTimeChange;

        }

        return offspring;
    }

    public void BreedNewPopulation()
    {
        List<GameObject> sortedPop = population.OrderBy(o => (o.GetComponent<ParticleSystemController>().fitness)).ToList();

        population.Clear();
        for (int i = (int)(3 * sortedPop.Count / 4.0f) - 1; i < sortedPop.Count - 1; i++)
        {
            population.Add(Breed(sortedPop[i], sortedPop[i + 1]));
            population.Add(Breed(sortedPop[i + 1], sortedPop[i]));
            population.Add(Breed(sortedPop[i], sortedPop[i + 1]));
            population.Add(Breed(sortedPop[i + 1], sortedPop[i]));
        }

        //destroy all parents and previous population
        for (int i = 0; i < sortedPop.Count; i++)
        {
            Destroy(sortedPop[i]);
        }
        generation++;

        //put random new individuals on display
        this.GetComponent<DisplayIndividuals>().putRandomIndividualsOnDisplay();
    }


    GUIStyle gui = new GUIStyle();
    void OnGUI()
    {
        gui.fontSize = 15;
        gui.normal.textColor = Color.white;
        GUI.BeginGroup(new Rect(10, 2, 250, 150));
        GUI.Label(new Rect(10, 2, 200, 30), "Gen: " + generation, gui);
        GUI.Label(new Rect(10, 20, 200, 30), "Population: " + population.Count, gui);
        //GUI.Label(new Rect(10, 38, 200, 30), string.Format("Time: {0:0.00}", timePassed), gui);
        GUI.EndGroup();
    }


    //------------Comparison Functions----------------//

    //Linear Value Comparison
    private static float CompareValues(float a, float b)
    {
        if (Mathf.Approximately(a, b)) return 1f;
            float maxPossibleDifference = Mathf.Max(a, b);
        if (maxPossibleDifference == 0) return 1f; // Avoid division by zero
            float difference = Mathf.Abs(a - b);
        return 1f - (difference / maxPossibleDifference);
    }

    //Colour Value Comparison
    private static float CompareColors(Color a, Color b)
    {
        float rDiff = Mathf.Abs(a.r - b.r);
        float gDiff = Mathf.Abs(a.g - b.g);
        float bDiff = Mathf.Abs(a.b - b.b);
        float aDiff = Mathf.Abs(a.a - b.a);
        float avgDiff = (rDiff + gDiff + bDiff + aDiff) / 4f;
        return 1f - avgDiff;
    }

    //Vector Comparison
    private static float CompareVectors(Vector3 a, Vector3 b)
    {
        if (a == b) 
            return 1f;
        else
            return Vector3.Dot(a.normalized, b.normalized) * 0.5f + 0.5f; // Convert [-1,1] to [0,1]
    }
}
