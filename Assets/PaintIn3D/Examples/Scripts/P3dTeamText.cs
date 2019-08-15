using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;

namespace PaintIn3D
{
	[CustomEditor(typeof(P3dTeamText))]
	public class P3dTeamText_Editor : P3dEditor<P3dTeamText>
	{
		protected override void OnInspector()
		{
			BeginError(Any(t => t.Team == null));
				DrawDefault("team");
			EndError();
		}
	}
}
#endif

namespace PaintIn3D
{
	/// <summary>This component will output the total pixels for the specified team to a UI Text component.</summary>
	[RequireComponent(typeof(Text))]
	[HelpURL(P3dHelper.HelpUrlPrefix + "P3dTeamText")]
	[AddComponentMenu(P3dHelper.ComponentMenuPrefix + "Team Text")]
	public class P3dTeamText : MonoBehaviour
	{
		/// <summary>This allows you to set which team will be handled by this component.</summary>
		public P3dTeam Team { set { team = value; } get { return team; } } [SerializeField] private P3dTeam team;

		[System.NonSerialized]
		private Text cachedText;

		protected virtual void OnEnable()
		{
			cachedText = GetComponent<Text>();
		}

		protected virtual void Update()
		{
			if (team != null)
			{
				cachedText.text = team.name + " = " + team.Total;
			}
		}
	}
}