using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    //
    public int xSize = 10, zSize = 10;
    public float yPos = -10;
    public Vector3 scale = Vector3.one;

    //
    private Mesh mesh;
    //private MeshFilter meshFilter;
    private int[] triangles;
    private Vector3[] vertices;
    private MeshCollider meshCollider;

    // Start is called before the first frame update
    void Start()
    {
        mesh = new Mesh();
        mesh.name = "Test";
        GetComponent<MeshFilter>().mesh = mesh;

        AdjustPosition();
        CreateShape();
        UpdateMesh();
        AddCollider();
    }

    // Update is called once per frame
    void Update()
    {
        CenterOnPlayer();
    }

    //private void OnDrawGizmos()
    //{
    //    for (int i = 0; i < vertices.Length; i++)
    //    {
    //        Gizmos.DrawSphere(vertices[i], 0.2f);
    //    }
    //}

    void CenterOnPlayer()
    {
        transform.position = new Vector3(
            -(transform.localScale.x * xSize / 2) + EM_PlayerController.Instance.transform.position.x, 
            yPos, 
            -(transform.localScale.z * zSize / 2) + EM_PlayerController.Instance.transform.position.z
            );
        CreateShape();
        UpdateMesh();
        AddCollider();
    }

    void AdjustPosition()
    {
        transform.position = new Vector3(-(transform.localScale.x * xSize / 2), yPos, -(transform.localScale.z * zSize / 2));
    }

    void CreateShape()
    {
        vertices = new Vector3[(xSize + 1) * (zSize + 1)];

        int i = 0; 
        for(int z = 0; z <= zSize; z++)
        {
            for(int x = 0; x <= xSize; x++)
            {
                //float y = Mathf.PerlinNoise(x * 0.3f, z * 0.3f) * 2;

                //float y = Mathf.PerlinNoise(
                //        (x + (transform.position.x / transform.localScale.x)) * 0.3f, 
                //        (z + (transform.position.z / transform.localScale.z)) * 0.3f
                //    ) * 2;

                float y = Mathf.PerlinNoise(
                        (x + (transform.position.x / scale.x)) * 0.3f,
                        (z + (transform.position.z / scale.z)) * 0.3f
                    ) * 2;

                //vertices[i] = new Vector3(x, y, z);
                //vertices[i] = new Vector3(x * scale.x, y * scale.y, z * scale.z);
                vertices[i] = new Vector3(
                    x * scale.x - (xSize * scale.x / 2), 
                    y * scale.y, 
                    z * scale.z - (zSize * scale.z / 2));

                i++;
            }
        }

        triangles = new int[xSize * zSize * 6];

        int vert = 0;
        int tris = 0;
        for (int z = 0; z < zSize; z++)
        {
            for (int x = 0; x < xSize; x++)
            {
                triangles[tris + 0] = vert + 0;
                triangles[tris + 1] = vert + xSize + 1;
                triangles[tris + 2] = vert + 1;
                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + xSize + 1;
                triangles[tris + 5] = vert + xSize + 2;

                vert++;
                tris += 6;

            }
            vert++;
        }

    }

    void UpdateMesh()
    {
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }

    void AddCollider()
    {
        meshCollider = GetComponent<MeshCollider>();
        meshCollider.sharedMesh = mesh;
    }
}
