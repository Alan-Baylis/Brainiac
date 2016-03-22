﻿using UnityEngine;
using UnityEditor;
using Brainiac;
using System;

namespace BrainiacEditor
{
	public class BehaviourTreeEditor : EditorWindow
	{
		[SerializeField]
		private Texture m_gridTexture;
		[SerializeField]
		private BTAsset m_btAsset;

		private BTEditorGrid m_grid;
		private BTEditorGraph m_graph;
		private BTEditorCanvas m_canvas;
		private bool m_isDisposed;

		private void OnEnable()
		{
			if(m_gridTexture == null)
			{
				m_gridTexture = Resources.Load<Texture>("Brainiac/background");
			}
			
			if(m_graph == null)
			{
				m_graph = BTEditorGraph.Create();
			}
			if(m_canvas == null)
			{
				m_canvas = new BTEditorCanvas();
				BTEditorCanvas.Current = m_canvas;
			}
			if(m_grid == null)
			{
				m_grid = new BTEditorGrid(m_gridTexture);
			}

			ReloadBehaviourTree();
			m_isDisposed = false;
			m_canvas.OnRepaint += OnRepaint;
			EditorApplication.playmodeStateChanged += HandlePlayModeChanged;
		}

		private void OnDisable()
		{
			Dispose();
		}

		private void OnDestroy()
		{
			Dispose();
		}

		private void Dispose()
		{
			if(!m_isDisposed)
			{
				if(m_graph != null)
				{
					BTEditorGraph.DestroyImmediate(m_graph);
					m_graph = null;
				}
				if(m_btAsset != null)
				{
					SaveBehaviourTree();
					m_btAsset.Dispose();
				}

				EditorApplication.playmodeStateChanged -= HandlePlayModeChanged;
				m_isDisposed = true;
			}
		}

		private void SetBTAsset(BTAsset asset)
		{
			if(asset != null && asset != m_btAsset)
			{
				if(m_btAsset != null)
				{
					SaveBehaviourTree();
					m_btAsset.Dispose();
				}

				m_btAsset = asset;
				m_graph.SetBehaviourTree(m_btAsset.GetEditModeTree());
				m_canvas.Position = m_btAsset.CanvasPosition;
				m_canvas.Size = m_btAsset.CanvasSize;
				m_canvas.IsDebuging = false;
			}
		}

		private void SetBTAssetDebug(BTAsset asset, BehaviourTree btInstance)
		{
			if(asset != null && btInstance != null)
			{
				m_btAsset = asset;
				m_graph.SetBehaviourTree(btInstance);
				m_canvas.Position = m_btAsset.CanvasPosition;
				m_canvas.Size = m_btAsset.CanvasSize;
				m_canvas.IsDebuging = true;
			}
		}

		private void SaveBehaviourTree()
		{
			if(m_btAsset != null)
			{
				m_btAsset.CanvasPosition = m_canvas.Position;
				m_btAsset.CanvasSize = m_canvas.Size;
				m_btAsset.Serialize();
				EditorUtility.SetDirty(m_btAsset);
			}
		}

		private void ReloadBehaviourTree()
		{
			if(m_btAsset != null)
			{
				m_graph.SetBehaviourTree(m_btAsset.GetEditModeTree());
				m_canvas.Position = m_btAsset.CanvasPosition;
				m_canvas.Size = m_btAsset.CanvasSize;
				m_canvas.IsDebuging = false;
			}
		}

		private void HandlePlayModeChanged()
		{
			if(!EditorApplication.isPlaying)
			{
				if(EditorApplication.isPlayingOrWillChangePlaymode)
				{
					SaveBehaviourTree();
				}
				else
				{
					ReloadBehaviourTree();
				}
			}
		}

		private void OnGUI()
		{
			if(m_btAsset != null)
			{
				Rect footerRect = new Rect(-5.0f, position.height - 18, position.width + 5, 20);
				Rect graphRect = new Rect(0.0f, 0.0f, position.width, position.height - footerRect.height);
				Rect canvasRect = new Rect(0.0f, 0.0f, position.width, position.height - footerRect.height);
				string behaviourTreePath = AssetDatabase.GetAssetPath(m_btAsset).Substring(7);

				BTEditorStyle.EnsureStyle();
				m_grid.DrawGUI();
				m_graph.DrawGUI(graphRect);
				m_canvas.HandleEvents(canvasRect);
				
				EditorGUI.LabelField(footerRect, behaviourTreePath, BTEditorStyle.EditorFooter);

				if(m_canvas.IsDebuging)
				{
					OnRepaint();
				}
			}
		}

		public void OnRepaint()
		{
			Repaint();
		}

		public static void Open(BTAsset behaviourTree)
		{
			var window = EditorWindow.GetWindow<BehaviourTreeEditor>("Brainiac");
			window.SetBTAsset(behaviourTree);
		}

		public static void StartDebug(BTAsset btAsset, BehaviourTree btInstance)
		{
			var window = EditorWindow.GetWindow<BehaviourTreeEditor>("Brainiac");
			window.SetBTAssetDebug(btAsset, btInstance);
		}
	}
}