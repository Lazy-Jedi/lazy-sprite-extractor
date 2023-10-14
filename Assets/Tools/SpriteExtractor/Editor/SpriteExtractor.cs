#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEditor.U2D.Sprites;

namespace Uee.SpriteExtractor
{
    public static class SpriteExtractor
    {
        #region METHODS

        [MenuItem("Assets/Extract/Sprites/Extract All Sprites", priority = 10)]
        public static void ExtractAllSprites()
        {
            Object selectedObject = Selection.activeObject;
            if (selectedObject is not Texture2D texture2D)
            {
                return;
            }

            string assetPath = AssetDatabase.GetAssetPath(selectedObject);
            string path = Path.GetDirectoryName(assetPath);
            string format = Path.GetExtension(assetPath);

            ISpriteEditorDataProvider spriteDataProvider = GetSpriteDateProvider(selectedObject);
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

        [MenuItem("Assets/Extract/Sprites/Extract Selected Sprites", priority = 10)]
        public static void ExtractSprites()
        {
            Object[] selectedObjects = Selection.objects;
            if (selectedObjects.Length == 0)
            {
                return;
            }

            string assetPath = AssetDatabase.GetAssetPath(selectedObjects[0]);
            string path = Path.GetDirectoryName(assetPath);
            string format = Path.GetExtension(assetPath);

            ISpriteEditorDataProvider spriteDataProvider = GetSpriteDateProvider(selectedObjects[0]);
            List<SpriteRect> spriteRects = spriteDataProvider.GetSpriteRects().ToList();
            TextureImporter textureImporter = AssetImporter.GetAtPath(assetPath) as TextureImporter;

            if (!textureImporter!.isReadable)
            {
                textureImporter.isReadable = true;
                textureImporter.SaveAndReimport();
            }

            foreach (Object selectedObject in selectedObjects)
            {
                if (selectedObject is not Sprite sprite)
                {
                    continue;
                }

                SpriteRect spriteRect = spriteRects.Find(spriteRect => string.CompareOrdinal(spriteRect.name, sprite.name) == 0);
                Texture2D newTexture = sprite.texture.GetTextureFromBounds(
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

        #endregion

        #region HELPER METHODS

        private static ISpriteEditorDataProvider GetSpriteDateProvider(Object selectedObject)
        {
            SpriteDataProviderFactories factory = new SpriteDataProviderFactories();
            factory.Init();
            ISpriteEditorDataProvider spriteDataProvider = factory.GetSpriteEditorDataProviderFromObject(selectedObject);
            spriteDataProvider.InitSpriteEditorDataProvider();
            return spriteDataProvider;
        }

        #endregion
    }
}
#endif