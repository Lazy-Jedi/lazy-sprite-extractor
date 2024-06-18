#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEditor.U2D.Sprites;

namespace Uee.SpriteExtractor
{
    public static class SpriteExtractor
    {
        #region METHODS


        [MenuItem("Assets/Extract/Sprites/Extract Selected Sprites", priority = 10)]
        public static void ExtractSelectedSprites()
        {
            Object[] selectedObjects = Selection.objects;
            if (selectedObjects.Length == 0)
            {
                return;
            }

            TextureImporter textureImporter = null;

            foreach (Object selectedObject in selectedObjects)
            {
                string assetPath = AssetDatabase.GetAssetPath(selectedObject);
                string path = Path.GetDirectoryName(assetPath);
                string format = Path.GetExtension(assetPath);
                textureImporter = AssetImporter.GetAtPath(assetPath) as TextureImporter;

                if (!textureImporter!.isReadable)
                {
                    textureImporter.isReadable = true;
                    textureImporter.SaveAndReimport();
                }

                if (selectedObject is not Sprite sprite)
                {
                    continue;
                }

                Texture2D newTexture = sprite.texture.GetTextureFromBounds(
                    (int)sprite.rect.x,
                    (int)sprite.rect.y,
                    (int)sprite.rect.width,
                    (int)sprite.rect.height);
                newTexture.filterMode = textureImporter.filterMode;
                newTexture.Apply();
                byte[] pngTexture = newTexture.EncodeTexture2D(format);
                File.WriteAllBytes($"{Path.Combine(path!, sprite.name)}{format}", pngTexture);
            }

            if (textureImporter)
            {
                textureImporter.isReadable = false;
                textureImporter.SaveAndReimport();
            }

            AssetDatabase.Refresh();
        }

        [MenuItem("Assets/Extract/Sprites/Extract All", priority = 10)]
        public static void ExtractAllFromSelectedSpritesheets()
        {
            Object[] selectedObjects = Selection.objects;
            if (selectedObjects.Length == 0)
            {
                return;
            }

            foreach (Object selectedObject in selectedObjects)
            {
                ExtractAllSpritesHelper(selectedObject);
            }
        }

        #endregion

        #region HELPER METHODS

        private static void ExtractAllSpritesHelper(Object sourceObject)
        {
            if (sourceObject is not Texture2D texture2D)
            {
                return;
            }

            string assetPath = AssetDatabase.GetAssetPath(sourceObject);
            string path = Path.GetDirectoryName(assetPath);
            string format = Path.GetExtension(assetPath);

            ISpriteEditorDataProvider spriteDataProvider = GetSpriteDateProvider(sourceObject);
            SpriteRect[] spriteRects = spriteDataProvider.GetSpriteRects();
            TextureImporter textureImporter = AssetImporter.GetAtPath(assetPath) as TextureImporter;

            if (!textureImporter!.isReadable)
            {
                textureImporter.isReadable = true;
                textureImporter.SaveAndReimport();
            }

            foreach (SpriteRect spriteRect in spriteRects)
            {
                Texture2D newTexture = texture2D.GetTextureFromBounds(
                    (int)spriteRect.rect.x,
                    (int)spriteRect.rect.y,
                    (int)spriteRect.rect.width,
                    (int)spriteRect.rect.height);
                newTexture.filterMode = textureImporter.filterMode;
                newTexture.Apply();
                byte[] pngTexture = newTexture.EncodeTexture2D(format);
                File.WriteAllBytes($"{Path.Combine(path!, spriteRect.name)}{format}", pngTexture);
            }

            textureImporter.isReadable = false;
            textureImporter.SaveAndReimport();
            AssetDatabase.Refresh();
        }

        private static ISpriteEditorDataProvider GetSpriteDateProvider(Object selectedObject)
        {
            SpriteDataProviderFactories factory = new SpriteDataProviderFactories();
            factory.Init();
            ISpriteEditorDataProvider spriteDataProvider =
                factory.GetSpriteEditorDataProviderFromObject(selectedObject);
            spriteDataProvider.InitSpriteEditorDataProvider();
            return spriteDataProvider;
        }

        #endregion
    }
}
#endif
