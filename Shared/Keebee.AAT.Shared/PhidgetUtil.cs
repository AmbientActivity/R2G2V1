namespace Keebee.AAT.Shared
{
    public static class PhidgetUtil
    {
        private const int StepTolerance = 70;
        public static int GetSensorStepValue(int val)
        {
            var returnValue = -1;

            if (val >= (int)RotationSensorStep.Value1 - StepTolerance / 2 && val <= (int)RotationSensorStep.Value1 + StepTolerance / 2)
                returnValue = (int)RotationSensorStep.Value1;
            else if (val >= (int)RotationSensorStep.Value2 - StepTolerance / 2 && val <= (int)RotationSensorStep.Value2 + StepTolerance / 2)
                returnValue = (int)RotationSensorStep.Value2;
            else if (val >= (int)RotationSensorStep.Value3 - StepTolerance / 2 && val <= (int)RotationSensorStep.Value3 + StepTolerance / 2)
                returnValue = (int)RotationSensorStep.Value3;
            else if (val >= (int)RotationSensorStep.Value4 - StepTolerance / 2 && val <= (int)RotationSensorStep.Value4 + StepTolerance / 2)
                returnValue = (int)RotationSensorStep.Value4;
            else if (val >= (int)RotationSensorStep.Value5 - StepTolerance)
                return (int)RotationSensorStep.Value5;

            return returnValue;
        }
    }
}
