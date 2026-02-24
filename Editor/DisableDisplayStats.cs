#if UNITY_EDITOR
namespace KadenZombie8.BIMOS
{
    public static class DisableDisplayStats
    {
        [UnityEditor.InitializeOnLoadMethod]
        private static void OnEditorLoaded() => UnityEngine.Rendering.DebugManager.instance.enableRuntimeUI = false;
    }
}
#endif
