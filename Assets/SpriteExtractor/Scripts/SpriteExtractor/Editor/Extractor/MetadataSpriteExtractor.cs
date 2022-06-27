#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Uee.SpriteExtractor
{
    public static partial class SpriteExtractor
    {
        #region SIMPLE SPRITE EXTRACTION METHODS

        public static void ExtractSubSprites(Object meta, string savePath = "")
        {
            string name = meta.name;
            string path = AssetDatabase.GetAssetPath(meta);
            if (string.IsNullOrEmpty(savePath))
            {
                savePath = path.Replace(Path.GetFileName(path), "");
            }

            Texture2D sourceTexture2D = GetSourceTexture2D(path);

            TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;
            MakeReadable(textureImporter, path);
            foreach (SpriteMetaData metadata in textureImporter.spritesheet)
            {
                if (string.CompareOrdinal(metadata.name, name) == 0)
                {
                    Texture2D extracted =
                        sourceTexture2D.GetTextureFromBounds(
                            (int)metadata.rect.x,
                            (int)metadata.rect.y,
                            (int)metadata.rect.width,
                            (int)metadata.rect.height
                        );

                    byte[] textureData = extracted.EncodeToPNG();
                    File.WriteAllBytes(Path.Combine(savePath, $"{metadata.name}.png"), textureData);
                }
            }

            _source = null;
            AssetDatabase.Refresh();
        }

        #endregion

        #region EDITOR EXTRACTOR METHODS

        [MenuItem("Assets/Create/2D/Spritesheet/Extract From Metadata/Extract Here", priority = 20)]
        public static void ExtractSubSpriteHere()
        {
            Object[] objects = Selection.objects;
            foreach (Object metaObject in objects)
            {
                ExtractSubSprites(metaObject);
            }
        }

        [MenuItem("Assets/Create/2D/Spritesheet/Extract From Metadata/Extract to Folder", priority = 20)]
        public static void ExtractSubSpriteToFolder()
        {
            Object textureMetadata = Selection.activeObject;
            string path = EditorUtility.SaveFolderPanel("Save Sprite", Application.dataPath, textureMetadata.name);
            if (string.IsNullOrEmpty(path))
            {
                Debug.Log("Invalid Path Selected.");
            }

            ExtractSubSprites(textureMetadata, path);
        }

        #endregion
    }
}
#endif