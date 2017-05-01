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
		* Main procedural terrain generator node
		*/
 
		public class CProceduralMap : CNode
		{

				public override void Initialize (int windowID, int type, int x, int y)
				{

						status_after_click = CNodeManager.STATUS_CHANGED;
						InitializeWindow (windowID, type, x, y, "Terrain Generator");		
						map = new C2DMap ();
						setupParameters ();

						color = LStyle.Colors [1] * 2;
						color.b *= 0.5f;
						
						Outputs.Add (new CConnection (this, 0, CConnection.TYPE0));
						if (type == TYPE_MULTIRIDGED) 
								helpMessage = "<size=24>" + LStyle.hexColors [4] + "Multi-Ridged Perlin Noise Generator </color></size>" +
										"\n" +
										"\n" +
										"The Multi-rided Perlin Noise Generator creates mountain landscapes with peaks and ridges. " +
										"\n" +
										"<size=15>" + LStyle.hexColors [4] + "Parameters:</color></size>\n" +
										"  " + LStyle.hexColors [1] + "Amplitude</color>: Controls the overall height of the mountains. \n" +
										"  " + LStyle.hexColors [1] + "Gain</color>: How 'pointy' the mountains are. Low gain for hilly landscape, high gain for Mordor. Parameter diverges, so play carefully. \n" +
										"  " + LStyle.hexColors [1] + "Offset</color>: Moves the entire landscape up and down. Interfers strongly with gain, so tune this carefully. \n" +
										"  " + LStyle.hexColors [1] + "Lacunarity</color>: Controls the behaviour of small structures. Low lacunarity gives a smooth, hilly landscape, high lacunarity gives lots of small structures, and flat lake areas. Parameter diverges, so play carefully. \n" +
										"  " + LStyle.hexColors [1] + "Scale</color>: The width of the mountains. Low scale produces wide mountains, high scale produces many narrow mountains. \n" + 
										"  " + LStyle.hexColors [1] + "Seed</color>: The initial random seed for this generator. Change this to produce a different terrain with similar properties.\n";


						if (type == TYPE_SWISS) 
								helpMessage = "<size=24>" + LStyle.hexColors [4] + "Swiss Perlin Noise Generator </color></size>" +
										"\n" +
										"\n" +
										"The Swiss Perlin Noise Generator creates landscapes with fake erosion patterns, both tangentially and radial to the peak centers. " +
										"\n" +
										"<size=15>" + LStyle.hexColors [4] + "Parameters:</color></size>\n" +
										"  " + LStyle.hexColors [1] + "Amplitude</color>: Controls the overall height of the mountains. \n" +
										"  " + LStyle.hexColors [1] + "Gain</color>:  How 'pointy' the mountains are. Low gain for hilly landscape, high gain for Mordor. Parameter diverges, so play carefully. \n" +
										"  " + LStyle.hexColors [1] + "Offset</color>: Moves the entire landscape up and down. Interfers strongly with gain, so tune this carefully.\n" +
										"  " + LStyle.hexColors [1] + "Lacunarity</color>: Controls the behaviour of small structures. Low lacunarity gives a smooth, hilly landscape, high lacunarity gives lots of small structures, and flat lake areas. Parameter diverges, so play carefully. \n" +
										"  " + LStyle.hexColors [1] + "Power</color>: Smooths the signal internally. Low power gives a contineous low mountain landscape, high power yields smooth valleys between high peaks. \n" +
										"  " + LStyle.hexColors [1] + "Warp</color>: Defines directon of fake erosion patterns. Warp > 1 produces radial erosion (ravines), warp < 1 produces tangential erosion (circles).  \n" +
										"  " + LStyle.hexColors [1] + "Scale</color>: The width of the mountains. Low scale produces wide mountains, high scale produces many narrow mountains. \n" + 
										"  " + LStyle.hexColors [1] + "Seed</color>: The initial random seed for this generator. Change this to produce a different terrain with similar properties. \n";


						if (type == TYPE_PERLIN) 
								helpMessage = "<size=24>" + LStyle.hexColors [4] + "Perlin Noise Generator </color></size>" +
										"\n" +
										"\n" +
										"Standard Perlin noise signal. Generates a hilly terrain." +
										"\n" +
										"<size=15>" + LStyle.hexColors [4] + "Parameters:</color></size>\n" +
										"  " + LStyle.hexColors [1] + "Amplitude</color>: Controls the overall height of the hills. \n" +
										"  " + LStyle.hexColors [1] + "Octaves</color>: The number of different scales. 1 octave produces hills of roughly the same size. A higher number produces a rich landscape consisting of both small and large structures. Note that more octaves are more computationally demanding. \n" + 
										"  " + LStyle.hexColors [1] + "Scale</color>: The width of the hills. Low scale produces wide hills, high scale produces dense, narrow hills. \n" + 
										"  " + LStyle.hexColors [1] + "Damping</color>: Damping of small scales. A higher number will more strongly suppress the height of small structures. \n" +
										"  " + LStyle.hexColors [1] + "Seed</color>: The initial random seed for this generator. Change this to produce a different terrain with similar properties.\n";


				}

				string[] types = {"Perlin","Multiridged", "Swiss"};
				public static int TYPE_PERLIN = 0;
				public static int TYPE_MULTIRIDGED = 1;
				public static int TYPE_SWISS = 2;

				public override void Calculate ()
				{
						if (!verified)
								return;

						if (Type == TYPE_PERLIN) 
								map.calculatePerlin (getValue ("amplitude"), getValue ("scale"), getValue ("octaves"), getValue ("kscale"), getValue ("seed"));
						if (Type == TYPE_MULTIRIDGED) 
								map.calculateMultiridged (
				getValue ("seed"),
				getValue ("amplitude"),
				getValue ("scale") * 0.2f,
				getValue ("lacunarity"),
				getValue ("gain"),
				getValue ("offset"));

						if (Type == TYPE_SWISS) 
								map.calculateSwiss (
				getValue ("seed"),
				getValue ("amplitude"),
				getValue ("scale") * 0.2f,
				getValue ("lacunarity"),
				getValue ("gain"),
				getValue ("offset"),
				getValue ("warp"),
				getValue ("power")
								);
		

						//changed = false;
				}

				public void setupParameters ()
				{
						if (Type == TYPE_PERLIN) {
								parameters ["amplitude"] = new Parameter ("Amplitude:", 0.2f, 0, 1);
								parameters ["scale"] = new Parameter ("Scale:", 1.0f, 0.1f, 20);
								parameters ["octaves"] = new Parameter ("Octaves:", 1.0f, 1, 40);
								parameters ["kscale"] = new Parameter ("Damping:", 1.0f, 0.1f, 4f);
								parameters ["seed"] = new Parameter ("Seed:", 0.0f, 0.0f, 1000f);
								//parameters["kdiv"] = new Parameter("k_div:", 1.0f, 0, 10);
								color = new Color (0.5f, 1.0f, 0.7f);

						}
						if (Type == TYPE_MULTIRIDGED) {
								color = new Color (0.8f, 1.0f, 0.4f);
								parameters ["seed"] = new Parameter ("Seed:", 0.0f, 0, 1000);
								parameters ["amplitude"] = new Parameter ("Amplitude:", 0.5f, 0, 1);
								parameters ["scale"] = new Parameter ("Scale:", 4.0f, 0.1f, 30);
								parameters ["lacunarity"] = new Parameter ("Lacunarity:", 2.7f, 0, 8);
								parameters ["offset"] = new Parameter ("Offset:", 0.1f, 0, 4);
								parameters ["gain"] = new Parameter ("Gain:", 1.2f, 0, 3f);

						}
						if (Type == TYPE_SWISS) {
								color = new Color (0.2f, 1.0f, 0.2f);
								parameters ["seed"] = new Parameter ("Seed:", 0.0f, 0, 1000);
								parameters ["amplitude"] = new Parameter ("Amplitude:", 0.37f, 0, 1);
								parameters ["scale"] = new Parameter ("Scale:", 8.60f, 0.1f, 40);
								parameters ["lacunarity"] = new Parameter ("Lacunarity:", 2.83f, 0, 8);
								parameters ["offset"] = new Parameter ("Offset:", 0.85f, 0, 4);
								parameters ["gain"] = new Parameter ("Gain:", 0.63f, 0, 3f);
								parameters ["warp"] = new Parameter ("Warp:", 0.36f + 1.0f, 0, 2f);
								parameters ["power"] = new Parameter ("Power:", 0.33f, 0, 1f);

						}
				}

				public override void Draw (int ID)
				{
						base.Draw (ID);

						buildGUI (types [Type]);
						renderPresets ();

						GUI.DragWindow ();
						window.height = size.y;

				}

		}
}
