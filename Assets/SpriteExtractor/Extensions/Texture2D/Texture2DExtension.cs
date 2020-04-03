/**
 * Created By: Ubaidullah Effendi-Emjedi
 * LinkedIn : https://www.linkedin.com/in/ubaidullah-effendi-emjedi-202494183/
 */

using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

#endif

public static class Texture2DExtension
{
	#region VARIABLES

	/// <summary>
	/// Cached Pixel Color Data of Sprite.
	/// </summary>
	private static Color[] _pixelData;

	/// <summary>
	/// Cached Sprite Texture 2D.
	/// </summary>
	private static Texture2D _texture2D;

#if UNITY_EDITOR
	/// <summary>
	/// Cached Texture Importer used to Modify Texture Import Settings.
	/// </summary>
	private static TextureImporter _textureImporter;
#endif

	#endregion


	#region EXTENSION METHODS

	/// <summary>
	/// Crop Textures from Sprite Bounds.
	/// </summary>
	/// <param name="texture2D"></param>
	/// <param name="x"></param>
	/// <param name="y"></param>
	/// <param name="width"></param>
	/// <param name="height"></param>
	/// <returns></returns>
	public static Texture2D CropTexture2D(this Texture2D texture2D, int x, int y, int width, int height)
	{
		/* Checks if the crop data (x, y, width & height) are within the boundaries of the Original Texture. */
		if (x < 0)
		{
			width += x;
			x     =  0;
		}

		if (y < 0)
		{
			height += y;
			y      =  0;
		}

		if (x + width > texture2D.width) width = texture2D.width - x;

		if (y + height > texture2D.height) height = texture2D.height - y;

		if (width <= 0 || height <= 0) return null;

		/* Get the Pixel Data of the Specified Region within the Texture. */
		_pixelData = texture2D.GetPixels(x, y, width, height);

		/* Make a new Texture2D */
		_texture2D = new Texture2D(width, height, TextureFormat.RGBA32, false);

		/* Overwrite the old Pixel Data with the New Extracted Pixel Data. */
		_texture2D.SetPixels(_pixelData);

		/* Save the Changes to the new Texture. */
		_texture2D.Apply();

		/* Return the Cropped out Texture. */
		return _texture2D;
	}

#if UNITY_EDITOR
	/// <summary>
	///     Change the Readable Import Property of the Texture.
	/// </summary>
	/// <param name="source"></param>
	/// <param name="readable"></param>
	public static void MakeReadable(this Texture2D source, bool readable)
	{
		string texturePath = AssetDatabase.GetAssetPath(source);
		_textureImporter = AssetImporter.GetAtPath(texturePath) as TextureImporter;

		if (_textureImporter == null) return;

		_textureImporter.isReadable = readable;
		AssetDatabase.ImportAsset(texturePath);
		AssetDatabase.Refresh();
	}
#endif

	#endregion
}