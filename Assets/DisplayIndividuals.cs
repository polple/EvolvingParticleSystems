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
        int[] specimentNumbers = new int[SpecimenContainers.Length];
        ga = this.GetComponent<GeneticAlgorithm>();

        //In order of Best
        //loop over number of containers
        for (int i = 0; i < SpecimenContainers.Length; i++)
        {
            //do things with the selected individual
            GameObject topInd = ga.population[i];
            topInd.transform.position = SpecimenContainers[i].transform.position;
            topInd.transform.parent = SpecimenContainers[i].transform;
        }
    }

}
