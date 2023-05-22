using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HandlerController : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {
	
	[HideInInspector]
	public bool isDragging = false;

	private RectTransform rect;
	private Vector3 posDiff;
	
	void Start() {
		
	}

	// Update is called once per frame
	void Update() {
		rect = GetComponent<RectTransform>();
	}

	public void OnBeginDrag(PointerEventData eventData) {
		isDragging = true;
		if (eventData.pointerEnter != null && eventData.pointerEnter.transform as RectTransform != null) {
			RectTransform draggingPlane = eventData.pointerEnter.transform as RectTransform;
			Vector3 globalMousePos;
			if (RectTransformUtility.ScreenPointToWorldPointInRectangle(draggingPlane, eventData.position, eventData.pressEventCamera, out globalMousePos)) {
				posDiff = rect.position - globalMousePos;
			}
		}
	}
	
	public virtual void OnDrag(PointerEventData eventData) {
		if (eventData.pointerEnter != null && eventData.pointerEnter.transform as RectTransform != null) {
			RectTransform draggingPlane = eventData.pointerEnter.transform as RectTransform;
			Vector3 globalMousePos;
			if (RectTransformUtility.ScreenPointToWorldPointInRectangle(draggingPlane, eventData.position, eventData.pressEventCamera, out globalMousePos)) {
				rect.position = posDiff + globalMousePos;
			}
		}
	}

	public void OnEndDrag(PointerEventData eventData) {
		isDragging = false;
	}
}
