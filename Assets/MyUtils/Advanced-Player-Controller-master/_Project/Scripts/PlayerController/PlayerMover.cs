using UnityEngine;

namespace AdvancedController {
    //需要刚体和胶囊碰撞体
    [RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider))]
    public class PlayerMover : MonoBehaviour {
        #region Fields
        [Header("Collider Settings:")]
        [Tooltip("脚部占身体比率,脚部不计算碰撞体,允许角色跨越小台阶小障碍")][Range(0f, 1f)] [SerializeField] float stepHeightRatio = 0.1f;
        [Tooltip("该值与steoHeightRatio的乘积即为能跨过的台阶")][SerializeField] float colliderHeight = 2f;
        [SerializeField] float colliderThickness = 1f;
        [SerializeField] Vector3 colliderOffset = Vector3.zero;
        
        Rigidbody rb;
        Transform tr;
        CapsuleCollider col;
        RaycastSensor sensor;
        
        bool isGrounded;
        //这个值会根据stepHeightRatio调整
        float baseSensorRange;
        //用于平滑地上下移动以保持与地面的接触
        Vector3 currentGroundAdjustmentVelocity; // Velocity to adjust player position to maintain ground contact
        int currentLayer;
        
        [Header("Sensor Settings:")]
        [SerializeField] bool isInDebugMode;
        //是否开启扩展传感器范围
        bool isUsingExtendedSensorRange = true; // Use extended range for smoother ground transitions
        #endregion

        void Awake() {
            Init();
            RecalculateColliderDimensions();
        }
        //当在编辑器中修改属性时调用,
        void OnValidate() {
            if (gameObject.activeInHierarchy) {
                RecalculateColliderDimensions();
            }
        }
        
        void LateUpdate() {
            if (isInDebugMode) {
                sensor.DrawDebug();
            }
        }
        //检测地面
        public void CheckForGround() {
            if (currentLayer != gameObject.layer) {
                RecalculateSensorLayerMask();
            }
            
            currentGroundAdjustmentVelocity = Vector3.zero;
            sensor.castLength = isUsingExtendedSensorRange 
                ? baseSensorRange + colliderHeight * tr.localScale.x * stepHeightRatio//
                : baseSensorRange;
            sensor.Cast();
            
            isGrounded = sensor.HasDetectedHit();
            if (!isGrounded) return;
            //获取传感器检测到的距离
            float distance = sensor.GetDistance();
            //玩家的头顶边界
            float upperLimit = colliderHeight * tr.localScale.x * (1f - stepHeightRatio) * 0.5f;
            //脚到人物中心的距离,
            float middle = upperLimit + colliderHeight * tr.localScale.x * stepHeightRatio;
            float distanceToGo = middle - distance;
            Debug.Log("DistanceToGround" + distanceToGo);
            //计算出需要的速度以在下一次物理更新中调整位置,移动玩家所需的速度
            currentGroundAdjustmentVelocity = tr.up * (distanceToGo / Time.fixedDeltaTime);
        }
        
        public bool IsGrounded() => isGrounded;
        public Vector3 GetGroundNormal() => sensor.GetNormal();

        //我们亲自设置刚体的速度
        public void SetVelocity(Vector3 velocity) => rb.linearVelocity = velocity + currentGroundAdjustmentVelocity;
        public void SetExtendSensorRange(bool isExtended) => isUsingExtendedSensorRange = isExtended;

        void Init() {
            tr = transform;
            rb = GetComponent<Rigidbody>();
            col = GetComponent<CapsuleCollider>();
            //这些由我们亲自操作
            rb.freezeRotation = true;
            rb.useGravity = false;
        }
        //重新计算碰撞体的体积和位置,当游戏对象的缩放比例改变时调用
        void RecalculateColliderDimensions() {
            //确保碰撞体已初始化
            if (col == null) {
                Init();
            }
            
            col.height = colliderHeight * (1f - stepHeightRatio);
            col.radius = colliderThickness / 2f;
            
            col.center = colliderOffset * colliderHeight + new Vector3(0f, stepHeightRatio * col.height / 2f, 0f);

            if (col.height / 2f < col.radius) {
                col.radius = col.height / 2f;
            }
            //重新校准传感器
            RecalibrateSensor();
        }

        void RecalibrateSensor() {
            //空值合并运算符,如果sensor为空,则创建一个新的RaycastSensor实例
            sensor ??= new RaycastSensor(tr);
            //设置本地坐标系中的投射起点为碰撞体的中心位置,是个本地坐标
            sensor.SetCastOrigin(col.bounds.center);
            //设置投射方向为向下
            sensor.SetCastDirection(RaycastSensor.CastDirection.Down);
            RecalculateSensorLayerMask();
            //避免裁剪问题
            const float safetyDistanceFactor = 0.001f;
            //射线长度计算
            float length = colliderHeight * (1f - stepHeightRatio) * 0.5f + colliderHeight * stepHeightRatio;//半个身体高度加上脚部高度
            baseSensorRange = length * (1f + safetyDistanceFactor) * tr.localScale.x;//要稍微长一点避免裁剪问题
            sensor.castLength = length * tr.localScale.x;
        }

        void RecalculateSensorLayerMask() {
            //获取当前游戏对象所在的层
            int objectLayer = gameObject.layer;
            //初始化一个层掩码，默认包含所有层（Physics.AllLayers 表示所有层，即每一层都被包含）
            int layerMask = Physics.AllLayers;
            // 步骤1：排除与当前对象层忽略碰撞的所有层
            for (int i = 0; i < 32; i++) {
                //检查当前对象层与其他层之间是否设置为忽略碰撞
                if (Physics.GetIgnoreLayerCollision(objectLayer, i)) {
                    //
                    layerMask &= ~(1 << i);
                }
            }
            // 步骤2：强制排除"Ignore Raycast"层
            int ignoreRaycastLayer = LayerMask.NameToLayer("Ignore Raycast");
            layerMask &= ~(1 << ignoreRaycastLayer);
            // 步骤3：应用计算后的层掩码
            sensor.layermask = layerMask;
            currentLayer = objectLayer;
        }
    }
}