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
		*  Combines two terrain singnals into one
		*/
		public class CCombiner : CNode
		{
			
			
				public override void Initialize (int windowID, int type, int x, int y)
				{
						InitializeWindow (windowID, type, x, y, "Terrain Combiner");		
						map = new C2DMap ();
						//Allocate(sx,sy);

						color = LStyle.Colors [1] * 2;//new Color(0.5f,0.7f,1.0f);
						color.b += 0.25f;
						//output.from = this;
						setupGUI ();	

						Inputs.Add (new CConnection (this, 0, CConnection.TYPE0));
						Inputs.Add (new CConnection (this, 1, CConnection.TYPE0));
						Outputs.Add (new CConnection (this, 2, CConnection.TYPE0));
						
						clickErrorMessage = "You need to assign two inputs to the terrain combiner before usage.";
						
						status_after_click = CNodeManager.STATUS_CHANGED;

						helpMessage = "<size=24>" + LStyle.hexColors [4] + "Terrain combiner </color></size>" +
								"\n" +
								"\n" +
								"Combines two signals to a single one. Parameter [Amplitude] will always scale the output signal." +
								"\n" +
								"<size=15>" + LStyle.hexColors [4] + "Combining options:</color></size>\n" +
								"  " + LStyle.hexColors [1] + "Blend</color>: Blend the two signals using the [Value] parameter. \n" +
								"  " + LStyle.hexColors [1] + "Add</color>: Add the two signals, scaling the latter using the [Value] parameter. \n" +
								"  " + LStyle.hexColors [1] + "Sub</color>: Subtract the second signal from the first, with a scaling using the [Value] parameter. \n" +
								"  " + LStyle.hexColors [1] + "Mul</color>: Multiply the two signals. \n" +
								"  " + LStyle.hexColors [1] + "Min</color>: Choose the lowest value of the two signals. \n" +
								"  " + LStyle.hexColors [1] + "Max</color>: Choose the largest value of the two signals. \n" +
								"  " + LStyle.hexColors [1] + "Perlin</color>: Blend the two signals via Perlin noise scaled with [Value]. \n";
				}

				public void setupGUI ()
				{
						parameters ["blendval"] = new Parameter ("Value:", 0.5f, 0, 1);
						parameters ["amplitude"] = new Parameter ("Amplitude:", 1, 0, 3);

						alternativeNames.Add (new AlternativeName ("amplitude", "Seed:", TYPE_PERLIN));
						alternativeNames.Add (new AlternativeName ("amplitude", "Amplitude:", TYPE_BLEND));
						alternativeNames.Add (new AlternativeName ("amplitude", "Amplitude:", TYPE_ADD));
						alternativeNames.Add (new AlternativeName ("amplitude", "Amplitude:", TYPE_SUB));
						alternativeNames.Add (new AlternativeName ("amplitude", "Amplitude:", TYPE_MUL));
						alternativeNames.Add (new AlternativeName ("amplitude", "Amplitude:", TYPE_MAX));
						alternativeNames.Add (new AlternativeName ("amplitude", "Amplitude:", TYPE_MIN));

				}

				public static int TYPE_BLEND = 0;
				public static int TYPE_ADD = 1;
				public static int TYPE_SUB = 2;
				public static int TYPE_MUL = 3;
				public static int TYPE_MAX = 4;
				public static int TYPE_MIN = 5;
				public static int TYPE_PERLIN = 6;
				static public string[] types = {
						"Blender",
						"Add",
						"Sub",
						"Mul",
						"Max",
						"Min",
						"Perlin"
				};

				public override void Calculate ()
				{
						if (!verified)
								return;

						CNode m1 = ((CConnection)Inputs [0]).pointer.parent;
						CNode m2 = ((CConnection)Inputs [1]).pointer.parent;

						float blendVal = getValue ("blendval");
						float amplitude = getValue ("amplitude");
						if (m1.changed)
								m1.Calculate ();

						if (m2.changed)
								m2.Calculate ();

						for (int i=0; i<C2DMap.sizeX; i++) {
								for (int j=0; j<C2DMap.sizeY; j++) {	
										CNodeManager.Progress ();


										if (Type == TYPE_BLEND) 
												map [i, j] = blendVal * m1.map [i, j] + (1.0f - blendVal) * m2.map [i, j];
										if (Type == TYPE_PERLIN) { 
												float ss = 5;
												float seed = CNode.getSeed (amplitude * 10f);
												float x = i / (float)(C2DMap.sizeX - 1) + C2DMap.shift.y + seed;
												float y = j / (float)(C2DMap.sizeY - 1) + C2DMap.shift.x;
												float p = Mathf.Clamp (Mathf.Pow (Mathf.PerlinNoise (x * blendVal * ss, y * blendVal * ss) * 2, 4.0f), 0, 1);
												//p = Mathf.SmoothStep(0,1,p);
												map [i, j] = p * m1.map [i, j] + (1.0f - p) * m2.map [i, j];
										}
										if (Type == TYPE_ADD) 
												map [i, j] = m1.map [i, j] + blendVal * m2.map [i, j];
										if (Type == TYPE_SUB) 
												map [i, j] = blendVal * m1.map [i, j] - (1.0f - blendVal) * m2.map [i, j];
										if (Type == TYPE_MUL) 
												map [i, j] = m1.map [i, j] * m2.map [i, j];
										if (Type == TYPE_MAX) 
												map [i, j] = Mathf.Max (m1.map [i, j], m2.map [i, j]);
										if (Type == TYPE_MIN) 
												map [i, j] = Mathf.Min (m1.map [i, j], m2.map [i, j]);
										if (Type != TYPE_PERLIN)
												map [i, j] *= amplitude;
								}
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
						size.y += LStyle.FontSize*2;
						window.height = size.y;


						GUI.DragWindow ();

			
				}

		}
}
