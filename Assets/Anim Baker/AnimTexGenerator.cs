using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class AnimTexGenerator : MonoBehaviour
{
    private SkinnedMeshRenderer _meshRenderer;
    private Mesh _tempMesh;
    private Animator _animator;
    [SerializeField] private List<AnimationClip> clips;

    private Mesh _fileMesh;
    private String _pathToMesh;
    private const String _pathToFolder = "Assets/Anim Baker/";
    private void Init()
    {
        _animator = GetComponent<Animator>();
        _meshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        _tempMesh = new Mesh();
        
        transform.position = Vector3.zero; 
        _animator.enabled = false;

        
        if (AssetDatabase.IsValidFolder(_pathToFolder) == false)
        {
            AssetDatabase.CreateFolder("Assets", "Anim Baker");
        }
        //create a folder for this object
        if (AssetDatabase.IsValidFolder(_pathToFolder + gameObject.name + "/") == false)
        {
            AssetDatabase.CreateFolder(_pathToFolder, gameObject.name);
        }
        var pathToObjectFolder = _pathToFolder + gameObject.name + "/";
        
        //get the mesh from the files so changes are saved when scene/editor restarts
        var meshPath = pathToObjectFolder + gameObject.name + "_mesh" + ".mesh";
        Mesh m = new Mesh();
        _meshRenderer.BakeMesh(m);
        AssetDatabase.CreateAsset(m, meshPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        _fileMesh = AssetDatabase.LoadAssetAtPath<Mesh>(meshPath);
    }

    public void Generate()
    {
        Init();
        
        int verticesCount = _meshRenderer.sharedMesh.vertices.Length;
        int totalLengthInFrames = 0;
        for (int i = 0; i < clips.Count; i++)
        {
            totalLengthInFrames += GetLengthInFrames(clips[i]);
        }
        
        var sObj = ScriptableObject.CreateInstance<ObjectAnimData>();
        sObj.startFrames = new float[clips.Count];
        sObj.endFrames = new float[clips.Count];
        sObj.startFrames[0] = 0;
        sObj.endFrames[0] = 0;
        float l = 0;
        for (int i = 0; i < clips.Count; i++)
        {
            if (i>0) sObj.startFrames[i] = sObj.endFrames[i-1];
            l += GetLengthInFrames(clips[i]); 
            sObj.endFrames[i] = l / totalLengthInFrames;
        }
        AssetDatabase.CreateAsset(sObj, "Assets/Anim Baker/" + gameObject.name + "_data.asset");
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        
        float maxDist = -1;
        foreach (var clip in clips)
        {
            maxDist = GetMaxVertDist(totalLengthInFrames, maxDist, clip);
        }
        
        var pathToFolder = "Assets/Anim Baker/" + gameObject.name + "/";

        var posTexture = new Texture2D(verticesCount, totalLengthInFrames + clips.Count, TextureFormat.RGBAFloat, false);
        var normTexture = new Texture2D(verticesCount, totalLengthInFrames + clips.Count, TextureFormat.RGBAFloat, false);
        GenerateTextures(maxDist, posTexture, normTexture, pathToFolder);

        GenerateUvs(verticesCount, totalLengthInFrames);

        var mat = GenerateMaterial(maxDist, pathToFolder);
        
        GenerateObj(mat);
        gameObject.SetActive(false);
    }

    private static int GetLengthInFrames(AnimationClip clip)
    {
        return (int)(clip.length * clip.frameRate);
    }

    private void GenerateObj(Material mat)
    {
        var obj = new GameObject();
        Undo.RegisterCreatedObjectUndo(obj, "Created obj");
        obj.transform.position = transform.position;
        var objFilter = obj.AddComponent<MeshFilter>();
        var objMat = obj.AddComponent<MeshRenderer>();

        objFilter.mesh = _fileMesh;
        objMat.material = mat;
        obj.name = transform.name + "_baked";
    }

    private Material GenerateMaterial(float maxDist, String pathToFolder)
    {
        var mat = new Material(Shader.Find("Custom/VertexAnimation"));
        mat.SetFloat("_MaxDist", maxDist);
        mat.SetFloat("_SpeedController", 1f/ clips.Count);

        var savedPosTexture =
            AssetDatabase.LoadAssetAtPath<Texture2D>(pathToFolder + gameObject.name + "_PositionTex.exr");
        var savedNormTexture =
            AssetDatabase.LoadAssetAtPath<Texture2D>(pathToFolder + gameObject.name + "_NormalTex.exr");

        mat.SetTexture("_MainTex", _meshRenderer.sharedMaterial.mainTexture);
        mat.SetTexture("_PosTex", savedPosTexture);
        mat.SetTexture("_NormTex", savedNormTexture);
        mat.enableInstancing = true;

        AssetDatabase.CreateAsset(mat, pathToFolder + gameObject.name + ".mat");

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        return mat;
    }

    private void GenerateUvs(int verticesCount, int lengthInFrames)
    {
        Vector2[] uvs = new Vector2[verticesCount];
        float pixelWidth = 1f / verticesCount;
        for (int j = 0; j < verticesCount; j++)
        {
            uvs[j] = new Vector2(pixelWidth * j + pixelWidth / 2, 1f / (lengthInFrames));
        }

        _fileMesh.uv2 = uvs;
        var guid = AssetDatabase.GUIDFromAssetPath(_pathToMesh);
        AssetDatabase.SaveAssetIfDirty(guid); //so it works when the scene restarts
    }

    private void GenerateTextures(float maxDist, Texture2D posTexture, Texture2D normTexture, String pathToFolder)
    {
        int startRange = 0;
        int endRange = GetLengthInFrames(clips[0]);

        int n = 0;
        //create texture
        while (n < clips.Count)
        {
            for (int i = startRange; i <= endRange; i++)
            {
                int lengthInFrames = endRange - startRange;
                
                clips[n].SampleAnimation(gameObject, (clips[n].length / lengthInFrames) * (i-startRange));

                _tempMesh.Clear();
                _meshRenderer.BakeMesh(_tempMesh);
                var verts = _tempMesh.vertices;

                for (int j = 0; j < verts.Length; j++)
                {
                    //Vertex pos ranges from 0 to 1
                    var compressedVertPos = verts[j] / maxDist;
                    var remappedVertPos = (compressedVertPos + Vector3.one) * 0.5f;

                    if (j == 0 && i == 0) print(remappedVertPos);
                    //Color texture
                    posTexture.SetPixel(j, i,
                        new Color(remappedVertPos.x, remappedVertPos.y, remappedVertPos.z));

                    //Same thing for normals 
                    var remappedNormals = (_tempMesh.normals[j] + Vector3.one) * 0.5f;

                    normTexture.SetPixel(j, i,
                        new Color(remappedNormals.x, remappedNormals.y, remappedNormals.z));
                }
            }
            startRange = endRange + 1;
            n++;
            if (n >= clips.Count) break;
            endRange += GetLengthInFrames(clips[n]) + 1;
        }


        //save the textures to file
        SaveTexture(pathToFolder, posTexture, "PositionTex");
        SaveTexture(pathToFolder, normTexture, "NormalTex");
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private float GetMaxVertDist(int lengthInFrames, float maxDist, AnimationClip clip)
    {
        for (int i = 0; i <= lengthInFrames; i++)
        {
            clip.SampleAnimation(gameObject, (clip.length / lengthInFrames) * i);

            _meshRenderer.BakeMesh(_tempMesh);
            var verts = _tempMesh.vertices;

            foreach (var vertPos in verts)
            {
                var dist = Vector3.Distance(vertPos, (transform.position));
                if (dist > maxDist)
                {
                    maxDist = dist;
                }
            }
        }

        return maxDist;
    }

    private void SaveTexture(String path, Texture2D texture, String name)
    {
        texture.Apply();
        var texBytes = texture.EncodeToEXR(Texture2D.EXRFlags.OutputAsFloat);
        File.WriteAllBytes(path + gameObject.name +  "_" + name + ".exr", texBytes);
    }
}
