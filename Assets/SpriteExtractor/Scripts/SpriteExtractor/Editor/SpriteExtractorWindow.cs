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
namespace JellyFish.Editor.Tools.Extractors
{
    public class SpriteExtractorWindow : EditorWindow
    {
        #region EDITOR WINDOW

        /// <summary>
        /// Sprite Extractor Window Instance.
        /// </summary>
        private static SpriteExtractorWindow _window;

        [MenuItem("JellyFish/Extractors/Image/Sprite Extractor")]
        public static void CreateWindow()
        {
            _window = GetWindow<SpriteExtractorWindow>("Sprite Extractor Tool");
            _window.Show();
        }

        #endregion

        #region VARIABLES

        /// <summary>
        /// Constant Image Width
        /// </summary>
        private const float IMAGE_WIDTH = 128f;

        /// <summary>
        /// Constant Image Height
        /// </summary>
        private const float IMAGE_HEIGHT = 128f;

        /// <summary>
        /// Selected Sprite Sheet.
        /// </summary>
        private Texture2D _spriteSheet;

        /// <summary>
        /// Selected Sprite Sheet Path
        /// </summary>
        private string _path = "";

        /// <summary>
        /// Save Path
        /// </summary>
        private string _savePath = "";

        /// <summary>
        /// Automatically Extract Selected Sprites
        /// </summary>
        private bool _autoExtractSelectedSprites;

        /// <summary>
        /// Mouse Scroll Wheel Value
        /// </summary>
        private Vector2 _delta;

        /// <summary>
        /// Width
        /// </summary>
        private float _width = 0f;

        /// <summary>
        /// Height
        /// </summary>
        private float _height = 0f;

        /// <summary>
        /// Cached Image Rect
        /// </summary>
        private Rect _imageRect = Rect.zero;

        /// <summary>
        /// Sprite GUIContent
        /// </summary>
        private GUIContent _spriteContent;

        #endregion

        #region UNITY METHODS

        private void OnEnable()
        {
            _spriteContent = new GUIContent("Sprite Sheet", _spriteSheet, "Selected Sprite Sheet");
        }

        public void OnGUI()
        {
            SelectedImageField();
            DragAndDropSpriteSheets();
            AutoExtractOnSelectionField();
            SpriteSheetPathFields();
            FileHandlingButtons();
            ExtractButton();
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

            _spriteSheet =
                EditorGUILayout.ObjectField(_spriteContent, _spriteSheet, typeof(Texture2D), false) as Texture2D;

            if (EditorGUI.EndChangeCheck())
            {
                if (_spriteSheet)
                {
                    _path = AssetDatabase.GetAssetPath(_spriteSheet);
                }
                else
                {
                    _path      = "";
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
                        _path     = AssetDatabase.GetAssetPath(objectReference);
                        _savePath = _path.Replace(Path.GetFileName(_path), "");

                        if (!(objectReference is Texture2D))
                        {
                            Debug.Log($"Not a Texture2D: {objectReference.name}");
                            continue;
                        }

                        _spriteSheet = objectReference as Texture2D;
                        Assert.IsNotNull(_spriteSheet);

                        if (!_autoExtractSelectedSprites)
                        {
                            break;
                        }

                        SpriteExtractor.ExtractAllSprites(_spriteSheet);
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
            _autoExtractSelectedSprites = EditorGUILayout.ToggleLeft("Auto Extract on Drop", _autoExtractSelectedSprites);
        }

        /// <summary>
        /// Spritesheet Path and Extracted Sprites Save Path Fields
        /// </summary>
        private void SpriteSheetPathFields()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            EditorGUILayout.TextField("Sprite Sheet Path:", _path, new GUIStyle
                                                                   {
                                                                       alignment = TextAnchor.MiddleLeft,
                                                                   });

            EditorGUILayout.TextField("Extracted Sprites Path:", _savePath, new GUIStyle
                                                                            {
                                                                                alignment = TextAnchor.MiddleLeft
                                                                            });

            EditorGUILayout.EndVertical();
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
                if (!string.IsNullOrEmpty(_path))
                {
                    _savePath = _path.Replace(Path.GetFileName(_path), "");
                }
                else
                {
                    _savePath = "";
                }
            }

            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// Sprite Extraction Button
        /// </summary>
        private void ExtractButton()
        {
            if (GUILayout.Button("Extract"))
            {
                if (_spriteSheet)
                {
                    SpriteExtractor.ExtractAllSprites(_spriteSheet);
                }
            }
        }

        /// <summary>
        /// Sprite Sheet Display Field.
        /// </summary>
        private void SpriteSheetDisplayField()
        {
            if (!_spriteSheet)
            {
                return;
            }


            if (_imageRect == Rect.zero)
            {
                _imageRect = new Rect(0, 196, IMAGE_WIDTH, IMAGE_HEIGHT);
            }

            _delta = Event.current.delta;

            if (_imageRect.Contains(Event.current.mousePosition) && Event.current.type == EventType.ScrollWheel)
            {
                _width  += _delta.y * -9f;
                _height += _delta.y * -9f;

                if (_width < IMAGE_WIDTH)
                {
                    _width = IMAGE_WIDTH;
                }

                if (_height < IMAGE_HEIGHT)
                {
                    _height = IMAGE_HEIGHT;
                }

                _imageRect.width  = _width;
                _imageRect.height = _height;
            }

            if (_imageRect.Contains(Event.current.mousePosition))
            {
                _imageRect.x += _delta.normalized.x;
                _imageRect.y += _delta.normalized.y;
            }

            Repaint();
            GUI.DrawTexture(_imageRect, _spriteSheet, ScaleMode.ScaleToFit);
        }

        #endregion
    }
}
#endif