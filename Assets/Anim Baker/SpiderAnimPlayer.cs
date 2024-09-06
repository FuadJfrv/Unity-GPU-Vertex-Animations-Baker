using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderAnimPlayer : MonoBehaviour
{
    private AnimTexManager _animTexManager;
    private int _index;
    private void Awake()
    {
        _animTexManager = FindObjectOfType<AnimTexManager>();
    }

    private void Start()
    {
        _animTexManager.PlayAnim(GetComponent<MeshRenderer>().sharedMaterial, _index);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _index++;
            _animTexManager.PlayAnim(GetComponent<MeshRenderer>().sharedMaterial, _index);
        }
    }
}
