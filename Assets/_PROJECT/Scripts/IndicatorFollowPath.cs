using UnityEngine;

public class IndicatorFollowPath : MonoBehaviour
{
	public Transform targetInWorld; // obiekt którego Y chcesz œledziæ (np. przeciwnik / œcie¿ka)
	public RectTransform indicatorRect; // twój indicator w UI
	public Camera uiCamera; // twoja camera przypisana do Canvas

	public float screenYOffset = 0f; // dodatkowy offset w pikselach jeœli chcesz np. +20 px

	void Update()
	{
		// przelicz world -> screen point
		Vector3 screenPoint = uiCamera.WorldToScreenPoint(targetInWorld.position);

		// teraz ustaw tylko Y w indicatorze (bo X jest przyklejony do prawej)
		Vector2 anchoredPos = indicatorRect.anchoredPosition;
		anchoredPos.y = screenPoint.y - (Screen.height * 0.5f) + screenYOffset;

		indicatorRect.anchoredPosition = anchoredPos;
	}
}
