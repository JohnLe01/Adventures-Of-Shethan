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
		* Editor window for terrain settings (sizes, grids etc)
		*/
		
		class TerrainDetailEditor : EditorWindow
		{
	
				private static Settings settings = null;

				public static void Init (Settings s)
				{
						settings = s;
						EditorWindow.GetWindow (typeof(TerrainDetailEditor));
				}
	
				void OnGUI ()
				{
						if (settings == null)
								return;
						EditorGUILayout.BeginVertical ();
						
						int w = 150;
						EditorGUILayout.BeginHorizontal ();
						GUILayout.Label ("Terrain data size:", GUILayout.Width (w));
						settings.newSize = EditorGUILayout.Popup (settings.newSize, settings.sizesStrings);
						EditorGUILayout.EndHorizontal ();		


						EditorGUILayout.BeginHorizontal ();
						GUILayout.Label ("Terrain grid size (x,y):", GUILayout.Width (w));
						settings.gridSizeX = EditorGUILayout.Popup (settings.gridSizeX, settings.gridStrings2);
						settings.gridSizeY = EditorGUILayout.Popup (settings.gridSizeY, settings.gridStrings2);
						EditorGUILayout.EndHorizontal ();		

						EditorGUILayout.BeginHorizontal ();
						GUILayout.Label ("Material:", GUILayout.Width (w));
						Material m = (Material)EditorGUILayout.ObjectField (settings.terrainMaterial,
			                                                           typeof(Material), true);
			            if (m!=settings.terrainMaterial) {
			            	settings.terrainMaterial = m;
			            	LimeStoneEditor.terrainManager.UpdateTerrainDetails();
			            }
			
						EditorGUILayout.EndHorizontal ();		

						settings.detailDistance = Util.floatTextField ("Detail distance:", w, settings.detailDistance);
						if (Util.LastFloatValueChanged)
							LimeStoneEditor.terrainManager.UpdateTerrainDetails ();
						settings.treeBillboardStart = Util.floatTextField ("Tree billboard start:", w, settings.treeBillboardStart);
						if (Util.LastFloatValueChanged)
							LimeStoneEditor.terrainManager.UpdateTerrainDetails ();
						settings.terrainPixelError = Util.floatTextField ("Terrain pixel error", w, settings.terrainPixelError);
						if (Util.LastFloatValueChanged)
							LimeStoneEditor.terrainManager.UpdateTerrainDetails ();
						settings.alphaLayerScale = Mathf.Clamp(Util.floatTextField ("Alpha layer scale:", w, settings.alphaLayerScale),0,4);
						if (Util.LastFloatValueChanged)
								LimeStoneEditor.terrainManager.UpdateAlphas ();
						settings.TerrainSmoothVal = (int)Mathf.Clamp(Util.floatTextField ("Terrain smooth value:", w, settings.TerrainSmoothVal), 0, 20);
			
						GUILayout.BeginHorizontal ();
		
						if (GUILayout.Button ("Close")) {
								this.Close ();
						}
						GUILayout.EndHorizontal ();
						GUILayout.EndVertical ();
		
				}
		}
}