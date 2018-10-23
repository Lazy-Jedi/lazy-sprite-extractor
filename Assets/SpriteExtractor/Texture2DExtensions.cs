/*
 * Created By: Uee
 * Last Modified: 16 October 2018
 *
 *
 * License CC-BY-NC:
 * Licensees may copy, distribute, display, and perform the work and make derivative works and remixes based on it only for non-commercial purposes.
 *
 *
 * Important
 * Whatever Artwork is used in conjunction with this tool and is extracted using this tool has no affiliation with this tool or its creator.
 * The Artwork(s) belong to their respective creators (the individuals that made them) and the user of this tool should adhere to the respective licenses stipulated by the owners/creators of the Artwork.
 *
 *
 * Attribution:
 * Uee
 *
 *
 * For more Information about the licence use the link below:
 * https://en.wikipedia.org/wiki/Creative_Commons_license#Non-commercial_licenses
 * 
 */

using UnityEngine;

static class Texture2DExtensions
{
    #region VARIABLES

    private static Color[]   _pixelData;
    private static Texture2D _texture2D;

    #endregion


    #region EXTENSION METHODS

    public static Texture2D CropTexture2D(this Texture2D texture2D, int x, int y, int width, int height)
    {
        /* Checks if the crop data (x, y, width & height) are within the boundaries of the Original Texture. */
        if (x < 0)
        {
            width += x;
            x     =  0;
        }

        if (y < 0)
        {
            height += y;
            y      =  0;
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

        /* Get the Pixel Data of the Specified Region within the Texture. */
        _pixelData = texture2D.GetPixels(x, y, width, height);

        /* Make a new Texture2D */
        _texture2D = new Texture2D(width, height, TextureFormat.RGBA32, false);

        /* Overwrite the old Pixel Data with the New Extracted Pixel Data. */
        _texture2D.SetPixels(_pixelData);

        /* Save the Changes to the new Texture. */
        _texture2D.Apply();

        /* Return the Cropped out Texture. */
        return _texture2D;
    }

    #endregion
}