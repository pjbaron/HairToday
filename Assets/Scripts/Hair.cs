using System.Collections.Generic;
using System.Transactions;
using UnityEngine;

public class Hair : MonoBehaviour
{
    [Header("-- set these --")]
    [SerializeField] int NumStrands = 256;
    [SerializeField] GameObject StrandPrefab = null;

    [Header("-- view only --")]
    [SerializeField] List<GameObject> strands = new List<GameObject>(2048);
    [SerializeField] GameObject head = null;



    void Start()
    {
        head = transform.parent.gameObject;
        for (int i = 0; i < NumStrands; i++)
        {
            GameObject strand = CreateStrand();
            strands.Add(strand);
        }
    }


    GameObject CreateStrand()
    {
        GameObject strand = Instantiate(StrandPrefab, transform);
        Strand script = strand.GetComponent<Strand>();
        script.head = head;
        Vector3 scale = StrandPrefab.transform.localScale;
        Vector3 invertScale = new Vector3(1.0f / transform.lossyScale.x, 1.0f / transform.lossyScale.y, 1.0f / transform.lossyScale.z);
        strand.transform.localScale = new Vector3(scale.x * invertScale.x, scale.y * invertScale.y, scale.z * invertScale.z);

        // TODO: replace this pudding bowl with styles which specify the root base locations for each strand
        MoveToSurface(strand);
        return strand;
    }


    void MoveToSurface(GameObject strand)
    {
        float theta = 2.0f * Mathf.PI * Random.value;
        float y = Random.value * 0.75f + 0.25f;
        float x = Mathf.Sqrt(1f - y * y) * Mathf.Cos(theta);
        float z = Mathf.Sqrt(1f - y * y) * Mathf.Sin(theta);
        strand.transform.position = new Vector3(x * head.transform.localScale.x * 0.5f, y * head.transform.localScale.y * 0.5f, z * head.transform.localScale.z * 0.5f);
    }


    void Update()
    {
        foreach(GameObject go in strands)
        {
            Strand script = go.GetComponent<Strand>();
            script.Grow(Time.deltaTime);
        }
    }
}
