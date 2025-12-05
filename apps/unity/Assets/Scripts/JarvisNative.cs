using System;
using System.Runtime.InteropServices;

public static class JarvisNative
{
#if UNITY_IOS && !UNITY_EDITOR
    private const string AudioLib = "__Internal";
    private const string WakeLib = "__Internal";
#else
    private const string AudioLib = "jarvis_audio";
    private const string WakeLib = "jarvis_wake";
#endif

    [StructLayout(LayoutKind.Sequential)]
    public struct AudioParams
    {
        public int sample_rate_hz;
        public int num_channels;
        public int frame_samples;
        [MarshalAs(UnmanagedType.I1)] public bool enable_aec;
        [MarshalAs(UnmanagedType.I1)] public bool enable_ns;
        [MarshalAs(UnmanagedType.I1)] public bool enable_agc;
        [MarshalAs(UnmanagedType.I1)] public bool enable_vad;
    }

    [DllImport(AudioLib, CallingConvention = CallingConvention.Cdecl)]
    public static extern int jarvis_audio_init(ref AudioParams parms);

    [DllImport(AudioLib, CallingConvention = CallingConvention.Cdecl)]
    public static extern int jarvis_audio_process(short[] pcm, int n, [MarshalAs(UnmanagedType.I1)] ref bool isSpeech);

    [DllImport(AudioLib, CallingConvention = CallingConvention.Cdecl)]
    public static extern void jarvis_audio_close();

    [DllImport(WakeLib, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern int jarvis_wake_init(string model_path, float sensitivity);

    [DllImport(WakeLib, CallingConvention = CallingConvention.Cdecl)]
    public static extern int jarvis_wake_score(short[] pcm, int n, out float score);

    [DllImport(WakeLib, CallingConvention = CallingConvention.Cdecl)]
    public static extern int jarvis_wake_reset();

    [DllImport(WakeLib, CallingConvention = CallingConvention.Cdecl)]
    public static extern void jarvis_wake_close();

    [DllImport("jarvis_asr", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern int jarvis_asr_init(string model_dir, string lang);

    [DllImport("jarvis_asr", CallingConvention = CallingConvention.Cdecl)]
    public static extern int jarvis_asr_start_stream();

    [DllImport("jarvis_asr", CallingConvention = CallingConvention.Cdecl)]
    public static extern int jarvis_asr_push(short[] pcm, int n);

    [DllImport("jarvis_asr", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern int jarvis_asr_pull_partial(System.Text.StringBuilder buf, UIntPtr len);

    [DllImport("jarvis_asr", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern int jarvis_asr_finalize(System.Text.StringBuilder buf, UIntPtr len);

    [DllImport("jarvis_asr", CallingConvention = CallingConvention.Cdecl)]
    public static extern void jarvis_asr_close();

    [DllImport("jarvis_tts", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern int jarvis_tts_init(string voice, float speed);

    [DllImport("jarvis_tts", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern int jarvis_tts_synthesize(string text, string out_wav_path);

    [DllImport("jarvis_tts", CallingConvention = CallingConvention.Cdecl)]
    public static extern void jarvis_tts_shutdown();
}
