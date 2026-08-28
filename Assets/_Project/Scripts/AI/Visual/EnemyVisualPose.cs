using UnityEngine;

namespace UnityRPG.AI
{
    public sealed class EnemyVisualPose
    {
        public Transform ModelRoot { get; }
        public Transform Body { get; }

        public Vector3 BodyBasePosition { get; }
        public Quaternion BodyBaseRotation { get; }
        public Vector3 ModelRootBaseScale { get; }
        public Quaternion ModelRootBaseRotation { get; }

        public EnemyVisualPose(Transform modelRoot, Transform body)
        {
            ModelRoot = modelRoot;
            Body = body;

            BodyBasePosition = body.localPosition;
            BodyBaseRotation = body.localRotation;
            ModelRootBaseScale = modelRoot.localScale;
            ModelRootBaseRotation = modelRoot.localRotation;
        }

        public void ResetModelRoot()
        {
            ModelRoot.localScale = ModelRootBaseScale;
        }
    }
}