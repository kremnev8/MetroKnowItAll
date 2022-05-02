using UnityEngine.U2D;

namespace Util
{
    /// <summary>
    /// Extension class to work with splines
    /// </summary>
    public static class SplineUtil
    {
        /// <summary>
        /// Copy all properties of the other spline to this one
        /// </summary>
        public static void CopyValues(this Spline spline, Spline other)
        {
            spline.Clear();
            spline.isOpenEnded = other.isOpenEnded;
            for (int i = 0; i < other.GetPointCount(); i++)
            {
                spline.InsertPointAt(i, other.GetPosition(i));
                spline.SetTangentMode(i, other.GetTangentMode(i));
                spline.SetLeftTangent(i, other.GetLeftTangent(i));
                spline.SetRightTangent(i, other.GetRightTangent(i));
                
                spline.SetHeight(i, other.GetHeight(i));
                spline.SetCorner(i, other.GetCorner(i));
            }
        }
    }
}