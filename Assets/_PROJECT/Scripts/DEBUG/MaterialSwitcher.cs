using UnityEngine;

public class MaterialSwitcher : MonoBehaviour
{
	[SerializeField] private Renderer targetRenderer;
	[SerializeField] private Material material1;
	[SerializeField] private Material material2;

	private bool usingFirstMaterial = true;

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.X))
		{
			if (targetRenderer != null)
			{
				Material[] mats = targetRenderer.materials;

				for (int i = 0; i < mats.Length; i++)
				{
					mats[i] = usingFirstMaterial ? material2 : material1;
				}

				targetRenderer.materials = mats;
				usingFirstMaterial = !usingFirstMaterial;

				Debug.Log("Materia³ podmieniony na: " + (usingFirstMaterial ? "material1" : "material2"));
			}
		}
	}
}
