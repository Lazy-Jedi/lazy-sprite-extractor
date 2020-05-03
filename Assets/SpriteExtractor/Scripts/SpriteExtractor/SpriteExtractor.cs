/**
 * Created By: Ubaidullah Effendi-Emjedi
 * LinkedIn : https://www.linkedin.com/in/ubaidullah-effendi-emjedi-202494183/
 */

#if UNITY_EDITOR
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using JellyFish.Editor.Extensions;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;


// ReSharper disable once CheckNamespace
namespace JellyFish.Editor.Tools.Extractors
{
    [Serializable]
    public class SpriteExtractor
    {
        #region VARIABLES

        private static string    _previousPath = "";
        private static Texture2D _source       = null;

        #endregion

        #region METHODS

        /// <summary>
        /// Extract All Outlined Sprites from the Original Sprite Sheet to the specified location.
        /// </summary>
        /// <param name="spriteSheet"></param>
        /// <param name="saveLocation"></param>
        public static void ExtractSprites_Old(Texture2D spriteSheet, string savePath = "")
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            if (!spriteSheet.isReadable)
            {
                spriteSheet.MakeReadable(true);
            }

            string path = AssetDatabase.GetAssetPath(spriteSheet);

            if (string.IsNullOrEmpty(savePath))
                savePath = path.Replace(Path.GetFileName(path), "");

            Sprite[] sprites = AssetDatabase.LoadAllAssetRepresentationsAtPath(path).OfType<Sprite>().ToArray();

            foreach (Sprite sprite in sprites)
            {
                Texture2D extractedTexture2D =
                    sprite.texture.CropTexture2D((int) sprite.textureRect.x, (int) sprite.textureRect.y,
                                                 (int) sprite.textureRect.width, (int) sprite.textureRect.height);

                byte[] data = extractedTexture2D.EncodeToPNG();
                File.WriteAllBytes(Path.Combine(savePath, $"{sprite.name}.png"), data);
            }

            AssetDatabase.Refresh();
            watch.Stop();
            Debug.Log($"Execution Time: {watch.ElapsedMilliseconds}");
        }

        /// <summary>
        /// Extract All Outlined Sprites from the Original Sprite Sheet to the specified location.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="savePath"></param>
        public static void ExtractAllSprites(Texture2D source, string savePath = "")
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();

            string path = AssetDatabase.GetAssetPath(source);
            if (string.IsNullOrEmpty(savePath))
                savePath = path.Replace(Path.GetFileName(path), "");

            TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;

            if (textureImporter != null)
            {
                MakeReadable(textureImporter, path);
                foreach (SpriteMetaData metaData in textureImporter.spritesheet)
                {
                    Texture2D extracted =
                        source.CropTexture2D((int) metaData.rect.x, (int) metaData.rect.y,
                                             (int) metaData.rect.width, (int) metaData.rect.height);

                    byte[] data = extracted.EncodeToPNG();
                    File.WriteAllBytes(Path.Combine(savePath, $"{metaData.name}.png"), data);
                }
            }

            AssetDatabase.Refresh();

            watch.Stop();
            Debug.Log($"Execution Time: {watch.ElapsedMilliseconds}");
        }

        public static void ExtractSubSprites(Object meta, string savePath = "")
        {
            string name = meta.name;
            string path = AssetDatabase.GetAssetPath(meta);
            if (string.IsNullOrEmpty(savePath))
            {
                savePath = path.Replace(Path.GetFileName(path), "");
            }

            Texture2D source = GetSourceTexture2D(path);

            TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;
            if (textureImporter != null)
            {
                MakeReadable(textureImporter, path);

                foreach (SpriteMetaData metaData in textureImporter.spritesheet)
                {
                    if (string.CompareOrdinal(metaData.name, name) == 0)
                    {
                        Texture2D extracted = source.CropTexture2D((int) metaData.rect.x, (int) metaData.rect.y,
                                                                   (int) metaData.rect.width, (int) metaData.rect.height);

                        byte[] data = extracted.EncodeToPNG();
                        File.WriteAllBytes(Path.Combine(savePath, $"{metaData.name}.png"), data);
                    }
                }
            }

            AssetDatabase.Refresh();
        }

        /// <summary>
        /// Make Texture2D Readable
        /// </summary>
        /// <param name="textureImporter"></param>
        /// <param name="path"></param>
        private static void MakeReadable(TextureImporter textureImporter, string path)
        {
            if (textureImporter != null && !textureImporter.isReadable)
            {
                textureImporter.isReadable = true;
                AssetDatabase.ImportAsset(path);
            }
        }

        /// <summary>
        /// Get Source Texture2D From a Path
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private static Texture2D GetSourceTexture2D(string path)
        {
            if (_previousPath != path)
            {
                _previousPath = path;
                _source       = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            }

            return _source;
        }

        [MenuItem("Assets/JellyFish/Sprite Extractor/From Meta/Extract Here", priority = 10)]
        public static void ExtractSubSpriteHere()
        {
            Object[] objects = Selection.objects;
            foreach (Object metaObject in objects)
            {
                ExtractSubSprites(metaObject);
            }
        }

        [MenuItem("Assets/JellyFish/Sprite Extractor/From Meta/Extract Elsewhere", priority = 10)]
        public static void ExtractSubSpriteToAnotherLocation()
        {
            Object meta = Selection.activeObject;
            string path = EditorUtility.SaveFolderPanel("Save Sprite", Application.dataPath, meta.name);
            ExtractSubSprites(meta, path);
        }

        [MenuItem("Assets/JellyFish/Sprite Extractor/From Source/Extract Here", priority = 10)]
        public static void ExtractSpritesHere()
        {
            Object[] objects = Selection.objects;

            foreach (Texture2D texture2D in objects)
            {
                ExtractAllSprites(texture2D);
            }
        }

        [MenuItem("Assets/JellyFish/Sprite Extractor/From Source/Extract Elsewhere", priority = 10)]
        public static void ExtractSpritesToAnotherLocation()
        {
            Texture2D texture2D = Selection.activeObject as Texture2D;

            string path = EditorUtility.SaveFolderPanel("Save Sprite", Application.dataPath, texture2D.name);
            ExtractAllSprites(texture2D, path);
        }
    }

    #endregion
}
#endif