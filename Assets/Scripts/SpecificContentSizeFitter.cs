using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[ExecuteInEditMode]
public class SpecificContentSizeFitter : MonoBehaviour {
	[SerializeField] LayoutElement layout;
	[SerializeField] GameObject targetContent;
	[SerializeField] Vector2 padding;
	ILayoutElement targetLayout;

	void Awake(){
		targetLayout = targetContent.GetComponent<ILayoutElement>();
		layout = GetComponent<LayoutElement>();
	}

	void Update(){
		layout.minHeight = targetLayout.preferredHeight + padding.y;
	}
}
