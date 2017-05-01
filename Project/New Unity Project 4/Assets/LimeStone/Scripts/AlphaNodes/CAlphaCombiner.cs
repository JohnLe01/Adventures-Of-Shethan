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
		* CNode class that combines two alpha signals to one
		*/
		public class CAlphaCombiner : CAlpha
		{

				public override void Initialize (int windowID, int type, int x, int y)
				{
						InitializeWindow(windowID, type, x,y, "Alpha Combiner");
						color = LStyle.Colors[4];
						clickErrorMessage = "You need to assign two inputs to the alpha combiner before usage.";
						setupGUI ();	
			
						Inputs.Add (new CConnection (this, 0, CConnection.TYPE1));
						Inputs.Add (new CConnection (this, 1, CConnection.TYPE1));
						Outputs.Add (new CConnection (this, 2, CConnection.TYPE1));
						status_after_click = CNodeManager.STATUS_TASK2;
						helpMessage = "<size=24>" + LStyle.hexColors [4] + "Terrain Alpha map combiner. </color></size>" +
								"\n" +
								"\n" +
								"This generator combines two alpha map signals. In all cases the amplitude parameter scales the output. " +
								"\n" +
								"<size=15>" + LStyle.hexColors [4] + "Combining options:</color></size>\n" +
								"  " + LStyle.hexColors [1] + "Blend</color>: Linearly blending the two alpha maps. \n" +
								"  " + LStyle.hexColors [1] + "Override</color>: Overrides the first input by the second. \n"+
								"  " + LStyle.hexColors [1] + "Perlin</color>: Using Perlin noise to assign the texture to randomized areas. \n";

				
				
		}
	
				public void setupGUI ()
				{
						parameters ["blendval"] = new Parameter ("Value:", 0.5f, 0, 1);
						parameters ["amplitude"] = new Parameter ("Amplitude:", 1, 0, 3);
						parameters ["seed"] = new Parameter ("Seed:", 1, 0, 100);

						alternativeNames.Add (new AlternativeName ("seed", "", TYPE_BLEND));
						alternativeNames.Add (new AlternativeName ("seed", "", TYPE_OVERRIDE));
						alternativeNames.Add (new AlternativeName ("seed", "Seed:", TYPE_PERLIN));

						alternativeNames.Add (new AlternativeName ("value", "Value:", TYPE_BLEND));
						alternativeNames.Add (new AlternativeName ("value", "Value:", TYPE_OVERRIDE));
						alternativeNames.Add (new AlternativeName ("value", "Scale:", TYPE_PERLIN));


				}
	
				public static int TYPE_BLEND = 0;
				public static int TYPE_OVERRIDE = 1;
				public static int TYPE_PERLIN = 2;
				static public string[] types = {"Blend", "Override", "Perlin"};

				public override void CalculateAlphas (int i, int j, float[,] heightVal, Vector3[,] n)
				{
			
						CAlpha m1 = (CAlpha)getNode (Inputs, 0);
						CAlpha m2 = (CAlpha)getNode (Inputs, 1);
		
						if (m1 == null || m2 == null)
								return;

						m1.CalculateAlphas (i, j, heightVal, n);
						m2.CalculateAlphas (i, j, heightVal, n);

		
						float blendVal = getValue ("blendval");
						float seed = getValue ("seed");
						float amplitude = getValue ("amplitude");

						if (Type == TYPE_BLEND) 
								for (int k=0; k< LStyle.NO_TEXTURES; k++) 
										alphas [k] = blendVal * m1.alphas [k] + (1.0f - blendVal) * m2.alphas [k];

						if (Type == TYPE_OVERRIDE) {
								float val = 0;
								for (int k=0; k< LStyle.NO_TEXTURES; k++) 
										val += m2.alphas [k];

								val = Mathf.Clamp (val, 0, 1);

								for (int k=0; k< LStyle.NO_TEXTURES; k++) 
										alphas [k] = m1.alphas [k] * (1 - val) + val * m2.alphas [k];
		
						}

						if (Type == TYPE_PERLIN) {

								float ss = 5;
								float x = i / (float)(CTerrain.alphaMapSize - 1) + C2DMap.shift.y + seed;
								float y = j / (float)(CTerrain.alphaMapSize - 1) + C2DMap.shift.x;
								float p = Mathf.Clamp (Mathf.Pow (Mathf.PerlinNoise (x * blendVal * ss, y * blendVal * ss) * 2, 8.0f), 0, 1);
								//p = Mathf.SmoothStep(0,1,p);
								for (int k=0; k< LStyle.NO_TEXTURES; k++) 
										alphas [k] = p * m1.alphas [k] + (1 - p) * m2.alphas [k];
			
						}
		
						for (int k=0; k<alphas.Length; k++) {
								alphas [k] = Mathf.Clamp (alphas [k] * amplitude, 0, 1);
						}
			

				}
	
				public override void Draw (int ID)
				{
						base.Draw (ID);
		
						buildGUI (types [Type]);
		
						int t = Type;
						GUILayout.Label("Combiner type:");
			
						Type = EditorGUILayout.Popup (Type, types);
						if (t != Type)
								Change ();
						changed = true;
						size.y += LStyle.FontSize;
						window.height = size.y;

						GUI.DragWindow ();
		
				}
	
		}
}
