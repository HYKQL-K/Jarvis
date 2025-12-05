using System;
using System.Collections.Concurrent;
using UnityEngine;

public sealed class ToolBridge : MonoBehaviour
{
    private static readonly ConcurrentQueue<string> OpenAppQueue = new();

    private void OnEnable()
    {
        EventBus.OnToolStarted += HandleToolStarted;
    }

    private void OnDisable()
    {
        EventBus.OnToolStarted -= HandleToolStarted;
    }

    private void Update()
    {
        while (OpenAppQueue.TryDequeue(out var app))
        {
            PlatformActions.OpenApp(app);
            EventBus.EmitToolFinished($"open_app:{app}");
        }
    }

    public static void QueueOpenApp(string app)
    {
        if (!string.IsNullOrEmpty(app))
        {
            OpenAppQueue.Enqueue(app);
        }
    }

    private void HandleToolStarted(string tool)
    {
        if (string.IsNullOrEmpty(tool)) return;
        if (tool.StartsWith("open_app", StringComparison.OrdinalIgnoreCase))
        {
            var parts = tool.Split(':');
            var app = parts.Length > 1 ? parts[1] : "browser";
            QueueOpenApp(app);
        }
    }
}
