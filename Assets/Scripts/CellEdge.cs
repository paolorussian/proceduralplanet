using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellEdge {

    public Vector3 inV, outV;
    public DCManager manager;
    float h;
    public bool hasIntersection=false;
    public Vector3 intersectionPoint;
    public Vector3[] cells = new Vector3[4];
    public Vector3[] centerPoints = new Vector3[4];
    public string name = "";
    public bool isFlipped = false;
    public Chunk parentChunk;
    public float noiseIn;
    public float noiseOut;
    public Vector3 position;
    public float treshold;

    public bool X_MAX = false;
    public bool X_MIN = false;
    public bool Y_MAX = false;
    public bool Y_MIN = false;
    public bool Z_MAX = false;
    public bool Z_MIN = false;

    public int px, py, pz;

    public CellEdge() {
        //internal use
    }



    public CellEdge(Vector3 inV, Vector3 outV, float noiseIn, float noiseOut, Vector3 position, float treshold) {
        

         if (noiseIn < treshold && noiseOut > treshold) {  // regular
            this.hasIntersection = true;
            this.name = outV + "|" + inV;
            this.noiseIn = noiseIn;
            this.noiseOut = noiseOut;
            this.position = position;
            this.treshold = treshold;

            intersectionPoint = Vector3.Lerp(inV, outV, (treshold - noiseIn) / (noiseOut - noiseIn));

        
        } else if (noiseOut < treshold && noiseIn > treshold) {
            this.hasIntersection = true;
            isFlipped = true;
            this.outV = inV;
            this.inV = outV;
             this.name = inV + "|" + outV;
            //this.name = outV + "|" + inV;
            intersectionPoint = Vector3.Lerp(inV, outV, (treshold - noiseIn) / (noiseOut - noiseIn));

        }
        



        }


}
