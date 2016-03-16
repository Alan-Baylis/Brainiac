﻿using UnityEngine;
using System;
using Brainiac;

namespace BrainiacEditor
{
	public class UndoNodeDeleted : BTUndoState
	{
		private BTEditorGraph m_graph;
		private string m_createdNodePath;
		private string m_parentNodePath;
		private string m_serializedNode;
		private int m_childIndex;

		public override bool CanUndo
		{
			get
			{
				return m_parentNodePath != null && !string.IsNullOrEmpty(m_serializedNode);
			}
		}

		public override bool CanRedo
		{
			get
			{
				return m_createdNodePath != null;
			}
		}

		public UndoNodeDeleted(BTEditorGraphNode node)
		{
			m_graph = node.Graph;
			m_parentNodePath = m_graph.GetNodePath(node.Parent);
			m_serializedNode = BTUtils.SaveNode(node.Node);
			m_childIndex = node.Parent.GetChildIndex(node);
			Title = "Deleted " + node.Node.Title;

			m_createdNodePath = null;
		}

		public UndoNodeDeleted(BTEditorGraphNode node, int childIndex)
		{
			m_graph = node.Graph;
			m_parentNodePath = m_graph.GetNodePath(node.Parent);
			m_serializedNode = BTUtils.SaveNode(node.Node);
			m_childIndex = childIndex;
			Title = "Deleted " + node.Node.Title;

			m_createdNodePath = null;
		}

		public override void Undo()
		{
			if(CanUndo)
			{
				BehaviourNode node = BTUtils.LoadNode(m_serializedNode);
				if(m_childIndex >= 0)
				{
					var parentNode = m_graph.GetNodeAtPath(m_parentNodePath);
					var createdNode = BTEditorGraphNode.Create(parentNode, node, m_childIndex);
					m_createdNodePath = m_graph.GetNodePath(createdNode);
				}
				else
				{
					var parentNode = m_graph.GetNodeAtPath(m_parentNodePath);
					var createdNode = BTEditorGraphNode.Create(parentNode, node);
					m_createdNodePath = m_graph.GetNodePath(createdNode);
				}

				m_parentNodePath = null;
				m_serializedNode = null;
			}
		}

		public override void Redo()
		{
			if(CanRedo)
			{
				var createdNode = m_graph.GetNodeAtPath(m_createdNodePath);
				m_parentNodePath = m_graph.GetNodePath(createdNode.Parent);
				m_serializedNode = BTUtils.SaveNode(createdNode.Node);
				m_childIndex = createdNode.Parent.GetChildIndex(createdNode);

				createdNode.OnDelete();
				m_createdNodePath = null;
			}
		}
	}
}