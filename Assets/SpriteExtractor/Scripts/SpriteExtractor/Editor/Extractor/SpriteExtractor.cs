/**
 * Created By: Ubaidullah Effendi-Emjedi
 * LinkedIn : https://www.linkedin.com/in/ubaidullah-effendi-emjedi-202494183/
 */

#if UNITY_EDITOR
using System.Diagnostics;
using System.IO;
using JellyFish.Extensions;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;


namespace JellyFish.Extractors
{
    public static partial class SpriteExtractor
    {
        #region VARIABLES

        private static string _previousPath = "";
        private static Texture2D _source = null;
        private static Texture2D _extracted = null;
        private static byte[] _data = null;

        #endregion

        #region SIMPLE SPRITE EXTRACTION METHODS

        public static void ExtractAllSprites(Texture2D source, string savePath = "")
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();

            string path = AssetDatabase.GetAssetPath(source);
            if (string.IsNullOrEmpty(savePath))
                savePath = path.Replace(Path.GetFileName(path), "");

            TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;

            MakeReadable(textureImporter, path);
            foreach (SpriteMetaData metaData in textureImporter.spritesheet)
            {
                _extracted =
                    source.CropTexture2D((int) metaData.rect.x, (int) metaData.rect.y,
                                         (int) metaData.rect.width, (int) metaData.rect.height);

                _data = _extracted.EncodeToPNG();
                File.WriteAllBytes(Path.Combine(savePath, $"{metaData.name}.png"), _data);
            }

            _extracted = null;
            _data = null;
            AssetDatabase.Refresh();

            watch.Stop();
            Debug.Log($"Execution Time: {watch.ElapsedMilliseconds}");
        }

        #endregion

        #region EDITOR EXTRACTOR METHODS

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

        #endregion

        #region HELPER METHODS

        private static void MakeReadable(TextureImporter textureImporter, string path)
        {
            if (textureImporter != null && !textureImporter.isReadable)
            {
                textureImporter.isReadable = true;
                AssetDatabase.ImportAsset(path);
            }
        }

        private static Texture2D GetSourceTexture2D(string path)
        {
            if (_previousPath != path)
            {
                _previousPath = path;
                _source = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            }

            return _source;
        }

        #endregion
    }
}
#endif