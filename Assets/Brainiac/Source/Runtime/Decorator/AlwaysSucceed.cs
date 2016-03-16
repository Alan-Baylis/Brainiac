﻿using UnityEngine;

namespace Brainiac
{
	[AddNodeMenu("Decorator/Always Succeed")]
	public class AlwaysSucceed : Decorator
	{
		public override string Title
		{
			get
			{
				return "Always Succeed";
			}
		}

		protected override BehaviourNodeStatus OnExecute(Agent agent)
		{
			if(m_child == null)
				return BehaviourNodeStatus.Success;

			BehaviourNodeStatus status = m_child.Run(agent);
			if(status == BehaviourNodeStatus.Running)
			{
				return BehaviourNodeStatus.Running;
			}

			return BehaviourNodeStatus.Success;
		}
	}
}