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
		*  Generates scenery stuff - trees, grass, rocks etc
		*/
		public class CSceneryNode : CNode
		{

				static string[] sceneryNames = null;
				static string[] detailNames = null;
				public int sceneryIndex = 0;
				public GameObject obj = null;
				public static int TYPE_TREE = 0;
				public static int TYPE_DETAIL = 1;

				public override void Initialize (int windowID, int type, int x, int y)
				{
						InitializeWindow (windowID, type, x, y, "Scenery Generator");		
						status_after_click = 0;
						setupParameters ();
						color = LStyle.Colors [3] * 2;//new Color(0.5f, 0.8f, 1.9f);

						Outputs.Add (new CConnection (this, 0, CConnection.TYPE3));

						clickErrorMessage = "An individual scenery generator is not clickable. It needs to be connected to a scenery output, which is clickable."; 

						if (type == TYPE_TREE) 
								helpMessage = "<size=24>" + LStyle.hexColors [4] + "Detail object generator </color></size>" +
										"\n" +
										"\n" +
										"This generator places out detail objects that are defined in the 'Detail Prototypes' in the settings. These detail objects are prefabs that " +
										"should be contained in the project." + 
										"\n" +
										"<size=15>" + LStyle.hexColors [4] + "Parameters:</color></size>\n" +
										"  " + LStyle.hexColors [1] + "Amount</color>: The initial amount of objects to be placed. Can be scaled by the output. \n" +
										"  " + LStyle.hexColors [1] + "Color</color>: Amplitude of random color variation for each object. \n" +
										"  " + LStyle.hexColors [1] + "Height min/max</color>: Limits the height of this particular object. \n" + 
										"  " + LStyle.hexColors [1] + "Item</color>: Which detail object to be placed (list is defined in the settings). \n" +
										"  " + LStyle.hexColors [1] + "Lightscale</color>: Amplitude of random variation in the intensity of the objects. \n" +
										"  " + LStyle.hexColors [1] + "Normal</color>: Defines the maximum terrain tilt where objects could be placed. \n" +
										"  " + LStyle.hexColors [1] + "Perlin</color>: Perlin scale - lower value means larger clusters of objects. \n" +
										"  " + LStyle.hexColors [1] + "Perlin cut</color>: Perlin value treshold, objects are only placed when the perlin value is large than the cut/threshold. \n" +
										"  " + LStyle.hexColors [1] + "Size</color>: Amplitude of random variations in the objects \n";

						if (type == TYPE_DETAIL) 
								helpMessage = "<size=24>" + LStyle.hexColors [4] + "Detail object generator </color></size>" +
										"\n" +
										"\n" +
										"This generator places out simple terrain textures (grass etc) that are defined in the 'Detail Textures' in the settings. " +
										"\n" +
										"<size=15>" + LStyle.hexColors [4] + "Parameters:</color></size>\n" +
										"  " + LStyle.hexColors [1] + "Amount</color>: The initial amount of objects to be placed. Can be scaled by the output. \n" +
										"  " + LStyle.hexColors [1] + "Item</color>: Which detail texture to be placed (list is defined in the settings). \n" +
										"  " + LStyle.hexColors [1] + "Perlin</color>: Perlin scale - a lower value means larger clusters of grass. \n" +
										"  " + LStyle.hexColors [1] + "Power</color>: Treshold for Perlin values. A higher value means more 'peaky' areas of grass.  \n" +
										"  " + LStyle.hexColors [1] + "Tilt</color>: Terrain normal threshold - maximum terrain tilt for grass. \n";
				}

				public void setupParameters ()
				{
						if (Type == TYPE_TREE) {
								parameters ["perlin"] = new Parameter ("Perlin", 0.5f, 0, 10);
								parameters ["count"] = new Parameter ("Amount", 1000, 0, 100000);
								parameters ["r"] = new Parameter ("Color R", 0, 0, 1);
								parameters ["g"] = new Parameter ("Color G", 0, 0, 1);
								parameters ["b"] = new Parameter ("Color B", 0, 0, 1);
								parameters ["size"] = new Parameter ("Size", 0.3f, 0, 1);
								parameters ["lightscale"] = new Parameter ("Lightscale", 0.3f, 0, 1);
								parameters ["normal"] = new Parameter ("Normal", 0.95f, 0.5f, 1);
								parameters ["perlinCut"] = new Parameter ("Perlin Cut", 0, -1, 1);
								parameters ["hmin"] = new Parameter ("Height min", 0, 0, 1);
								parameters ["hmax"] = new Parameter ("Height max", 1, 0, 1);
								parameters ["item"] = new Parameter ("Item", 0.0f, 0, LimeStoneEditor.settings.trees.Count - 1);
								parameters ["seed"] = new Parameter ("Seed", 0.0f, 0, 100f);
						}
						if (Type == TYPE_DETAIL) {
								parameters ["perlin"] = new Parameter ("Perlin", 0.5f, 0, 10);
								parameters ["threshold"] = new Parameter ("Power", 0.0f, 0, 12);
								parameters ["count"] = new Parameter ("Amount", 10000, 0, 100000);
								parameters ["item"] = new Parameter ("Item", 0.0f, 0, LimeStoneEditor.settings.detailPrototypes.Count - 1);
								parameters ["seed"] = new Parameter ("Seed", 0.0f, 0, 100f);
								parameters ["tilt"] = new Parameter ("Tilt", 0.8f, 0, 1f);
			}
				}

				public void buildSceneryNames ()
				{
						int cnt = LimeStoneEditor.settings.trees.Count;
						if (sceneryNames == null)
								sceneryNames = new string[cnt];
						if (sceneryNames.Length != cnt)
								sceneryNames = new string[cnt];
						int i = 0;
						foreach (GameObject go in  LimeStoneEditor.settings.trees)
								sceneryNames [i++] = go.name;


						int cnt2 = LimeStoneEditor.settings.detailPrototypes.Count;
						if (detailNames == null)
								detailNames = new string[cnt2];
						if (detailNames.Length != cnt2)
								detailNames = new string[cnt2];
						i = 0;
						foreach (DetailPrototypeSerializable dt in  LimeStoneEditor.settings.detailPrototypes)
								detailNames [i++] = dt.prototypeTexture.name;

				}

				public override void Verify ()
				{
						// Can never be clicked!
						verified = false;
				}

				public override void Draw (int ID)
				{
						//verified = true;
						base.Draw (ID);
						buildSceneryNames ();
						if (Type == TYPE_TREE && sceneryNames.Length == 0) {
								GUILayout.Label ("Please assign \nDetail prototypes\nin settings");
								size.y = LStyle.FontSize * 4;
								window.height = size.y;
								GUI.DragWindow ();
				
								return;
						}
						if (Type == TYPE_DETAIL && detailNames.Length == 0) {
								GUILayout.Label ("Please assign \nDetail textures\nin settings");
								size.y = LStyle.FontSize * 4;
								window.height = size.y;
								GUI.DragWindow ();
				
								return;
						}
						string s = "";
						if (Type == TYPE_TREE)
							if (sceneryIndex<sceneryNames.Length)
							s = sceneryNames [sceneryIndex];
						if (Type == TYPE_DETAIL)
							if (sceneryIndex<detailNames.Length)
								s = detailNames [sceneryIndex];


						buildGUI (s);

						sceneryIndex = (int)getValue ("item");
						setValue ("item", sceneryIndex);
						if (Type == TYPE_TREE)
								setMax ("item", LimeStoneEditor.settings.trees.Count - 1);
						if (Type == TYPE_DETAIL)
								setMax ("item", LimeStoneEditor.settings.detailPrototypes.Count - 1);
						if (Type == TYPE_TREE)
							if (sceneryIndex<LimeStoneEditor.settings.trees.Count)
							obj = LimeStoneEditor.settings.trees [sceneryIndex];
						window.height = size.y;

						GUI.DragWindow ();
		
				}
		}
}
