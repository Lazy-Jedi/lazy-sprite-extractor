using UnityEngine;

public static class Texture2DExtensions
{
    #region FIELDS

    private const string PNG = ".png";
    private const string JPG = ".jpg";
    private const string TGA = ".tga";
    private const string EXR = ".exr";

    #endregion

    #region EXTENSION METHODS

    /// <summary>
    /// Cutout a Texture from Sprite Bounds.
    /// </summary>
    /// <param name="texture2D"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <returns></returns>
    public static Texture2D GetTextureFromBounds(this Texture2D texture2D, int x, int y, int width, int height)
    {
        // Checks if the crop data (x, y, width & height) are within the boundaries of the Original Texture.
        if (x < 0)
        {
            width += x;
            x = 0;
        }

        if (y < 0)
        {
            height += y;
            y = 0;
        }

        if (x + width > texture2D.width)
        {
            width = texture2D.width - x;
        }

        if (y + height > texture2D.height)
        {
            height = texture2D.height - y;
        }

        if (width <= 0 || height <= 0)
        {
            return null;
        }

        // Get the Pixel Data of the Specified Region within the Texture.
        return CreateTexture2DFromPixels(width, height, texture2D.GetPixels(x, y, width, height));
    }

    /// <summary>
    /// Create a new Texture from Array of Colours
    /// </summary>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <param name="pixelData"></param>
    /// <returns></returns>
    private static Texture2D CreateTexture2DFromPixels(int width, int height, Color[] pixelData)
    {
        // Make a new Texture2D
        Texture2D newTexture2D = new Texture2D(width, height, TextureFormat.RGBA32, false);

        // Overwrite the old Pixel Data with the New Extracted Pixel Data.
        newTexture2D.SetPixels(pixelData);

        // Save the Changes to the new Texture.
        newTexture2D.Apply();

        // Return the Cropped out Texture.
        return newTexture2D;
    }

    /// <summary>
    /// Encode Texture2D to Specified Format.
    /// </summary>
    /// <param name="texture2D">Source Texture2D</param>
    /// <param name="format">Format of the Source Texture</param>
    public static byte[] EncodeTexture2D(this Texture2D texture2D, string format)
    {
        format = format.ToLower();
        return format switch
        {
            PNG => texture2D.EncodeToPNG(),
            JPG => texture2D.EncodeToJPG(),
            TGA => texture2D.EncodeToTGA(),
            EXR => texture2D.EncodeToEXR(),
            _   => texture2D.EncodeToPNG()
        };
    }

    #endregion
}