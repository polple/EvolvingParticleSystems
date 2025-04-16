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

        //loop over number of containers
        for (int i = 0; i < SpecimenContainers.Length; i++)
        {
            //Pick a random individual that has not already been picked
            Debug.Log(ga.population.Count);
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

}
