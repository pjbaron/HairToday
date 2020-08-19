using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

public class Strand : MonoBehaviour
{
    [Header("-- set these --")]
    [SerializeField] public float GrowthRate = 0.02f;
    [SerializeField] GameObject HairNodePrefab = null;
    [SerializeField] float MaxLength = 1;
    [SerializeField] int iterations = 10;

    [Header("-- view only --")]
    [SerializeField] public GameObject head = null;
    [SerializeField] float length;
    [SerializeField] float nodeLength;
    [SerializeField] List<GameObject> hairNodes = new List<GameObject>(32);
	
	
    void Start()
    {
        length = 0f;
        nodeLength = 0.2f;
    }

    void Update()
    {
//        Profiler.BeginSample("verlet");

        // iterate the hairNodes to update their Verlet parameters
        foreach (GameObject go in hairNodes)
        {
            HairNode script = go.GetComponent<HairNode>();
            script.VerletMove(Time.deltaTime);
        }

        // fix the Verlet line length constraints
        int c = iterations;
        while(c-- > 0)
        {
            ConstrainLines();
        }

        foreach (GameObject go in hairNodes)
        {
            HairNode script = go.GetComponent<HairNode>();
            script.VerletConstrainHead();
        }

        Vector3 prior = transform.position;
        foreach (GameObject go in hairNodes)
        {
            HairNode script = go.GetComponent<HairNode>();
            if (prior != null)
                script.VerletConnect(prior);
            prior = go.transform.position;
        }

//        Profiler.EndSample();
    }

    void ConstrainLines()
    {
        bool locked = true;
        GameObject node1 = gameObject;
        foreach(GameObject node2 in hairNodes)
        {
            ConstrainLine(node1, node2, locked);
            node1 = node2;
            locked = false;
        }
    }

    void ConstrainLine(GameObject node1, GameObject node2, bool locked1)
    {
        HairNode script1 = null;
        Vector3 p1;
        if (!locked1)
        {
            script1 = node1.GetComponent<HairNode>();
            p1 = script1.position;
        }
        else
        {
            p1 = transform.position;
        }
        HairNode script2 = node2.GetComponent<HairNode>();
        Vector3 p2 = script2.position;
        Vector3 diff = p2 - p1;
        float distance = diff.magnitude;
        float move = ((nodeLength - distance) / distance) / 2.0f;
        diff *= move;
        if (!locked1)
        {
            script1.position -= diff;
            script2.position += diff;
        }
        else
        {
            script2.position += diff * 2;
        }
    }

    public void Grow(float deltaTime)
    {
        if (length < MaxLength)
        {
            length += deltaTime * Random.Range(GrowthRate * 0.50f, GrowthRate);
            if (length > hairNodes.Count * nodeLength)
            {
                // add a new 'node' to this strand
                GameObject go = Instantiate(HairNodePrefab, transform);
                HairNode script = go.GetComponent<HairNode>();
                script.SetHead(head);
                go.transform.parent = transform;
                if (hairNodes.Count == 0)
                {
                    // grow first node straight out from the head
                    script.position = transform.position + (transform.position - head.transform.position).normalized * 0.1f;
                }
                else
                {
                    // grow next node in a straight line with previous nodes
                    GameObject prior = hairNodes[hairNodes.Count - 1];
                    GameObject priorer = NodeBefore(hairNodes.Count - 1);
                    script.position = prior.transform.position + (prior.transform.position - priorer.transform.position).normalized * 0.1f;
                }

                go.transform.position = script.lastPosition = script.position;
                hairNodes.Add(go);
            }
        }
    }

    GameObject NodeBefore(int index)
    {
        if (index > 0)
        {
            return hairNodes[index - 1];
        }
        return gameObject;
    }

}

