using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class scriptCloth : MonoBehaviour
{
    private bool debugModeConstraints;
    public cameraScript controlRef;
    private List<Vector3> triangles = new List<Vector3>();
    private List<int> indices = new List<int>();

    public clothParticles[] particles;
    public List<Constraint> constraints = new List<Constraint>();
    public int nodeWidth;
    public int nodeHeight;

    public GameObject sphere;
    public float width;
    public float height;

    private bool nodeUnmovable = false;


    public float damping = 0.01f;
    public int constraintsIterations = 15;


    public Vector3 movementForce = Vector3.zero;

    private Vector3 lastHit = Vector3.zero;

    int timeStepSkip = 0;

    // Use this for initialization
    void Start()
    {
        GameObject camera = GameObject.FindGameObjectWithTag("MainCamera");
        controlRef = camera.GetComponent<cameraScript>();

        particles = new clothParticles[nodeWidth * nodeHeight]; //Vector to store matrices of node positions.
        for (int x = 0; x < nodeWidth; x++)
        {
            for (int y = 0; y < nodeHeight; y++)
            {
                Vector3 pos = new Vector3(width * (x / (float)nodeWidth), -height * (y / (float)nodeHeight), 0);
                particles[y * nodeWidth + x] = new clothParticles(this, pos); // insert particle xth column of Y row
            }
        }

        // Connecting close proximity neighbours with constraints.
        for (int x = 0; x < nodeWidth; x++)
        {
            for (int y = 0; y < nodeHeight; y++)
            {
                if (x < nodeWidth - 1) makeConstraint(getParticle(x, y), getParticle(x + 1, y));
                if (y < nodeHeight - 1) makeConstraint(getParticle(x, y), getParticle(x, y + 1));
                if (x < nodeWidth - 1 && y < nodeHeight - 1) makeConstraint(getParticle(x, y), getParticle(x + 1, y + 1));
                if (x < nodeWidth - 1 && y < nodeHeight - 1) makeConstraint(getParticle(x + 1, y), getParticle(x, y + 1));
            }
        }
        // Connecting secondary neighbours with constraints.
        for (int x = 0; x < 3; x++)
        {
            for (int y = 0; y < nodeHeight; y++)
            {
                if (x < nodeWidth - 2) makeConstraint(getParticle(x, y), getParticle(x + 2, y));
                if (y < nodeHeight - 2) makeConstraint(getParticle(x, y), getParticle(x, y + 2));
                if (x < nodeWidth - 2 && y < nodeHeight - 2) makeConstraint(getParticle(x, y), getParticle(x + 2, y + 2));
                if (x < nodeWidth - 2 && y < nodeHeight - 2) makeConstraint(getParticle(x + 2, y), getParticle(x, y + 2));
            }
        }

        //attaching node to the pole, relative to the size of the flags size.
        for (int i = 0; i < width; i++)
        {
            getParticle(0 + i, 0).offsetPos(new Vector3(0.8f, 0.7f, 0.7f)); 
            getParticle(0 + i, 0).makeUnmovable();
            getParticle(nodeWidth - 1 - i, 0).offsetPos(new Vector3(-0.8f, 0.7f, 0.7f));
            if (nodeUnmovable)
            {
                getParticle(0, nodeHeight - 1 - i).offsetPos(new Vector3(0.8f, 0.7f, 0.7f));
                getParticle(0, nodeHeight - 1 - i).makeUnmovable();
                getParticle(nodeWidth - 1 - i, nodeHeight - 1 - i).offsetPos(new Vector3(-0.8f, 0.7f, 0.7f));
                getParticle(nodeWidth - 1 - i, nodeHeight - 1 - i).makeUnmovable();
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        movementForce.x = controlRef.windX;
        movementForce.y = controlRef.windY;
        movementForce.z = controlRef.windZ;
        addForce(new Vector3(0, -0.98f, 0) * (Time.deltaTime)); //Gravity force
        addForce(movementForce * Time.deltaTime);
        windForce(new Vector3(0.00f, 0, 0.000f) * (Time.deltaTime)); //Wind force calculation
        timeStep();
    }
    
    void timeStep()
    {
        for (int i = 0; i < constraintsIterations; i++)
        {
            for (int u = 0; u < constraints.Count; u++)
            {
                constraints[u].constraintsMet(debugModeConstraints);
            }
        }
        for (int u = 0; u < particles.Length; u++)
        {
            particles[u].timeStep();
        }
    }

    public clothParticles getParticle(int x, int y) { return particles[y * nodeWidth + x]; }
    void makeConstraint(clothParticles p1, clothParticles p2) {
        var c = new Constraint(p1, p2);
        constraints.Add(c);
        p1.addConstraint(c);
        p2.addConstraint(c);
    }



    Vector3 calcNormal(clothParticles p1, clothParticles p2, clothParticles p3)
    {
        Vector3 pos1 = p1.getPos();
        Vector3 pos2 = p2.getPos();
        Vector3 pos3 = p3.getPos();
        Vector3 v1 = pos2 - pos1;
        Vector3 v2 = pos3 - pos1;
        return Vector3.Cross(v1, v2);
    }

    //calculated the wind force for each 'triangle' node
    void addWindForcesForTriangle(clothParticles p1, clothParticles p2, clothParticles p3, Vector3 direction)
    {

        Vector3 normal = calcNormal(p1, p2, p3);
        Vector3 d = normal.normalized;
        Vector3 force = normal * (Vector3.Dot(d, direction));
        p1.addForce(force);
        p2.addForce(force);
        p3.addForce(force);
    }

    public void addForce(Vector3 direction)
    {
        foreach (var particle in particles)
        {
            particle.addForce(direction);
        }
    }

    public void windForce(Vector3 direction)
    {
        for (int x = 0; x < nodeWidth - 1; x++)
        {
            for (int y = 0; y < nodeHeight - 1; y++)
            {
                addWindForcesForTriangle(getParticle(x + 1, y), getParticle(x, y), getParticle(x, y + 1), direction);
                addWindForcesForTriangle(getParticle(x + 1, y + 1), getParticle(x + 1, y), getParticle(x, y + 1), direction);
            }
        }
    }

}