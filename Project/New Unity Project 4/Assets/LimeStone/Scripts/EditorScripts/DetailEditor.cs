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
		* Detail editor: edits properties of details (grass) on terrains. 
		*
		*/
	
		class DetailEditor : EditorWindow
		{
	
				Vector2 scrollPos;
				static DetailPrototypeSerializable detailPrototype = null;
				
				public static void Init (DetailPrototypeSerializable dt)
				{
						detailPrototype = dt;
						EditorWindow.GetWindow (typeof(DetailEditor));
				}
	
				void OnGUI ()
				{
						if (detailPrototype == null) 
								return;
						EditorGUILayout.BeginVertical ();
						detailPrototype.prototypeTexture = (Texture2D)EditorGUILayout.ObjectField ("Texture", detailPrototype.prototypeTexture, typeof(Texture2D), false);

						detailPrototype.noiseSpread = Util.floatTextField ("Noise spread", detailPrototype.noiseSpread);
						detailPrototype.minHeight = Util.floatTextField ("Min height", detailPrototype.minHeight);
						detailPrototype.maxHeight = Util.floatTextField ("Max height", detailPrototype.maxHeight);
						detailPrototype.minWidth = Util.floatTextField ("Min width", detailPrototype.minWidth);
						detailPrototype.maxWidth = Util.floatTextField ("Max width", detailPrototype.maxWidth);
						detailPrototype.dryColor = EditorGUILayout.ColorField ("Dry color", detailPrototype.dryColor);
						detailPrototype.healthyColor = EditorGUILayout.ColorField ("Healthy color", detailPrototype.healthyColor);

						GUILayout.BeginHorizontal ();

						if (GUILayout.Button ("Close")) {
								this.Close ();
						}
						GUILayout.EndHorizontal ();
						GUILayout.EndVertical ();
		
				}
		}
}