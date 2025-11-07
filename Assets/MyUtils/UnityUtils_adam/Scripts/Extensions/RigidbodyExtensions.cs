using UnityEngine;

namespace UnityUtils
{
    public static class RigidbodyExtensions
    {
        /// <summary>
        /// 改变Rigidbody速度的方向，同时保持其速度大小不变
        /// </summary>
        /// <param name="rigidbody">要改变方向的Rigidbody</param>
        /// <param name="direction">Rigidbody的新方向</param>
        /// <returns>修改后的Rigidbody，用于方法链式调用</returns>
        public static Rigidbody ChangeDirection(this Rigidbody rigidbody, Vector3 direction)
        {
            if (direction.sqrMagnitude == 0f) return rigidbody;
            direction.Normalize();

#if UNITY_6000_0_OR_NEWER
            rigidbody.linearVelocity = direction * rigidbody.linearVelocity.magnitude;
#else
            rigidbody.velocity = direction * rigidbody.velocity.magnitude;
#endif
            return rigidbody;
        }

        /// <summary>
        /// 通过将线速度和角速度设置为零来停止Rigidbody
        /// </summary>
        /// <param name="rigidbody">要停止的Rigidbody</param>
        /// <returns>修改后的Rigidbody，用于方法链式调用</returns>
        public static Rigidbody Stop(this Rigidbody rigidbody)
        {
#if UNITY_6000_0_OR_NEWER
            rigidbody.linearVelocity = Vector3.zero;
#else
            rigidbody.velocity = Vector3.zero;
#endif
            rigidbody.angularVelocity = Vector3.zero;
            return rigidbody;
        }
    }
}