using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class CubeMarching2D : MonoBehaviour
{
    public bool pointsGizmoOn, insideWalls, outsidedWalls, randomPerlinOffset;
    public float perlinOffset, markerSize, noiseScale;
    public Vector2Int size;

    float[,] points;

    //public Transform marker;


    // Start is called before the first frame update
    void Start()
    {
        //setup
        gameObject.AddComponent<MeshFilter>();
        gameObject.AddComponent<MeshRenderer>();
        Mesh labirynth = GetComponent<MeshFilter>().mesh;
        Vector3[] verticies;

        points = new float[size.x, size.y];

        //if (randomPerlinOffset)
        //{
        //    float randomOffset = Random.Ra;
        //}

        //assingning points values from Perlin Noise
        for (int y = 0; y < size.y; y++)
        {
            for (int x = 0; x < size.x; x++)
            {
                points[x, y] = CalculatePerlin(x, y, size.x, size.y, noiseScale);

                //------------------------------------------------ Special Marker System ------------------------------------------------
                /*
                //this part is important for both marking methods
                Vector3 offset = new Vector3(x, 0, y);

                //MACHINE KILLER!!!!

                GameObject marker = GameObject.CreatePrimitive(PrimitiveType.Quad);
                marker.name = "PerlinSampler" + y + x;
                marker.transform.position = (this.transform.position + offset);
                marker.transform.Rotate(Vector3.right, 90, Space.Self);
                //Scale = 1 is greate for visualizing Perlin Noise, but you can set it dor like 0.1 to see points of sampling in proper color
                marker.transform.localScale = new Vector3(1, 1, 1);
                marker.GetComponent<Renderer>().material.color = new UnityEngine.Color(points[x, y], points[x, y], points[x, y], 1);
                */

                //I was about to use instanciate, but didn't figure out how to change color of each marker. I left it here becouse maby later it will turn out this is better method
                /*
                marker.position = this.transform.position + offset;
                Instantiate(sphere, sphere.position, Quaternion.identity);
                */

            }
        }


        verticies = VerticiesFromPerlin();
        
        //This display verticies                    CAUTION!!!! MACHINE KILLER!!!
        /*
        int j = 0;
        while (j<verticies.Length)
        {
            GameObject lol = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            lol.transform.position = verticies[j];
            lol.name = "Vert"+j;
            lol.transform.localScale = new Vector3(markerSize, markerSize, markerSize);
            j++;
        }
        */
        
        labirynth.vertices = verticies;
        labirynth.triangles = Assemble();
    }

    void OnDrawGizmosSelected()
    {
        if (pointsGizmoOn)
        {
            for (int y = 0; y < size.y; y++)
            {
                for (int x = 0; x < size.x; x++)
                {
                    Vector3 offset = new Vector3(x, 0, y);
                    Gizmos.color = UnityEngine.Color.red;
                    Gizmos.DrawSphere(this.transform.position + offset, 0.1f);
                }
            }
        }
    }

    float CalculatePerlin(float x, float y, float sizeX, float sizeY, float scale)
    {
        //TODO: Reorganize
        return Mathf.PerlinNoise((float)x / size.x * noiseScale, (float)y / size.y * noiseScale);
    }

    Vector3[] VerticiesFromPerlin()
    {
        //setup
        int resultSize = (size.x * size.y * 4);
        Vector3[] result = new Vector3[resultSize];
        int tempIterator = 0;

        for (int y = 0; y < size.y; y++)
        {
            for (int x = 0; x < size.x; x++)
            {
                //Adding points is L-shape based in order: 0-down, 1-above, 2-left, 3-above
                //It's making point's outside of resolution, but I think preventing it is just not worth it (you will thank yourself when making triangles XD)

                result[tempIterator++] = new Vector3(x + 0.5f, gameObject.transform.position.y, y);
                result[tempIterator++] = new Vector3(x + 0.5f, gameObject.transform.position.y+1, y);

                result[tempIterator++] = new Vector3(x, gameObject.transform.position.y, y + 0.5f);
                result[tempIterator++] = new Vector3(x, gameObject.transform.position.y+1, y + 0.5f);
            }
        }

        return result;
    }

    int[] Assemble()
    {
        //setup
        int[] pointsBuffer = new int[4];
        int squareType, rowVertsAmmount = size.x*4;
        List<int> resultList = new List<int>();

        for (int y = 0; y < size.y-1; y++)
        {
            for (int x = 0; x < size.x-1; x++)
            {
                //calculating 4 points of square
                pointsBuffer[0] = (int)Mathf.Ceil(points[x, y] - 0.5f);
                pointsBuffer[1] = (int)Mathf.Ceil(points[x + 1, y] - 0.5f);
                pointsBuffer[2] = (int)Mathf.Ceil(points[x + 1, y + 1] - 0.5f);
                pointsBuffer[3] = (int)Mathf.Ceil(points[x, y + 1] - 0.5f);

                squareType = pointsBuffer[0] * 8 + pointsBuffer[1] * 4 + pointsBuffer[2] * 2 + pointsBuffer[3];

                //calculating first point of cube
                int localZero = y * rowVertsAmmount + x * 4;
                switch (squareType)
                {
                    case 1:
                        if (insideWalls)
                        {
                            resultList.Add(localZero + 2);
                            resultList.Add(localZero + 3);
                            resultList.Add(localZero + rowVertsAmmount);

                            resultList.Add(localZero + rowVertsAmmount);
                            resultList.Add(localZero + 3);
                            resultList.Add(localZero + rowVertsAmmount + 1);
                        }

                        if (outsidedWalls)
                        {
                            resultList.Add(localZero + rowVertsAmmount);
                            resultList.Add(localZero + 3);
                            resultList.Add(localZero + 2);

                            resultList.Add(localZero + rowVertsAmmount + 1);
                            resultList.Add(localZero + 3);
                            resultList.Add(localZero + rowVertsAmmount);
                        }
                        ;
                        break;
                    case 2:
                        if (insideWalls)
                        {
                            resultList.Add(localZero + rowVertsAmmount);
                            resultList.Add(localZero + 7);
                            resultList.Add(localZero + 6);

                            resultList.Add(localZero + rowVertsAmmount + 1);
                            resultList.Add(localZero + 7);
                            resultList.Add(localZero + rowVertsAmmount);
                        }

                        if (outsidedWalls)
                        {
                            resultList.Add(localZero + 6);
                            resultList.Add(localZero + 7);
                            resultList.Add(localZero + rowVertsAmmount);

                            resultList.Add(localZero + rowVertsAmmount);
                            resultList.Add(localZero + 7);
                            resultList.Add(localZero + rowVertsAmmount + 1);
                        }
                        ;
                        break;
                    case 3:
                        if (insideWalls)
                        {
                            resultList.Add(localZero + 6);
                            resultList.Add(localZero + 2);
                            resultList.Add(localZero + 3);

                            resultList.Add(localZero + 6);
                            resultList.Add(localZero + 7);
                            resultList.Add(localZero + 3);
                        }

                        if (outsidedWalls)
                        {
                            resultList.Add(localZero + 3);
                            resultList.Add(localZero + 2);
                            resultList.Add(localZero + 6);

                            resultList.Add(localZero + 3);
                            resultList.Add(localZero + 7);
                            resultList.Add(localZero + 6);
                        }
                        ;
                        break;
                    case 4:
                        if (insideWalls)
                        {
                            resultList.Add(localZero + 6);
                            resultList.Add(localZero + 1);
                            resultList.Add(localZero);

                            resultList.Add(localZero + 6);
                            resultList.Add(localZero + 7);
                            resultList.Add(localZero + 1);
                        }

                        if (outsidedWalls)
                        {
                            resultList.Add(localZero);
                            resultList.Add(localZero + 1);
                            resultList.Add(localZero + 6);

                            resultList.Add(localZero + 1);
                            resultList.Add(localZero + 7);
                            resultList.Add(localZero + 6);
                        }
                        ;
                        break;
                    case 5:
                        if (insideWalls)
                        {
                            resultList.Add(localZero + 2);
                            resultList.Add(localZero + 3);
                            resultList.Add(localZero + rowVertsAmmount);

                            resultList.Add(localZero + rowVertsAmmount);
                            resultList.Add(localZero + 3);
                            resultList.Add(localZero + rowVertsAmmount + 1);

                            resultList.Add(localZero + 6);
                            resultList.Add(localZero + 1);
                            resultList.Add(localZero);

                            resultList.Add(localZero + 6);
                            resultList.Add(localZero + 7);
                            resultList.Add(localZero + 1);
                        }

                        if (outsidedWalls)
                        {
                            resultList.Add(localZero + rowVertsAmmount);
                            resultList.Add(localZero + 3);
                            resultList.Add(localZero + 2);

                            resultList.Add(localZero + rowVertsAmmount + 1);
                            resultList.Add(localZero + 3);
                            resultList.Add(localZero + rowVertsAmmount);

                            resultList.Add(localZero);
                            resultList.Add(localZero + 1);
                            resultList.Add(localZero + 6);

                            resultList.Add(localZero + 1);
                            resultList.Add(localZero + 7);
                            resultList.Add(localZero + 6);
                        }
                        ;
                        break;
                    case 6:
                        if (insideWalls)
                        {
                            resultList.Add(localZero);
                            resultList.Add(localZero + 1);
                            resultList.Add(localZero + rowVertsAmmount);

                            resultList.Add(localZero + 1);
                            resultList.Add(localZero + rowVertsAmmount + 1);
                            resultList.Add(localZero + rowVertsAmmount);
                        }
                        if (outsidedWalls)
                        {
                            resultList.Add(localZero + rowVertsAmmount);
                            resultList.Add(localZero + 1);
                            resultList.Add(localZero);

                            resultList.Add(localZero + rowVertsAmmount);
                            resultList.Add(localZero + rowVertsAmmount + 1);
                            resultList.Add(localZero + 1);
                        }
                        ;
                        break;
                    case 7:
                        if (insideWalls)
                        {
                            resultList.Add(localZero + 2);
                            resultList.Add(localZero + 1);
                            resultList.Add(localZero);

                            resultList.Add(localZero + 2);
                            resultList.Add(localZero + 3);
                            resultList.Add(localZero + 1);
                        }
                        if (outsidedWalls)
                        {
                            resultList.Add(localZero);
                            resultList.Add(localZero + 1);
                            resultList.Add(localZero + 2);

                            resultList.Add(localZero + 1);
                            resultList.Add(localZero + 3);
                            resultList.Add(localZero + 2);
                        }
                        ;
                        break;
                    case 8:
                        if (insideWalls)
                        {
                            resultList.Add(localZero);
                            resultList.Add(localZero + 1);
                            resultList.Add(localZero + 2);

                            resultList.Add(localZero + 1);
                            resultList.Add(localZero + 3);
                            resultList.Add(localZero + 2);
                        }
                        if (outsidedWalls)
                        {
                            resultList.Add(localZero + 2);
                            resultList.Add(localZero + 1);
                            resultList.Add(localZero);

                            resultList.Add(localZero + 2);
                            resultList.Add(localZero + 3);
                            resultList.Add(localZero + 1);
                        }
                        ;
                        break;
                    case 9:
                        if (insideWalls)
                        {
                            resultList.Add(localZero + rowVertsAmmount);
                            resultList.Add(localZero + 1);
                            resultList.Add(localZero);

                            resultList.Add(localZero + rowVertsAmmount);
                            resultList.Add(localZero + rowVertsAmmount + 1);
                            resultList.Add(localZero + 1);
                        }
                        if (outsidedWalls)
                        {
                            resultList.Add(localZero);
                            resultList.Add(localZero + 1);
                            resultList.Add(localZero + rowVertsAmmount);

                            resultList.Add(localZero + 1);
                            resultList.Add(localZero + rowVertsAmmount + 1);
                            resultList.Add(localZero + rowVertsAmmount);
                        }
                        ;
                        break;
                    case 10:
                        if (insideWalls)
                        {
                            resultList.Add(localZero + rowVertsAmmount);
                            resultList.Add(localZero + 7);
                            resultList.Add(localZero + 6);

                            resultList.Add(localZero + rowVertsAmmount + 1);
                            resultList.Add(localZero + 7);
                            resultList.Add(localZero + rowVertsAmmount);


                            resultList.Add(localZero);
                            resultList.Add(localZero + 1);
                            resultList.Add(localZero + 2);

                            resultList.Add(localZero + 1);
                            resultList.Add(localZero + 3);
                            resultList.Add(localZero + 2);
                        }

                        if (outsidedWalls)
                        {
                            resultList.Add(localZero + 6);
                            resultList.Add(localZero + 7);
                            resultList.Add(localZero + rowVertsAmmount);

                            resultList.Add(localZero + rowVertsAmmount);
                            resultList.Add(localZero + 7);
                            resultList.Add(localZero + rowVertsAmmount + 1);

                            resultList.Add(localZero + 2);
                            resultList.Add(localZero + 1);
                            resultList.Add(localZero);

                            resultList.Add(localZero + 2);
                            resultList.Add(localZero + 3);
                            resultList.Add(localZero + 1);
                        }
                        ;
                        break;
                    case 11:
                        if (insideWalls)
                        {
                            resultList.Add(localZero);
                            resultList.Add(localZero + 1);
                            resultList.Add(localZero + 6);

                            resultList.Add(localZero + 1);
                            resultList.Add(localZero + 7);
                            resultList.Add(localZero + 6);
                        }

                        if (outsidedWalls)
                        {
                            resultList.Add(localZero + 6);
                            resultList.Add(localZero + 1);
                            resultList.Add(localZero);

                            resultList.Add(localZero + 6);
                            resultList.Add(localZero + 7);
                            resultList.Add(localZero + 1);
                        }
                        ;
                        break;
                    case 12:
                        if (insideWalls)
                        {
                            resultList.Add(localZero + 2);
                            resultList.Add(localZero + 3);
                            resultList.Add(localZero + 6);

                            resultList.Add(localZero + 6);
                            resultList.Add(localZero + 3);
                            resultList.Add(localZero + 7);
                        }

                        if (outsidedWalls)
                        {
                            resultList.Add(localZero + 6);
                            resultList.Add(localZero + 3);
                            resultList.Add(localZero + 2);

                            resultList.Add(localZero + 7);
                            resultList.Add(localZero + 3);
                            resultList.Add(localZero + 6);
                        }
                        ;
                        break;
                    case 13:
                        if (insideWalls)
                        {
                            resultList.Add(localZero + 6);
                            resultList.Add(localZero + 7);
                            resultList.Add(localZero + rowVertsAmmount);

                            resultList.Add(localZero + rowVertsAmmount);
                            resultList.Add(localZero + 7);
                            resultList.Add(localZero + rowVertsAmmount + 1);
                        }

                        if (outsidedWalls)
                        {
                            resultList.Add(localZero + rowVertsAmmount);
                            resultList.Add(localZero + 7);
                            resultList.Add(localZero + 6);

                            resultList.Add(localZero + rowVertsAmmount + 1);
                            resultList.Add(localZero + 7);
                            resultList.Add(localZero + rowVertsAmmount);
                        }
                        ;
                        break;
                    case 14:
                        if (insideWalls)
                        {
                            resultList.Add(localZero + rowVertsAmmount);
                            resultList.Add(localZero + 3);
                            resultList.Add(localZero + 2);

                            resultList.Add(localZero + rowVertsAmmount + 1);
                            resultList.Add(localZero + 3);
                            resultList.Add(localZero + rowVertsAmmount);
                        }

                        if (outsidedWalls)
                        {
                            resultList.Add(localZero + 2);
                            resultList.Add(localZero + 3);
                            resultList.Add(localZero + rowVertsAmmount);

                            resultList.Add(localZero + rowVertsAmmount);
                            resultList.Add(localZero + 3);
                            resultList.Add(localZero + rowVertsAmmount + 1);
                        }
                        ;
                        break;
                    default:
                        ;
                        break;
                }
            }
        }

        int[] result = resultList.ToArray();
        return result;
    }
}
