using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class MainController : MonoBehaviour {

    public int noOfLoaders;
    public int noOfMines;
    public GameObject loader;
    public GameObject mine;
    public Text generationCountText;
    private GameObject[] loaders;

    private int numberOfWeightsInNN;

    private GenAlg genAlg;
    private List<GenAlg.Genome> population;

    public float timeLeft;
    private float timeTicker;

    private int generationCount = 0;
        

    // Use this for initialization
    void Start () {

        genAlg = GetComponent<GenAlg>();
        timeTicker = timeLeft;

        for(int i = 0; i < noOfLoaders; i++)
        {
            float randomX = Random.Range(-4.5f, 4.5f);
            float randomZ = Random.Range(-4.5f, 4.5f);

            float randomRotationY = Random.Range(0.0f, 360.0f);

            Instantiate(loader, new Vector3(randomX, 0.3f, randomZ),  Quaternion.Euler(0.0f, randomRotationY, 0.0f));
        }


        for(int i = 0; i < noOfMines; i++)
        {
            float randomX = Random.Range(-4.5f, 4.5f);
            float randomZ = Random.Range(-4.5f, 4.5f);

            Instantiate(mine, new Vector3(randomX, 0.25f, randomZ), transform.rotation);
        }

        loaders = GameObject.FindGameObjectsWithTag("Loader");

        if (loaders.Length != 0)
        {
            numberOfWeightsInNN = loaders[0].GetComponent<LoaderController>().GetNumberOfWeights();

            genAlg.InitaliseGenAlg(noOfLoaders);

            population = genAlg.GetChromos();

            for (int i = 0; i < noOfLoaders; i++)
            {
                List<float> weights = population[i].GetWeights();
                loaders[i].GetComponent<LoaderController>().InstantiateID(i);
                loaders[i].GetComponent<NeuralNetwork>().PutWeights(ref weights);
            }

        }

        UpdateGenerationText();

	}
	
	// Update is called once per frame
	void Update () {

        timeTicker -= Time.deltaTime;

        Time.timeScale = 2;

        if(timeTicker < 0)
        {
            generationCount++;
            UpdateGenerationText();

            population = genAlg.Epoch(ref population);


            for (int i = 0; i < noOfLoaders; ++i)
            {
                NeuralNetwork neuralNetwork = loaders[i].GetComponent<NeuralNetwork>();
                LoaderController loaderController = loaders[i].GetComponent<LoaderController>();

                float randomX = Random.Range(-4.5f, 4.5f);
                float randomZ = Random.Range(-4.5f, 4.5f);
                List<float> weights = new List<float>(population[i].GetWeights());

                neuralNetwork.PutWeights( ref weights);

                Debug.Log(i);

                loaders[i].transform.position = new Vector3(randomX, 0.3f, randomZ);
                loaderController.ResetFitness();
                weights.Clear();
            }

            timeTicker = timeLeft;
        }
	}

    public int getNumberOfWeights()
    {
        return numberOfWeightsInNN;
    }

    public void UpdateMineAndFitness(int id, ref Collider mine)
    {
        float randomX = Random.Range(-4.5f, 4.5f);
        float randomZ = Random.Range(-4.5f, 4.5f);
        population[id].IncrementFitness();
        mine.transform.position = new Vector3(randomX, 0.3f, randomZ);
    }


    private void UpdateGenerationText()
    {
        generationCountText.text = "Generation Count: " + generationCount;
    }
    
}
