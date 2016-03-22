﻿using UnityEngine;

namespace Brainiac
{
	[AddNodeMenu("Action/Run Behaviour")]
	public class RunBehaviour : Action
	{
		private string m_behaviourTreePath;
		private BehaviourTree m_behaviourTree;

		public string BehaviourTreePath
		{
			get
			{
				return m_behaviourTreePath;
			}
			set
			{
				m_behaviourTreePath = value;
			}
		}

		public BehaviourTree BehaviourTree
		{
			get
			{
				return m_behaviourTree;
			}
		}

		public override void OnAwake()
		{
			base.OnAwake();

			if(!string.IsNullOrEmpty(m_behaviourTreePath))
			{
				BTAsset asset = Resources.Load<BTAsset>(m_behaviourTreePath);
				if(asset != null)
				{
					m_behaviourTree = asset.CreateRuntimeTree();
				}
			}
			else
			{
				m_behaviourTree = null;
			}
		}

		protected override BehaviourNodeStatus OnExecute(Agent agent)
		{
			if(m_behaviourTree != null)
			{
				return m_behaviourTree.Root.Run(agent);
			}
			else
			{
				return BehaviourNodeStatus.Failure;
			}
		}
	}
}