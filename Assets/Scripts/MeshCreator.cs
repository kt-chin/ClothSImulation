using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class meshCreator : MonoBehaviour
{

   [SerializeField]

    private scriptCloth cloth;
    private List<int> indices = new List<int>();
    private List<Vector3> vertices = new List<Vector3>();
    private List<Vector3> normals = new List<Vector3>();
    public MeshFilter viewMeshFilter;
    Mesh mesh;
    GameObject meshChild;
    public MeshFilter viewMeshFilterStrings;
    Mesh viewMeshStrings;
    GameObject meshChildStrings;
    
    // Use this for initialization
    void Start()
    {
        cloth = GetComponent<scriptCloth>();
        mesh = new Mesh();
        mesh.name = "Mesh";
        viewMeshStrings = new Mesh();
        viewMeshStrings.name = "View Mesh Strings";

        if (viewMeshFilter == null)
        {
            meshChild = new GameObject("Mesh");
            meshChild.transform.parent = transform;
            meshChild.transform.localPosition = Vector3.zero;
            meshChild.transform.localRotation = Quaternion.identity;
            var vmf = meshChild.AddComponent<MeshFilter>();
            var mr = meshChild.AddComponent<MeshRenderer>();
            var col = meshChild.AddComponent<MeshCollider>();
            mr.receiveShadows = false;
            mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            mr.sharedMaterial = Resources.Load("Material/clothMat",typeof(Material)) as Material;
            viewMeshFilter = vmf;
            col.sharedMesh = mesh;
        }

        viewMeshFilter.mesh = mesh;


        if (viewMeshFilterStrings == null)
        {
            meshChildStrings = new GameObject("MeshStrings");
            meshChildStrings.transform.parent = transform;
            meshChildStrings.transform.localPosition = Vector3.zero;
            meshChildStrings.transform.localRotation = Quaternion.identity;
            var vmf = meshChildStrings.AddComponent<MeshFilter>();
            var mr = meshChildStrings.AddComponent<MeshRenderer>();
            var col = meshChildStrings.AddComponent<MeshCollider>();
            mr.receiveShadows = false;
            mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            mr.sharedMaterial = Resources.Load("Material/stringMat", typeof(Material)) as Material;
            viewMeshFilterStrings = vmf;
            col.sharedMesh = viewMeshStrings;
        }

        viewMeshFilterStrings.mesh = viewMeshStrings;


    }

    // Update is called once per frame
    void Update()
    {
        vertices.Clear();
        normals.Clear();
        indices.Clear();



        for (int x = 0; x < cloth.nodeWidth - 1; x++)
        {
            for (int y = 0; y < cloth.nodeHeight - 1; y++)
            {
                Vector3 normal = calcTriangleNormal(cloth.getParticle(x + 1, y), cloth.getParticle(x, y), cloth.getParticle(x, y + 1));
                cloth.getParticle(x + 1, y).addToNormal(normal);
                cloth.getParticle(x, y).addToNormal(normal);
                cloth.getParticle(x, y + 1).addToNormal(normal);

                normal = calcTriangleNormal(cloth.getParticle(x + 1, y + 1), cloth.getParticle(x + 1, y), cloth.getParticle(x, y + 1));
                cloth.getParticle(x + 1, y + 1).addToNormal(normal);
                cloth.getParticle(x + 1, y).addToNormal(normal);
                cloth.getParticle(x, y + 1).addToNormal(normal);
            }
        }

        int index = 0;
        for (int x = 0; x < cloth.nodeWidth - 1; x++)
        {
            for (int y = 0; y < cloth.nodeHeight - 1; y++)
            {
                clothParticles p1 = cloth.getParticle(x + 1, y);
                clothParticles p2 = cloth.getParticle(x, y);
                clothParticles p3 = cloth.getParticle(x, y + 1);
                clothParticles p4 = cloth.getParticle(x + 1, y + 1);

                if (!p1.constraintsAttached(p2) && !p1.constraintsAttached(p3) && !p3.constraintsAttached(p2))
                {
                    vertices.Add(p1.getPos());
                    normals.Add(p1.getNormal().normalized);
                    indices.Add(index++);
                    vertices.Add(p2.getPos());
                    normals.Add(p2.getNormal().normalized);
                    indices.Add(index++);
                    vertices.Add(p3.getPos());
                    normals.Add(p3.getNormal().normalized);
                    indices.Add(index++);
                }

                if (!p1.constraintsAttached(p4) && !p1.constraintsAttached(p3) && !p4.constraintsAttached(p3))
                {
                    vertices.Add(p4.getPos());
                    normals.Add(p4.getNormal().normalized);
                    indices.Add(index++);
                    vertices.Add(p1.getPos());
                    normals.Add(p1.getNormal().normalized);
                    indices.Add(index++);
                    vertices.Add(p3.getPos());
                    normals.Add(p3.getNormal().normalized);
                    indices.Add(index++);
                }
            }
        }
        mesh.Clear();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = indices.ToArray();
        mesh.normals = normals.ToArray();
        meshChild.GetComponent<MeshCollider>().sharedMesh = mesh;
        viewMeshStrings.Clear();
        drawStrings();
    }

    void drawStrings()
    {
        int index = 0;
        for (int i = 0; i < cloth.constraints.Count; i++)
        {
            if (cloth.constraints[i].IsConstraintDead())
                continue;
            vertices.Add(cloth.constraints[i].GetPositions()[0]);
            vertices.Add(cloth.constraints[i].GetPositions()[1]);
            indices.Add(index++);
            indices.Add(index++);
        }
        viewMeshStrings.Clear();
        viewMeshStrings.vertices = vertices.ToArray();
        viewMeshStrings.SetIndices(indices.ToArray(), MeshTopology.Lines, 0);
        viewMeshStrings.RecalculateBounds();
    }

    Vector3 calcTriangleNormal(clothParticles p1, clothParticles p2, clothParticles p3)
    {
        Vector3 pos1 = p1.getPos();
        Vector3 pos2 = p2.getPos();
        Vector3 pos3 = p3.getPos();
        Vector3 v1 = pos2 - pos1;
        Vector3 v2 = pos3 - pos1;
        return Vector3.Cross(v1, v2);
    }
}

