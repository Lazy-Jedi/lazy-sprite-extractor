#if UNITY_EDITOR
using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;

namespace Uee.SpriteExtractor
{
    public static partial class SpriteExtractor
    {
        #region VARIABLES

        private static string _previousPath = string.Empty;
        private static Texture2D _source = null;

        #endregion

        #region SIMPLE SPRITE EXTRACTION METHODS

        public static void ExtractAllSprites(Texture2D sourceTexture2D, string savePath = "")
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();

            string path = AssetDatabase.GetAssetPath(sourceTexture2D);
            if (string.IsNullOrEmpty(savePath)) savePath = path.Replace(Path.GetFileName(path), "");

            TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;
            MakeReadable(textureImporter, path);

            foreach (SpriteMetaData metaData in textureImporter.spritesheet)
            {
                Texture2D newTexture2D =
                    sourceTexture2D.GetTextureFromBounds(
                        (int)metaData.rect.x,
                        (int)metaData.rect.y,
                        (int)metaData.rect.width,
                        (int)metaData.rect.height
                    );
                byte[] textureData = newTexture2D.EncodeToPNG();
                File.WriteAllBytes(Path.Combine(savePath, $"{metaData.name}.png"), textureData);
            }

            AssetDatabase.Refresh();

            watch.Stop();
            Debug.Log($"Execution Time: {watch.ElapsedMilliseconds}");
        }

        #endregion

        #region EDITOR EXTRACTOR METHODS

        [MenuItem("Assets/Create/2D/Extract From Source/Extract Here", priority = 10)]
        public static void ExtractSpritesHere()
        {
            Object[] objects = Selection.objects;

            foreach (Texture2D texture2D in objects)
            {
                ExtractAllSprites(texture2D);
            }
        }

        [MenuItem("Assets/Create/2D/Extract From Source/Extract to Folder", priority = 10)]
        public static void ExtractSpritesToFolder()
        {
            Texture2D texture2D = Selection.activeObject as Texture2D;
            string path = EditorUtility.SaveFolderPanel("Save Sprite", Application.dataPath, texture2D.name);
            ExtractAllSprites(texture2D, path);
        }

        #endregion

        #region HELPER METHODS

        private static void MakeReadable(TextureImporter textureImporter, string path)
        {
            if (!textureImporter || textureImporter.isReadable) return;
            textureImporter.isReadable = true;
            AssetDatabase.ImportAsset(path);
        }

        private static Texture2D GetSourceTexture2D(string path)
        {
            if (_previousPath == path)
            {
                if (!_source) _source = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
                return _source;
            }

            _previousPath = path;
            _source = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            return _source;
        }

        #endregion
    }
}
#endif