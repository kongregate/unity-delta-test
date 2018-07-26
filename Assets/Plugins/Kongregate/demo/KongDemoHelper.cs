using UnityEngine;
using System.Collections;

namespace Kongregate
{

	public class KongDemoHelper
	{

		public static GUIStyle buttonStyle;
		public static GUIStyle labelStyle;
		public static GUIStyle textFieldStyle;

		public static void PrepareGUI() {
			//write your GUI elements for 1280x720
			GUI.matrix = Matrix4x4.TRS(new Vector3(0, 0, 0), Quaternion.identity, new Vector3(Screen.width / 1280.0f, Screen.height / 720.0f, 1));
		}


		static KongDemoHelper ()
		{
			int fontSize = (int)(10.0f * (float)(Screen.width) / 1280.0f);
			buttonStyle = new GUIStyle (GUI.skin.button);
			buttonStyle.fontSize = fontSize;

			labelStyle = new GUIStyle (GUI.skin.label);
			labelStyle.fontSize = fontSize;
			labelStyle.normal.textColor = Color.black;

			textFieldStyle = new GUIStyle (GUI.skin.textField);
			textFieldStyle.fontSize = fontSize;
		}
	}
}