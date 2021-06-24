using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestrucionController : MonoBehaviour
{
    Mesh mesh;
    Vector3[] vertices;


    void Start()
    {
        mesh = GetComponent<MeshFilter>().mesh;
        vertices = mesh.vertices;

        ////DEV STUFF
        //for (int i = 0; i < vertices.length; i++)
        //{
        //    debug.log(this.gameobject.name + ": " + vertices[i]);
        //}
    }


    void Update()
    {

    }
}
