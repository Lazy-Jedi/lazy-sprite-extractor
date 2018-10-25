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

using System.IO;
using SpriteExtractor.Extensions;
using UnityEditor;
using UnityEngine;

namespace SpriteExtractor.Editor
{
    public class SEWindow : EditorWindow
    {
        #region WINDOW METHOD

        public static SEWindow window;

        [MenuItem("Window/Sprite Extractor")]
        public static void CreateWindow()
        {
            window = GetWindow<SEWindow>(false, "Sprite Extractor");
            window.Focus();
            window.Show();
            window.minSize = new Vector2(800, 600); // Change Window Min Size
        }

        #endregion


        #region VARIABLES

        private SpriteExtractor _spriteExtractor = new SpriteExtractor(); // Sprite Exporter
        private GUIContent _label = new GUIContent("", "");               // Custom Label
        private Vector2 _labelDimensions = Vector2.zero;                  // Positioning of Labels
        private string _path = "";                                        // Custom Save Location

        #endregion


        #region GUI

        private void OnGUI()
        {
            /* SETTINGS */
            Settings();

            /* Export Sprites */
            ExportSprites();

            /* Display Selected Texture */
            DisplaySelectedTexture();
        }

        #endregion


        #region WINDOW GUI METHODS

        /// <summary>
        /// Contains code for the basic settings such as: Selecting a Texture and Displaying Paths of the Texture and
        /// Where to the Extracted sprites will be Saved.
        /// </summary>
        private void Settings()
        {
            /* LABEL */
            SetLabel("Settings");
            EditorGUI.LabelField(new Rect(Width / 2 - _labelDimensions.x / 2, 4, _labelDimensions.x + 16, 16), _label,
                EditorStyles.boldLabel);

            /* Begin Area */
            GUILayout.BeginArea(new Rect(0, 24, Width, 128), EditorStyles.helpBox);


            /* Select Texture Field */
            SelectTextureField();

            /* Sprite Sheet Path and Extracted Sprites Save Path Fields */
            PathFields();


            /* End Area */
            GUILayout.EndArea();
        }


        /// <summary>
        /// Settings Option for Selecting Textures.
        /// </summary>
        private void SelectTextureField()
        {
            /* TEXTURE */
            SetLabel("Sprite Sheet", "Select a Sprite Sheet with Sub Sprites");
            EditorGUI.LabelField(new Rect(8, 20, _labelDimensions.x + 16, 16), _label);

            EditorGUI.BeginChangeCheck();

            SpriteSheet =
                (Texture2D) EditorGUI.ObjectField(new Rect(14, 40, 64, 64), SpriteSheet, typeof(Texture2D), false);

            // Set Read/Write Mode = True 
            if (EditorGUI.EndChangeCheck())
            {
                if (SpriteSheet && !SpriteSheet.IsReadable())
                {
                    SpriteSheet.MakeReadable(true);
                }
            }
        }


        /// <summary>
        /// Settings Option that Displays Where the Selected Texture is Located and Where the Extracted Sprites will be
        /// Saved.
        /// </summary>
        private void PathFields()
        {
            /* Set the Path String. */
            SpriteSheetPath = SpriteSheet != null ? AssetDatabase.GetAssetPath(SpriteSheet) : "";

            /* Sprite Sheet Path Label */
            SetLabel("Sprite Sheet Path");
            EditorGUI.LabelField(new Rect(Width / 2 - _labelDimensions.x / 2, 10, _labelDimensions.x + 16, 16), _label);

            SetLabel(SpriteSheetPath != "" ? SpriteSheetPath : "No Sprite Sheet Selected.",
                "This is the Path Location of the Sprite Sheet.");
            EditorGUI.LabelField(new Rect(Width / 2 - _labelDimensions.x / 2, 30, _labelDimensions.x, 16),
                _label, EditorStyles.textField);


            /* Save Path Label */
            SetLabel("Extracted Sprites Save Location", "Path to a Folder to Save the Extracted Sprites.");
            EditorGUI.LabelField(new Rect(Width / 2 - _labelDimensions.x / 2 - 8, 55, _labelDimensions.x + 24, 16),
                _label);


            SetLabel(SavePath, "Path to a Folder to Save the Extracted Sprites.");
            EditorGUI.LabelField(new Rect(Width / 2 - _labelDimensions.x / 2 - 16, 75, _labelDimensions.x + 48, 16),
                _label, EditorStyles.textField);


            SetLabel("Custom Save Path", "Choose a Custom Folder.");
            /* Button -> Choose Custom Save Path */
            if (GUI.Button(new Rect(Width / 2 - _labelDimensions.x / 2 - 100, 100, _labelDimensions.x + 48, 16), _label,
                EditorStyles.miniButton))
            {
                _path = EditorUtility.SaveFolderPanel("Save Extracted Sprites to Folder", "", "");
            }


            SetLabel("Reset Save Path", "");
            /* Button -> Reset Save Path */
            if (GUI.Button(new Rect(Width / 2 - _labelDimensions.x / 2 + 80, 100, _labelDimensions.x + 48, 16), _label,
                EditorStyles.miniButton))
            {
                _path = "";
            }


            /* Save Path -> Default String */
            if (SpriteSheet == null && _path == "")
                SavePath = "Sprite Sheet not Selected";


            /* When GUI Changed Check */
            if (GUI.changed)
            {
                /* One Liner Ternary Operation */
                SavePath = SpriteSheet != null && _path == ""
                    ? SpriteSheetPath.Replace(SpriteSheet.name + Extension, "")
                    : SpriteSheet != null && _path != ""
                        ? _path
                        : "Sprite Sheet not Selected";
            }
        }


        /// <summary>
        /// Displays a Larger Scaled Image of the Selected Texture.
        /// </summary>
        private void DisplaySelectedTexture()
        {
            /* LABEL */
            SetLabel("Selected Texture");
            EditorGUI.LabelField(new Rect(Width / 2 - _labelDimensions.x / 2, 204, _labelDimensions.x, 16),
                _label, EditorStyles.boldLabel);

            /* Begin Area */
            GUILayout.BeginArea(new Rect(16, 224, Width - 32, Height - 256), EditorStyles.helpBox);

            /* Safty Check */
            if (SpriteSheet != null)
                /* Draw in the Window a Scaled Version of the Selected Texture. */
                GUI.DrawTexture(new Rect(16, 4, Width - 64, Height - 264), SpriteSheet, ScaleMode.ScaleToFit);

            /* End Area */
            GUILayout.EndArea();
        }


        /// <summary>
        /// GUI Button that Calls the Extract Sprites Function from the Sprite Exporter.
        /// </summary>
        private void ExportSprites()
        {
            /* Set Button Label */
            SetLabel("Extract and Save Sprites");
            GUIStyle buttonStyle = new GUIStyle(EditorStyles.miniButton);
            buttonStyle.wordWrap = true;

            if (GUI.Button(new Rect(Width / 2 - _labelDimensions.x / 2 + 36, 160, 96, 32), _label, buttonStyle))
            {
                if (SpriteSheet != null)
                    _spriteExtractor.ExtractSprites();
            }
        }

        #endregion


        #region HELPER METHODS

        /// <summary>
        /// Editor Window Width
        /// </summary>
        private float Width
        {
            get { return position.width; }
        }


        /// <summary>
        /// Editor Window Height
        /// </summary>
        private float Height
        {
            get { return position.height; }
        }

        /// <summary>
        /// Modify the Label Text and Tooltip
        /// </summary>
        /// <param name="text"></param>
        /// <param name="tip"></param>
        private void SetLabel(string text, string tip)
        {
            _label.text      = text;
            _label.tooltip   = tip;
            _labelDimensions = EditorStyles.boldLabel.CalcSize(_label);
        }


        /// <summary>
        /// Simple Overload Set Label Method -> Used the SetLabel(text, tip) Function
        /// </summary>
        /// <param name="text"></param>
        private void SetLabel(string text)
        {
            SetLabel(text, "");
        }


        /// <summary>
        /// Sprite Sheet Property
        /// </summary>
        private Texture2D SpriteSheet
        {
            set { _spriteExtractor.SpriteSheet = value; }
            get { return _spriteExtractor.SpriteSheet; }
        }


        /// <summary>
        /// Sprite Sheet Path Property
        /// </summary>
        private string SpriteSheetPath
        {
            set
            {
                _spriteExtractor.SpriteSheetPath = value;

                if (_spriteExtractor.SpriteSheetPath != "")
                {
                    Extension = Path.GetExtension(AssetDatabase.GetAssetPath(SpriteSheet));
                }
            }
            get { return _spriteExtractor.SpriteSheetPath; }
        }


        /// <summary>
        /// Save Path Property
        /// </summary>
        private string SavePath
        {
            set { _spriteExtractor.SavePath = value; }
            get { return _spriteExtractor.SavePath; }
        }


        /// <summary>
        /// Extension of the Sprite Sheet.
        /// </summary>
        private string Extension
        {
            set { _spriteExtractor.Extension = value; }
            get { return _spriteExtractor.Extension; }
        }

        #endregion
    }
}