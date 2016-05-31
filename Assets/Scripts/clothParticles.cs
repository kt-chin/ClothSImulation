using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class clothParticles
{

    private bool move;
    private float mass;
    private Vector3 currentPos;
    private Vector3 previousPos;
    private Vector3 acceleration;
    private Vector3 normals;
    private scriptCloth parent;
    private List<Constraint> constraints = new List<Constraint>();

    public bool deadParticles { get; private set; }

    public clothParticles(scriptCloth _parent, Vector3 pos)
    {
        parent = _parent;
        currentPos = pos;
        previousPos = pos;
        acceleration = Vector3.zero;
        mass = 1;
        move  = true;
        deadParticles = false;
        normals = Vector3.zero;
    }

    public void addConstraint(Constraint c)
    {
        constraints.Add(c);
    }
    public bool constraintsAttached(clothParticles p)
    {
        foreach (var c in constraints)
            if (c.checkTornFromParticle(p))
                return true;
        return false;
    }

    public void addForce(Vector3 f)
    {
        acceleration += f / mass;
    }

    public void timeStep()
    {
        if (move && !deadParticles)
        {
            Vector3 temp = currentPos;
            currentPos = currentPos + (currentPos - previousPos) * (1.0f - parent.damping) + acceleration;
            previousPos = temp;
            acceleration = Vector3.zero;

            int aliveConstraints = 0;
            foreach (Constraint c in constraints)
                if (c.isTorn == false)
                    aliveConstraints++;
            if (aliveConstraints == 0)
                deadParticles = true;


        }
    }

    public Vector3 getPos() { return currentPos; }

    public void resetAcceleration()
    {
        acceleration = Vector3.zero;
    }
    public void offsetPos(Vector3 v)
    {
        if (move) currentPos += v;
    }

    public void makeUnmovable()
    {
        move = false;
    }

    public void addToNormal(Vector3 normal)
    {
        normals += normal.normalized;
    }

    public Vector3 getNormal()
    {
        return normals;
    }

    public void resetNormal()
    {
        normals = Vector3.zero;
    }
}
