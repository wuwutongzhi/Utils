using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace JPS {
    /// <summary>
    /// 创建人：一个人心
    /// 功能说明：
    /// </summary>
    public class JPSCell : MonoBehaviour {
        public Image image;
        public Vector2Int Pos;
        private bool _isObstacle = false;
        private bool isStartOrEnd = false;

        private Color obstacleColor = new Color(0, 0, 0, 1);
        private Color moveableColor = new Color(1, 1, 1, 1);
        private Color startColor = new Color(0, 0, 1, 1);
        private Color endColor = new Color(1, 0, 0, 1);
        private Color searchColor = new Color(.5f, .5f, .5f, 1);
        private Color pathColor = new Color(1, 1, 0, 1);

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

        bool isFirstSearch = true;
        public void SetNormal() {
            IsObstacle = false;
            isStartOrEnd = false;
            image.color = moveableColor;
            isFirstSearch = true;
        }

        public void SetSearch() {
            if (isStartOrEnd) return;
            if (isFirstSearch) {
                isFirstSearch = false;
                image.color = searchColor;
            }
            else {
                image.color = image.color * new Color(.7f, .7f, .7f, 1);
            }
        }

        Color oldSelect;
        public void Select() {
            oldSelect = image.color;
            image.color = Color.green;
        }

        public void UnSelect() {
            image.color = oldSelect;
        }

        public void SetPath() {
            if (isStartOrEnd) return;
            image.color = pathColor;
        }

        public void SetJumpPoint() {
            if (isStartOrEnd) return;
            image.color = new Color(1, 0, .5f, 1);
        }
    }
}