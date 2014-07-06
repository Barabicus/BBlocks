using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Text;
using System;
using System.Threading;

public class PerlinTesting : EditorWindow
{

    Texture2D texture = new Texture2D(100, 100);
    float fx = 0.0f, fy = 0.0f, dx = 0.0f, dy = 0.0f;
    float heightWeight = 5f;
    int item = 0;
    bool match = true;
    bool applyHeight = true;
    bool round = true;
    bool abs = false;
    float Octaves = 5f;
    // Add menu item named "My Window" to the Window menu
    [MenuItem("Window/Perlin")]
    public static void ShowWindow()
    {
        //Show existing window instance. If one doesn't exist, make one.
        EditorWindow.GetWindow(typeof(PerlinTesting));
    }
    int width = 100;
    int height = 100;
    void OnGUI()
    {
        EditorGUI.DrawPreviewTexture(new Rect(50, 10, width, height), texture);

        int yy = 0;
        GUILayout.BeginArea(new Rect(50, 120, 500, 350));
        fx = EditorGUI.Slider(new Rect(0, yy, 250, 20), "fx", fx, 0f, 100f);
        fy = EditorGUI.Slider(new Rect(0, yy += 22, 250, 20), "fy", fy, 0f, 100f);
        dx = EditorGUI.Slider(new Rect(0, yy += 22, 250, 20), "dx", dx, 0f, 100f);
        dy = EditorGUI.Slider(new Rect(0, yy += 22, 250, 20), "dy", dy, 0f, 100f);
        Octaves = EditorGUI.Slider(new Rect(0, yy += 22, 250, 20), "Octaves", Octaves, 0f, 100f);
        item = int.Parse(EditorGUI.TextField(new Rect(0, yy += 22, 250, 20), "item", item + ""));
        match = EditorGUI.Toggle(new Rect(0, yy += 22, 250, 20), "Match X & Y", match);
        applyHeight = EditorGUI.Toggle(new Rect(0, yy += 22, 250, 20), "Apply Height", applyHeight);
        if (applyHeight)
            heightWeight = EditorGUI.Slider(new Rect(0, yy += 22, 250, 20), "HeightWeight", heightWeight, 0f, height);
        round = EditorGUI.Toggle(new Rect(0, yy += 22, 250, 20), "Round", round);
        abs = EditorGUI.Toggle(new Rect(0, yy += 22, 250, 20), "Absolute", abs);



        if (match)
        {
            fy = fx;
        }

        if (GUI.Button(new Rect(0, yy += 22, 250, 25), "Update"))
        {
            UpdatePerlin();
        }
        GUILayout.EndArea();

       // UpdatePerlin();
    }

    void UpdatePerlin()
    {
        texture = new Texture2D(width, height);
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float col = 0;
                switch (item)
                {
                    case 0:
                        col = UnityPerlin(x, y);
                        break;
                    case 1:
                        col = ThreeDNoisePerlin(x, y);
                        break;
                    case 2:
                        col = ChunkPerlin(x, y);
                        break;
                    case 3:
                        col = BiomeChooser(x, y);
                        break;
                }

                if (abs)
                {
                    col = Mathf.Abs(col);

                    //   col += .2f * (1 - col);
                    //col += 1 * 0.5f;
                }

                if (applyHeight)
                    col *= HeightOp(y, heightWeight);

                if (round)
                    col = Mathf.Round(col);

                texture.SetPixel(x, y, new Color(col, col, col));
            }
        }
        texture.Apply();
    }

    float HeightOp(int y, float weight)
    {
        return (((height - y) / 100f)) * heightWeight;
    }

    float UnityPerlin(int x, int y)
    {
        // return Mathf.PerlinNoise(dx + (x / (float)width) * fx, dy + (y / (float)height) * fy);
        return Mathf.PerlinNoise(dx + (x / (float)width) * Octaves, dy + (y / (float)height) * Octaves);
    }

    float ChunkPerlin(int x, int y)
    {
        try
        {
            x -= x % (int)fx;
            y -= y % (int)fy;
        }
        catch (Exception e)
        {
            return 0.0f;
        }
        return Mathf.PerlinNoise((dx + (x / (float)width) * Octaves), (dy + (y / (float)height) * Octaves));
    }

    float ChunkPerlinOther(int x, int y)
    {
        try
        {
            x -= x % (int)fx;
            y -= y % (int)fy;
        }
        catch (Exception e)
        {
            return 0.0f;
        }
        return Mathf.PerlinNoise((dx) * 1, (dy)) * 1;
    }

    float ThreeDNoisePerlin(int x, int y)
    {
        return SimplexNoise.Noise(dx + (x / (float)width) * Octaves, (y / (float)height * Octaves), dy / 100f * Octaves);
    }

    float[] closeArr = new float[] { 0.0f, 1f };
    float ClosestTo(float x)
    {
        return 0.0f;
    }

    float AddedNoise(int x, int y)
    {
        int mul = 1;
        float perlin = 1;
        for (int i = 0; i < (int)fx + 1; i++)
        {
            perlin *= Mathf.PerlinNoise((dx + (x / (float)width) * Octaves * mul) * 1, (dy + (y / (float)height) * Octaves * mul)) * 1;
            mul++;

        }
        return perlin;
    }

    float BiomeChooser(int x, int y)
    {
        Debug.Log("test");
        UnityEngine.Random.seed = 3121512;
        int rand = UnityEngine.Random.Range(0, 2);
        switch (rand)
        {
            case 0:
                return BiomeOne(x, y);
            case 1:
                return BiomeTwo(x, y);
        }
        return 0.0f;
    }

    float BiomeOne(int x, int y)
    {
        return Mathf.PerlinNoise((dx + (x / (float)width) * Octaves), (dy + (y / (float)height) * Octaves));

    }

    float BiomeTwo(int x, int y)
    {
        return Mathf.PerlinNoise((dx * 2 + (x / (float)width) * Octaves * 2), (dy * 2 + (y / (float)height) * Octaves * 2));

    }



}