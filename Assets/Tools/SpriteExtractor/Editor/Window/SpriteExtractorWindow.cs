#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

namespace Uee.SpriteExtractor
{
    public class SpriteExtractorWindow : EditorWindow
    {
        #region EDITOR WINDOW

        /// <summary>
        /// Sprite Extractor Window Instance.
        /// </summary>
        public static SpriteExtractorWindow Window;

        [MenuItem("Lazy-Jedi/Tools/Sprite Extractor", priority = 400)]
        public static void CreateWindow()
        {
            Window = GetWindow<SpriteExtractorWindow>("Sprite Extractor");
            Window.Show();
        }

        #endregion

        #region EDITOR VARIABLES

        private GUIContent _spriteContent;
        private GUIStyle _pathStyle;

        #endregion

        #region VARIABLES

        private const float IMAGE_WIDTH = 128f;
        private const float IMAGE_HEIGHT = 128f;

        private Texture2D _spritesheet;

        private string _spritesheetPath = "";
        private string _savePath = "";

        private bool _autoExtractSelectedSprites;

        private Vector2 _delta;

        private float _width = 0f;
        private float _height = 0f;

        private Rect _imageRect = Rect.zero;

        #endregion

        #region UNITY METHODS

        private void OnEnable()
        {
            _spriteContent = new GUIContent("Spritesheet: ", _spritesheet, "Selected Sprite Sheet");
        }

        public void OnGUI()
        {
            Initialization();
            SelectedImageField();
            DragAndDropSpriteSheets();
            AutoExtractOnSelectionField();
            SpriteSheetPathFields();
            FileHandlingButtons();
            ExtractButtons();
            SpriteSheetDisplayField();
        }

        #endregion

        #region METHODS

        /// <summary>
        /// Selected Sprite Sheet Preview
        /// </summary>
        private void SelectedImageField()
        {
            EditorGUI.BeginChangeCheck();

            _spritesheet = EditorGUILayout.ObjectField(_spriteContent, _spritesheet, typeof(Texture2D), false) as Texture2D;

            if (EditorGUI.EndChangeCheck())
            {
                if (_spritesheet)
                {
                    _spritesheetPath = AssetDatabase.GetAssetPath(_spritesheet);
                }
                else
                {
                    _spritesheetPath = "";
                    _imageRect = Rect.zero;
                }
            }
        }

        /// <summary>
        /// Drag and Drop Sprite Sheets into the Window.
        /// </summary>
        private void DragAndDropSpriteSheets()
        {
            if (Event.current.type == EventType.DragUpdated)
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                Event.current.Use();
            }
            else if (Event.current.type == EventType.DragPerform)
            {
                // To consume drag data.
                DragAndDrop.AcceptDrag();

                // Unity Assets including folder.
                if (DragAndDrop.objectReferences.Length > 0)
                {
                    foreach (Object objectReference in DragAndDrop.objectReferences)
                    {
                        _spritesheetPath = AssetDatabase.GetAssetPath(objectReference);
                        _savePath = _spritesheetPath.Replace(Path.GetFileName(_spritesheetPath), "");

                        if (!(objectReference is Texture2D))
                        {
                            Debug.Log($"Not a Texture2D: {objectReference.name}");
                            continue;
                        }

                        _spritesheet = objectReference as Texture2D;
                        Assert.IsNotNull(_spritesheet);

                        if (!_autoExtractSelectedSprites)
                        {
                            break;
                        }

                        SpriteExtractor.ExtractAllSprites(_spritesheet);
                    }
                }
                // Log to make sure we cover all cases.
                else
                {
                    Debug.Log("Out of reach");
                    Debug.Log("Paths:");

                    foreach (string path in DragAndDrop.paths)
                    {
                        Debug.Log("- " + path);
                    }

                    Debug.Log("ObjectReferences:");

                    foreach (Object obj in DragAndDrop.objectReferences)
                    {
                        Debug.Log("- " + obj);
                    }
                }
            }
        }

        /// <summary>
        /// Automatically Extract Selected Sprites GUI
        /// </summary>
        private void AutoExtractOnSelectionField()
        {
            _autoExtractSelectedSprites = EditorGUILayout.ToggleLeft("Auto Extract on Drop?", _autoExtractSelectedSprites);
        }

        /// <summary>
        /// Spritesheet Path and Extracted Sprites Save Path Fields
        /// </summary>
        private void SpriteSheetPathFields()
        {
            using (EditorGUILayout.VerticalScope verticalScope = new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                EditorGUILayout.TextField("Sprite Sheet Path:", _spritesheetPath, _pathStyle);
                EditorGUILayout.TextField("Extracted Sprites Path:", _savePath, _pathStyle);
            }
        }

        /// <summary>
        /// File Path Handling Button
        /// </summary>
        private void FileHandlingButtons()
        {
            using (EditorGUILayout.HorizontalScope horizontalScope = new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Select New Extraction Path", EditorStyles.miniButtonLeft))
                {
                    string path = EditorUtility.SaveFolderPanel("Extracted Sprites Path", Application.dataPath, "");
                    if (string.IsNullOrEmpty(path)) return;
                    _savePath = path;
                }

                if (GUILayout.Button("Reset Extracted Location", EditorStyles.miniButtonRight))
                {
                    _savePath =
                        !string.IsNullOrEmpty(_spritesheetPath)
                            ? _spritesheetPath.Replace(Path.GetFileName(_spritesheetPath), "")
                            : "";
                }
            }
        }

        /// <summary>
        /// Sprite Extraction Button
        /// </summary>
        private void ExtractButtons()
        {
            if (GUILayout.Button("Extract", EditorStyles.miniButton))
            {
                if (_spritesheet) SpriteExtractor.ExtractAllSprites(_spritesheet);
            }
        }

        /// <summary>
        /// Sprite Sheet Display Field.
        /// </summary>
        private void SpriteSheetDisplayField()
        {
            if (!_spritesheet) return;

            if (_imageRect == Rect.zero) _imageRect = new Rect(0, 196, IMAGE_WIDTH, IMAGE_HEIGHT);

            _delta = Event.current.delta;

            if (_imageRect.Contains(Event.current.mousePosition) && Event.current.type == EventType.ScrollWheel)
            {
                _width += _delta.y * -9f;
                _height += _delta.y * -9f;

                if (_width < IMAGE_WIDTH)
                {
                    _width = IMAGE_WIDTH;
                }

                if (_height < IMAGE_HEIGHT)
                {
                    _height = IMAGE_HEIGHT;
                }

                _imageRect.width = _width;
                _imageRect.height = _height;
            }

            if (_imageRect.Contains(Event.current.mousePosition))
            {
                _imageRect.x += _delta.normalized.x;
                _imageRect.y += _delta.normalized.y;
            }

            Repaint();
            GUI.DrawTexture(_imageRect, _spritesheet, ScaleMode.ScaleToFit);
        }

        #endregion

        #region HELPER METHODS

        private void Initialization()
        {
            if (_pathStyle == null)
            {
                _pathStyle = new GUIStyle
                {
                    alignment = TextAnchor.MiddleLeft,
                };
                _pathStyle.normal.textColor = EditorGUIUtility.isProSkin ? Color.white : Color.black;
            }
        }

        #endregion
    }
}
#endif