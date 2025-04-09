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
        //take the prime specimen

        //determine distance of each individual to it

        //set the distance for each individual

        //breed next population //CHANGED THIS SO BUTTON JUST CALLS DIRECTLY AFTER PRIME SPECIMIN
        //BreedNewPopulation();
    }

    GameObject Breed(GameObject parent1, GameObject parent2)
    {
        GameObject offspring = Instantiate(IndividualPrefab, this.transform.position, this.transform.rotation);
        //offspring.transform.Translate(Random.Range(-2.05f, 2.05f), Random.Range(-2.05f, 2.05f), 0); //spawn position noise
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
            int ran = Random.Range(0, 20);
            if (ran < 10) { cont.startSpeed = parent1.GetComponent<ParticleSystemController>().startSpeed; }
            else { cont.startSpeed = parent2.GetComponent<ParticleSystemController>().startSpeed; }
            cont.setStartSpeed();

            //Size to inherit
            ran = Random.Range(0, 20);
            if (ran < 10) { cont.startSize = parent1.GetComponent<ParticleSystemController>().startSize; }
            else { cont.startSize = parent2.GetComponent<ParticleSystemController>().startSize; }
            cont.setStartSize();

            //Direction to inherit
            ran = Random.Range(0, 20);
            if (ran < 10) { cont.direction = parent1.GetComponent<ParticleSystemController>().direction; }
            else { cont.direction = parent2.GetComponent<ParticleSystemController>().direction; }
            cont.setDirection();

            //colour to inherit
            ran = Random.Range(0, 20);
            if (ran < 10) { cont.colour = parent1.GetComponent<ParticleSystemController>().colour; }
            else { cont.colour = parent2.GetComponent<ParticleSystemController>().colour; }
            cont.setColour();

        }

        //Potential Smaller Additional Mutation
        if (rand > 90) //smaller mutation with 10% chance
        {
            float directionChange = Random.Range(-1f, 1f);
            float speedChange = Random.Range(-0.2f, 0.6f);
            float sizeChange = Random.Range(-0.2f, 0.6f);
            float colourChange = Random.Range(-1, 1f);

            int pickDir = Random.Range(0, 4);
            if (pickDir == 1) { cont.direction.x = cont.direction.x + directionChange; }
            else if (pickDir == 2) { cont.direction.z = cont.direction.z + directionChange; }
            else if (pickDir == 3) { cont.direction.y = cont.direction.y + directionChange; }

            int changeSpeed = Random.Range(0, 4);
            if (changeSpeed < 2)
            {
                cont.startSpeed = cont.startSpeed + speedChange;
            }

            cont.startSize = cont.startSize + sizeChange;

            cont.colour = new Color(cont.colour.r + colourChange, cont.colour.g + colourChange, cont.colour.b + colourChange, 1f);


        }

        return offspring;
    }

    public void BreedNewPopulation()
    {
        List<GameObject> sortedPop= population.OrderBy(o => (o.GetComponent<ParticleSystemController>().fitness)).ToList();

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
}
