
using UnityEditor;
using UnityEngine;
using System.IO;
namespace IACGGames
{
	public class IACGSaveWindow : EditorWindow
	{
		public string Key;
		public string Value;

		public DataType TypeDef = DataType.INTEGER;

		private static IACGSaveWindow MyWindow;
		[MenuItem("IACG/Save/SetPlayerPrefsKey %w")]
		public static void InitPlayerPrefsWindow()
		{
			MyWindow = EditorWindow.GetWindow(typeof(IACGSaveWindow)) as IACGSaveWindow;
		}

		void OnGUI()
		{
			GUILayout.Label("Enter key here", EditorStyles.boldLabel);
			Key = EditorGUILayout.TextField("Key : ", Key);
			GUILayout.Label("Enter value here", EditorStyles.boldLabel);
			Value = EditorGUILayout.TextField("Value : ", Value);
			GUILayout.Label("Select data type here", EditorStyles.boldLabel);
			TypeDef = (DataType)EditorGUILayout.EnumPopup("Type : ", TypeDef);
			if (GUILayout.Button("Save"))
			{
				SetPlayerPref();
			}
		}

		void SetPlayerPref()
		{
			if (string.IsNullOrEmpty(Key))
			{
				EditorUtility.DisplayDialog("Warning!", "Key should not be empty", "Ok");
				return;
			}
			if (TypeDef != DataType.STRING)
			{
				if (string.IsNullOrEmpty(Value))
				{
					EditorUtility.DisplayDialog("Warning!", "Value should not be empty", "Ok");
				}
			}

			switch (TypeDef)
			{
				case DataType.INTEGER:
					PlayerPrefs.SetInt(Key, int.Parse(Value));
					break;
				case DataType.FLOAT:
					PlayerPrefs.SetFloat(Key, float.Parse(Value));
					break;
				case DataType.STRING:
					PlayerPrefs.SetString(Key, Value);
					break;
			}
			Debug.Log("Key : " + Key + " Value : " + Value);
			MyWindow.Close();
		}

		[MenuItem("IACG/Save/DeleteAllSave %q")]
		public static void DeleteSaveFile()
		{
			if (EditorUtility.DisplayDialog("IACG Save", "Are you sure? Do you wanna delete all data.", "Yes", "No"))
			{
				File.Delete(SaveFileExtension.saveDataPath);
				PlayerPrefs.DeleteAll();
				EditorUtility.DisplayDialog("IACG Save", "Data deleted successfully", "OK");
			}
		}
	}

	public enum DataType
	{
		INTEGER,
		FLOAT,
		STRING
	}
}