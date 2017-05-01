using UnityEngine;
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
		* Alpha generator class : height, normals and curvature.
		*/
		public class CAlphaGenerator : CAlpha
		{
	
				public override void Initialize (int windowID, int type, int x, int y)
				{
		
						InitializeWindow (windowID, type, x, y, "Alpha Generator");		
						status_after_click = CNodeManager.STATUS_TASK2;
						setupParameters ();
						//Tops.Add (new CConnection(this,0, CConnection.TYPE1));
						Outputs.Add (new CConnection (this, 1, CConnection.TYPE1));


						if (type == TYPE_HEIGHT) 
								helpMessage = "<size=24>" + LStyle.hexColors [4] + "Terrain Alpha map height generator. </color></size>" +
										"\n" +
										"\n" +
										"This generator creates an alpha map from the height map, blending two textures. One texture is used for the lower regions, and the other for the higher regions. " +
										"\n" +
										"<size=15>" + LStyle.hexColors [4] + "Parameters:</color></size>\n" +
										"  " + LStyle.hexColors [1] + "Height</color>: Controls the height threshold to blend textures. \n" +
										"  " + LStyle.hexColors [1] + "Texture1</color>: Select the texture to use for the lowlands. \n" +
										"  " + LStyle.hexColors [1] + "Texture2</color>: Select the texture to use for the highlands \n" +
										"  " + LStyle.hexColors [1] + "T1weight</color>: Texture 1 weight (intensity). \n" +
										"  " + LStyle.hexColors [1] + "T2weight</color>: Texture 2 weight (intensity). \n" +
										"  " + LStyle.hexColors [1] + "Power</color>: Sharpness of texture blending edge. \n";

						if (type == TYPE_NORMAL) 
								helpMessage = "<size=24>" + LStyle.hexColors [4] + "Terrain Alpha map normal generator. </color></size>" +
										"\n" +
										"\n" +
										"This generator creates an alpha map from the tilt (normal) of the terrain heightmap.  " +
										"\n" +
										"<size=15>" + LStyle.hexColors [4] + "Parameters:</color></size>\n" +
										"  " + LStyle.hexColors [1] + "Texture</color>: Which texture to use for the tilted areas. \n" +
										"  " + LStyle.hexColors [1] + "Texture weight</color>: Texture weight (intensity). \n" +
										"  " + LStyle.hexColors [1] + "Tilt</color>: Controls the amount of tilt required to blend in the texture. \n";

						if (type == TYPE_CURVATURE) 
								helpMessage = "<size=24>" + LStyle.hexColors [4] + "Terrain Alpha map curvature generator. </color></size>" +
										"\n" +
										"\n" +
										"This generator creates an alpha map from the negative curvature of the terrain heightmap.  " +
										"\n" +
										"<size=15>" + LStyle.hexColors [4] + "Parameters:</color></size>\n" +
										"  " + LStyle.hexColors [1] + "Texture</color>: Which texture to use for the curved areas. \n" +
										"  " + LStyle.hexColors [1] + "Texture weight</color>: Texture weight (intensity). \n" +
										"  " + LStyle.hexColors [1] + "Tilt</color>: Threshold tilt of the curvature. \n";


						//"  " +  LStyle.hexColors[1] + "</color>: . \n" +

				}

				string[] types = {"Height","Normal", "Curvature"};
				public static int TYPE_HEIGHT = 0;
				public static int TYPE_NORMAL = 1;
				public static int TYPE_CURVATURE = 2;
				private int texture1, texture2;
				public static string[] textureList = {
						"Texture 1",
						"Texture 2",
						"Texture 3",
						"Texture 4"
				};

				public override void CalculateAlphas (int i, int j, float[,] heightVal, Vector3[,] n)
				{
		
						texture1 = (int)getValue ("texture1");
						texture2 = (int)getValue ("texture2");

						for (int k=0; k<alphas.Length; k++)
								alphas [k] = 0;
						if (Type == TYPE_HEIGHT) { 
								float h = getValue ("height");
								float t1 = getValue ("t1weight");
								float t2 = getValue ("t2weight");
								float power = getValue ("power");

								float val = Mathf.SmoothStep (0, 1.0f, heightVal [i, j] - h + 1.2f);
								val = Mathf.Clamp (Mathf.Pow (val, power), 0, 1);
								alphas [texture1] = t1 * val;
								alphas [texture2] = t2 * (1.0f - val);
						}
						if (Type == TYPE_NORMAL) { 
								float tilt = getValue ("tilt");
								float t1 = getValue ("t1weight");
								if (n [i, j].y < (1.0f - tilt))
										alphas [texture1] = t1;
						}

						if (Type == TYPE_CURVATURE) { 
								float tilt = getValue ("tilt");
								float t1 = getValue ("t1weight");
								float f = getNormalChange (i, j, n);
								float h = heightVal [i, j];
								float avgH = getAverage (i, j, heightVal);
								if (f < tilt && h < avgH) 
										alphas [texture1] = t1;
						}

						//changed = false;
				}

				Vector3 avg = new Vector3 ();

				private float getNormalChange (int i, int j, Vector3[,] n)
				{
						avg.Set (n [i, j].x, n [i, j].y, n [i, j].z);
						float val = 1;
						int s = 1;
						for (int x =-1*s; x<=1*s; x++)
								for (int y =-1*s; y<=1*s; y++) {
										int xx = x + i;
										int yy = y + j;
										if (xx >= 0 && xx < n.GetLength (0) && yy >= 0 && yy < n.GetLength (1)) {
												val *= Vector3.Dot (avg, n [xx, yy]);
										}
								}

						return val;

				}

				private float getAverage (int i, int j, float[,] h)
				{
						float avg = 0;
						int s = 1;
						for (int x =-1*s; x<=1*s; x++)
								for (int y =-1*s; y<=1*s; y++) {
										int xx = x + i;
										int yy = y + j;
										if (xx >= 0 && xx < h.GetLength (0) && yy >= 0 && yy < h.GetLength (1)) {
												avg += h [xx, yy];
										}

								}
						avg /= (2f * s + 1.0f) * (2f * s + 1.0f);
						return avg;
						// Find if it is top or bottom
		
				}
	
				public void setupParameters ()
				{
						if (Type == TYPE_HEIGHT) {
								parameters ["height"] = new Parameter ("Height", 0.5f, 0, 1);
								parameters ["t1weight"] = new Parameter ("T1 weight", 0.8f, 0, 1);
								parameters ["t2weight"] = new Parameter ("T2 weight", 0.8f, 0, 1);

								parameters ["texture1"] = new Parameter ("Texture 1", 0.0f, 0, LStyle.NO_TEXTURES - 1);
								parameters ["texture2"] = new Parameter ("Texture 2", 1.0f, 0, LStyle.NO_TEXTURES - 1);
								parameters ["power"] = new Parameter ("Power", 50f, 1f, 100f);
						}
						if (Type == TYPE_NORMAL) {
								parameters ["tilt"] = new Parameter ("Tilt", 0.5f, 0, 1);
								parameters ["t1weight"] = new Parameter ("Texture weight", 0.5f, 0, 1);
								parameters ["texture1"] = new Parameter ("Texture", 0.0f, 0, LStyle.NO_TEXTURES - 1);
						}
						if (Type == TYPE_CURVATURE) {
								parameters ["tilt"] = new Parameter ("Curvature", 0.8f, 0.75f, 1);
								parameters ["t1weight"] = new Parameter ("Texture weight", 0.5f, 0, 1);
								parameters ["texture1"] = new Parameter ("Texture", 0.0f, 0, LStyle.NO_TEXTURES - 1);
						}
						color = 1 * LStyle.Colors [4];//new Color(0.9f, 0.5f, 0.2f);
						color.g *= 0.5f;

				}

				private void renderTextures ()
				{

				}
	
				public override void Draw (int ID)
				{
						base.Draw (ID);

						buildGUI (types [Type]);
						texture1 = (int)getValue ("texture1");
						texture2 = (int)getValue ("texture2");
						setValue ("texture1", texture1);
						setValue ("texture2", texture2);
						GUI.DragWindow ();
		
				}
	
		}
}
