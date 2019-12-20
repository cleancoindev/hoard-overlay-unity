namespace WebGLPlugin
{
#if UNITY_WEBG
    public class WebGLHelper
    {
        public static void SyncFilesystem()
        {
            if (Application.platform == RuntimePlatform.WebGLPlayer)
            {
                SyncFiles();
            }
        }
    
        [System.Runtime.InteropServices.DllImport("__Internal")]
        private static extern void SyncFiles();
    }
#endif
}
