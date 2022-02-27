using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum Background { Black, White, Random, RandomStatic };
public enum Quantization { Full, Twelve, Nine, Six };
public enum Filter { Normal, Blue, Negative };
public enum Force { Fall, FallBack, FallFront, RandomScatter };


public class ConfigurationsManager : MonoBehaviour
{

    private static ConfigurationsManager instance;

    public static ConfigurationsManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.FindObjectOfType<ConfigurationsManager>();
            }
            return instance;
        }
    }
    [SerializeField]
    Background background;
    [SerializeField]
    Quantization quantization;
    [SerializeField]
    Filter filter;
    public Force force;

    Color[,] randomPixels = new Color[40, 60];


    void Start()
    {

        for (int x = 0; x < 40; x++)
        {

            for (int y = 0; y < 60; y++)
            {
                randomPixels[x, y] = new Color((Random.Range(0f, 1f) > 0.5f ? 0 : 1), (Random.Range(0f, 1f) > 0.5f ? 0 : 1f), (int)(Random.Range(0f, 1f) > 0.5f ? 0 : 1));

            }
        }
    }
    public Color GetBackground(int x, int y)
    {
        // 

        if (background == Background.White)
        {
            return new Color(1, 1, 1);
        }
        else if (background == Background.Random)
        {
            return new Color((Random.Range(0f, 1f) > 0.5f ? 0 : 1), (Random.Range(0f, 1f) > 0.5f ? 0 : 1f), (int)(Random.Range(0f, 1f) > 0.5f ? 0 : 1));
        }
        else if (background == Background.RandomStatic)
        {
            return randomPixels[x, y];
        }
        else
        {
            return new Color(0, 0, 0);

        }
    }

    public Color GetPixelColor(Color pixelColor)
    {
        if (quantization == Quantization.Full)
            return pixelColor;
        float factor = 0;

        if (quantization == Quantization.Six)
            factor = 1 / 2f;
        else if (quantization == Quantization.Nine)
            factor = 1 / 3f;
        else if (quantization == Quantization.Twelve)
            factor = 1 / 4f;


        float r = 0, g = 0, b = 0;

        for (float i = 0; i <= 1; i += factor)
        {
            if (pixelColor.r <= i)
            {
                r = i;
                break;

            }
        }
        for (float i = 0; i <= 1; i += factor)
        {
            if (pixelColor.g <= i)
            {
                g = i;
                break;

            }
        }
        for (float i = 0; i <= 1; i += factor)
        {
            if (pixelColor.b <= i)
            {
                b = i;
                break;
            }
        }

        return new Color(r, g, b);

    }
    public Material SetFilter(Material[] mats)
    {
        Material mat = null;
        if (filter == Filter.Blue)
        {
            mats[1].SetColor("_EmissionColor", new Color(0, 0, 1));
            mat = mats[1];

        }
        else if (filter == Filter.Negative)
        {
            mats[2].SetFloat("_Threshold", 0.1f);
            mat = mats[2];

        }
        else
        {
            mat = mats[0];
        }

        return mat;

    }
    // Update is called once per frame
    void Update()
    {

    }
}
