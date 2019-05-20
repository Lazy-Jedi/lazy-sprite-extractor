/*
 *
 * Created By: Uee
 * Modified By: 
 *
 * Last Modified: 19 May 2019
 *
 */

using System.IO;
using SpriteExtractor.Extensions;
using SpriteExtractor.Utilities;
using UnityEditor;
using UnityEngine;

namespace SpriteExtractor.Editor
{
	public class SpriteExtractorWindow : EditorWindow
	{
		#region EDITOR WINDOW

		private static SpriteExtractorWindow _window;

		[MenuItem("Window/Sprite Extractor")]
		public static void CreateWindow()
		{
			_window = GetWindow<SpriteExtractorWindow>("Sprite Extractor Tool");
			_window.Show();
		}

		#endregion


		#region VARIABLES

		/// <summary>
		/// Field Colors
		/// </summary>
		private readonly Color _colorFields = new Color(0.54f, 0.44f, 1f);

		/// <summary>
		/// Window Color.
		/// </summary>
		private readonly Color _colorWindow = new Color(0.35f, 0.64f, 1f);

		/// <summary>
		/// Sprite Extractor
		/// </summary>
		private readonly Extractor _extractor = new Extractor();

		#endregion


		#region PROPERTIES

		/// <summary>
		/// Selected Sprite Sheet with Sub Sprites to Extract.
		/// </summary>
		private Texture2D SpriteSheet
		{
			get => _extractor?.SpriteSheet;
			set => _extractor.SpriteSheet = value;
		}

		/// <summary>
		/// Sprite Sheet Location Path.
		/// </summary>
		private string SpriteSheetPath
		{
			get => _extractor.SpriteSheetPath;
			set
			{
				_extractor.SpriteSheetPath = value;
				Extension                  = Path.GetExtension(SpriteSheetPath);
			}
		}

		/// <summary>
		/// Save Location of the Sub Sprites.
		/// </summary>
		private string SavePath
		{
			get => _extractor.SavePath;
			set => _extractor.SavePath = value;
		}

		/// <summary>
		/// Sprite Sheet Texture Extension.
		/// </summary>
		private string Extension
		{
			get => _extractor.Extension;
			set => _extractor.Extension = value;
		}

		/// <summary>
		/// Editor Window Width
		/// </summary>
		private float Width => position.width;


		/// <summary>
		/// Editor Window Height
		/// </summary>
		private float Height => position.height;

		#endregion


		#region GUI METHOD

		private void OnGUI()
		{
			GUI.backgroundColor = _colorWindow;
			EditorGUILayout.BeginVertical(EditorStyles.helpBox);

			// ToDo : Settings
			Settings();
			// ToDo : Display Sprite Sheet
			DisplayTexture2D();
			// ToDo : Extract Sprites Button
			ExtractButton();

			EditorGUILayout.EndVertical();
		}

		#endregion


		#region GUI METHODS

		/// <summary>
		/// All the Required Setting Fields, Labels and Buttons.
		/// </summary>
		private void Settings()
		{
			GUI.backgroundColor = _colorFields;
			SpriteSheetField();
			LabelFields();
			PathLocationButtons();
		}


		/// <summary>
		/// Sprite Sheet Field.
		/// </summary>
		private void SpriteSheetField()
		{
			EditorGUI.BeginChangeCheck();

			SpriteSheet = EditorGUILayout.ObjectField(
				new GUIContent("Sprite Sheet", SpriteSheet, "Sprite Sheet"), SpriteSheet, typeof(Texture2D)
				, false) as Texture2D;

			if (EditorGUI.EndChangeCheck())
			{
				if (SpriteSheet != null && !SpriteSheet.isReadable)
				{
					SpriteSheet.MakeReadable(true);
					ResetLabels();
				}
				else if (SpriteSheet != null)
				{
					ResetLabels();
				}
				else
				{
					SpriteSheetPath = SavePath = "No Sprite Sheet Selected!";
				}
			}
		}

		/// <summary>
		/// Sprite Sheet and Save Path Location Labels.
		/// </summary>
		private void LabelFields()
		{
			EditorGUILayout.LabelField(new GUIContent("Sprite Sheet Location: ", SpriteSheet)
				, new GUIContent(SpriteSheetPath), EditorStyles.textField);


			EditorGUILayout.LabelField(new GUIContent("Save Location", "Save location of the Extracted Sprites.")
				, new GUIContent(SavePath), EditorStyles.textField);
		}


		/// <summary>
		/// Path Location Buttons.
		/// </summary>
		private void PathLocationButtons()
		{
			EditorGUILayout.BeginHorizontal();

			if (GUILayout.Button(new GUIContent("New Save Location", "Choose a Different Save Location.")
				, EditorStyles.miniButtonLeft))
			{
				SavePath = EditorUtility.SaveFolderPanel("Save Location", "Assets/", "");
			}

			if (GUILayout.Button(new GUIContent("Reset Save Location"
					, "Reset the Save Location of the Sprites to the Default Sprite Sheet Path Location.")
				, EditorStyles.miniButtonRight))
			{
				SavePath = SpriteSheet != null
					? SpriteSheetPath.Replace(SpriteSheet.name + Extension, "")
					: "No Sprite Sheet Selected!";
			}

			EditorGUILayout.EndHorizontal();
		}


		/// <summary>
		/// Display the Sprite Sheet.
		/// </summary>
		private void DisplayTexture2D()
		{
			// ToDo : Display Sprite Sheet Correctly.
			if (SpriteSheet != null)
			{
				GUI.DrawTexture(new Rect(32, 196, Width - 64, Height - 264), SpriteSheet, ScaleMode.ScaleToFit);
			}
		}

		/// <summary>
		///  Extract Sub Sprites Button.
		/// </summary>
		private void ExtractButton()
		{
			if (GUILayout.Button(
				new GUIContent("Extract Sprites?", "Extract all the Sub Sprites from the Sprite Sheet.")
				, EditorStyles.miniButton))
			{
				if (SpriteSheet != null)
					_extractor.ExtractSprites();
			}
		}

		#endregion


		#region HELPER METHODS

		/// <summary>
		/// Reset Labels.
		/// </summary>
		private void ResetLabels()
		{
			SpriteSheetPath =
				SpriteSheet != null ? AssetDatabase.GetAssetPath(SpriteSheet) : "No Sprite Sheet Selected";
			SavePath = SpriteSheetPath.Replace(SpriteSheet.name + Extension, "");
		}

		#endregion
	}
}