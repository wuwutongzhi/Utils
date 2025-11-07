using UnityEngine;

namespace UnityUtils {
    /// <summary>
    /// 游戏管理器、音频管理器、场景管理器等
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PersistentSingleton<T> : MonoBehaviour where T : Component {
        public bool AutoUnparentOnAwake = true; // 在Awake时自动解除父级关系,避免随着父对象被意外销毁

        protected static T instance; // 单例实例

        public static bool HasInstance => instance != null; // 检查是否存在实例
        public static T TryGetInstance() => HasInstance ? instance : null; // 尝试获取实例

        public static T Instance {
            get {
                if (instance == null) {
                    instance = FindAnyObjectByType<T>(); // 查找场景中的实例
                    if (instance == null) {
                        var go = new GameObject(typeof(T).Name + " Auto-Generated"); // 创建新游戏对象
                        instance = go.AddComponent<T>(); // 添加组件
                    }
                }

                return instance;
            }
        }

        /// <summary>
        /// 如果需要使用Awake，在重写时确保调用base.Awake()
        /// </summary>
        protected virtual void Awake() {
            InitializeSingleton(); // 初始化单例
        }

        protected virtual void InitializeSingleton() {
            if (!Application.isPlaying) return; // 仅在运行模式下处理

            if (AutoUnparentOnAwake) {
                transform.SetParent(null); // 解除父级关系
            }

            if (instance == null) {
                instance = this as T; // 设置实例
                DontDestroyOnLoad(gameObject); // 跨场景不销毁
            } else {
                if (instance != this) {
                    Destroy(gameObject); // 销毁重复实例
                }
            }
        }
    }
}