using UnityEngine;
using System.Linq;

public class DisplayIndividuals : MonoBehaviour
{
    
    private GeneticAlgorithm ga;
    [SerializeField] private GameObject[] SpecimenContainers;


    private void Start()
    {
        //ga = this.GetComponent<GeneticAlgorithm>();
    }

    public void putRandomIndividualsOnDisplay()
    {
        int[] specimentNumbers = new int[SpecimenContainers.Length];
        ga = this.GetComponent<GeneticAlgorithm>();

        //Random Pick
        //loop over number of containers
        for (int i = 0; i < SpecimenContainers.Length; i++)
        {
            //Pick a random individual that has not already been picked
            int ran = Random.Range(0, ga.population.Count);
            while (specimentNumbers.Contains(ran))
            {
                ran = Random.Range(0, ga.population.Count);
            }
            specimentNumbers[i] = ran;
            //do things with the selected individual
            GameObject ranInd = ga.population[ran];
            ranInd.transform.position = SpecimenContainers[i].transform.position;
            ranInd.transform.parent = SpecimenContainers[i].transform;
        }
    }

    public void putTopIndividualsOnDisplay()
    {
        int[] specimentNumbers = new int[SpecimenContainers.Length]; //used to see if individual has already been picked
        int newestSelectedSpecimen = 0;
        ga = this.GetComponent<GeneticAlgorithm>();

        //In order of Best
        //loop over number of containers
        for (int i = 0; i < SpecimenContainers.Length; i++)
        {
            int numOfInnerLoops = 0;
            //go again if the individual selected has already been picked
            do
            {
                //If first Pick the best (In order of Best)
                if (i == 0)
                {
                    //do things with the selected individual
                    newestSelectedSpecimen = i + numOfInnerLoops; //add number of inner loops in case the best is already taken
                }
                //if forth one pick the fieriest one
                else if (i == 3)
                {
                    float mostFireScore = -1;
                  
                    for (int j=0; j< ga.population.Count; j++)
                    {
                        var script = ga.population[j].GetComponent<ParticleSystemController>();

                        if (script.fireSimilarity > mostFireScore)
                        {
                            //dumb janky fix to avoid out of index when already in another container (yumm such a good fix (im tired))
                            if (j + numOfInnerLoops >= 20) numOfInnerLoops = numOfInnerLoops - 10;

                            mostFireScore = script.fireSimilarity;
                            newestSelectedSpecimen = j + numOfInnerLoops;
                        }
                    }
                }
                //if fifth one pick the bubbliest one
                else if (i == 4)
                {
                    float mostBubbleScore = -1;

                    for (int j = 0; j < ga.population.Count; j++)
                    {
                        ParticleSystemController par = ga.population[j].GetComponent<ParticleSystemController>();

                        if (par.bubbleSimilarity > mostBubbleScore)
                        {
                            //dumb janky fix to avoid out of index when already in another container (yumm such a good fix (im tired))
                            if (j+numOfInnerLoops>=20) numOfInnerLoops = numOfInnerLoops-10;

                            mostBubbleScore = par.bubbleSimilarity;
                            newestSelectedSpecimen = j + numOfInnerLoops;
                        }
                    }
                }
                //if second or third then use tournament on 2 random individuals
                else if (i == 1 || i == 2)
                {
                    int specimen_1 = Random.Range(0, ga.population.Count);
                    int specimen_2 = Random.Range(0, ga.population.Count);
                    if (specimen_1 >= specimen_2)
                        newestSelectedSpecimen = specimen_1;
                    else
                        newestSelectedSpecimen = specimen_2;
                }
                //if final iteration, get a wildcard (least similar option)
                else if (i == SpecimenContainers.Length - 1)
                {
                    newestSelectedSpecimen = ga.population.Count - 1 - numOfInnerLoops;
                }
                //else pick randomly
                else
                {
                    newestSelectedSpecimen = Random.Range(0, ga.population.Count);
                }
                numOfInnerLoops++;
            } while (specimentNumbers.Contains(newestSelectedSpecimen));

            //Put the newest speciment in the list of things already selected
            specimentNumbers[i] = newestSelectedSpecimen;

            try
            {
                //Put the selected Individual in the current container
                GameObject selectedInd = ga.population[newestSelectedSpecimen];
                selectedInd.transform.position = SpecimenContainers[i].transform.position;
                selectedInd.transform.parent = SpecimenContainers[i].transform;
            }
            catch { Debug.Log(newestSelectedSpecimen + " df innerloop " + numOfInnerLoops +" df container "+ i); }
        }
    }

}
