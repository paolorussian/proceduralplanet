using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;
using System.Linq;

public class Chunk : MonoBehaviour {


    [HideInInspector] public DCManager manager;
    public float edgeSize;
    public int lodLevel = -1;
    public Chunk parent;
    public List<Chunk> children = new List<Chunk>();
    public Boolean deleteMe = false;
    public Shader shader;
    public Material material;
    public int childrenReady = 0;
    public Boolean isSplitting = false;
    public Boolean isRestoringParent = false;
    public Mesh mesh;
    public Dictionary<string, CellEdge> edgeDictionary = new Dictionary<string, CellEdge>();

    public Dictionary<string, CellEdge> edgeDictionaryXPlusNormalPatch = new Dictionary<string, CellEdge>();
    public Dictionary<string, CellEdge> edgeDictionaryXMinusNormalPatch = new Dictionary<string, CellEdge>();

    public Dictionary<string, CellEdge> edgeDictionaryYPlusNormalPatch = new Dictionary<string, CellEdge>();
    public Dictionary<string, CellEdge> edgeDictionaryYMinusNormalPatch = new Dictionary<string, CellEdge>();

    public Dictionary<string, CellEdge> edgeDictionaryZPlusNormalPatch = new Dictionary<string, CellEdge>();
    public Dictionary<string, CellEdge> edgeDictionaryZMinusNormalPatch = new Dictionary<string, CellEdge>();

    public float distanceToPlayer = 100000000f;

    private MeshData meshData;
    private MapData mapData;
    public Boolean needsMeshUpdate = false;

    void Start () {
		
	}
	


	void Update () {


        

        Vector3 worldPt = gameObject.transform.TransformPoint(transform.position);
        distanceToPlayer = Vector3.Distance(gameObject.transform.TransformPoint(Camera.main.transform.parent.position), worldPt);
        //float dist2 = Vector3.Distance(gameObject.transform.TransformPoint(Camera.main.transform.parent.position), worldPt);
        
        float diagonal = Mathf.Sqrt(2*(edgeSize * edgeSize));

        if (isSplitting  || isRestoringParent) return;
        if (parent != null && parent.isRestoringParent) return;
        if (parent != null && parent.isSplitting) return;

        if (distanceToPlayer < diagonal
                && lodLevel < manager.maxLodLevel
                && childrenReady < 8
                && !isSplitting
                && !isRestoringParent
                && mesh != null) {
            if (!DCManager.greenLight) return;
            DCManager.greenLight = false;
            isSplitting = true;
           // Debug.Log("splitting");
            splitCube();

        } else if (lodLevel>0 && distanceToPlayer > diagonal * 3 && mesh != null  && !parent.isRestoringParent) {
            
            if (name.EndsWith("|DBR")) {
              
                parent.isRestoringParent = true;
              //  Debug.Log("restoring");
                restoreParentCube();
            }
        
        }

        //// patches
        /*
        if (meshData != null && DCManager.chunkList.Contains(this)) {
            if (DCManager.chunkDictionary.ContainsKey(transform.position + new Vector3(edgeSize, 0f, 0f))) {
                Chunk chunkXplus = DCManager.chunkDictionary[transform.position + new Vector3(edgeSize, 0f, 0f)];
                if(DCManager.chunkList.Contains(chunkXplus)){
                    GetComponent<MeshFilter>().mesh = meshData.CreateMesh(MeshData.TYPE_X_NORMAL_PLUS);
                } else {
                    GetComponent<MeshFilter>().mesh = meshData.CreateMesh(MeshData.TYPE_NORMAL);
                    //GetComponent<MeshFilter>().mesh = mesh;
                }
            }
        }*/
        


    }

    public void splitCube() {

     

        float delta = (float)edgeSize / 4;


        GameObject go1 = new GameObject();
        Chunk newChunk1 = go1.gameObject.AddComponent<Chunk>();
        newChunk1.createCube(this, manager, edgeSize / 2, lodLevel + 1, transform.position.x - delta, transform.position.y - delta, transform.position.z - delta, name + "|DBR");

        GameObject go2 = new GameObject();
        Chunk newChunk2 = go2.gameObject.AddComponent<Chunk>();
        newChunk2.createCube(this, manager, edgeSize / 2, lodLevel + 1, transform.position.x - delta, transform.position.y - delta, transform.position.z + delta, name + "|DBL");

        GameObject go3 = new GameObject();
        Chunk newChunk3 = go3.gameObject.AddComponent<Chunk>();
        newChunk3.createCube(this, manager, edgeSize / 2, lodLevel + 1, transform.position.x + delta, transform.position.y - delta, transform.position.z - delta, name + "|DTR");

        GameObject go4 = new GameObject();
        Chunk newChunk4 = go4.gameObject.AddComponent<Chunk>();
        newChunk4.createCube(this, manager, edgeSize / 2, lodLevel + 1, transform.position.x + delta, transform.position.y - delta, transform.position.z + delta, name + "|DTL");

        GameObject go5 = new GameObject();
        Chunk newChunk5 = go5.gameObject.AddComponent<Chunk>();
        newChunk5.createCube(this, manager, edgeSize / 2, lodLevel + 1, transform.position.x - delta, transform.position.y + delta, transform.position.z - delta, name + "|UBR");

        GameObject go6 = new GameObject();
        Chunk newChunk6 = go6.gameObject.AddComponent<Chunk>();
        newChunk6.createCube(this, manager, edgeSize / 2, lodLevel + 1, transform.position.x - delta, transform.position.y + delta, transform.position.z + delta, name + "|UBL");

        GameObject go7 = new GameObject();
        Chunk newChunk7 = go7.gameObject.AddComponent<Chunk>();
        newChunk7.createCube(this, manager, edgeSize / 2, lodLevel + 1, transform.position.x + delta, transform.position.y + delta, transform.position.z - delta, name + "|UTR");

        GameObject go8 = new GameObject();
        Chunk newChunk8 = go8.gameObject.AddComponent<Chunk>();
        newChunk8.createCube(this, manager, edgeSize / 2, lodLevel + 1, transform.position.x + delta, transform.position.y + delta, transform.position.z + delta, name + "|UTL");

    }

    public void restoreParentCube() {

        float delta = edgeSize / 2;

       
        parent.isRestoringParent = true;
       
        parent.createCube(parent.parent, manager, edgeSize * 2, lodLevel - 1, transform.position.x + delta, transform.position.y + delta, transform.position.z + delta, name.Substring(0, name.Length - 4));
        edgeDictionary.Clear();
       
        
    }


    public Chunk createCube(Chunk parentTarget, DCManager manager, float size, int lodLevel,float delta1, float delta2, float delta3, string name) {

        if (this == null) return null;
        parent = parentTarget;
        if (parentTarget != null) {
            parentTarget.children.Add(this);
        } else {
            manager.children.Add(this);
        }

        this.edgeSize = size;
        this.manager = manager;
        this.lodLevel = lodLevel;
        this.name = name;

        /////////////
        
        GameObject o = gameObject; 
        
        o.name = name;
        if(o.GetComponent<MeshFilter>()==null) o.AddComponent<MeshFilter>();
        if (o.GetComponent<MeshRenderer>() == null) o.AddComponent<MeshRenderer>();
        //o.AddComponent<MeshCollider>();
        
        o.transform.parent = manager.transform;

        
        transform.position = new Vector3(delta1, delta2, delta3);


       
        //MeshFilter mf = o.GetComponent<MeshFilter>();
        MeshRenderer mr = o.GetComponent<MeshRenderer>();
        //MeshCollider mc = o.GetComponent<MeshCollider>();

        shader = Shader.Find("paolo/NormalColor");
        material = new Material(shader);
        mr.sharedMaterial = material;

        
        //mf.sharedMesh = m;
        //m = md.CreateMesh();
        manager.meshGenerator.RequestMapData(edgeSize, manager.subdivisions, transform.position, manager.cnoise, 
            OnMapDataReceived, edgeDictionary, 
            edgeDictionaryXPlusNormalPatch,
            edgeDictionaryYPlusNormalPatch,
            edgeDictionaryZPlusNormalPatch
            );

        return this;

    }

    public void OnMapDataReceived(MapData mapData) {
        //Debug.Log("map data received ("+name+")"+mapData.edgeDictionary.Count);
        //this.mapData = mapData;
        //if(mapData.edgeDictionary.Count>0 || parent.isRestoringParent || isRestoringParent)
        manager.meshGenerator.RequestMeshData(mapData, OnMeshDataReceived);

       
        
    }


    public void OnMeshDataReceived(MeshData meshData) {

        //DCManager.chunkDictionary.Add(transform.position, this);
        this.meshData = meshData;
        DCManager.addChunkToList(this);
        //DCManager.chunkList.Add(this);
    }

    public void CreateFinalMesh() { 

        if (meshData == null) return;
        mesh = meshData.CreateMesh(MeshData.TYPE_NORMAL);
        Mesh XplusPatchMesh = meshData.CreateMesh(MeshData.TYPE_X_NORMAL_PLUS);

        GetComponent<MeshFilter>().mesh = mesh;

       
        if (parent != null) {
            if (parent.isSplitting) {
                parent.childrenReady++;
                if (parent.childrenReady == 8) {
                    Destroy(parent.GetComponent<MeshFilter>().mesh);
                    parent.mesh = null;
                    parent.childrenReady = 0;
                    parent.isSplitting = false;
                    //DCManager.chunkDictionary.Remove(parent.transform.position);
                    DCManager.removeChunkFromList(this);
                    DCManager.greenLight = true;
                }
            }
        } else {

            if (manager.isSplitting) {
                manager.childrenReady++;
                if (manager.childrenReady == 8) {
                    Destroy(parent.GetComponent<MeshFilter>().mesh);
                    manager.GetComponent<MeshFilter>().mesh = null;
                    manager.childrenReady = 0;
                    manager.isSplitting = false;
                    DCManager.greenLight = true;

                }
            }
        }

        if (isRestoringParent) {


            int k = children.Count;
            for (int i = 0; i < k; i++) {
                Destroy(children[i].gameObject);
                //DCManager.chunkDictionary.Remove(children[i].transform.position);
                //DCManager.chunkList.Remove(children[i]);
                DCManager.removeChunkFromList(children[i]);

            }

            children.Clear();
            isRestoringParent = false;
        }

        


        
  
    }

    


    void OnDrawGizmos() {

                if ( manager==null || !manager.showGizmos) return;



                    switch (lodLevel) {
          
           /* case 0:
                Gizmos.color = Color.red;
                break;
                
            case 1:
                Gizmos.color = Color.yellow;
                break;*/
            case 2:
                Gizmos.color = Color.white;
                break;
            case 3:
                Gizmos.color = Color.red;
                break;
                
            default:
                Gizmos.color = Color.white;
                Gizmos.color = new Color(1F, 1F, 1F, 0.25F);
                break;
                
        }
            

            Vector3 s = new Vector3(edgeSize, edgeSize, edgeSize);

            Gizmos.DrawWireCube(transform.position, s);
             
    }
  

}
