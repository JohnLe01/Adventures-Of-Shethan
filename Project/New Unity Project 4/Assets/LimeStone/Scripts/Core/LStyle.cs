using UnityEngine;
using System.Collections;

namespace LimeStone
{
		/*
		* LimeStone styles, constants, colors and whatnot. 
		*/

		public class LStyle
		{
				public static int LargeFontSize = 20;
				public static int FontSize = 20;
				
				
				public static int ConnectionTypeX = 20;
				public static int ConnectionTypeY = 25;
		
				public static string PresetsFilename = "Assets/LimeStone/Data/Presets/presets.dat";
				public static string SnippetDirectory = "Assets/LimeStone/Data/Snippets/";
				public static string TerrainDirectory = "Assets/LimeStone/Data/Terrains/";
		
				public static Color[] Colors = { 
					new Color (0, 67f / 255f, 88f / 255f, 1.0f), 
			//		new Color(31f/255f,138f/255f, 112f/255f, 1.0f),
					new Color (31f / 255f, 138f / 255f, 70f / 255f, 1.0f),
					new Color (190f / 255f, 219f / 255f, 57f / 255f, 1.0f),
			//new Color(255f/255f,255f/255f, 26f/255f, 1.0f) ,
					new Color (26f / 255f, 105f / 255f, 255f / 255f, 1.0f) ,
					new Color (253f / 255f, 116f / 255f, 0f / 255f, 1.0f) 
					};
		
		// Colors of the links
		public static Color[] connectionColors = {
			Colors [1] * 1.5f,
			Colors [4],
			Colors [0] * 3,
			Colors [0] * 2
		};
		
		// Display name of textures		
		public static string[] TextureNames = {
			"Texture 0",
			"Texture 1",
			"Texture 2",
			"Normal map" ,
			"WTF"
		};
		
		
		public static string[] hexColors = {
					"<color=" + Util.ColorToHex (Colors [0]) + ">", 
					"<color=" + Util.ColorToHex (Colors [1]) + ">", 
					"<color=" + Util.ColorToHex (Colors [2]) + ">", 
					"<color=" + Util.ColorToHex (Colors [3]) + ">", 
					"<color=" + Util.ColorToHex (Colors [4]) + ">" };
					
					
		public static string TerrainFiletype = "ter";
		public static string TerrainGameobjectName = "Terrains";
		
		// Check if everything is initialized		
		public static int NO_TEXTURES = 3;
		
	}
	
	
	
}