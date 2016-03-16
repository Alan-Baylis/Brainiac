using UnityEngine;

namespace Brainiac
{
	[AddBehaviourNodeMenu("Composite/Sequence")]
	public class Sequence : Composite
	{
		private int m_currentChild;

		public override string Title
		{
			get
			{
				return "Sequence";
			}
		}

		protected override BehaviourNodeStatus OnExecute(Agent agent)
		{
			BehaviourNodeStatus status = BehaviourNodeStatus.Success;
			while(m_currentChild < m_children.Count)
			{
				status = m_children[m_currentChild].Run(agent);
				if (status == BehaviourNodeStatus.Success)
					m_currentChild++;
				else
					break;
			}

			return status;
		}

		protected override void OnStart(Agent agent)
		{
			base.OnStart(agent);
			m_currentChild = 0;
		}
	}
}
