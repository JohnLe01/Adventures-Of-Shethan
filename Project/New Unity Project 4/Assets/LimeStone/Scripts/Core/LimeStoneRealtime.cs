using UnityEngine;
using System.Collections;

namespace LimeStone
{
		/*
		* Used for in-game generation of terrains. NOT YET SUPPORTED - USE ON YOUR OWN RISK. 
		*/

		public class LimeStoneRealtime
		{

				Settings settings = null;
				ArrayList nodes = new ArrayList ();
				CTerrainManager terrainManager = null;
				GameObject parent = null;

				public LimeStoneRealtime (string fname, GameObject settingsObject)
				{
						settings = settingsObject.GetComponent<Settings> ();
						LimeStoneEditor.settings = settings;
						CSerializedNode.Load (fname, nodes);
						parent = settingsObject;

						terrainManager = parent.AddComponent< CTerrainManager> ();
						terrainManager.Init (parent, settings);
						ArrayList outputs = CNodeManager.findType (nodes, typeof(COutput));
						if (outputs.Count != 1) {
								Debug.Log ("LimeStone Error: The file '" + fname + "' contains " + outputs.Count + " outputs, and should only contain 1. Please fix the file.");
								//active = false;
								return;
						}

						COutput output = (COutput)outputs [0];
						terrainManager.calculateInGame (output);

				}

				public void Maintain (Camera c)
				{

				}




		}

}