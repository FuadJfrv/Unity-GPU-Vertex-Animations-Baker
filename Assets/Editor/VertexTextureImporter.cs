using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class VertexTextureImporter : AssetPostprocessor
{

    private void OnPreprocessTexture()
    {
        String folderName = "Anim Baker";
        if (assetPath.IndexOf("/Anim Baker/") == -1) 
            return;
        
        TextureImporter textureImporter = (TextureImporter)assetImporter;
        
        textureImporter.maxTextureSize = 16384;
        textureImporter.textureCompression = TextureImporterCompression.Uncompressed;
        textureImporter.sRGBTexture = false;
        textureImporter.filterMode = FilterMode.Bilinear;
        textureImporter.npotScale = TextureImporterNPOTScale.None;
        textureImporter.mipmapEnabled = false;
        
        textureImporter.SaveAndReimport();
    }
}
