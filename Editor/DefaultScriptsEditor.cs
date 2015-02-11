using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class DefaultScriptsEditor : EditorWindow 
{
	string[] files = null;

	int currentFile = -1;
	string fileContent = string.Empty;

	Vector2 scrollPos = Vector2.zero;
	
	float padding = 8f;
	float buttonHeight = 16f;

	[MenuItem("Window/Edit default scripts")]
	static void Init()
	{
		var window = (DefaultScriptsEditor)GetWindow(typeof(DefaultScriptsEditor), false, "Default scripts editor");
		window.position = new Rect(window.position.xMin + 100f, window.position.yMin + 100f, 400f, 300f);
		window.minSize = new Vector2(400f, 300f);
	}

	void OnGUI()
	{
		if( files == null )
		{
			string filesPath = Path.Combine(EditorApplication.applicationContentsPath, "Resources/ScriptTemplates");
			files = Directory.GetFiles(filesPath);
		}

		float w = (position.width - (padding * 2)) / files.Length;
		Rect pos = new Rect(padding, padding, w, buttonHeight);

		int newFile = currentFile;
		for(int i = 0; i < files.Length; ++i)
		{
			pos.x = padding + w * i;

			string f = Path.GetFileNameWithoutExtension(files[i]);
			f = Path.GetExtension(f);
			if( GUI.Toggle(pos, i == newFile, f, "Button") )
				newFile = i;
		}
		// load file data if the file change
		if( currentFile != newFile )
			LoadFileData(newFile);

		// stop gui here if nothing is selected
		if( currentFile < 0 || currentFile >= files.Length )
			return;


		pos.width = 64f;
		pos.x = position.width - pos.width - padding;
		pos.y = position.height - pos.height - padding;
		if( GUI.Button(pos, "Revert") )
			LoadFileData(currentFile);
		
		pos.x = pos.x - pos.width;
		if( GUI.Button(pos, "Save") )
			File.WriteAllText( files[currentFile], fileContent);

		pos.x = padding;
		pos.y = padding * 2 + buttonHeight;
		pos.width = position.width - (padding * 2);
		pos.height = position.height - (buttonHeight * 2) - (padding * 3);

		Vector2 contSize = new GUIStyle("TextArea").CalcSize( new GUIContent(fileContent) );

		Rect contPos = pos;
		contPos.width = Mathf.Max(pos.width, contSize.x);
		contPos.height = Mathf.Max(pos.height, contSize.y);

		scrollPos = GUI.BeginScrollView(pos, scrollPos, contPos);
		fileContent = EditorGUI.TextArea(contPos, fileContent);
		GUI.EndScrollView();
	}

	void LoadFileData(int index)
	{
		currentFile = index;
		fileContent = string.Empty;

		if( index >= 0 && index < files.Length )
			fileContent = File.ReadAllText( files[currentFile] );

		GUI.FocusControl(string.Empty);// HACK to fix text not updating visualy when the textarea has focus
	}
}
