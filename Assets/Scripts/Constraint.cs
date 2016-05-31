using UnityEngine;
using System.Collections;

public class Constraint {

	private float particleDiff;
    clothParticles p1;
    clothParticles p2;
    public bool isTorn
    {
        get; private set;
    }
	public Constraint(clothParticles _p1, clothParticles _p2)
    {
        p1 = _p1;
        p2 = _p2;
        isTorn = false;
        Vector3 vec = p1.getPos() - p2.getPos();
        particleDiff = vec.magnitude;
    }

    public bool checkTornFromParticle(clothParticles point)
    {
        return isTorn && (p1 == point || p2 == point);
    }

    public clothParticles GetOtherParticle(clothParticles p)
    {
        if (p == p1) return p2;
        if (p == p2) return p1;
        return null;
    }

    public Vector3[] GetPositions()
    {
        return new Vector3[] { p1.getPos(), p2.getPos() };
    }

    public bool IsConstraintDead()
    {
        return p1.deadParticles || p2.deadParticles;
    }

    public void constraintsMet(bool debug = false)
    {
        if (p1.deadParticles || p2.deadParticles)
            return;
        if (isTorn)
        {
            if (debug)
                Debug.DrawLine(p1.getPos(), p2.getPos(), Color.black);
            return;
        }
        Vector3 p1_to_p2 = p2.getPos() - p1.getPos();
        if (Mathf.Abs(p1_to_p2.magnitude - particleDiff) < 0.005f) return;
        float current_distance = p1_to_p2.magnitude;
        Vector3 correctionVector = p1_to_p2 * (1 - particleDiff / current_distance);
        Vector3 correctVectorHalf = correctionVector * 0.5f;
        p1.offsetPos(correctVectorHalf);
        p2.offsetPos(-correctVectorHalf);
    }

}
