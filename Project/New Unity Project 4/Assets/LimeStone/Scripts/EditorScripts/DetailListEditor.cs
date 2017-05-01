using UnityEngine;
using System.Collections;
using UnityEditor;

/*
 * 
 * © 2014 LemonSpawn. All rights reserved. Source code in this project ("LimeStone") are not supported under any LemonSpawn standard support program or service. 
 * The scripts are provided AS IS without warranty of any kind. LemonSpawn disclaims all implied warranties including, without limitation, 
 * any implied warranties of merchantability or of fitness for a particular purpose. 
 * 
 * Basically, do what you want with this code, but don't blame us if something blows up. 
 * 
 * 
*/

namespace LimeStone
{
		/*
		* Detail List Editor: editorwindow for maintaining detail textures (Grass etc) 
		*
		*/
		class DetailListEditor : EditorWindow
		{
	
				Vector2 scrollPos;
				public static void Init ()
				{
						EditorWindow.GetWindow (typeof(DetailListEditor));
				}

				void OnGUI ()
				{
						EditorGUILayout.BeginVertical ();
						scrollPos = 
						EditorGUILayout.BeginScrollView (scrollPos);
						foreach (DetailPrototypeSerializable t in  LimeStoneEditor.settings.detailPrototypes) {
								string name = "none";
								if (t != null)
								if (t.prototypeTexture != null)
										name = t.prototypeTexture.name;
								else
										name = "undefined";
								EditorGUILayout.BeginHorizontal ();
								GUILayout.Label (name);
								if (GUILayout.Button ("Edit")) {
										DetailEditor.Init (t);
								}
								if (GUILayout.Button ("Delete")) {
										LimeStoneEditor.settings.detailPrototypes.Remove (t);
										return;
								}
								EditorGUILayout.EndHorizontal ();

						} 
		
						EditorGUILayout.EndScrollView ();
						GUILayout.BeginHorizontal ();
						if (GUILayout.Button ("Add detail texture")) {
								LimeStoneEditor.settings.detailPrototypes.Add (new DetailPrototypeSerializable ());
						}
						if (GUILayout.Button ("Edit detail texture")) {
						}
						GUILayout.EndHorizontal ();
						GUILayout.EndVertical ();
		
				}
		}
}