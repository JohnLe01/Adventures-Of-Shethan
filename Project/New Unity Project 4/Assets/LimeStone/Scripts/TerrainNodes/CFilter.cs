using UnityEngine;
using UnityEditor;
using System.Collections;

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
		* Transforms a terrain signal
		*/

		public class CFilter : CNode
		{
				public override void Initialize (int windowID, int type, int x, int y)
				{
						InitializeWindow (windowID, type, x, y, "Terrain Filter");		
						map = new C2DMap ();
						Inputs.Add (new CConnection (this, 0, CConnection.TYPE0));
						Outputs.Add (new CConnection (this, 1, CConnection.TYPE0));
						color = LStyle.Colors [1] * 2;//new Color(0.5f,0.7f,1.0f);
						color.b += 0.25f;

						//output.from = this;
						setupGUI ();
						status_after_click = CNodeManager.STATUS_CHANGED;
						clickErrorMessage = "You need to assign an inputs to the terrain filter before usage.";

						helpMessage = "<size=24>" + LStyle.hexColors [4] + "Terrain filters </color></size>" +
								"\n" +
								"\n" +
								"Transforms a single signal into a new." +
								"\n" +
								"<size=15>" + LStyle.hexColors [4] + "Filter types:</color></size>\n" +
								"  " + LStyle.hexColors [1] + "Exponential</color>: signal = amplitude*exp(spread) - offset. \n" +
								"  " + LStyle.hexColors [1] + "Power</color>: signal = amplitude*signal^exponential - offset. \n" +
								"  " + LStyle.hexColors [1] + "Invert</color>: signal = amplitude*(1-signal) - offset . \n" +
								"  " + LStyle.hexColors [1] + "Minmax</color>: signal is clamped within the min and max values. \n" +
								"  " + LStyle.hexColors [1] + "Scale</color>: signal = amplitude * signal - offset \n" +
								"  " + LStyle.hexColors [1] + "Erode</color>: Erodes the terrain by amount using a tilt threshold. Warning: Does not work yet for multiple terrains, seams are incorrect. \n";

				}

				public void setupGUI ()
				{
						parameters ["amplitude"] = new Parameter ("Amplitude", 0.5f, 0, 2f);
						parameters ["offset"] = new Parameter ("Param2:", 0.01f, 0, 2f);
						parameters ["value"] = new Parameter ("Param3:", 0.5f, 0, 8);

						alternativeNames.Add (new AlternativeName ("amplitude", "Amplitude:", TYPE_EXP));
						alternativeNames.Add (new AlternativeName ("value", "Spread:", TYPE_EXP));
						alternativeNames.Add (new AlternativeName ("offset", "Offset:", TYPE_EXP));

						alternativeNames.Add (new AlternativeName ("amplitude", "Amplitude:", TYPE_POW));
						alternativeNames.Add (new AlternativeName ("value", "Exponential:", TYPE_POW));
						alternativeNames.Add (new AlternativeName ("offset", "Offset:", TYPE_POW));

						alternativeNames.Add (new AlternativeName ("amplitude", "Amplitude:", TYPE_INV));
						alternativeNames.Add (new AlternativeName ("value", "", TYPE_INV));
						alternativeNames.Add (new AlternativeName ("offset", "Offset:", TYPE_INV));

						alternativeNames.Add (new AlternativeName ("amplitude", "", TYPE_MAX));
						alternativeNames.Add (new AlternativeName ("value", "Max:", TYPE_MAX));
						alternativeNames.Add (new AlternativeName ("offset", "Min:", TYPE_MAX));

						alternativeNames.Add (new AlternativeName ("amplitude", "Tilt:", TYPE_ERODE));
						alternativeNames.Add (new AlternativeName ("value", "", TYPE_ERODE));
						alternativeNames.Add (new AlternativeName ("offset", "Amount", TYPE_ERODE));

						alternativeNames.Add (new AlternativeName ("amplitude", "Amplitude:", TYPE_SCALE));
						alternativeNames.Add (new AlternativeName ("value", "", TYPE_SCALE));
						alternativeNames.Add (new AlternativeName ("offset", "Offset:", TYPE_SCALE));


				}
	
				public static int TYPE_EXP = 0;
				public static int TYPE_POW = 1;
				public static int TYPE_INV = 2;
				public static int TYPE_MAX = 3;
				public static int TYPE_ERODE = 4;
				public static int TYPE_SCALE = 5;
				static public string[] types = {
						"Exponential",
						"Power",
						"Invert",
						"Minmax",
						"Erode",
						"Scale"
				};

				public void Erode (int N, CNode m1)
				{
						C2DMap map2 = new C2DMap ();
						map2.CopyFrom (m1.map);
						map.CopyFrom (m1.map);


						float val = (getValue ("amplitude")) * 0.01f;
						for (int count = 0; count <N; count++) {
								for (int i=2; i<C2DMap.sizeX-2; i++) {
										for (int j=2; j<C2DMap.sizeY-2; j++) {
												if (count == N - 1)
														CNodeManager.Progress ();

												for (int k=-1; k<=1; k++) {
														for (int l=-1; l<=1; l++) 
																if (k != 0 && l != 0) {
																		float diff = map2 [i, j] - map [i + k, j + l];
																		float v = diff * 0.5f;
																		if (diff > val) {
																				map [i, j] -= v;
																				map [i + k, j + l] += v;
																		}


																}
												}
										}
								}
								map2.CopyFrom (map);
						}
				}

				public override void Calculate ()
				{
						Verify ();
						if (!verified)
								return;
		
						CNode m1 = ((CConnection)Inputs [0]).pointer.parent;
						float amplitude = getValue ("amplitude");
						float value = getValue ("value") - 0;
						float offset = getValue ("offset") - 0;
						if (m1.changed)
								m1.Calculate ();
	
						m1.map.calculateStatistics ();


						float i1 = offset / 2.0f;
						float i2 = value / 10.0f;
						if (Type == TYPE_ERODE) 
								Erode ((int)(offset * 20f), m1);
						else
								for (int i=0; i<C2DMap.sizeX; i++) {
										for (int j=0; j<C2DMap.sizeY; j++) {	
												CNodeManager.Progress ();
												if (Type == TYPE_EXP) 
														map [i, j] = amplitude * (Mathf.Exp ((value - 1) * m1.map [i, j])) + offset - 1;
												if (Type == TYPE_POW) {
														map [i, j] = Mathf.Clamp (amplitude * Mathf.Pow (4 * m1.map [i, j], (value/4f)), 0, 1) + offset - 1;
												}
												if (Type == TYPE_INV) 
														map [i, j] = amplitude * (1 - m1.map [i, j] + offset - 1);
												if (Type == TYPE_MAX) 
														map [i, j] = (Mathf.Clamp (m1.map [i, j], i1, i2));
												if (Type == TYPE_SCALE) 
														map [i, j] = Mathf.Clamp (amplitude * (m1.map [i, j] + (offset - 1f) / 2f), 0, 1);


										}
								}
				}

				public override void Draw (int ID)
				{
						base.Draw (ID);


						buildGUI (types [Type]);

						renderPresets ();
						GUILayout.Label("Filter type:");
						Type = EditorGUILayout.Popup (Type, types);
						size.y += 2*LStyle.FontSize;
						window.height = size.y;

						GUI.DragWindow ();

				}

		}
}
