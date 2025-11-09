using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 创建人：一个人心
/// 功能说明：
/// </summary>
namespace AStar {
    public class AstarCell : MonoBehaviour {
        public Image image;
        public Vector2Int Pos;
        private bool _isObstacle = false;
        private bool isStartOrEnd = false;

        private Color obstacleColor = new Color(0, 0, 0, 1);    // 黑色
        private Color moveableColor = new Color(1, 1, 1, 1);    // 白色
        private Color startColor = new Color(0, 0, 1, 1);       // 蓝色
        private Color endColor = new Color(1, 0, 0, 1);         // 红色
        private Color searchColor = new Color(0.7f, 0.7f, 0.7f, 1);     // 灰色
        private Color pathColor = new Color(1, 1, 0, 1);                // 黄色

        public bool IsObstacle {
            get => _isObstacle;
            set {
                isStartOrEnd = false;
                _isObstacle = value;
                image.color = _isObstacle ? obstacleColor : moveableColor;
            }
        }

        public void SetStart() {
            IsObstacle = false;
            isStartOrEnd = true;
            image.color = startColor;
        }

        public void SetEnd() {
            IsObstacle = false;
            isStartOrEnd = true;
            image.color = endColor;
        }

        public void SetNormal() {
            IsObstacle = false;
            isStartOrEnd = false;
            image.color = moveableColor;
        }

        public void SetOpenList() {
            if (isStartOrEnd) return;
            image.color = searchColor;
        }

        public void SetCloseList() {
            if (isStartOrEnd) return;
            image.color = searchColor * Color.gray;
        }

        Color oldSelect;
        public void Select() {
            oldSelect = image.color;
            image.color = Color.yellow;
        }

        public void UnSelect() {
            image.color = oldSelect;
        }

        public void SetPath() {
            if (isStartOrEnd) return;
            image.color = pathColor;
        }
    }
}
