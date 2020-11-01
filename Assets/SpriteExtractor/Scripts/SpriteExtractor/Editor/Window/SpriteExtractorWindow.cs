/**
 * Created By: Ubaidullah Effendi-Emjedi
 * LinkedIn : https://www.linkedin.com/in/ubaidullah-effendi-emjedi-202494183/
 */

#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

// ReSharper disable once CheckNamespace
namespace JellyFish.Extractors
{
    public class SpriteExtractorWindow : EditorWindow
    {
        #region EDITOR WINDOW

        /// <summary>
        /// Sprite Extractor Window Instance.
        /// </summary>
        public static SpriteExtractorWindow Window;

        [MenuItem("JellyFish/Extractors/Image/Sprite Extractor")]
        public static void CreateWindow()
        {
            Window = GetWindow<SpriteExtractorWindow>("Sprite Extractor Tool");
            Window.Show();
        }

        #endregion

        #region VARIABLES

        private const float IMAGE_WIDTH = 128f;
        private const float IMAGE_HEIGHT = 128f;

        private Texture2D _spritesheet;
        private TextAsset _xmlAsset;

        private string _spritesheetPath = "";
        private string _savePath = "";
        private string _xmlPath = "";

        private bool _autoExtractSelectedSprites;

        private Vector2 _delta;

        private float _width = 0f;
        private float _height = 0f;

        private Rect _imageRect = Rect.zero;

        private GUIContent _spriteContent;

        #endregion

        #region PROPERTIES

        public Texture2D Spritesheet
        {
            get => _spritesheet;
            set => _spritesheet = value;
        }

        #endregion

        #region UNITY METHODS

        private void OnEnable()
        {
            _spriteContent = new GUIContent("Sprite Sheet", _spritesheet, "Selected Sprite Sheet");
        }

        public void OnGUI()
        {
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

            _spritesheet =
                EditorGUILayout.ObjectField(_spriteContent, _spritesheet, typeof(Texture2D), false) as Texture2D;

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
                    Debug.Log("UnityAsset");

                    int length = DragAndDrop.objectReferences.Length;

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
                        _xmlPath = _spritesheetPath.Replace(Path.GetExtension(_spritesheetPath), ".xml");
                        _xmlAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(_xmlPath);

                        if (!_xmlAsset)
                        {
                            _xmlPath = "";
                        }

                        if (!_autoExtractSelectedSprites)
                        {
                            break;
                        }

                        if (_xmlAsset)
                        {
                            SpriteExtractor.ExtractFromXml(_spritesheet, _xmlAsset, _savePath);
                        }
                        else
                        {
                            SpriteExtractor.ExtractAllSprites(_spritesheet);
                        }
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
            _autoExtractSelectedSprites =
                EditorGUILayout.ToggleLeft("Auto Extract on Drop", _autoExtractSelectedSprites);

            EditorGUILayout.Space();
        }

        /// <summary>
        /// Spritesheet Path and Extracted Sprites Save Path Fields
        /// </summary>
        private void SpriteSheetPathFields()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            EditorGUILayout.TextField("Sprite Sheet Path:", _spritesheetPath, new GUIStyle
            {
                alignment = TextAnchor.MiddleLeft,
            });

            EditorGUILayout.TextField("Extracted Sprites Path:", _savePath, new GUIStyle
            {
                alignment = TextAnchor.MiddleLeft
            });

            EditorGUILayout.TextField("XML Path:", _xmlPath, new GUIStyle
            {
                alignment = TextAnchor.MiddleLeft
            });

            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();
        }

        /// <summary>
        /// File Path Handling Button
        /// </summary>
        private void FileHandlingButtons()
        {
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Select New Extraction Path"))
            {
                _savePath = EditorUtility.SaveFolderPanel("Extracted Sprites Path", Application.dataPath, "");
            }

            if (GUILayout.Button("Reset Save Location"))
            {
                if (!string.IsNullOrEmpty(_spritesheetPath))
                {
                    _savePath = _spritesheetPath.Replace(Path.GetFileName(_spritesheetPath), "");
                }
                else
                {
                    _savePath = "";
                }
            }

            if (GUILayout.Button("Find XML"))
            {
                _xmlPath = EditorUtility.OpenFilePanel("Image XML", Application.dataPath, "xml");
                _xmlAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(_xmlPath);
            }

            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// Sprite Extraction Button
        /// </summary>
        private void ExtractButtons()
        {
            if (GUILayout.Button("Extract"))
            {
                if (_spritesheet)
                {
                    SpriteExtractor.ExtractAllSprites(_spritesheet);
                }
            }

            if (GUILayout.Button("Extract From XML"))
            {
                if (_spritesheet && !string.IsNullOrEmpty(_xmlPath))
                {
                    SpriteExtractor.ExtractFromXml(_spritesheet, File.ReadAllText(_xmlPath), _savePath);
                }
            }

            EditorGUILayout.Space();
        }

        /// <summary>
        /// Sprite Sheet Display Field.
        /// </summary>
        private void SpriteSheetDisplayField()
        {
            if (!_spritesheet)
            {
                return;
            }

            if (_imageRect == Rect.zero)
            {
                _imageRect = new Rect(0, 196 + 32, IMAGE_WIDTH, IMAGE_HEIGHT);
            }

            _delta = Event.current.delta;

            if (_imageRect.Contains(Event.current.mousePosition) && Event.current.type == EventType.ScrollWheel)
            {
                _width += _delta.y  * -9f;
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

        public void OpenFromExternalSource(Texture2D source)
        {
            _spritesheet = source;
            _spritesheetPath = AssetDatabase.GetAssetPath(source);
            _savePath = _spritesheetPath.Replace(Path.GetFileName(_spritesheetPath), "");
        }

        #endregion
    }
}
#endif