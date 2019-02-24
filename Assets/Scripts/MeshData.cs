using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshData  {

    public const string TYPE_NORMAL = "TYPE_NORMAL";
    public const string TYPE_X_NORMAL_PLUS = "TYPE_X_NORMAL_PLUS";

    public Vector3[] vertices;
    public int[] triangles;
    public Vector2[] uvs;
    public Mesh mesh;
    public Mesh meshXplusNormal;

    public List<Vector3> vertexList = new List<Vector3>();
    public List<int[]> triangleList = new List<int[]>();

    public List<Vector3> vertexListXplusNormal = new List<Vector3>();
    public List<int[]> triangleListXplusNormal = new List<int[]>();

    //int triangleIndex = 0; 
    int vertexIndex = 0;


    public void AddTriangle(Vector3 a, Vector3 b, Vector3 c, string type) {

        switch (type) {

            case TYPE_NORMAL:
                vertexList.Add(a);
                vertexList.Add(b);
                vertexList.Add(c);
                break;

            case TYPE_X_NORMAL_PLUS:
                vertexListXplusNormal.Add(a);
                vertexListXplusNormal.Add(b);
                vertexListXplusNormal.Add(c);
                break;

        }



    }




    public Mesh CreateMesh(string type) {

        switch (type) {

            case TYPE_NORMAL:
                vertices = new Vector3[vertexList.Count + vertexListXplusNormal.Count];
                vertexIndex = 0;

                foreach (Vector3 v in vertexList) {
                    vertices[vertexIndex] = v;
                    vertexIndex++;
                }

                triangles = new int[vertexList.Count+ vertexListXplusNormal.Count];

                if (triangles.Length > 0) {
                    for (int i = 0; i < vertexList.Count; i += 3) {
                        triangles[i] = i;
                        triangles[i + 1] = i + 1;
                        triangles[i + 2] = i + 2;
                    }
                }
                /*
                mesh = new Mesh();
                mesh.vertices = vertices;
                mesh.triangles = triangles;
                mesh.uv = uvs;
                break;

            case TYPE_X_NORMAL_PLUS:
                vertices = new Vector3[vertexListXplusNormal.Count];
                vertexIndex = 0;
                */
                foreach (Vector3 v in vertexListXplusNormal) {
                    vertices[vertexIndex] = v;
                    vertexIndex++;
                }

               // triangles = new int[vertexListXplusNormal.Count];

                if (triangles.Length > 0) {
                    for (int i = vertexList.Count; i < vertexList.Count+vertexListXplusNormal.Count; i += 3) {
                        triangles[i] = i;
                        triangles[i + 1] = i + 1;
                        triangles[i + 2] = i + 2;
                    }
                }

                mesh = new Mesh();
                mesh.vertices = vertices;
                mesh.triangles = triangles;
                mesh.uv = uvs;
 
                break;




        }
        mesh.RecalculateNormals();
        return mesh;
    }
}
