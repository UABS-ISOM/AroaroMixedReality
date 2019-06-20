using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;

namespace PaintIn3D
{
	[CustomEditor(typeof(P3dTeam))]
	public class P3dTeam_Editor : P3dEditor<P3dTeam>
	{
		protected override void OnInspector()
		{
			DrawDefault("color");
		}
	}
}
#endif

namespace PaintIn3D
{
	/// <summary>This component allows you to define a team with a specified color. Put it in its own GameObject so you can give it a unique name.</summary>
	[HelpURL(P3dHelper.HelpUrlPrefix + "P3dTeam")]
	[AddComponentMenu(P3dHelper.ComponentMenuPrefix + "Team")]
	public class P3dTeam : P3dLinkedBehaviour<P3dTeam>
	{
		[SerializeField]
		private class Contribution
		{
			public P3dTeamCounter Counter;
			public int            Count;
		}

		/// <summary>The color associated with this team.</summary>
		public Color Color { set { color = value; } get { return color; } } [SerializeField] private Color color;

		[SerializeField]
		private List<Contribution> contributions;

		public int Total
		{
			get
			{
				var total = 0;

				if (contributions != null)
				{
					for (var i = contributions.Count - 1; i >= 0; i--)
					{
						var contribution = contributions[i];

						if (contribution.Counter != null && contribution.Counter.isActiveAndEnabled == true)
						{
							total += contribution.Count;
						}
						else
						{
							contributions.RemoveAt(i);
						}
					}
				}

				return total;
			}
		}

		public void Contribute(P3dTeamCounter counter, int count)
		{
			if (contributions != null)
			{
				for (var i = contributions.Count - 1; i >= 0; i--)
				{
					var contribution = contributions[i];

					if (contribution.Counter == counter)
					{
						contribution.Count = count;

						return;
					}
				}
			}
			else
			{
				contributions = new List<Contribution>();
			}

			var newContribution = new Contribution();

			newContribution.Counter = counter;
			newContribution.Count   = count;

			contributions.Add(newContribution);
		}
	}
}