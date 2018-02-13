using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LibNoise;
using LibNoise.Unity.Generator;
using LibNoise.Unity.Operator;
using System.Threading;
using System;



public class MeshGenerator : MonoBehaviour {


    Queue<MapThreadInfo<MapData>> mapDataThreadInfoQueue = new Queue<MapThreadInfo<MapData>>();
    Queue<MeshThreadInfo<MeshData>> meshDataThreadInfoQueue = new Queue<MeshThreadInfo<MeshData>>();

    //Dictionary<string, CellEdge> edgeDictionaryXNormalPatch = new Dictionary<string, CellEdge>();

    Vector3 position;

    public MapData GenerateTerrainData(float size, int subdivisions,Vector3 position, CustomImprovedNoise noise,
        Dictionary<string, CellEdge> edgeDictionary, 
        Dictionary<string, CellEdge> edgeDictionaryXPlusNormalPatch,
        Dictionary<string, CellEdge> edgeDictionaryYPlusNormalPatch,
        Dictionary<string, CellEdge> edgeDictionaryZPlusNormalPatch
        ) {

        this.position = position;

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
        for (x = 0; x < subdivisions; x++) {
            for (y = 0; y < subdivisions; y++) {
                for (z = 0; z < subdivisions; z++) {
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
                    edgeList.Add(new CellEdge(v0, v1, noiseCube[0], noiseCube[1], position, treshold));
                    edgeList.Add(new CellEdge(v2, v3, noiseCube[2], noiseCube[3], position, treshold));
                    edgeList.Add(new CellEdge(v4, v5, noiseCube[4], noiseCube[5], position, treshold));
                    edgeList.Add(new CellEdge(v6, v7, noiseCube[6], noiseCube[7], position, treshold));

                    edgeList.Add(new CellEdge(v1, v5, noiseCube[1], noiseCube[5], position, treshold));
                    edgeList.Add(new CellEdge(v0, v4, noiseCube[0], noiseCube[4], position, treshold));
                    edgeList.Add(new CellEdge(v2, v6, noiseCube[2], noiseCube[6], position, treshold));
                    edgeList.Add(new CellEdge(v3, v7, noiseCube[3], noiseCube[7], position, treshold));

                    edgeList.Add(new CellEdge(v0, v3, noiseCube[0], noiseCube[3], position, treshold));
                    edgeList.Add(new CellEdge(v1, v2, noiseCube[1], noiseCube[2], position, treshold));
                    edgeList.Add(new CellEdge(v4, v7, noiseCube[4], noiseCube[7], position, treshold));
                    edgeList.Add(new CellEdge(v5, v6, noiseCube[5], noiseCube[6], position, treshold));
                   

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
                            c.cells[k] = centerPoint;

                        }
                        k++;
                        if (k > 3) k = 0;
                    }
                    

                  



                }
            }
        } // fine del mega ciclo

        
        ///////////////

        // spigoli esterni X+
        for (x = subdivisions; x < subdivisions+1; x++) {
            for (y = 0; y < subdivisions+1; y++) {
                for (z = 0; z < subdivisions+1; z++) {
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

                        if (noiseCube[i] <= treshold) flagIndex |= 1 << i;   // flagIndex = flagIndex | 1<<i
                        //if(cube[i])
                    }

                    if (flagIndex == 0 || flagIndex == 255) { continue; }


                    // prepara gli spigoli e ordinali in un dizionario
                    edgeList.Add(new CellEdge(v0, v1, noiseCube[0], noiseCube[1], position, treshold));
                    edgeList.Add(new CellEdge(v2, v3, noiseCube[2], noiseCube[3], position, treshold));
                    edgeList.Add(new CellEdge(v4, v5, noiseCube[4], noiseCube[5], position, treshold));
                    edgeList.Add(new CellEdge(v6, v7, noiseCube[6], noiseCube[7], position, treshold));

                    edgeList.Add(new CellEdge(v1, v5, noiseCube[1], noiseCube[5], position, treshold));
                    edgeList.Add(new CellEdge(v0, v4, noiseCube[0], noiseCube[4], position, treshold));
                    edgeList.Add(new CellEdge(v2, v6, noiseCube[2], noiseCube[6], position, treshold));
                    edgeList.Add(new CellEdge(v3, v7, noiseCube[3], noiseCube[7], position, treshold));

                    edgeList.Add(new CellEdge(v0, v3, noiseCube[0], noiseCube[3], position, treshold));
                    edgeList.Add(new CellEdge(v1, v2, noiseCube[1], noiseCube[2], position, treshold));
                    edgeList.Add(new CellEdge(v4, v7, noiseCube[4], noiseCube[7], position, treshold));
                    edgeList.Add(new CellEdge(v5, v6, noiseCube[5], noiseCube[6], position, treshold));


                    // per ogni cubo trova quanti spigoli hanno intersezioni
                    numEdgesWithIntersections = 0;
                    centerPoint = new Vector3();
                    foreach (CellEdge ce in edgeList) {
                        if (ce.hasIntersection) {
                            CellEdge c = getOrAdd(ce, edgeDictionaryXPlusNormalPatch);
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
                            CellEdge c = getOrAdd(ce, edgeDictionaryXPlusNormalPatch);
                            c.cells[k] = centerPoint;

                        }
                        k++;
                        if (k > 3) k = 0;
                    }

                }
            }
        }

        /////////////////

        // spigoli esterni Y+
        for (x = 0; x < subdivisions; x++) {
            for (y = subdivisions; y < subdivisions+1; y++) {
                for (z = 0; z < subdivisions; z++) {
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

                        if (noiseCube[i] <= treshold) flagIndex |= 1 << i;   // flagIndex = flagIndex | 1<<i
                        //if(cube[i])
                    }

                    if (flagIndex == 0 || flagIndex == 255) { continue; }


                    // prepara gli spigoli e ordinali in un dizionario
                    edgeList.Add(new CellEdge(v0, v1, noiseCube[0], noiseCube[1], position, treshold));
                    edgeList.Add(new CellEdge(v2, v3, noiseCube[2], noiseCube[3], position, treshold));
                    edgeList.Add(new CellEdge(v4, v5, noiseCube[4], noiseCube[5], position, treshold));
                    edgeList.Add(new CellEdge(v6, v7, noiseCube[6], noiseCube[7], position, treshold));

                    edgeList.Add(new CellEdge(v1, v5, noiseCube[1], noiseCube[5], position, treshold));
                    edgeList.Add(new CellEdge(v0, v4, noiseCube[0], noiseCube[4], position, treshold));
                    edgeList.Add(new CellEdge(v2, v6, noiseCube[2], noiseCube[6], position, treshold));
                    edgeList.Add(new CellEdge(v3, v7, noiseCube[3], noiseCube[7], position, treshold));

                    edgeList.Add(new CellEdge(v0, v3, noiseCube[0], noiseCube[3], position, treshold));
                    edgeList.Add(new CellEdge(v1, v2, noiseCube[1], noiseCube[2], position, treshold));
                    edgeList.Add(new CellEdge(v4, v7, noiseCube[4], noiseCube[7], position, treshold));
                    edgeList.Add(new CellEdge(v5, v6, noiseCube[5], noiseCube[6], position, treshold));


                    // per ogni cubo trova quanti spigoli hanno intersezioni
                    numEdgesWithIntersections = 0;
                    centerPoint = new Vector3();
                    foreach (CellEdge ce in edgeList) {
                        if (ce.hasIntersection) {
                            CellEdge c = getOrAdd(ce, edgeDictionaryYPlusNormalPatch);
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
                            CellEdge c = getOrAdd(ce, edgeDictionaryYPlusNormalPatch);
                            c.cells[k] = centerPoint;

                        }
                        k++;
                        if (k > 3) k = 0;
                    }

                }
            }
        }

        /////////////////

        // spigoli esterni Z+
        for (x = 0; x < subdivisions; x++) {
            for (y = 0; y < subdivisions; y++) {
                for (z = subdivisions; z < subdivisions+1; z++) {
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

                        if (noiseCube[i] <= treshold) flagIndex |= 1 << i;   // flagIndex = flagIndex | 1<<i
                        //if(cube[i])
                    }

                    if (flagIndex == 0 || flagIndex == 255) { continue; }


                    // prepara gli spigoli e ordinali in un dizionario
                    edgeList.Add(new CellEdge(v0, v1, noiseCube[0], noiseCube[1], position, treshold));
                    edgeList.Add(new CellEdge(v2, v3, noiseCube[2], noiseCube[3], position, treshold));
                    edgeList.Add(new CellEdge(v4, v5, noiseCube[4], noiseCube[5], position, treshold));
                    edgeList.Add(new CellEdge(v6, v7, noiseCube[6], noiseCube[7], position, treshold));

                    edgeList.Add(new CellEdge(v1, v5, noiseCube[1], noiseCube[5], position, treshold));
                    edgeList.Add(new CellEdge(v0, v4, noiseCube[0], noiseCube[4], position, treshold));
                    edgeList.Add(new CellEdge(v2, v6, noiseCube[2], noiseCube[6], position, treshold));
                    edgeList.Add(new CellEdge(v3, v7, noiseCube[3], noiseCube[7], position, treshold));

                    edgeList.Add(new CellEdge(v0, v3, noiseCube[0], noiseCube[3], position, treshold));
                    edgeList.Add(new CellEdge(v1, v2, noiseCube[1], noiseCube[2], position, treshold));
                    edgeList.Add(new CellEdge(v4, v7, noiseCube[4], noiseCube[7], position, treshold));
                    edgeList.Add(new CellEdge(v5, v6, noiseCube[5], noiseCube[6], position, treshold));


                    // per ogni cubo trova quanti spigoli hanno intersezioni
                    numEdgesWithIntersections = 0;
                    centerPoint = new Vector3();
                    foreach (CellEdge ce in edgeList) {
                        if (ce.hasIntersection) {
                            CellEdge c = getOrAdd(ce, edgeDictionaryZPlusNormalPatch);
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
                            CellEdge c = getOrAdd(ce, edgeDictionaryZPlusNormalPatch);
                            c.cells[k] = centerPoint;

                        }
                        k++;
                        if (k > 3) k = 0;
                    }

                }
            }
        }

        /////////////////


        mapData.edgeDictionary = edgeDictionary;
        mapData.edgeDictionaryXPlusNormalPatch = edgeDictionaryXPlusNormalPatch;
        //mapData.edgeDictionaryYPlusNormalPatch = edgeDictionaryYPlusNormalPatch;
        //mapData.edgeDictionaryZPlusNormalPatch = edgeDictionaryZPlusNormalPatch;


        return mapData;
    }

    public MeshData GenerateTerrainMesh(MapData mapData) {
        MeshData meshData = new MeshData();
             
            // per ogni spigolo del dizionario tira fuori i 4 punti centrali delle celle che lo condividono e collegali con 2 triangolis
            foreach (CellEdge edg in mapData.edgeDictionary.Values) {
            
                //crea i 2 triangoli su tutti gli spigoli che sono condivisi da almeno 4 celle di questo stesso chunk
                if (edg.cells[0] != Vector3.zero && edg.cells[1] != Vector3.zero &&
                    edg.cells[2] != Vector3.zero && edg.cells[3] != Vector3.zero) {
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

                
                //X+ patches
                if (mapData.edgeDictionaryXPlusNormalPatch.ContainsKey(edg.name)){

                      CellEdge patch = new CellEdge(edg.inV, edg.outV, edg.noiseIn, edg.noiseOut, edg.position, edg.treshold);

                    if (edg.cells[0] == Vector3.zero && edg.cells[1] == Vector3.zero
                         && edg.cells[2] != Vector3.zero && edg.cells[3] != Vector3.zero
                          ) {
                        
                        if (!edg.isFlipped) {

                            meshData.AddTriangle(
                                       mapData.edgeDictionaryXPlusNormalPatch[edg.name].cells[1],
                                        edg.cells[3],
                                        edg.cells[2],
                                        MeshData.TYPE_X_NORMAL_PLUS
                                        );
                            meshData.AddTriangle(
                                      mapData.edgeDictionaryXPlusNormalPatch[edg.name].cells[0],
                                      mapData.edgeDictionaryXPlusNormalPatch[edg.name].cells[1],
                                       edg.cells[2],
                                        MeshData.TYPE_X_NORMAL_PLUS
                                       );

                        } else {

                            meshData.AddTriangle(
                                     mapData.edgeDictionaryXPlusNormalPatch[edg.name].cells[1],
                                     edg.cells[2],
                                     edg.cells[3],
                                        MeshData.TYPE_X_NORMAL_PLUS
                                     );
                            meshData.AddTriangle(
                                     mapData.edgeDictionaryXPlusNormalPatch[edg.name].cells[1],
                                     mapData.edgeDictionaryXPlusNormalPatch[edg.name].cells[0],
                                      edg.cells[2],
                                        MeshData.TYPE_X_NORMAL_PLUS
                                      );
                        }

                    } else {
                        /*
                        Debug.Log(" 0:"+ edg.cells[0].Equals(Vector3.zero) 
                            + " 1:" + edg.cells[1].Equals(Vector3.zero)
                            + " 2:" + edg.cells[2].Equals(Vector3.zero)
                            + " 3:" + edg.cells[3].Equals(Vector3.zero)
                            );
                        */
                        //secondo giro di patch ? (se no fa una si e una no)
                        if (edg.cells[0] == Vector3.zero && edg.cells[2] == Vector3.zero
                            && edg.cells[1] != Vector3.zero && edg.cells[3] != Vector3.zero
                             ) {

                            if (edg.isFlipped) {

                                meshData.AddTriangle(
                                 mapData.edgeDictionaryXPlusNormalPatch[edg.name].cells[0],
                                  edg.cells[3],
                                  edg.cells[1],
                                        MeshData.TYPE_X_NORMAL_PLUS
                                  );

                                meshData.AddTriangle(
                                mapData.edgeDictionaryXPlusNormalPatch[edg.name].cells[2],
                                edg.cells[3],
                                mapData.edgeDictionaryXPlusNormalPatch[edg.name].cells[0],
                                        MeshData.TYPE_X_NORMAL_PLUS
                                 );

                            } else {
                                meshData.AddTriangle(
                                 mapData.edgeDictionaryXPlusNormalPatch[edg.name].cells[0],
                                  edg.cells[1],
                                  edg.cells[3],
                                        MeshData.TYPE_X_NORMAL_PLUS
                                  );
                                meshData.AddTriangle(
                               mapData.edgeDictionaryXPlusNormalPatch[edg.name].cells[2],
                               mapData.edgeDictionaryXPlusNormalPatch[edg.name].cells[0],
                               edg.cells[3],
                                        MeshData.TYPE_X_NORMAL_PLUS
                                );
                            }
                        }

                        //celletta angolo X+ Y+

                        if (
                            edg.cells[0] == Vector3.zero
                           && edg.cells[1] == Vector3.zero
                            && edg.cells[2] == Vector3.zero 
                            && edg.cells[3] != Vector3.zero
                              ) {
                            


                            meshData.AddTriangle(
                                // mapData.edgeDictionaryXPlusNormalPatch[edg.name].cells[3],
                                // mapData.edgeDictionaryXPlusNormalPatch[edg.name].cells[2],
                                edg.cells[3],
                                // edg.cells[3] + new Vector3(80, 80, 0),
                                mapData.edgeDictionaryXPlusNormalPatch[edg.name].cells[0],
                                 mapData.edgeDictionaryXPlusNormalPatch[edg.name].cells[2],
                                        MeshData.TYPE_X_NORMAL_PLUS
                                  // 

                                  );

                        }

                        }
                }
                
               
                ///////
                /*
                //Y+ patches
                if (mapData.edgeDictionaryYPlusNormalPatch.ContainsKey(edg.name)) {
                    
                    CellEdge patch = new CellEdge(edg.inV, edg.outV, edg.noiseIn, edg.noiseOut, edg.position, edg.treshold);

                    if (edg.cells[0] == Vector3.zero && edg.cells[1] == Vector3.zero
                        && edg.cells[2] != Vector3.zero && edg.cells[3] != Vector3.zero
                        ) {

                        if (!edg.isFlipped) {

                            meshData.AddTriangle(
                                       mapData.edgeDictionaryYPlusNormalPatch[edg.name].cells[1],
                                        edg.cells[3],
                                        edg.cells[2]
                                        );
                            meshData.AddTriangle(
                                      mapData.edgeDictionaryYPlusNormalPatch[edg.name].cells[0],
                                      mapData.edgeDictionaryYPlusNormalPatch[edg.name].cells[1],
                                       edg.cells[2]
                                       );

                        } else {

                            meshData.AddTriangle(
                                     mapData.edgeDictionaryYPlusNormalPatch[edg.name].cells[1],
                                     edg.cells[2],
                                     edg.cells[3]
                                     );
                            meshData.AddTriangle(
                                     mapData.edgeDictionaryYPlusNormalPatch[edg.name].cells[1],
                                     mapData.edgeDictionaryYPlusNormalPatch[edg.name].cells[0],
                                      edg.cells[2]
                                      );
                        }
                    }
                }
                */
                ///////
               
                // addNormalPatchTriangles(meshData, edg, mapData.edgeDictionaryXPlusNormalPatch);
                 //addNormalPatchTriangles(meshData, edg, mapData.edgeDictionaryYPlusNormalPatch);
                // addNormalPatchTriangles(meshData, edg, mapData.edgeDictionaryZPlusNormalPatch);






            }

            }
             return meshData;

     }
    /*

    public void addNormalPatchTriangles(MeshData meshData, CellEdge edg, Dictionary<String,CellEdge> edgeDic) {

        if (edgeDic.ContainsKey(edg.name)) {

            CellEdge patch = new CellEdge(edg.inV, edg.outV, edg.noiseIn, edg.noiseOut, edg.position, edg.treshold);

            if (edg.cells[0] == Vector3.zero && edg.cells[1] == Vector3.zero
                && edg.cells[2] != Vector3.zero && edg.cells[3] != Vector3.zero
                ) {

                if (!edg.isFlipped) {

                    meshData.AddTriangle(
                               edgeDic[edg.name].cells[1],
                                edg.cells[3],
                                edg.cells[2]
                                );
                    meshData.AddTriangle(
                              edgeDic[edg.name].cells[0],
                              edgeDic[edg.name].cells[1],
                               edg.cells[2]
                               );

                } else {

                    meshData.AddTriangle(
                             edgeDic[edg.name].cells[1],
                             edg.cells[2],
                             edg.cells[3]
                             );
                    meshData.AddTriangle(
                             edgeDic[edg.name].cells[1],
                             edgeDic[edg.name].cells[0],
                              edg.cells[2]
                              );
                }
            }
        } 


    }

    */

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
                threadInfo.callback(threadInfo.parameter);
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


   
