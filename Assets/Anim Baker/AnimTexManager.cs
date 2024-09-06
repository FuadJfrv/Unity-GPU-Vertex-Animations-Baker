using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class AnimTexManager : MonoBehaviour
{
    public void PlayAnim(Material mat, int index)
    {
        var sObj = AssetDatabase.LoadAssetAtPath<ObjectAnimData>("Assets/Anim Baker/" + mat.name + "_data.asset");
        index %= sObj.startFrames.Length;
        
        mat.SetFloat("_StartOffset", sObj.startFrames[index]);
        
        float value = (sObj.endFrames[index] - sObj.startFrames[index]) * 100f;
        value = Mathf.Floor(value) / 100f;
        mat.SetFloat("_ClipSize", value);
    }
}
