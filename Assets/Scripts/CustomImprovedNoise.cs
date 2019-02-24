using UnityEngine;
using System.Collections;

public class CustomImprovedNoise {

    int[] p = new int[512];

    public CustomImprovedNoise(int seed) {
        shuffle(seed);
    }

    public double noise(Vector3 v) {
        return noise(v.x, v.y, v.z);
    }

    public double noise(float x, float y, float z) {

        int X = (int)Mathf.Floor(x) & 255, // FIND UNIT CUBE THAT
        Y = (int)Mathf.Floor(y) & 255, // CONTAINS POINT.
        Z = (int)Mathf.Floor(z) & 255;
        x -= Mathf.Floor(x); // FIND RELATIVE X,Y,Z
        y -= Mathf.Floor(y); // OF POINT IN CUBE.
        z -= Mathf.Floor(z);
        double u = fade(x), // COMPUTE FADE CURVES
        v = fade(y), // FOR EACH OF X,Y,Z.
        w = fade(z);
        int A = p[X] + Y, AA = p[A] + Z, AB = p[A + 1] + Z, // HASH COORDINATES OF
        B = p[X + 1] + Y, BA = p[B] + Z, BB = p[B + 1] + Z; // THE 8 CUBE CORNERS,

        return lerp(w, lerp(v, lerp(u, grad(p[AA], x, y, z), // AND ADD
                grad(p[BA], x - 1, y, z)), // BLENDED
                lerp(u, grad(p[AB], x, y - 1, z), // RESULTS
                        grad(p[BB], x - 1, y - 1, z))),// FROM  8
                lerp(v, lerp(u, grad(p[AA + 1], x, y, z - 1), // CORNERS
                        grad(p[BA + 1], x - 1, y, z - 1)), // OF CUBE
                        lerp(u, grad(p[AB + 1], x, y - 1, z - 1), grad(p[BB + 1], x - 1, y - 1, z - 1))));
    }

    double fade(double t) {
        return t * t * t * (t * (t * 6 - 15) + 10);
    }

    double lerp(double t, double a, double b) {
        return a + t * (b - a);
    }

    double grad(int hash, double x, double y, double z) {
        int h = hash & 15; // CONVERT LO 4 BITS OF HASH CODE
        double u = h < 8 ? x : y, // INTO 12 GRADIENT DIRECTIONS.
        v = h < 4 ? y : h == 12 || h == 14 ? x : z;
        return ((h & 1) == 0 ? u : -u) + ((h & 2) == 0 ? v : -v);
    }

   

    public double perlinNoise(float x, float y) {
        float n = 0;

        for (int i = 0; i < 8; i++) {
            float stepSize = 64.0f / ((1 << i));
            n += ((float)noise(x / stepSize, y / stepSize, 128f) * 1.0f / (1 << i)); 
        }

        return n;
    }

    public void shuffle(int seed) {

        //Random.seed = seed;
        Random.InitState(seed);
        //Random random = new Random();
        int[] permutation = new int[256];
        for (int i = 0; i < 256; i++) {
            permutation[i] = i;
        }

        for (int i = 0; i < 256; i++) {
            //int j = random.nextInt(256 - i) + i;
            int j = Random.Range(0, 256-i) + i;
            int tmp = permutation[i];
            permutation[i] = permutation[j];
            permutation[j] = tmp;
            p[i + 256] = p[i] = permutation[i];
        }
    }

   
}