using System.Collections;
using UnityEngine;

public class RangeBoost : BasePowerUp
{
    [SerializeField] private float _newRangeSize = 2f;
    [SerializeField] private float _powerUpDuration = 15f;

    public override void ApplyEffect()
    {       
        RangeCircleController rangeCircle = Resources.FindObjectsOfTypeAll<RangeCircleController>()[0];
        ObjectGrabber objectGrabber = FindFirstObjectByType<ObjectGrabber>();
        if (rangeCircle != null)
        {
            Debug.Log("Acrivated range!");
            GameController.Instance.StartPowerUpTimer(_powerUpDuration);
            StartCoroutine(SpeedBoostCoroutine(rangeCircle, objectGrabber));
        }
	}

    IEnumerator SpeedBoostCoroutine(RangeCircleController rangeCircle, ObjectGrabber objectGrabber)
    {
        float originalSize = rangeCircle.GetRawRange();
        float boostedSize = originalSize * _newRangeSize;

        float originalDistance = objectGrabber.GetRawThrowRange();
        float boostedDistance = originalDistance * _newRangeSize;

        Vector3 originalScale = rangeCircle.transform.localScale;
        Vector3 boostedScale = originalScale * _newRangeSize * 1.1f;

        rangeCircle.SetRange(boostedSize);
        objectGrabber.SetThrowRange(boostedDistance);
        rangeCircle.transform.localScale = boostedScale;

        Debug.Log("Effect start!");

        yield return new WaitForSeconds(_powerUpDuration);

        GameController.Instance.ClearPowerUp();
        rangeCircle.SetRange(originalSize);
        objectGrabber.SetThrowRange(originalDistance);
        rangeCircle.transform.localScale = originalScale;
        Debug.Log("Effect end!");

        Destroy(gameObject);
    }

	protected override SoundType GetPickupSound()
	{
		return SoundType.GAME_PowerUp_Range; // <- albo inny pasuj¹cy dŸwiêk
	}
}
