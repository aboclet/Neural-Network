using UnityEngine;
using System.Collections.Generic;

public class GenAlg : MonoBehaviour {


    public struct Genome
    {
        public List <float> weights;

        public float fitness;

        public Genome(List<float> insertedWeights, float fitness)
        {
            weights = new List<float>();
            this.fitness = fitness;
        }

        public List<float> GetWeights()
        {
            return weights;
        }

        public void AddWeight(float weightValue)
        {
            weights.Add(weightValue);
        }

        public void IncrementFitness()
        {
            fitness++;
        }

    };

    private int populationSize;

    private float totalFitness = 0;
    private float bestFitness = 0;
    private float worstFitness = 99999999;
    private float averageFitness = 0;

    private int fittestGenome = 0;

    private int numberOfElite = 2;
    private int numberOfCopiesElite = 1;



    private float mutationRate = 0.1f;
    private float crossoverRate = 0.7f;
    private float perturbation = 0.3f;

    private List<Genome> genomes;

    private MainController mainController;

    public void InitaliseGenAlg(int size)
    {
        mainController = GetComponent<MainController>();
        genomes = new List<Genome>();

        populationSize = size;

        for (int i = 0; i < populationSize; ++i)
        {
            Genome genome = new Genome(null, 0);
            genomes.Add(genome);
        }
        for (int i = 0; i < genomes.Count; ++i)
        {
            for (int j = 0; j < mainController.getNumberOfWeights(); ++j)
            {
                
                genomes[i].AddWeight(Random.Range(-1.0f, 1.0f));
            }
        }
        

    }


    private void Mutate(ref List<float>chromo)
    {
        for(int i = 0; i < chromo.Count; ++i)
        {
            if(Random.Range(0.0f, 1.0f) < mutationRate)
            {
                chromo[i] += Random.Range(-1.0f, 1.0f * perturbation);
            }
        }
    }

    private Genome GetChromoRoulette()
    {
        float slice = Random.Range(0.0f, 1.0f) * totalFitness;

        Genome chosenOne = new Genome(); 

        float fitnessSoFar = 0;

        for(int i = 0; i < populationSize; ++i)
        {
            fitnessSoFar += genomes[i].fitness;

            if(fitnessSoFar >= slice)
            {
                chosenOne = genomes[i];
                break;
            }
        }


        return chosenOne;
    }
	
    private void Crossover(ref List<float> mum, ref List<float> dad, ref List<float>baby1, ref List<float>baby2)
    {
        if(Random.Range(0.0f, 1.0f) > crossoverRate || mum == dad)
        {
            baby1 = mum;
            baby2 = dad;

            return;
        }

        int cp = Random.Range(0, populationSize - 1);

        for(int i = 0; i < cp; ++i)
        {
            baby1.Add(mum[i]);
            baby2.Add(dad[i]);
        }

        for(int i = cp; i < mum.Count; ++i)
        {
            baby1.Add(dad[i]);
            baby2.Add(mum[i]);
        }

        return;
    }

    public List<Genome> Epoch(ref List<Genome> oldPopulation)
    {
        genomes = oldPopulation;

        Reset();

        
        //PROBLEM HERE//
        //genomes.Sort();

        CalculateBestAndWorst();

        List<Genome> newPopulation = new List<Genome>();

        
        //GrabBest(numberOfElite, numberOfCopiesElite, ref newPopulation);
        
        while (newPopulation.Count < populationSize)
        {
            Genome mum = GetChromoRoulette();
            Genome dad = GetChromoRoulette();

            List<float> mumWeights = mum.GetWeights();
            List<float> dadWeights = dad.GetWeights();

            List<float> baby1 = new List<float>();
            List<float> baby2 = new List<float>();

            Crossover(ref mumWeights, ref dadWeights, ref baby1, ref baby2);

            Mutate(ref baby1);
            Mutate(ref baby2);

            Genome genome1 = new Genome(null, 0);
            Genome genome2 = new Genome(null, 0);
            genome1.weights = baby1;
            genome2.weights = baby2;

            newPopulation.Add(genome1);
            newPopulation.Add(genome2);
        }

        Debug.Log(newPopulation[0].GetWeights().Count);

        genomes = newPopulation;

        Debug.Log(genomes[0].GetWeights().Count);

        return genomes;
    }

    private void CalculateBestAndWorst()
    {
        totalFitness = 0;

        float highestSoFar = 0;
        float lowestSoFar = 9999999;

        for(int i = 0; i < populationSize; i++)
        {
            if(genomes[i].fitness > highestSoFar)
            {
                highestSoFar = genomes[i].fitness;

                fittestGenome = i;

                bestFitness = highestSoFar;
            }

            if(genomes[i].fitness < lowestSoFar)
            {
                lowestSoFar = genomes[i].fitness;

                worstFitness = lowestSoFar;
            }

            totalFitness += genomes[i].fitness;
        }

        averageFitness += totalFitness / populationSize;
    }

    private void GrabBest(int best, int numCopies, ref List<Genome> population)
    {
        while(best > 0)
        {
            for(int i = 0; i < numCopies; ++i)
            {
                population.Add(genomes[(populationSize - 1) - best]);
            }

            best--; 
        }
    }

    private void Reset()
    {
        totalFitness = 0;
        bestFitness = 0;
        worstFitness = 99999999;
        averageFitness = 0;
    }


    public List<Genome> GetChromos()
    {
        return genomes;
    }

	// Update is called once per frame
	void Update () {
	
	}
}
