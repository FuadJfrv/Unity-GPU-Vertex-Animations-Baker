using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class AnimAsynchronizer : MonoBehaviour
{
    [SerializeField] private GameObject[] objects;

    private void Start()
    {
        MaterialPropertyBlock props = new MaterialPropertyBlock();
        MeshRenderer renderer;
        Material mat = objects[0].GetComponent<MeshRenderer>().material;
        foreach (GameObject obj in objects)
        {
            float r = Random.Range(0.00f, 1.00f);
            props.SetFloat("_AsyncOffset", r);

            renderer = obj.GetComponent<MeshRenderer>();
            renderer.SetPropertyBlock(props);
        }
        
        /*foreach (GameObject obj in objects)
        {
            props.SetFloat("_AsyncOffset", 0);

            renderer = obj.GetComponent<MeshRenderer>();
            renderer.SetPropertyBlock(props);
        }*/
    }
}
