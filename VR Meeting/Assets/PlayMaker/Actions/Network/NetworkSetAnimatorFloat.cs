using UnityEngine;
using Unity.Netcode;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Network)]
    [Tooltip("Sets the value of a float parameter")]
    public class NetworkSetAnimatorFloat : FsmStateActionAnimatorBase
    {
        [RequiredField]
        [CheckForComponent(typeof(Animator))]
        [Tooltip("The target.")]
        public FsmOwnerDefault gameObject;

        [RequiredField]
        [UIHint(UIHint.AnimatorFloat)]
        [Tooltip("The animator parameter")]
        public FsmString parameter;

        [Tooltip("The float value to assign to the animator parameter")]
        public FsmFloat Value;

        [Tooltip("Optional: The time allowed for the parameter to reach the value. Requires Every Frame to be checked.")]
        public FsmFloat dampTime;

        private Animator animator
        {
            get { return cachedComponent; }
        }

        private string cachedParameter;
        private int paramID;

        public override void Reset()
        {
            base.Reset();
            gameObject = null;
            parameter = null;
            dampTime = new FsmFloat { UseVariable = true };
            Value = null;
        }

        public override void OnEnter()
        {
            SetParameter();

            if (!everyFrame)
            {
                Finish();
            }
        }

        public override void OnActionUpdate()
        {
            SetParameter();
        }

        private void SetParameter()
        {
            if (!UpdateCache(Fsm.GetOwnerDefaultTarget(gameObject)))
            {
                Finish();
                return;
            }

            // Check if the object is networked and owned by the local player
            var networkObject = cachedComponent.GetComponent<NetworkObject>();
            if (networkObject != null && !networkObject.IsOwner)
            {
                // Do not set parameters if not owned by the local player
                return;
            }

            if (cachedParameter != parameter.Value)
            {
                cachedParameter = parameter.Value;
                paramID = Animator.StringToHash(parameter.Value);
            }

            if (dampTime.Value > 0f)
            {
                animator.SetFloat(paramID, Value.Value, dampTime.Value, Time.deltaTime);
            }
            else
            {
                animator.SetFloat(paramID, Value.Value);
            }
        }

    }
}
