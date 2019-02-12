/*
 *
 * Created By: Uee
 * Modified By: 
 *
 * Last Modified: 12 February 2019
 *
 *
 * This software is released under the terms of the
 * GNU license. See https://www.gnu.org/licenses/#GPL
 * for more information.
 *
 *
 * Important
 * Whatever Artwork is used in conjunction with this tool and is extracted using this tool has no 
 * affiliation with this tool or its creator.
 * 
 * The Artwork(s) belong to their respective creators (the individuals that made them) 
 * and the user of this tool should adhere to the respective licenses stipulated by the 
 * owners/creators of the Artwork.
 *
 */

using UnityEditor;
using UnityEngine;

namespace SpriteExtractor.Extensions
{
    public static class Texture2DExtensions
    {
        #region VARIABLES

        private static Color[] _pixelData;
        private static Texture2D _texture2D;
        private static TextureImporter _textureImporter;

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

        /// <summary>
        /// Check the Readable Import Property of the Texture
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool IsReadable(this Texture2D source)
        {
            string texturePath = AssetDatabase.GetAssetPath(source);
            _textureImporter = AssetImporter.GetAtPath(texturePath) as TextureImporter;

            return _textureImporter != null && _textureImporter.isReadable;
        }

        /// <summary>
        /// Change the Readable Import Property of the Texture
        /// </summary>
        /// <param name="source"></param>
        /// <param name="readable"></param>
        public static void MakeReadable(this Texture2D source, bool readable)
        {
            string texturePath = AssetDatabase.GetAssetPath(source);
            _textureImporter = AssetImporter.GetAtPath(texturePath) as TextureImporter;

            if (_textureImporter == null) return;

            _textureImporter.isReadable = readable;
            AssetDatabase.ImportAsset(texturePath);
            AssetDatabase.Refresh();
        }

        #endregion
    }
}