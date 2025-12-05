using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public sealed class PerfOverlay : MonoBehaviour
{
    [SerializeField] private bool visible = true;
    [SerializeField] private Rect rect = new Rect(10, 10, 260, 120);

    private readonly List<float> _latenciesMs = new();
    private readonly Queue<float> _wakeTimestamps = new();
    private float _fpsSmoothed;

    private void Update()
    {
        var dt = Time.deltaTime;
        _fpsSmoothed = Mathf.Lerp(_fpsSmoothed, 1f / Mathf.Max(0.001f, dt), 0.1f);
    }

    private void OnGUI()
    {
        if (!visible) return;

        GUI.Box(rect, "Perf");
        GUILayout.BeginArea(rect);
        GUILayout.Label($"FPS: {_fpsSmoothed:F1}");

        if (_latenciesMs.Count > 0)
        {
            var sorted = _latenciesMs.OrderBy(x => x).ToArray();
            float p50 = sorted[(int)(sorted.Length * 0.5f)];
            float p95 = sorted[(int)(sorted.Length * 0.95f)];
            GUILayout.Label($"ASR Latency P50: {p50:F0} ms");
            GUILayout.Label($"ASR Latency P95: {p95:F0} ms");
        }
        else
        {
            GUILayout.Label("ASR Latency: n/a");
        }

        PruneWakeTimestamps();
        GUILayout.Label($"Wake detections/min: {_wakeTimestamps.Count}");
        GUILayout.EndArea();
    }

    public void RecordAsrLatencyMs(float ms)
    {
        _latenciesMs.Add(ms);
        if (_latenciesMs.Count > 200)
        {
            _latenciesMs.RemoveRange(0, _latenciesMs.Count - 200);
        }
    }

    public void RecordWake()
    {
        _wakeTimestamps.Enqueue(Time.time);
        PruneWakeTimestamps();
    }

    private void PruneWakeTimestamps()
    {
        float cutoff = Time.time - 60f;
        while (_wakeTimestamps.Count > 0 && _wakeTimestamps.Peek() < cutoff)
        {
            _wakeTimestamps.Dequeue();
        }
    }
}
