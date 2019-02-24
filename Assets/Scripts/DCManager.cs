using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LibNoise.Unity;
using LibNoise.Unity.Generator;
using LibNoise.Unity.Operator;
using System;
using System.Threading;


public class DCManager : MonoBehaviour {


    public bool showGizmos=false;
    public static int radius = 1000;
    //public int chunksPerSide = 4;
    [Range(0, 8)]
    public int maxLodLevel = 3;

    [Range(0, 32)]
    public int subdivisions = 16;

    [HideInInspector]
    public Perlin noise = new Perlin();
    [HideInInspector]
    public CustomImprovedNoise cnoise;

    [HideInInspector] public List<Chunk> chunks;
    [HideInInspector] public static Perlin perlinNoise1;
    [HideInInspector] public float frequency = 3f;
    [HideInInspector] public double lacunarity = 1f;
    [HideInInspector] public double persistence = 1;
    [HideInInspector] public float smoothness = 1;
    [HideInInspector] public int octaves = 3;
    [HideInInspector] private QualityMode quality = QualityMode.High;


    
    [HideInInspector]
    public List<Chunk> children = new List<Chunk>();
    [HideInInspector]
    public int childrenReady = 0;
    [HideInInspector]
    GameObject root_cube;
    [HideInInspector]
    public Boolean isSplitting = false;
    [HideInInspector]
    public Boolean isRestoringParent = false;
    [HideInInspector]
    public static Perlin perlinNoise;

    [HideInInspector]
    public static Boolean greenLight = true;

    [HideInInspector]
    Chunk rootChunk;

    [HideInInspector]
    public static Dictionary<Vector3, Chunk> chunkDictionary = new Dictionary<Vector3, Chunk>();
    public static List<Chunk> chunkList = new List<Chunk>();

    public  MeshGenerator meshGenerator;
    private bool needsGlobalMeshUpdate = true;

    void Start () {

        meshGenerator = gameObject.AddComponent<MeshGenerator>();

        int seed = 151178;
        //int seed = (int)UnityEngine.Random.value;
        perlinNoise1 = new Perlin(frequency , lacunarity, persistence, octaves, seed, quality);
        perlinNoise = new Perlin(frequency, lacunarity, persistence, octaves, seed, quality);
        cnoise = new CustomImprovedNoise(151178);

        root_cube =  new GameObject();
        rootChunk = root_cube.AddComponent<Chunk>();
        rootChunk.transform.parent = gameObject.transform;
        rootChunk.createCube(null, this, radius, 0, 0, 0, 0, "root");
       

    }

    void Update() {
        bool global = false;

        for (int k = maxLodLevel; k >= 0; k--) {
            for (int i = 0; i < chunkList.Count; i++) {
                Chunk c = chunkList[i];
                if ((c.needsMeshUpdate || global) && c.lodLevel == k) {
                    //global = true;
                    c.CreateFinalMesh();
                    c.needsMeshUpdate = false;
                }
            }
        }
        global = false;


            


        }

    public static void addChunkToList(Chunk c) {

        if (!chunkList.Contains(c)) {
            chunkList.Add(c);
            if(!chunkDictionary.ContainsKey(c.transform.position)) chunkDictionary.Add(c.transform.position, c);
        }
        c.needsMeshUpdate = true;


    }

    public static void removeChunkFromList(Chunk c) {
        if (chunkList.Contains(c)) {
            chunkList.Remove(c);
            if (chunkDictionary.ContainsKey(c.transform.position)) chunkDictionary.Remove(c.transform.position);
            c.needsMeshUpdate = true;
        }
        
    }


    public static void destroyChildrenOf(string name) {

        Destroy(GameObject.Find(name + "|DBR").gameObject);
        Destroy(GameObject.Find(name + "|DBL").gameObject);
        Destroy(GameObject.Find(name + "|DTR").gameObject);
        Destroy(GameObject.Find(name + "|DTL").gameObject);


        Destroy(GameObject.Find(name + "|UBR").gameObject);
        Destroy(GameObject.Find(name + "|UBL").gameObject);
        Destroy(GameObject.Find(name + "|UTR").gameObject);
        Destroy(GameObject.Find(name + "|UTL").gameObject);


    }

    

    
    // OBSOLETO
    /*
    public static float[] GenerateEmptySurface(float[] field, Vector3 pos, int size, int increment) {



        int rS = size + 1;

        
        for (int x = 0; x < rS; x++) {
            for (int y = 0; y < rS; y++) {
                for (int z = 0; z < rS; z++) {

                    // if (max == 0) break;

                    // Vector3 p = pos + new Vector3(x * increment, y * increment, z * increment) + new Vector3(-rS / 2, -rS / 2, -rS / 2);
                    Vector3 p = pos + new Vector3(x, y, z );


                    // distance center - cube chunk position
                    float d = Mathf.Abs((Vector3.zero - p).magnitude);
                    //convert to 0-1 range
                    //float t = 1f - 1f / (1f + d);

                    //---
                   // Vector3 pt = (Vector3.zero - p);
                    //Vector3 ptn = p.normalized;
                    //float u = (float) (Mathf.Atan2(pt.x, pt.z) / (2 * Mathf.PI) + 0.5);
                    //float v = (float) (Mathf.Asin(pt.y) / Mathf.PI + .5);


                    //----

                    //float d = Mathf.Abs((Vector3.zero - p).magnitude); // distance from center - position
                    //float t = 1f - 1f / (1f + d); //convert to a range from 0 to 1


                    // NORMAL PERTURBATION
                    float n = (float)perlinNoise.GetValue((Vector3.zero - p));



                   
                //    float val = -t * radius + radius;
                    //field[x + y * rS + z * rS * rS] = val +(n/(rS));
                  //  float n0 = val + (n / (rS));
                   




                    //field 3d but symmetrical
                  //  pt = (Vector3.zero - p);
                   // float w = d / 100;
                   // float sm = 0.05f;
                    //float n2 = -(float)perlinNoise.GetValue(pt.x, pt.y,w*2);
                    //float n2 = -(float)perlinNoise.GetValue(pt.x , pt.y , pt.z );
                    //float n1 = 1 - (float)perlinNoise.GetValue(pt.x * sm, pt.y * sm, pt.z * sm);
                    //if (x == 10 && y ==10) Debug.Log(n1);

                    // 3d mode usage test, field is filled when the noise is above some value (and distance less than planet radius just to make it rounded)
                    
                    if ((p.x > 0 && p.x < size * num)
                        && (p.y > 0 && p.y < size * num)
                        && (p.z > 0 && p.z < size * num)
                       // && d<radius*2
                       ) {

                        //  if (n0 > n1) {
                        //      field[x + y * rS + z * rS * rS] = n1;// 1-n1;
                        //  } else {
                        //      field[x + y * rS + z * rS * rS] = n0;
                        //  }


                    }
                   

                    // field[x + y * rS + z * rS * rS] = n1;
                    if (n < 0f && d < 50) {
                        field[x + y * rS + z * rS * rS] = 1;
                    } else {
                        field[x + y * rS + z * rS * rS] = 0;
                    }




                }
            }
        }
        return field;
    }
    */

    }


