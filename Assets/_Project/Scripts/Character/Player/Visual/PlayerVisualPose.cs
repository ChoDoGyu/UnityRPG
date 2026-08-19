using UnityEngine;

namespace UnityRPG.Character.Player
{
    public sealed class PlayerVisualPose
    {
        public Transform ModelRoot { get; }
        public Transform Body { get; }
        public Transform LeftHand { get; }
        public Transform RightHand { get; }
        public Transform LeftFoot { get; }
        public Transform RightFoot { get; }

        public Vector3 BodyBasePosition { get; }
        public Vector3 LeftHandBasePosition { get; }
        public Vector3 RightHandBasePosition { get; }
        public Vector3 LeftFootBasePosition { get; }
        public Vector3 RightFootBasePosition { get; }

        public Quaternion BodyBaseRotation { get; }

        public Quaternion ModelRootBaseRotation { get; }
        public Vector3 ModelRootBaseScale { get; }

        public PlayerVisualPose(
            Transform modelRoot,
            Transform body,
            Transform leftHand,
            Transform rightHand,
            Transform leftFoot,
            Transform rightFoot)
        {
            ModelRoot = modelRoot;
            Body = body;
            LeftHand = leftHand;
            RightHand = rightHand;
            LeftFoot = leftFoot;
            RightFoot = rightFoot;

            BodyBasePosition = body.localPosition;
            LeftHandBasePosition = leftHand.localPosition;
            RightHandBasePosition = rightHand.localPosition;
            LeftFootBasePosition = leftFoot.localPosition;
            RightFootBasePosition = rightFoot.localPosition;

            BodyBaseRotation = body.localRotation;

            ModelRootBaseRotation = modelRoot.localRotation;
            ModelRootBaseScale = modelRoot.localScale;
        }

        public void ResetModelRoot()
        {
            ModelRoot.localRotation = ModelRootBaseRotation;
            ModelRoot.localScale = ModelRootBaseScale;
        }
    }
}