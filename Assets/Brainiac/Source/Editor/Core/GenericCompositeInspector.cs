﻿using UnityEngine;
using UnityEditor;
using System;
using Brainiac;

namespace BrainiacEditor
{
	public class GenericCompositeInspector : GenericNodeInspector
	{
		protected const int HEADER_HEIGHT = 18;
		protected const int ITEM_SPACING_VERT = 5;
		protected const int ITEM_SPACING_HORZ = 4;
		protected const int FIELD_HEIGHT = 20;

		public override void OnInspectorGUI(BehaviourNode node)
		{
			if(node is Composite)
			{
				Composite composite = (Composite)node;

				composite.Name = EditorGUILayout.TextField("Name", composite.Name);
				EditorGUILayout.LabelField("Description");
				composite.Description = EditorGUILayout.TextArea(composite.Description, BTEditorStyle.MultilineTextArea);

				EditorGUILayout.Space();
				DrawChildren(composite);

				EditorGUILayout.Space();
				DrawProperties(composite);

				if(BTEditorCanvas.Current != null)
				{
					BTEditorCanvas.Current.Repaint();
				}
			}
		}

		protected void DrawChildren(Composite composite)
		{
			float itemCount = composite.ChildCount;
			float itemHeight = FIELD_HEIGHT;

			Rect groupRect = GUILayoutUtility.GetRect(0, CalculateContentHeight(composite), GUILayout.ExpandWidth(true));
			Rect headerRect = new Rect(0.0f, 0.0f, groupRect.width, HEADER_HEIGHT);
			Rect bgRect = new Rect(headerRect.x, headerRect.yMax, headerRect.width, itemCount * itemHeight + ITEM_SPACING_VERT * 2);
			Rect itemRect = new Rect(bgRect.x + ITEM_SPACING_HORZ, bgRect.y + ITEM_SPACING_VERT, bgRect.width - ITEM_SPACING_HORZ * 2, bgRect.height - ITEM_SPACING_VERT);
			
			GUI.BeginGroup(groupRect);

			EditorGUI.LabelField(headerRect, "Children", BTEditorStyle.ListHeader);
			GUI.Box(bgRect, "", BTEditorStyle.ListBackground);

			GUI.BeginGroup(itemRect);

			for(int i = 0; i < composite.ChildCount; i++)
			{
				Rect handleRect = new Rect(0, i * itemHeight + itemHeight / 4, 10, itemHeight);
				Rect childRect = new Rect(15, i * itemHeight, itemRect.width - 55, itemHeight);
				Rect upButtonRect = new Rect(childRect.xMax + 5, childRect.y + 5, 10, 10);
				Rect downButtonRect = new Rect(upButtonRect.xMax + 5, childRect.y + 5, 10, 10);
				BehaviourNode child = composite.GetChild(i);
				string childName = string.IsNullOrEmpty(child.Name) ? child.Title : child.Name;

				EditorGUI.LabelField(handleRect, "", BTEditorStyle.ListDragHandle);
				EditorGUI.LabelField(childRect, childName);

				if(i > 0)
				{
					if(GUI.Button(upButtonRect, "", BTEditorStyle.ArrowUp))
					{
						composite.MoveChildPriorityUp(i);
					}
				}
				if(i < composite.ChildCount - 1)
				{
					if(GUI.Button(downButtonRect, "", BTEditorStyle.ArrowDown))
					{
						composite.MoveChildPriorityDown(i);
					}
				}
			}

			GUI.EndGroup();

			GUI.EndGroup();
		}

		private float CalculateContentHeight(Composite composite)
		{
			float itemCount = composite.ChildCount;
			float itemHeight = FIELD_HEIGHT;

			return itemCount * itemHeight + HEADER_HEIGHT + ITEM_SPACING_VERT * 2;
		}
	}
}
