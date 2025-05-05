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

    public GameObject lastPrimeCandidateContainer;
    public GameObject lastPrimeCandidate;

    // Start is called before the first frame update
    void Start()
    {
        StartGA();
    }

    public void StartGA()
    {
        FindAnyObjectByType<WriteDataToFile>().WriteToFile("GENERATION NO 1");

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

        lastPrimeCandidate = PrimeIndividualContainer.transform.GetChild(0).gameObject;
        lastPrimeCandidate.transform.parent = lastPrimeCandidateContainer.transform;
        lastPrimeCandidate.transform.position = lastPrimeCandidateContainer.transform.position;

        FindAnyObjectByType<WriteDataToFile>().WriteToFile("SELECTED_INDIVIDUAL: " + lastPrimeCandidate.GetComponent<ParticleSystemController>().DNA);
        FindAnyObjectByType<WriteDataToFile>().WriteToFile("--------------------");
        FindAnyObjectByType<WriteDataToFile>().WriteToFile("GENERATION NO " + (generation+1)); //start of next generation

    }

    public float getFitnessBasedOnSimilarity(GameObject PrimeIndividual, GameObject IndividualBeingChecked)
    {
        //get their controllers
        ParticleSystemController prime = PrimeIndividual.GetComponent<ParticleSystemController>();
        ParticleSystemController indiv = IndividualBeingChecked.GetComponent<ParticleSystemController>();

        //break each feature into a 0 to 1 similarity scrore
        float sizeSimilarity = CompareValues(prime.startSize, indiv.startSize);
        float speedSimilarity = CompareValues(prime.startSpeed, indiv.startSpeed);
        float lifetimeSimilarity = CompareValues(prime.startLifetime, indiv.startLifetime);
        float rateSimilarity = CompareValues(prime.rateOverTime, indiv.rateOverTime);
        float colorSimilarity = CompareColors(prime.colour, indiv.colour);
        float directionSimilarity = CompareVectors(prime.direction, indiv.direction);

        float totalSimilarity = (sizeSimilarity + speedSimilarity + lifetimeSimilarity + rateSimilarity + colorSimilarity + directionSimilarity);
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

            //Size to inherit
            if (Random.value > 0.5f) { 
                cont.startSize = parent1.GetComponent<ParticleSystemController>().startSize; }
            else { 
                cont.startSize = parent2.GetComponent<ParticleSystemController>().startSize; }

            //Lifetime to inherit
            if (Random.value > 0.5f){
                cont.startLifetime = parent1.GetComponent<ParticleSystemController>().startLifetime;}
            else{
                cont.startLifetime = parent2.GetComponent<ParticleSystemController>().startLifetime;}

            //Direction to inherit
            if (Random.value > 0.5f) { 
                cont.direction = parent1.GetComponent<ParticleSystemController>().direction; }
            else { 
                cont.direction = parent2.GetComponent<ParticleSystemController>().direction; }

            //colour to inherit
            if (Random.value > 0.5f) { 
                cont.colour = parent1.GetComponent<ParticleSystemController>().colour; }
            else { 
                cont.colour = parent2.GetComponent<ParticleSystemController>().colour; }

            //RateOverTime to inherit
            if (Random.value > 0.5f) { 
                cont.rateOverTime = parent1.GetComponent<ParticleSystemController>().rateOverTime; }
            else { 
                cont.rateOverTime = parent2.GetComponent<ParticleSystemController>().rateOverTime; }

            //SizeOverTime to inherit
            if (Random.value > 0.5f) { 
                cont.sizeOverTimeVar = parent1.GetComponent<ParticleSystemController>().sizeOverTimeVar; }
            else { 
                cont.sizeOverTimeVar = parent2.GetComponent<ParticleSystemController>().sizeOverTimeVar; }

        }

        //Potential Smaller Additional Mutation
        if (rand > 30) //smaller mutations with 30% chance to occur
        {
            float directionChange = Random.Range(-10f, 10f);
            float speedChange = Random.Range(-0.9f, 0.9f);
            float sizeChange = Random.Range(-0.2f, 0.2f);
            float lifetimeChange = Random.Range(-3f, 3f);
            float colourChange_r = Random.Range(-0.3f, 0.3f);
            float colourChange_g = Random.Range(-0.3f, 0.3f);
            float colourChange_b = Random.Range(-0.3f, 0.3f);
            float rateOverTimeChange = Random.Range(-3, 3f);
            int sizeOverTimeChange = (int)Random.Range(0, 3f);

            //can alter x,y, and/or z direction
            int pickDir = Random.Range(0, 4);
            if (pickDir == 1) { cont.direction.x = cont.direction.x + directionChange; }
            else if (pickDir == 2) { cont.direction.z = cont.direction.z + directionChange; }
            else if (pickDir == 3) { cont.direction.y = cont.direction.y + directionChange; }

            //partial mutate speed
            if (Random.value > 0.5f)
                cont.startSpeed = cont.startSpeed + speedChange;

            //partial mutate startSize //cant go below 0.1
            if (Random.value > 0.5f && (cont.startSize+sizeChange > 0.1f))
                cont.startSize = cont.startSize + sizeChange;

            //partial mutate startLifetime //cant go below 0.1
            if (Random.value > 0.5f && (cont.startLifetime+lifetimeChange > 0.1f))
                cont.startLifetime = cont.startLifetime + lifetimeChange;

            //partial mutate Colour
            if (Random.value > 0.5f && cont.colour.r+colourChange_r<=1 && cont.colour.r+colourChange_r>=0)
                cont.colour = new Color(cont.colour.r + colourChange_r, cont.colour.g, cont.colour.b, 1f);
            if (Random.value > 0.5f && cont.colour.g+colourChange_g<=1 && cont.colour.g + colourChange_g >= 0)
                cont.colour = new Color(cont.colour.r, cont.colour.g + colourChange_g, cont.colour.b, 1f);
            if (Random.value > 0.5f && cont.colour.b+colourChange_b<=1 && cont.colour.b + colourChange_b >= 0)
                cont.colour = new Color(cont.colour.r, cont.colour.g, cont.colour.b + colourChange_b, 1f);

            //partial mutate rateOverTime
            if (Random.value > 0.5f && (cont.rateOverTime + rateOverTimeChange > 0.5f))
                cont.rateOverTime = cont.rateOverTime + rateOverTimeChange;

            //mutate the sizeOverTime
            if (Random.value > 0.7f)
            {
                if (sizeOverTimeChange == 0)
                    cont.sizeOverTimeVar = ParticleSystemController.SizeOverTime.Shrinks;
                else if (sizeOverTimeChange == 1)
                    cont.sizeOverTimeVar = ParticleSystemController.SizeOverTime.NoChange;
                else if (sizeOverTimeChange == 2)
                    cont.sizeOverTimeVar = ParticleSystemController.SizeOverTime.Grows;
            }

        }

        //Apply the DNA to the offsprings' particle system
        cont.setAllBasedOnController();

        //set fitness of offspring to similarity to last prime
        float fitness = getFitnessBasedOnSimilarity(lastPrimeCandidate, offspring);
        offspring.GetComponent<ParticleSystemController>().fitness = fitness;

        return offspring;
    }

    public void BreedNewPopulation()
    {
        List<GameObject> sortedPop = population.OrderBy(o => (o.GetComponent<ParticleSystemController>().fitness)).ToList();

        population.Clear();


        //Tournament Style Selection
        for (int i = 0; i < sortedPop.Count; i++)
        {
            GameObject parent1;
            GameObject parent2;

            //Selected parent is given a 5% chance to breed otherwise select normally
            if (Random.value > 0.05f)
            {
                parent1 = lastPrimeCandidate;
                parent2 = SelectParentViaTourney(sortedPop, 3);
            }
            // Select two parents via tournament
            else
            {
                parent1 = SelectParentViaTourney(sortedPop, 3);
                parent2 = SelectParentViaTourney(sortedPop, 3);
            }
            population.Add(Breed(parent1, parent2));
        }

        //destroy all parents and previous population
        //Unless is prime individual
        for (int i = 0; i < sortedPop.Count; i++)
        {
            if (!GameObject.ReferenceEquals(sortedPop[i], lastPrimeCandidate))
            {
                Destroy(sortedPop[i]);
            }
                
        }
        //sortedPop.Clear();
        generation++;

        //put top new individuals on display
        population = population.OrderByDescending(o => (o.GetComponent<ParticleSystemController>().fitness)).ToList(); //note currently you are checking for highest fitness at the end of the list in the for loop above but then highest fitness for the beginnning of the list for the upcoming line
        this.GetComponent<DisplayIndividuals>().putTopIndividualsOnDisplay();
    }

    //Parent Selection via Tournament 
    public GameObject SelectParentViaTourney(List<GameObject> population, int tournamentSize)
    {
        GameObject bestIndiv = null;

        for (int i = 0; i < tournamentSize; i++)
        {
            int randomIndex = Random.Range(0, population.Count);
            GameObject contender = population[randomIndex];

            if (bestIndiv == null || contender.GetComponent<ParticleSystemController>().fitness > bestIndiv.GetComponent<ParticleSystemController>().fitness)
            {
                bestIndiv = contender;
            }
        }

        return bestIndiv;
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
