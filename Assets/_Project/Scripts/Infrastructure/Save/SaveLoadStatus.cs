namespace UnityRPG.Infrastructure.Save
{
    public enum SaveLoadStatus
    {
        Success,
        FileNotFound,
        InvalidData,
        UnsupportedVersion,
        SceneMismatch,
        CaptureFailed,
        RestoreFailed,
        IoError
    }
}