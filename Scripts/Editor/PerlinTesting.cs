using UnityEngine;
using System.Collections;
using UnityEditor;

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

        UpdatePerlin();
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

    float ThreeDNoisePerlin(int x, int y){
        return SimplexNoise.Noise( dx + ( x / (float) width) * Octaves, (y / (float) height * Octaves ), dy / 100f * Octaves);
    }



}