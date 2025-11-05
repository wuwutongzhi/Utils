using System.Collections.Generic;
using System;
using UnityEngine;

namespace AdvancedController {
    /// <summary>
    /// 检测墙壁或者物体
    /// </summary>
    public class RaycastSensor {
        //射线长度
        public float castLength = 1f;
        //图层遮罩
        public LayerMask layermask = 255;
        
        //射线的起点位置
        Vector3 origin = Vector3.zero;
        //关联的变换组件
        Transform tr;
        
        //公共枚举类型,往六个方向投射射线,cast投
        public enum CastDirection { Forward, Right, Up, Backward, Left, Down } 
        CastDirection castDirection;
        //如果产生了碰撞,将碰撞信息存储到这个变量中
        RaycastHit hitInfo;

        public RaycastSensor(Transform playerTransform) {
            tr = playerTransform;
        }

        public void Cast() {
            Vector3 worldOrigin = tr.TransformPoint(origin);
            Vector3 worldDirection = GetCastDirection();
            //out常用用法,用于接收方法的返回值,QueryTriggerInteraction.Ignore不想检测触发器
            Physics.Raycast(worldOrigin, worldDirection, out hitInfo, castLength, layermask, QueryTriggerInteraction.Ignore);
        }
        
        public bool HasDetectedHit() => hitInfo.collider != null;
        public float GetDistance() => hitInfo.distance;
        public Vector3 GetNormal() => hitInfo.normal;
        public Vector3 GetPosition() => hitInfo.point;
        public Collider GetCollider() => hitInfo.collider;
        public Transform GetTransform() => hitInfo.transform;
        
        public void SetCastDirection(CastDirection direction) => castDirection = direction;
        //世界转本地坐标系
        public void SetCastOrigin(Vector3 pos) => origin = tr.InverseTransformPoint(pos);

        Vector3 GetCastDirection() {
            return castDirection switch {
                //本地的tansform.forward等方向转换为世界坐标系的方向
                CastDirection.Forward => tr.forward,
                CastDirection.Right => tr.right,
                CastDirection.Up => tr.up,
                CastDirection.Backward => -tr.forward,
                CastDirection.Left => -tr.right,
                CastDirection.Down => -tr.up,
                _ => Vector3.one
            };
        }
        
        public void DrawDebug() {
            if (!HasDetectedHit()) return;

            Debug.DrawRay(hitInfo.point, hitInfo.normal, Color.red, Time.deltaTime);
            float markerSize = 0.2f;
            Debug.DrawLine(hitInfo.point + Vector3.up * markerSize, hitInfo.point - Vector3.up * markerSize, Color.green, Time.deltaTime);
            Debug.DrawLine(hitInfo.point + Vector3.right * markerSize, hitInfo.point - Vector3.right * markerSize, Color.green, Time.deltaTime);
            Debug.DrawLine(hitInfo.point + Vector3.forward * markerSize, hitInfo.point - Vector3.forward * markerSize, Color.green, Time.deltaTime);
        }
    }
}