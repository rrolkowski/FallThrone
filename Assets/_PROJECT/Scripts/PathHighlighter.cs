using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathHighlighter : MonoBehaviour
{
	[ColorUsage(true, true)]
	[SerializeField] private Color _highlightColor = Color.yellow;

	[Header("Settings")]
	[SerializeField] private float _intensity = 1f;
	[SerializeField] private float _stepDelay = 0.1f;
	[SerializeField] private float _highlightDuration = 0.4f;

	[Header("A* References")]
	[SerializeField] private GridManager _gridManager;
	[SerializeField] private PathFinderManager _pathManager;

	[Header("Checkpoint Positions")]
	[SerializeField] private Vector3Int _mustPassA1;
	[SerializeField] private Vector3Int _mustPassA2;
	[SerializeField] private Vector3Int _mustPassA3;
	[Header("")]
	[SerializeField] private Vector3Int _mustPassB1;
	[SerializeField] private Vector3Int _mustPassB2;
	[SerializeField] private Vector3Int _mustPassB3;

	[Header("Audio")]
	[SerializeField] private AudioSource _highlightAudioSource;

	[Header("Audio Pitch Settings")]
	[SerializeField] private float _startPitch = 0.8f;
	[SerializeField] private float _endPitch = 1.2f;

	[Header("Audio Playback Settings")]
	[SerializeField] private float _playbackSpeed = 1.0f;

	private Coroutine _pitchCoroutine = null;
	private Dictionary<Vector3Int, Transform> _tileLookup = new();
	private HashSet<Renderer> _activeFlashes = new();

	private bool _isHighlighting = false;
	private float _targetPitch = 1f;

	private bool IsAudioPauseState => GameState.STATE_Paused || GameState.STATE_Shop;

	private void Start()
	{
		BuildTileLookup();
		StartCoroutine(DelayedStart());
	}

	private IEnumerator DelayedStart()
	{
		yield return new WaitForSeconds(1f);
		StartCoroutine(WaitUntilReadyAndHighlight());
	}

	private void BuildTileLookup()
	{
		_tileLookup.Clear();

		if (_gridManager.tilemap.transform == null) return;

		foreach (Transform child in _gridManager.tilemap.transform)
		{
			Vector3Int gridPos = _gridManager.tilemap.WorldToCell(child.position);
			if (!_tileLookup.ContainsKey(gridPos))
				_tileLookup.Add(gridPos, child);
		}
	}

	private IEnumerator WaitUntilReadyAndHighlight()
	{
		yield return new WaitForSeconds(0.5f);

		var spawnPoints = _pathManager.GetSpawnPositions();
		if (spawnPoints == null || spawnPoints.Count == 0)
			yield break;

		if (spawnPoints.Count == 1)
		{
			var spawn = spawnPoints[0];

			var pathA1 = _pathManager.GetPathFromTo(spawn, _mustPassA1);
			var pathA2 = _pathManager.GetPathFromTo(_mustPassA1, _mustPassA2);
			var pathA3 = _pathManager.GetPathFromTo(_mustPassA2, _mustPassA3);
			var pathA4 = _pathManager.GetPathFromTo(_mustPassA3, _pathManager.endPoint);

			if (pathA1 != null && pathA2 != null && pathA3 != null && pathA4 != null)
			{
				var fullA = new List<TileNode>(pathA1);
				fullA.AddRange(pathA2);
				fullA.AddRange(pathA3);
				fullA.AddRange(pathA4);
				StartCoroutine(HighlightPath(fullA));
			}

			var pathB1 = _pathManager.GetPathFromTo(spawn, _mustPassB1);
			var pathB2 = _pathManager.GetPathFromTo(_mustPassB1, _mustPassB2);
			var pathB3 = _pathManager.GetPathFromTo(_mustPassB2, _mustPassB3);
			var pathB4 = _pathManager.GetPathFromTo(_mustPassB3, _pathManager.endPoint);

			if (pathB1 != null && pathB2 != null && pathB3 != null && pathB4 != null)
			{
				var fullB = new List<TileNode>(pathB1);
				fullB.AddRange(pathB2);
				fullB.AddRange(pathB3);
				fullB.AddRange(pathB4);
				StartCoroutine(HighlightPath(fullB));
			}
		}
		else
		{
			foreach (var spawn in spawnPoints)
			{
				var path = _pathManager.GetPathFromTo(spawn, _pathManager.endPoint);
				if (path != null)
					StartCoroutine(HighlightPath(path));
			}
		}
	}

	private IEnumerator HighlightPath(List<TileNode> path)
	{
		Debug.Log($"[PathHighlighter] START Highlighting path with {path.Count} tiles");
		_isHighlighting = true;

		if (_highlightAudioSource != null)
		{
			_highlightAudioSource.loop = true;
			_highlightAudioSource.pitch = _playbackSpeed * _startPitch;
			_targetPitch = _startPitch;
			_highlightAudioSource.Play();

			if (_pitchCoroutine != null)
				StopCoroutine(_pitchCoroutine);

			_pitchCoroutine = StartCoroutine(AnimatePitch(path.Count * _stepDelay));
		}

		foreach (var node in path)
		{
			if (!_tileLookup.TryGetValue(node.position, out Transform tile))
				continue;

			Renderer rend = null;
			foreach (Renderer r in tile.GetComponentsInChildren<Renderer>())
			{
				if (r.enabled)
				{
					rend = r;
					break;
				}
			}

			if (rend == null)
				continue;

			StartCoroutine(FlashTile(rend));
			yield return new WaitForSeconds(_stepDelay);
		}

		if (_pitchCoroutine != null)
		{
			StopCoroutine(_pitchCoroutine);
			_pitchCoroutine = null;
		}

		if (_highlightAudioSource != null)
		{
			_highlightAudioSource.Stop();
			_highlightAudioSource.pitch = _playbackSpeed * 1.0f;
		}

		_isHighlighting = false;
		Debug.Log("[PathHighlighter] END Highlighting path");
	}

	private IEnumerator AnimatePitch(float duration)
	{
		float timeElapsed = 0f;

		while (timeElapsed < duration)
		{
			float t = timeElapsed / duration;
			_targetPitch = Mathf.Lerp(_startPitch, _endPitch, t);
			timeElapsed += Time.deltaTime;
			yield return null;
		}

		_targetPitch = _endPitch;
	}

	private void Update()
	{
		if (!_isHighlighting || _highlightAudioSource == null)
			return;

		if (IsAudioPauseState)
		{
			if (_highlightAudioSource.isPlaying)
				_highlightAudioSource.Pause();
		}
		else
		{
			if (!_highlightAudioSource.isPlaying)
				_highlightAudioSource.UnPause();

			_highlightAudioSource.pitch = _playbackSpeed * _targetPitch;
		}
	}

	private IEnumerator FlashTile(Renderer rend)
	{
		if (_activeFlashes.Contains(rend))
			yield break;

		_activeFlashes.Add(rend);

		Material mat = rend.material;
		Color originalEmission = mat.GetColor("_EmissionColor");

		mat.EnableKeyword("_EMISSION");
		mat.SetColor("_EmissionColor", _highlightColor * _intensity);

		yield return new WaitForSeconds(_highlightDuration);

		mat.SetColor("_EmissionColor", originalEmission);

		_activeFlashes.Remove(rend);
	}
}
