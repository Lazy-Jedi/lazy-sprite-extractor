﻿#if UNITY_EDITOR
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using UnityEditor;
using UnityEngine;

namespace Uee.SpriteExtractor
{
    public static class GIFExtractor
    {
        /// <summary>
        /// Extract the Frames of the GIF at its Current Location.
        /// </summary>
        [MenuItem("Assets/Create/2D/Extract GIF Frames/Extract Here", priority = 10)]
        public static void ExtractFramesHere()
        {
            Object[] _objects = Selection.objects;

            foreach (Object asset in _objects)
            {
                string path = AssetDatabase.GetAssetPath(asset);
                string folder = Path.GetDirectoryName(path);

                Image gif = Image.FromFile(path);
                FrameDimension frameDimension = new FrameDimension(gif.FrameDimensionsList[0]);

                int frameCount = gif.GetFrameCount(frameDimension);

                for (int index = 0; index < frameCount; index++)
                {
                    gif.SelectActiveFrame(frameDimension, index);

                    // Create a new Frame from the Original Source.
                    Image frame = (Image)gif.Clone();
                    string filename = $"{Path.GetFileNameWithoutExtension(path)}_{index}.png";
                    frame.Save(Path.Combine(folder, filename), ImageFormat.Png);
                }
            }

            AssetDatabase.Refresh();
        }

        /// <summary>
        /// Extract GIF Frames to a Folder in/outside of the Current Unity Project.
        /// </summary>
        [MenuItem("Assets/Create/2D/Extract GIF Frames/Extract to Folder", priority = 10)]
        public static void ExtractFramesElseWhere()
        {
            Object selectedGif = Selection.activeObject;

            if (!selectedGif) return;

            string assetPath = AssetDatabase.GetAssetPath(selectedGif);

            string extractionPath =
                EditorUtility.SaveFilePanel("Save GIF Frames", Application.dataPath, selectedGif.name, "gif");

            if (string.IsNullOrEmpty(extractionPath))
            {
                Debug.Log("No Extraction Path Selected");
                return;
            }

            string folder = Path.GetDirectoryName(extractionPath);

            Image gif = Image.FromFile(assetPath);
            FrameDimension frameDimension = new FrameDimension(gif.FrameDimensionsList[0]);

            int frameCount = gif.GetFrameCount(frameDimension);

            for (int index = 0; index < frameCount; index++)
            {
                gif.SelectActiveFrame(frameDimension, index);

                // Create a new Frame from the Original Source.
                Image frame = (Image)gif.Clone();
                string filename = $"{Path.GetFileNameWithoutExtension(assetPath)}_{index}.png";
                frame.Save(Path.Combine(folder, filename), ImageFormat.Png);
            }

            AssetDatabase.Refresh();
        }
    }
}
#endif