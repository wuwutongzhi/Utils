using UnityEngine;

namespace UnityUtils
{
    public static class RendererExtensions
    {
        /// <summary>
        /// 为此Renderer中具有'_Color'属性的材质启用ZWrite。这将允许材质写入Z缓冲区，
        /// 可用于影响后续渲染的处理方式，例如确保透明对象的正确分层。
        /// </summary>    
        public static void EnableZWrite(this Renderer renderer)
        {
            foreach (Material material in renderer.materials)
            {
                if (material.HasProperty("_Color"))
                {
                    material.SetInt("_ZWrite", 1);
                    material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
                }
            }
        }

        /// <summary>
        /// 为此Renderer中具有'_Color'属性的材质禁用ZWrite。这将停止材质写入Z缓冲区，
        /// 在某些情况下可能是可取的，以防止后续渲染被遮挡，例如在半透明或分层对象的渲染中。
        /// </summary>
        public static void DisableZWrite(this Renderer renderer)
        {
            foreach (Material material in renderer.materials)
            {
                if (material.HasProperty("_Color"))
                {
                    material.SetInt("_ZWrite", 0);
                    material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent + 100;
                }
            }
        }
    }
}