/**
 * Created By: Ubaidullah Effendi-Emjedi
 * LinkedIn : https://www.linkedin.com/in/ubaidullah-effendi-emjedi-202494183/
 */

#if UNITY_EDITOR
using System;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEditor;
using Object = UnityEngine.Object;

// ReSharper disable once CheckNamespace
namespace JellyFish.EditorTools.SpriteExtractor
{
    [Serializable]
    public class SpriteExtractor
    {
        /// <summary>
        /// Extract All Outlined Sprites from the Original Sprite Sheet to the specified location.
        /// </summary>
        /// <param name="spriteSheet"></param>
        /// <param name="saveLocation"></param>
        public static void ExtractSprites(Texture2D spriteSheet, string saveLocation)
        {
            // Ensure that the Sprite Sheet is Readable.
            if (!spriteSheet.isReadable)
            {
                spriteSheet.MakeReadable(true);
            }

            // Get the Sprite Sheet's Path.
            string path = AssetDatabase.GetAssetPath(spriteSheet);

            // Get The meta Data of the Sub Sprite Artwork.
            Sprite[] sprites = AssetDatabase.LoadAllAssetRepresentationsAtPath(path).OfType<Sprite>().ToArray();

            foreach (Sprite sprite in sprites)
            {
                Texture2D extractedTexture2D =
                    sprite.texture.CropTexture2D((int) sprite.textureRect.x, (int) sprite.textureRect.y,
                                                 (int) sprite.textureRect.width, (int) sprite.textureRect.height);

                byte[] data = extractedTexture2D.EncodeToPNG();
                File.WriteAllBytes(Path.Combine(saveLocation, $"{sprite.name}.png"), data);
            }

            AssetDatabase.Refresh();
        }

        [MenuItem("Assets/JellyFish/Extractors/Sprite Extractor/Extract Here", priority = 10)]
        public static void ExtractToCurrentLocation()
        {
            Object[] _objects = Selection.objects;

            foreach (Texture2D spriteSheet in _objects)
            {
                string path = AssetDatabase.GetAssetPath(spriteSheet);
                ExtractSprites(spriteSheet, path.Replace(Path.GetFileName(path), ""));
            }
        }

        [MenuItem("Assets/JellyFish/Extractors/Sprite Extractor/Extract Elsewhere", priority = 10)]
        public static void ExtractToAnotherLocation()
        {
            Texture2D spriteSheet = Selection.activeObject as Texture2D;

            string path = EditorUtility.SaveFolderPanel("Save Sprite", Application.dataPath, spriteSheet.name);
            ExtractSprites(spriteSheet, path);
        }
    }
}
#endif