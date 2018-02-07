using UnityEngine;
using System.Collections.Generic;

public class LoaderController : MonoBehaviour {

    private new Rigidbody rigidbody;

    private NeuralNetwork brain;
    private GameObject gameController;
    private MainController mainController;

    private int id;
    private float fitness = 0f;
    private float leftSteer;
    private float rightSteer;

    private RaycastHit hit;

    void OnEnable()
    {
        brain = GetComponent<NeuralNetwork>();
        
        brain.InitialiseNeuralNetwork();
    }

	// Use this for initialization
	void Start () {

        gameController = GameObject.FindWithTag("GameController");
        mainController = gameController.GetComponent<MainController>();
        rigidbody = GetComponent<Rigidbody>();
    }
	
	// Update is called once per frame
	void FixedUpdate ()
    {
        rigidbody.velocity = transform.forward * 1.0f;
    }


    public void InstantiateID(int id)
    {
        this.id = id;
    }

    public int ReturnID()
    {
        return id;
    }

    void Update()
    {

        Debug.DrawRay(transform.position, transform.forward, Color.green);

        List<float> inputs = new List<float>();

        GameObject closestMine = GetClosestMine();

        Vector3.Normalize(closestMine.transform.position);

        inputs.Add(closestMine.transform.position.x);
        inputs.Add(closestMine.transform.position.z);

        inputs.Add(transform.forward.x);
        inputs.Add(transform.forward.z);

        List<float> outputs = brain.GetOutputs(ref inputs);

        leftSteer = outputs[0];
        rightSteer = outputs[1];

        float rotationForce = leftSteer - rightSteer;

        Mathf.Clamp(rotationForce, -360f, 360f);

        transform.Rotate(0.0f, rotationForce * 10.0f, 0.0f);

        if(transform.position.x > 4.5)
        {
            transform.position = new Vector3(-4.5f, transform.position.y, transform.position.z);
        }
        if (transform.position.x < -4.5)
        {
            transform.position = new Vector3(4.5f, transform.position.y, transform.position.z);
        }
        if (transform.position.z > 4.5)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, -4.5f);
        }
        if (transform.position.z < -4.5)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, 4.5f);
        }

        GameObject[] loaders = GameObject.FindGameObjectsWithTag("Loader");
        for(int i = 0; i < loaders.Length; i++)
        {
            Physics.IgnoreCollision(loaders[i].GetComponent<Collider>(), GetComponent<Collider>(), true);
        }
        

    }

    public int GetNumberOfWeights()
    {
        return brain.GetNumberOfWeights();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Mine")
        {
            IncrementFitness();
            mainController.UpdateMineAndFitness(id, ref other);
        }
        if(other.tag == "Loader")
        {
            Debug.Log("Collision");
            Physics.IgnoreCollision(other, GetComponent<Collider>());
        }
    }

    private GameObject GetClosestMine()
    {
        GameObject[] mines = GameObject.FindGameObjectsWithTag("Mine");
        GameObject closestMine = null;

        float distance = Mathf.Infinity;
        Vector3 position = transform.position;
        foreach(GameObject mine in mines)
        {
            Vector3 diff = mine.transform.position - position;
            float curDistance = diff.sqrMagnitude;
            if(curDistance < distance)
            {
                closestMine = mine;
                distance = curDistance;
            }
        }
        return closestMine;

    }

    public void IncrementFitness()
    {
        fitness++;
    }

    public float GetFitness()
    {
        return fitness;
    }

    public void ResetFitness()
    {
        fitness = 0;
    }
}
