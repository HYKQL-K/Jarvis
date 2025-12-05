using UnityEngine;

public static class PrivacyMode
{
    public static bool Enabled { get; private set; }

    public static void Set(bool enabled)
    {
        Enabled = enabled;
        if (Enabled)
        {
            Debug.Log("Privacy mode enabled: no disk writes, redaction active.");
        }
    }

    public static object Redact(object payload)
    {
        if (!Enabled || payload == null) return payload;
        return new { redacted = true };
    }
}
