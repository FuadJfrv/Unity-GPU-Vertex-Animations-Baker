using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/ObjectAnimData")]
public class ObjectAnimData : ScriptableObject
{
    public float[] startFrames;
    public float[] endFrames;
}
