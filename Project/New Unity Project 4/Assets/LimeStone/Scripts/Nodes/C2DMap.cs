
using UnityEngine;
using System.Collections;

/*
 * 
 * © 2014 LemonSpawn. All rights reserved. Source code in this project ("LimeStone") are not supported under any LemonSpawn standard support program or service. 
 * The scripts are provided AS IS without warranty of any kind. LemonSpawn disclaims all implied warranties including, without limitation, 
 * any implied warranties of merchantability or of fitness for a particular purpose. 
 * 
 * Basically, do what you want with this code, but don't blame us if something blows up. 
 * 
 * 
*/

namespace LimeStone
{

/*
 * 
 * 2D map class - stores all data to be used in the terrain generation process. 
 * 
 * */
    public class C2DMap
    {
        public float[,] map = null;
        public static int sizeX, sizeY;
        // The current random shift due to terrain position
        public static Vector2 shift = new Vector2(0, 0);
        public float maxVal, minVal;

        public float this [int i, int j]
        {
            get
            {
                return map [i, j];
            }
            set
            {
                map [i, j] = value;
            }
        }

        // Make sure Size is always set. 
        public C2DMap()
        {
            map = new float[sizeX, sizeY];
            zero();
        }

        public void CopyFrom(C2DMap m)
        {
            for (int i=0; i<sizeX; i++)
            {
                for (int j=0; j<sizeY; j++)
                {   
                    map [i, j] = m.map [i, j];
                }
            }
        }

        public void zero()
        {
            for (int i=0; i<sizeX; i++)
                for (int j=0; j<sizeY; j++)
                    map [i, j] = 0;
        }

        public void calculateStatistics()
        {
            minVal = 1E16f;
            maxVal = -1E16f;
            for (int i=0; i<sizeX; i++)
                for (int j=0; j<sizeY; j++)
                {
                    minVal = Mathf.Min(map [i, j], minVal);
                    maxVal = Mathf.Max(map [i, j], maxVal);
                }
        
        }

        public void calculatePerlin(float amplitude, float scale, float octaves, float kscale, float seed)
        {
            seed = 3.119f * seed;

            float rx = seed * 7.72423f;
            float ry = seed * 5.12352f;
            for (int i=0; i<sizeX; i++)
            {
                for (int j=0; j<sizeY; j++)
                {
                    CNodeManager.Progress();
                    float x = i / (float)(sizeX - 1) + rx + shift.y;
                    float y = j / (float)(sizeY - 1) + ry + shift.x;
                    map [i, j] = 0;
                    for (int k=1; k<=octaves+1; k++)
                    {
                        float kk = k * scale;
                        map [i, j] += amplitude * Mathf.PerlinNoise(x * kk, y * kk) / Mathf.Pow(k, kscale);///(float)kk;
                    }
                }
            }
        }

        public void calculateMultiridged(float seed, float heightScale, float frequency, float lacunarity, float gain, float offset)
        {
            float octaves = 8;
        
            float rx = seed * 7.72423f;
            float ry = seed * 5.12352f;
        
            float nx = sizeX;
            float ny = sizeY;

            float sx = shift.x;
            float sy = shift.y;

            for (int i=0; i<nx; i++)
            {
                for (int j=0; j<ny; j++)
                {
                    CNodeManager.Progress();
                    Vector3 p = new Vector3((i / (nx - 1) + sy) * frequency + rx, 0, (j / (ny - 1) + sx) * frequency + ry);
                    float v = Util.getRidgedMf(p, 1, (int)octaves, lacunarity, 0, offset, gain);
                    map [i, j] = v * heightScale;
                }
            }   
        
        
        
        }

        public void calculateSwiss(float seed, float heightScale, float frequency, float lacunarity, float gain, float offset, float warp, float power)
        {
        
            float octaves = 6;
        
            float rx = seed * 7.72423f;
            float ry = seed * 5.12352f;
        
            float nx = sizeX;
            float ny = sizeY;
        
            float sx = shift.x;
            float sy = shift.y;

            /*lacunarity = 2.0f;
        gain = 1.25f + 0.5f;
        offset = 0.5f;
*/
            for (int i=0; i<nx; i++)
            {
                for (int j=0; j<ny; j++)
                {
                    CNodeManager.Progress();
                    Vector3 p = new Vector3((i / (nx - 1) + sy) + rx, 0, (j / (ny - 1) + sx) + ry);
                    //(tmp, 240*s2, m_seed, (int)Math.min(14, level), 2.0f, twist, 1.25f + h2 , 0.45f,fff, (double)f)-0.0f))
                    float v = Util.swissTurbulence(p, frequency, (int)octaves, lacunarity, warp - 1.0f, offset, gain, (power), 0f);
                    map [i, j] = v * heightScale;
                }
            }   
        }
        // Simple smoothing
        public void Smooth(int N)
        {
            for (int i=0; i<N; i++)
                for (int x = 1; x < sizeX-1; x++)
                {
                    for (int y = 1; y < sizeY-1; y++)
                    {
                        float sum = 0;
                        for (int k=-1; k<2; k++)
                            for (int l=-1; l<2; l++)
                            {
                                sum += map [x + k, y + l];
                            }
                        map [x, y] = sum / 9f;
                    }
                }
        }
    }

}
