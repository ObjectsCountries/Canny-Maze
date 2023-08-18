﻿using UnityEngine;

public class TextureGenerator : MonoBehaviour
{
    public Texture2D[] textures;
    public int gridDimensions;
    public int[,] textureIndices;

    void Awake()
    {
        if (textures.Length == 0)
        {
            Debug.LogError("Please assign textures to the TextureGenerator script in the inspector.");
            return;
        }

        gridDimensions = Random.Range(5, 9);
        int cellSize = Mathf.FloorToInt(Mathf.Min(Screen.width, Screen.height) / gridDimensions);

        Texture2D gridTexture = new Texture2D(gridDimensions * cellSize, gridDimensions * cellSize, TextureFormat.RGB24, false);

        textureIndices = new int[gridDimensions, gridDimensions];

        for (int y = 0; y < gridDimensions; y++)
        {
            for (int x = 0; x < gridDimensions; x++)
            {
                int randomIndex = Random.Range(0, textures.Length);
                textureIndices[y, x] = randomIndex+1;
                Texture2D texture = textures[randomIndex];
                Color[] pixels = texture.GetPixels();

                for (int j = 0; j < cellSize; j++)
                {
                    for (int i = 0; i < cellSize; i++)
                    {
                        int destX = x * cellSize + i;
                        int destY = (gridDimensions - y - 1) * cellSize + j;

                        if (destX < gridTexture.width && destY < gridTexture.height)
                        {
                            int sourceX = Mathf.FloorToInt(i * (float)texture.width / cellSize);
                            int sourceY = Mathf.FloorToInt(j * (float)texture.height / cellSize);

                            gridTexture.SetPixel(destX, destY, pixels[sourceY * texture.width + sourceX]);
                        }
                    }
                }
            }
        }

        gridTexture.Apply();

        // Create a RenderTexture to scale up the texture
        int finalResolution = gridDimensions*cellSize; // Choose the desired final resolution
        RenderTexture renderTexture = new RenderTexture(finalResolution, finalResolution, 24);
        Graphics.Blit(gridTexture, renderTexture);

        // Create a new Texture2D from the RenderTexture
        Texture2D finalTexture = new Texture2D(finalResolution, finalResolution, TextureFormat.RGB24, false);
        RenderTexture.active = renderTexture;
        finalTexture.filterMode=FilterMode.Trilinear;
        finalTexture.ReadPixels(new Rect(0, 0, finalResolution, finalResolution), 0, 0,false);
        finalTexture.Apply();
        RenderTexture.active = null;

        MeshRenderer renderer = GetComponent<MeshRenderer>();
        if (renderer != null)
        {
            Material material = new Material(Shader.Find("Standard"));
            material.mainTexture = finalTexture;
            renderer.material = material;
        }
        else
        {
            Debug.LogError("MeshRenderer component not found on the plane.");
        }
    }
}
