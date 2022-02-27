using UnityEngine;
using UI = UnityEngine.UI;
using MediaPipe.Selfie;
using Klak.TestTools;

public sealed class Compositor : MonoBehaviour
{
    enum OutputMode { Source, Mask, StaticBG, DynamicBG }
    [SerializeField] ImageSource source = null;
    [SerializeField] OutputMode outputMode = OutputMode.StaticBG;
    [SerializeField] Texture2D bgImage;
    [SerializeField] UI.RawImage outputUI = null;
    [SerializeField] ResourceSet resources = null;
    [SerializeField] Material[] materials;
    [SerializeField] Shader shader = null;
    [SerializeField] GameObject blockPrefab;

    SegmentationFilter filter;
    RenderTexture composited;
    Material material;
    static int rows = 40;
    static int colomns = 60;
    GameObject[,] myArray = new GameObject[rows, colomns];
    bool enableInteraction = false;

    void Start()
    {
        filter = new SegmentationFilter(resources);
        composited = new RenderTexture(640, 480, 0);
        material = new Material(shader);
        for (int x = 0; x < rows; x++)
        {

            for (int y = 0; y < colomns; y++)
            {
                GameObject block = Instantiate(blockPrefab, Vector3.zero, blockPrefab.transform.rotation) as GameObject;
                block.transform.parent = transform;
                block.transform.localPosition = new Vector3(x, y, 0);
                myArray[x, y] = block;
                myArray[x, y].GetComponent<PixelHandler>().targetPosition = myArray[x, y].transform.position;
            }
        }

    }

    void OnDestroy()
    {
        filter.Dispose();
        Destroy(composited);
        Destroy(material);
    }

    private Texture2D ScaleTexture(Texture2D source, int targetWidth, int targetHeight)
    {
        Texture2D result = new Texture2D(targetWidth, targetHeight, source.format, true);
        Color[] rpixels = result.GetPixels(0);
        float incX = (1.0f / (float)targetWidth);
        float incY = (1.0f / (float)targetHeight);
        for (int px = 0; px < rpixels.Length; px++)
        {
            rpixels[px] = source.GetPixelBilinear(incX * ((float)px % targetWidth), incY * ((float)Mathf.Floor(px / targetWidth)));
        }
        result.SetPixels(rpixels, 0);
        result.Apply();
        return result;
    }

    Texture2D toTexture2D(RenderTexture rTex)
    {
        Texture2D tex = new Texture2D(640, 480, TextureFormat.RGB24, false);
        // ReadPixels looks at the active RenderTexture.
        RenderTexture.active = rTex;
        tex.ReadPixels(new Rect(0, 0, rTex.width, rTex.height), 0, 0);
        return tex;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            enableInteraction = true;
        }


        filter.ProcessImage(source.Texture);
        material.SetTexture("_SourceTexture", source.Texture);
        material.SetTexture("_MaskTexture", filter.MaskTexture);
        material.SetTexture("_BGTexture", bgImage);
        Graphics.Blit(null, composited, material, (int)outputMode);
        Texture2D scaledTexture = ScaleTexture(toTexture2D(composited), rows, colomns);
        outputUI.texture = scaledTexture;

        for (int x = 0; x < rows; x++)
        {
            for (int y = 0; y < colomns; y++)
            {
                if (enableInteraction)
                {
                    myArray[x, y].GetComponent<PixelHandler>().enablePhysics = true;
                }

                if (!(scaledTexture.GetPixel(x, y).r == 1 && scaledTexture.GetPixel(x, y).g == 1 && scaledTexture.GetPixel(x, y).b == 1))
                {
                    myArray[x, y].GetComponent<Renderer>().material = ConfigurationsManager.Instance.SetFilter(materials);
                    myArray[x, y].GetComponent<Renderer>().material.color = ConfigurationsManager.Instance.GetPixelColor(new Color(scaledTexture.GetPixel(x, y).r, scaledTexture.GetPixel(x, y).g, scaledTexture.GetPixel(x, y).b));
                }
                else
                    myArray[x, y].GetComponent<Renderer>().material.color = ConfigurationsManager.Instance.GetBackground(x, y);

            }
        }
        enableInteraction = false;
    }
}
