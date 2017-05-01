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
		* Basic editor window for maintaining the list of detail objects (trees etc)
		*/
		class SceneryEditor : EditorWindow
		{
	
				Vector2 scrollPos;
				
				public static void Init ()
				{
						EditorWindow.GetWindow (typeof(SceneryEditor));
				}

				void OnGUI ()
				{
						EditorGUILayout.BeginVertical ();
						scrollPos = 
						EditorGUILayout.BeginScrollView (scrollPos);
						foreach (GameObject gob in  LimeStoneEditor.settings.trees) {
								string name = "none";
								if (gob != null)
										name = gob.name;

								EditorGUILayout.BeginHorizontal ();

								GameObject go = (GameObject)EditorGUILayout.ObjectField (name, gob, typeof(GameObject), false);
								if (go != gob) {
										LimeStoneEditor.settings.trees.Remove (gob);
										LimeStoneEditor.settings.trees.Add (go);
										break;
								}
								if (GUILayout.Button ("Delete")) {
										LimeStoneEditor.settings.trees.Remove (go);
										break;
								}
								EditorGUILayout.EndHorizontal ();


						} 

						EditorGUILayout.EndScrollView ();
						GUILayout.BeginHorizontal ();
						if (GUILayout.Button ("Add object")) {
								LimeStoneEditor.settings.trees.Add (null);
						}
						if (GUILayout.Button ("Edit object")) {
						}
						GUILayout.EndHorizontal ();
						GUILayout.EndVertical ();

				}
		}
}