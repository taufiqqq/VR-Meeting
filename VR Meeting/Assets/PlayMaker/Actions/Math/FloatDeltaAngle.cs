using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{

	[ActionCategory(ActionCategory.Math)]
	[Tooltip("Calculates the shortest difference between two given angles given in degrees.")]
	public class FloatDeltaAngle : FsmStateAction
	{
        [RequiredField]
        [UIHint(UIHint.Variable)]
        [Tooltip("The current angle.")]
        public FsmFloat fromAngle;
        [RequiredField]
        [Tooltip("The target angle.")]
        public FsmFloat toAngle;
        [RequiredField]
        [UIHint(UIHint.Variable)]
        [Tooltip("Store the deltaAngle in a variable.")]
        public FsmFloat deltaAngle;
        public bool everyFrame;

        public override void Reset()
        {
            fromAngle = null;
            toAngle = null;
            deltaAngle = null;
            everyFrame = false;
        }

        public override void OnEnter()
        {
            DoDeltaAngle();

            if (!everyFrame)
            {
                Finish();
            }
        }

        public override void OnUpdate()
        {
            DoDeltaAngle();
        }

        void DoDeltaAngle()
        {
            // Ensure that both angles are within the range [0, 360).
            float current = NormalizeAngle(fromAngle.Value);
            float target = NormalizeAngle(toAngle.Value);

            // Calculate the raw difference between the angles.
            float delta = target - current;

            // Adjust the difference to represent the shortest path.
            if (delta > 180.0F)
                delta -= 360.0F;
            else if (delta < -180.0F)
                delta += 360.0F;

            // Store the calculated shortest difference in the deltaAngle variable.
            deltaAngle.Value = delta;
        }

        // Helper function to normalize an angle to the range [0, 360).
        private float NormalizeAngle(float angle)
        {
            angle %= 360.0F; // Modulo operation to bring the angle within the range [0, 360).
            if (angle < 0.0F)
                angle += 360.0F; // If the angle is negative, adjust it to the positive equivalent.
            return angle;
        }


    }

}
