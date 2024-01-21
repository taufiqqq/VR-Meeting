using UnityEngine;
using Unity.Netcode;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Network)]
    [Tooltip("Moves a Game Object with a Character Controller. Velocity along the y-axis is ignored. Speed is in meters/s. Gravity is automatically applied.")]
    public class NetworkControllerSimpleMove : ComponentAction<CharacterController>
    {
        [RequiredField]
        [CheckForComponent(typeof(CharacterController))]
        [Tooltip("A Game Object with a Character Controller.")]
        public FsmOwnerDefault gameObject;

        [RequiredField]
        [Tooltip("The movement vector.")]
        public FsmVector3 moveVector;

        [Tooltip("Multiply the Move Vector by a speed factor.")]
        public FsmFloat speed;

        [Tooltip("Move in local or world space.")]
        public Space space;

        [Tooltip("Event sent if the Character Controller starts falling.")]
        public FsmEvent fallingEvent;

        private CharacterController controller
        {
            get { return cachedComponent; }
        }

        public override void Reset()
        {
            gameObject = null;
            moveVector = new FsmVector3 { UseVariable = true };
            speed = new FsmFloat { Value = 1 };
            space = Space.World;
            fallingEvent = null;
        }

        public override void OnUpdate()
        {
            if (!UpdateCacheAndTransform(Fsm.GetOwnerDefaultTarget(gameObject)))
                return;

            // Check if the object is networked and owned by the local player
            var networkObject = cachedComponent.GetComponent<NetworkObject>();
            if (networkObject != null && !networkObject.IsOwner)
            {
                // Do not move if not owned by the local player
                return;
            }

            var move = space == Space.World ? moveVector.Value : cachedTransform.TransformDirection(moveVector.Value);
            controller.SimpleMove(move * speed.Value);

            // controller.isGrounded doesn't work well when walking down slopes or stairs
            // https://forum.unity.com/threads/charactercontroller-and-walking-down-a-stairs.101859/
            // So we double-check if we're really falling using a raycast down.
            // We use the controller stepOffset as the ray distance. 
            // If the ray hits something we assume we're just stepping down and not really falling.
            if (!controller.isGrounded && // Note: raycast is only called if isGrounded is false 
                !Physics.Raycast(cachedTransform.position, Vector3.down, controller.stepOffset))
            {
                Fsm.Event(fallingEvent);
            }
        }
    }
}
