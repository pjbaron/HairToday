using System.Collections.Generic;
using UnityEngine;

public class HairNode : MonoBehaviour
{
    //[Header("-- set these --")]
    [SerializeField] float drag = 0.75f;
    [SerializeField] Vector3 gravity = new Vector3(0, -0.01f, 0);

    [Header("-- view only --")]
    [SerializeField] public Vector3 externalForces = Vector3.zero;
    [SerializeField] GameObject head;
    [SerializeField] Vector3 headFactors;
    [SerializeField] public Vector3 position;
    [SerializeField] public Vector3 lastPosition;
    [SerializeField] Transform cylinder;
    [SerializeField] Head headScript;
	
	
    void OnEnable()
    {
        position = lastPosition = transform.position;
        cylinder = transform.GetChild(0);
    }

    public void VerletMove(float deltaTime)
    {
        Vector3 vel = (lastPosition - position) * drag;
        lastPosition = position;
        position += vel + (gravity + headScript.externalForces) * deltaTime;
    }

    public void VerletConstrainHead()
    {
        // don't penetrate the head mesh
        MoveToSurface();
    }


    public void VerletConnect(Vector3 point)
    {
        transform.position = position;
        Vector3 diff = point - position;
        Vector3 scale = cylinder.localScale;
        scale.y = diff.magnitude * 0.5f;
        cylinder.localScale = scale;
        gameObject.transform.LookAt(point);
        //Debug.DrawLine(transform.position, point, Color.white, 0.5f, true);
    }

    public void SetHead(GameObject _head)
    {
        head = _head;
        headFactors = new Vector3(head.transform.localScale.x * 0.5f, head.transform.localScale.y * 0.5f, head.transform.localScale.z * 0.5f);
        headScript = head.GetComponent<Head>();
    }

    // push point out of 'head' ellipsoid (assumes ellipsoid is axis-aligned)
    void MoveToSurface()
    {
        float dx = (position.x - head.transform.position.x) / headFactors.x;
        float dy = (position.y - head.transform.position.y) / headFactors.y;
        float dz = (position.z - head.transform.position.z) / headFactors.z;
        if (dx*dx + dy*dy + dz*dz < 1.0f)
        {
            Vector3 dirToSurface = (new Vector3(dx, dy, dz)).normalized;
            dirToSurface.x *= headFactors.x;
            dirToSurface.y *= headFactors.y;
            dirToSurface.z *= headFactors.z;
            position = head.transform.position + dirToSurface;
        }
        lastPosition = position;
    }

}
