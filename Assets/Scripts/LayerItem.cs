using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LayerItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {

	public static int count = -1;

	[HideInInspector]
	public RectTransform upper;
	[HideInInspector]
	public RectTransform lower;
	[HideInInspector]
	public Transform image;

	private RectTransform rect;
	private float posDiff;

	private bool isDragging = false;
	
	void Start() {
		rect = GetComponent<RectTransform>();
	}

	void Update() {
		if (!isDragging) {
			rect.anchoredPosition = Vector3.Lerp(rect.anchoredPosition, upper.anchoredPosition - new Vector2(0, 216), 0.1f);
		}
	}

	public void OnBeginDrag(PointerEventData eventData) {
		isDragging = true;
		if (eventData.pointerEnter != null && eventData.pointerEnter.transform as RectTransform != null) {
			RectTransform draggingPlane = eventData.pointerEnter.transform as RectTransform;
			Vector3 globalMousePos;
			if (RectTransformUtility.ScreenPointToWorldPointInRectangle(draggingPlane, eventData.position, eventData.pressEventCamera, out globalMousePos)) {
				posDiff = rect.position.y - globalMousePos.y;
			}
		}
	}
	
	public void OnDrag(PointerEventData eventData) {
		if (eventData.pointerEnter != null && eventData.pointerEnter.transform as RectTransform != null) {
			RectTransform draggingPlane = eventData.pointerEnter.transform as RectTransform;
			Vector3 globalMousePos;
			if (RectTransformUtility.ScreenPointToWorldPointInRectangle(draggingPlane, eventData.position, eventData.pressEventCamera, out globalMousePos)) {
				rect.position = new Vector3(rect.position.x, posDiff + globalMousePos.y, rect.position.z);
			}
		}
		if (rect.anchoredPosition.y - upper.anchoredPosition.y > -108) {
			RectTransform upperupper = upper.GetComponent<LayerItem>().upper;
			if (upperupper.GetComponent<LayerItem>() != null) {
				upperupper.GetComponent<LayerItem>().lower = rect;
			}
			else {
				LayerMenuController.first = rect;
			}
			upper.GetComponent<LayerItem>().upper = rect;
			upper.GetComponent<LayerItem>().lower = lower;
			if (lower != null) {
				lower.GetComponent<LayerItem>().upper = upper;
			}
			lower = upper;
			upper = upperupper;
			getUppest().reorderImages();
		}
		if (rect.anchoredPosition.y - upper.anchoredPosition.y < -324 && lower != null) {
			RectTransform lowerlower = lower.GetComponent<LayerItem>().lower;
			if (lowerlower != null) {
				lowerlower.GetComponent<LayerItem>().upper = rect;
			}
			if (upper.GetComponent<LayerItem>() != null) {
				upper.GetComponent<LayerItem>().lower = lower;
			}
			lower.GetComponent<LayerItem>().upper = upper;
			lower.GetComponent<LayerItem>().lower = rect;
			upper = lower;
			lower = lowerlower;
			getUppest().reorderImages();
		}
	}

	public void OnEndDrag(PointerEventData eventData) {
		isDragging = false;
	}

	private void reorderImages() {
		image.SetSiblingIndex(count - getOrder());
		if (lower != null) {
			lower.GetComponent<LayerItem>().reorderImages();
		}
	}

	private LayerItem getUppest() {
		if (upper.GetComponent<LayerItem>() == null) {
			return this;
		}
		else {
			return upper.GetComponent<LayerItem>().getUppest();
		}
	}

	private int getOrder() {
		if (upper.GetComponent<LayerItem>() == null) {
			return 0;
		}
		else {
			return upper.GetComponent<LayerItem>().getOrder() + 1;
		}
	}

}
