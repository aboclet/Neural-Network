using UnityEngine;
using System.Collections.Generic;


struct Neuron
{
    public List<float> weights;

    public int noOfInputs;

    public Neuron(int noOfInputs)
    {
        this.noOfInputs = noOfInputs;
        weights = new List<float>();

        for(int i = 0; i < this.noOfInputs + 1; ++i)
        {
            weights.Add(Random.Range(-1.0f, 1.0f));
        }
    }

    public List<float> GetWeights()
    {
        return weights;
    }
}

struct NeuronLayer
{
    public List<Neuron> neurons;

    public int noOfNeurons;

    public NeuronLayer(int noOfNeurons, int noOfInputsPerNeuron)
    {
        this.noOfNeurons = noOfNeurons;
        neurons = new List<Neuron>();

        for (int i = 0; i < this.noOfNeurons; ++i)
        {
            neurons.Add(new Neuron(noOfInputsPerNeuron));
            
        }
    }

    public List<Neuron> GetNeurons()
    {
        return neurons;
    }
}

public class NeuralNetwork : MonoBehaviour {


    private int numberOfInputs = 4;
    private int numberOfOutputs = 2;
    private int numberOfNeuronsPerHiddenLayer = 6;
    private int numberOfLayers = 1;

    private int bias = -1;
    private int activationResponse = 1;




    private List<NeuronLayer> neuronLayers;

    public void InitialiseNeuralNetwork()
    {
        neuronLayers = new List<NeuronLayer>();

        if (numberOfLayers > 0)
        {
           
            neuronLayers.Add(new NeuronLayer(numberOfNeuronsPerHiddenLayer, numberOfInputs));

            for (int i = 0; i < numberOfLayers - 1; ++i)
            {
                neuronLayers.Add(new NeuronLayer(numberOfNeuronsPerHiddenLayer, numberOfNeuronsPerHiddenLayer));
            }

            neuronLayers.Add(new NeuronLayer(numberOfOutputs, numberOfNeuronsPerHiddenLayer));
        }
        else
        {
            neuronLayers.Add(new NeuronLayer(numberOfOutputs, numberOfInputs));
        }

    }

    public List<float> GetOutputs(ref List<float> inputs)
    {
       // Debug.Log(inputs.Count);

        List<float> outputs = new List<float>();

        int cWeight = 0;

        if (inputs.Count != numberOfInputs)
        {
            return outputs;
        }

        for (int i = 0; i < numberOfLayers + 1; ++i)
        {
            if(i > 0)
            {
                
                inputs = new List<float>(outputs); ;
            }

            outputs.Clear();

            cWeight = 0;

            for (int j = 0; j < neuronLayers[i].noOfNeurons; ++j)
            {
                float netinput = 0;

                int numInputs = neuronLayers[i].GetNeurons()[j].noOfInputs;           

                for (int k = 0; k < numInputs - 1; ++k)
                {
                    netinput += neuronLayers[i].GetNeurons()[j].GetWeights()[k] * inputs[cWeight++];                 
                }
                
                netinput += neuronLayers[i].GetNeurons()[j].GetWeights()[numInputs] * bias;

                outputs.Add(SigmoidFunction(netinput, activationResponse));


                cWeight = 0;
            }

        }

        return outputs;
        
    }

    public List<float> GetWeights()
    {
        List<float> weights = new List<float>();

        for(int i = 0; i < numberOfLayers + 1; i++)
        {
            for(int j = 0; j < neuronLayers[i].noOfNeurons; j++)
            {
                for(int k = 0; k < neuronLayers[i].GetNeurons()[j].noOfInputs; k++)
                {
                    weights.Add(neuronLayers[i].GetNeurons()[j].GetWeights()[k]);
                }
            }
        }

        return weights;
    }

    public void PutWeights(ref List<float> weights)
    {
        int cWeight = 0;

        for(int i = 0; i < numberOfLayers + 1; ++i)
        {
            for(int j = 0; j < neuronLayers[i].noOfNeurons; ++j)
            {
                for(int k = 0; k < neuronLayers[i].GetNeurons()[j].noOfInputs; ++k)
                {
                    neuronLayers[i].GetNeurons()[j].GetWeights()[k] = weights[cWeight++];
                }
            }
        }
    }

    public int GetNumberOfWeights()
    {
        int weights = 0;

        for (int i = 0; i < numberOfLayers + 1; ++i)
        {
            for(int j = 0; j <neuronLayers[i].noOfNeurons; ++j)
            {
                for(int k = 0; k < neuronLayers[i].GetNeurons()[j].noOfInputs; ++k)
                {
                    weights++;
                }
            }
        }
        return weights;
    }

    private float SigmoidFunction(float netinput, float response)
    {
        return (1 / (1 + Mathf.Exp(-netinput / response)));
    }


}
