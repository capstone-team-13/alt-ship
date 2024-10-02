namespace EE.Prototype.CC
{
    public class HingeJoint : Joint
    {
        public override JointType Type => JointType.Hinge;
        public override int DegreeOfFreedom => 2;
    }
}