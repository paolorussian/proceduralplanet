using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LibNoise;
using LibNoise.Unity.Generator;
using LibNoise.Unity.Operator;
using System.Threading;
using System;
using System.Linq;



public class MeshGenerator : MonoBehaviour {

    //MapData mapData = new MapData();
    Queue<MapThreadInfo<MapData>> mapDataThreadInfoQueue = new Queue<MapThreadInfo<MapData>>();
    Queue<MeshThreadInfo<MeshData>> meshDataThreadInfoQueue = new Queue<MeshThreadInfo<MeshData>>();

    //Dictionary<string, CellEdge> edgeDictionaryXNormalPatch = new Dictionary<string, CellEdge>();

    Vector3 position;
    int subdivisions;

    public MapData GenerateTerrainData(float size, int subdivisions,Vector3 position, CustomImprovedNoise noise,
        Dictionary<string, CellEdge> edgeDictionary, 
        Dictionary<string, CellEdge> edgeDictionaryXPlusNormalPatch,
        Dictionary<string, CellEdge> edgeDictionaryYPlusNormalPatch,
        Dictionary<string, CellEdge> edgeDictionaryZPlusNormalPatch
        ) {

        this.position = position;
        this.subdivisions = subdivisions;
        float increment = size / subdivisions;

        int flagIndex = 0;
        //float treshold = 0.4f;
        float treshold = DCManager.radius*0.55f;
        Vector3 centerPoint;
        int i, x, y, z;
        Vector3[] edgeVertex;
        List<CellEdge> edgeList = new List<CellEdge>();
        int numEdgesWithIntersections = 0;

        MapData mapData = new MapData();
        Vector3[] cube;
        float[] noiseCube;
        
        
        // spigoli interni
        for (x = 0; x <= subdivisions; x++) {
            for (y = 0; y <= subdivisions; y++) {
                for (z = 0; z <= subdivisions; z++) {
                    edgeList.Clear();
                    cube = new Vector3[8];
                    noiseCube = new float[8];
                    edgeVertex = new Vector3[12];
                    

                    numEdgesWithIntersections = 0;
                    flagIndex = 0;

                    Vector3 v0 = new Vector3(-size / 2 + x * increment, -size / 2 + y * increment, -size / 2 + z * increment);
                    Vector3 v1 = new Vector3(-size / 2 + x * increment, -size / 2 + y * increment, -size / 2 + z * increment + increment);
                    Vector3 v2 = new Vector3(-size / 2 + x * increment + increment, -size / 2 + y * increment, -size / 2 + z * increment + increment);
                    Vector3 v3 = new Vector3(-size / 2 + x * increment + increment, -size / 2 + y * increment, -size / 2 + z * increment);

                    Vector3 v4 = new Vector3(-size / 2 + x * increment, -size / 2 + y * increment + increment, -size / 2 + z * increment);
                    Vector3 v5 = new Vector3(-size / 2 + x * increment, -size / 2 + y * increment + increment, -size / 2 + z * increment + increment);
                    Vector3 v6 = new Vector3(-size / 2 + x * increment + increment, -size / 2 + y * increment + increment, -size / 2 + z * increment + increment);
                    Vector3 v7 = new Vector3(-size / 2 + x * increment + increment, -size / 2 + y * increment + increment, -size / 2 + z * increment);

                    cube[0] = v0;
                    cube[1] = v1;
                    cube[2] = v2;
                    cube[3] = v3;
                    cube[4] = v4;
                    cube[5] = v5;
                    cube[6] = v6;
                    cube[7] = v7;

                   

                    for (i = 0; i < 8; i++) {
                        noiseCube[i] = DCManager.radius - (cube[i] + position).magnitude;
                     
                        if (noiseCube[i] <= treshold) flagIndex |= 1 << i; 
                    }

                    if (flagIndex == 0 || flagIndex == 255) { continue; }
                  
                   
                    // prepara gli spigoli e ordinali in un dizionario
                    edgeList.Add(new CellEdge(v0, v1, noiseCube[0], noiseCube[1], position, treshold) { px = x, py = y, pz = z });
                    edgeList.Add(new CellEdge(v2, v3, noiseCube[2], noiseCube[3], position, treshold) { px = x, py = y, pz = z });
                    edgeList.Add(new CellEdge(v4, v5, noiseCube[4], noiseCube[5], position, treshold) { px = x, py = y, pz = z });
                    edgeList.Add(new CellEdge(v6, v7, noiseCube[6], noiseCube[7], position, treshold) { px = x, py = y, pz = z });

                    edgeList.Add(new CellEdge(v1, v5, noiseCube[1], noiseCube[5], position, treshold) { px = x, py = y, pz = z });
                    edgeList.Add(new CellEdge(v0, v4, noiseCube[0], noiseCube[4], position, treshold) { px = x, py = y, pz = z });
                    edgeList.Add(new CellEdge(v2, v6, noiseCube[2], noiseCube[6], position, treshold) { px = x, py = y, pz = z });
                    edgeList.Add(new CellEdge(v3, v7, noiseCube[3], noiseCube[7], position, treshold) { px = x, py = y, pz = z });

                    edgeList.Add(new CellEdge(v0, v3, noiseCube[0], noiseCube[3], position, treshold) { px = x, py = y, pz = z });
                    edgeList.Add(new CellEdge(v1, v2, noiseCube[1], noiseCube[2], position, treshold) { px = x, py = y, pz = z });
                    edgeList.Add(new CellEdge(v4, v7, noiseCube[4], noiseCube[7], position, treshold) { px = x, py = y, pz = z });
                    edgeList.Add(new CellEdge(v5, v6, noiseCube[5], noiseCube[6], position, treshold) { px = x, py = y, pz = z });


                    // per ogni cubo trova quanti spigoli hanno intersezioni
                    numEdgesWithIntersections = 0;
                    centerPoint = new Vector3();
                    foreach (CellEdge ce in edgeList) {
                        if (ce.hasIntersection) {
                            CellEdge c = getOrAdd(ce, edgeDictionary);
                            numEdgesWithIntersections++;
                            centerPoint += ce.intersectionPoint;
                        }
                    }
                    if (numEdgesWithIntersections == 0 || numEdgesWithIntersections == 8) continue;
                    centerPoint = centerPoint / numEdgesWithIntersections;

                    // ora che sappiamo quanti spigoli hanno intersezioni calcola il punto medio tra loro e settalo come punto centrale della cella
                    // dual contour - per infighettarlo qua dovresti applicare la QEF, invece del punto medio tra gli spigoli che hanno intersezioni
                    int k = 0;
                    foreach (CellEdge ce in edgeList) {
                        if (ce.hasIntersection) {
                            CellEdge c = getOrAdd(ce, edgeDictionary);
                            //if (c.cells[k] != Vector3.zero) Debug.Log("ERROR"); //TODO: check why I would possibly overwrite a preesisting(?) [k] value
                            c.cells[k] = centerPoint;

                        }
                        k++;
                        if (k > 3) k = 0;
                    }
                    

                  



                }
            }
        } // fine del mega ciclo

        
       
        mapData.edgeDictionary = edgeDictionary;
        mapData.edgeDictionaryXPlusNormalPatch = edgeDictionaryXPlusNormalPatch;
        //mapData.edgeDictionaryYPlusNormalPatch = edgeDictionaryYPlusNormalPatch;
        //mapData.edgeDictionaryZPlusNormalPatch = edgeDictionaryZPlusNormalPatch;


        return mapData;
    }

    public MeshData GenerateTerrainMesh(MapData mapData) {
        MeshData meshData = new MeshData();
        int orphans = 0;

        //Debug.Log("restarting "+ mapData.edgeDictionary.Count());

        // per ogni spigolo del dizionario tira fuori i 4 punti centrali delle celle che lo condividono e collegali con 2 triangolis
        foreach (CellEdge edg in mapData.edgeDictionary.Values) {

            

            if ((   edg.px >= 0 && edg.px < subdivisions
                &&  edg.py >= 0 && edg.py < subdivisions
                &&  edg.pz >= 0 && edg.pz < subdivisions)) {

                List<Vector3> points = edg.cells.Cast<Vector3>().ToList();
                //crea i 2 triangoli su tutti gli spigoli che sono condivisi da almeno 4 celle di questo stesso chunk
                if (points.Count(v => v != Vector3.zero)==4) {

                    //Gizmos.color = Color.blue;
                   // Gizmos.DrawLine(edg.inV,edg.outV);

                    if (edg.isFlipped) {

                        meshData.AddTriangle(
                            edg.cells[0],
                            edg.cells[2],
                            edg.cells[1],
                            MeshData.TYPE_NORMAL
                            );
                        meshData.AddTriangle(
                           edg.cells[2],
                           edg.cells[3],
                           edg.cells[1],
                            MeshData.TYPE_NORMAL
                           );
                        /*
                        meshData.AddTriangle(
                            edg.cells[2],
                            edg.cells[3],
                            edg.cells[0],
                             MeshData.TYPE_NORMAL
                            );
                            */



                    } else {
                       
                        meshData.AddTriangle(
                        edg.cells[0],
                        edg.cells[1],
                        edg.cells[2],
                            MeshData.TYPE_NORMAL
                        );
                        meshData.AddTriangle(
                           edg.cells[2],
                           edg.cells[1],
                           edg.cells[3],
                            MeshData.TYPE_NORMAL
                           );
                           
                    }


                } else {
                    //orphans: these intersections has 1 to 3 cells only to triangulate
                   


                    Debug.Log("> orphan points");
                    //Debug.Log(edg.cells[0]+","+ edg.cells[1] + "," + edg.cells[2] + "," + edg.cells[3]);

                }



            }
           




        }
        
        
        return meshData;

     }
   

    void Update() {
        if (mapDataThreadInfoQueue.Count > 0) {
            for (int i = 0; i < mapDataThreadInfoQueue.Count; i++) {
                MapThreadInfo<MapData> threadInfo = mapDataThreadInfoQueue.Dequeue();
                threadInfo.callback(threadInfo.parameter);
            }
        }

        if (meshDataThreadInfoQueue.Count > 0) {
            for (int i = 0; i < meshDataThreadInfoQueue.Count; i++) {
                MeshThreadInfo<MeshData> threadInfo = meshDataThreadInfoQueue.Dequeue();
                if (!default(MeshThreadInfo<MeshData>).Equals(threadInfo)) {
                    threadInfo.callback(threadInfo.parameter);
                }
            }
        }

    }
  
    public void RequestMapData(float edgeSize, int subdivisions, Vector3 position, CustomImprovedNoise noise, 
        Action<MapData> callback, Dictionary<string, CellEdge> edgeDictionary, 
        Dictionary<string, CellEdge> edgeDictionaryXPlusNormalPatch,
        Dictionary<string, CellEdge> edgeDictionaryYPlusNormalPatch,
        Dictionary<string, CellEdge> edgeDictionaryZPlusNormalPatch

        ) {


        ThreadStart threadStart = delegate {
            MapDataThread(edgeSize, subdivisions, position, noise, callback, edgeDictionary, 
                edgeDictionaryXPlusNormalPatch, edgeDictionaryYPlusNormalPatch, edgeDictionaryZPlusNormalPatch);
        };

        new Thread(threadStart).Start();
    }
    
  void MapDataThread(float edgeSize, int subdivisions, Vector3 position, CustomImprovedNoise noise, Action<MapData> callback, 
      Dictionary<string, CellEdge> edgeDictionary, 
      Dictionary<string, CellEdge> edgeDictionaryXPlusNormalPatch,
      Dictionary<string, CellEdge> edgeDictionaryYPlusNormalPatch,
        Dictionary<string, CellEdge> edgeDictionaryZPlusNormalPatch
      ) {

        MapData mapData = GenerateTerrainData(edgeSize, subdivisions, position, noise, 
            edgeDictionary, edgeDictionaryXPlusNormalPatch, edgeDictionaryYPlusNormalPatch, edgeDictionaryZPlusNormalPatch);

      lock (mapDataThreadInfoQueue) {
          mapDataThreadInfoQueue.Enqueue(new MapThreadInfo<MapData>(callback, mapData, edgeDictionary));
      }
  }
  

    struct MapThreadInfo<T> {

        public readonly Action<T> callback;
        public readonly T parameter;
        public readonly Dictionary<string, CellEdge> edgeDictionary;

        public MapThreadInfo(Action<T> callback, T parameter, Dictionary<string, CellEdge> edgeDictionary) {
            this.callback = callback;
            this.parameter = parameter;
            this.edgeDictionary = edgeDictionary;
        }

    }
    
    
    public void RequestMeshData(MapData mapData, Action<MeshData> callback) {
        ThreadStart threadStart = delegate {
            MeshDataThread(mapData, callback);
        };

        new Thread(threadStart).Start();
    }
    
    void MeshDataThread(MapData mapData, Action<MeshData> callback) {
        MeshData meshData = GenerateTerrainMesh(mapData);
        lock (meshDataThreadInfoQueue) {
            meshDataThreadInfoQueue.Enqueue(new MeshThreadInfo<MeshData>(callback, meshData));
        }
    }

    struct MeshThreadInfo<T> {
        public readonly Action<T> callback;
        public readonly T parameter;
      

        public MeshThreadInfo(Action<T> callback, T parameter) {
            this.callback = callback;
            this.parameter = parameter;

        }

    }

    private CellEdge getOrAdd(CellEdge c, Dictionary<String,CellEdge> dic) {

        if (dic.ContainsKey(c.name)) {
            return dic[c.name];
        } else {
            dic.Add(c.name, c);
            return dic[c.name];
        }

    }
}


   
