using System.Collections.Generic;
using OPH.PathSearch;
using TMPro;
using UnityEngine;
/// <summary>
/// 创建人：一个人心
/// 功能说明：地图管理器
/// </summary>
namespace AStar {
    public class MapManager : MonoBehaviour, IGrid {
        public static MapManager Instance;
        public SettingType type = SettingType.Obstacle;
        public GameObject cellPrefab;
        public Vector2Int mapSize;
        bool[,] cellFlag;
        AstarCell[,] cells;
        public GameObject Tips;
        public TextMeshProUGUI typeText;
        public TextMeshProUGUI tipText;
        AStarTest astarTest = new AStarTest();

        public enum SettingType {
            Obstacle,
            StartEnd,
            Search,
        }

        public AstarCell start, end;

        private void Awake() {
            Instance = this;
            astarTest.SetEnv(this);
            BuildMap();
            UpdateTextType(type);
        }

        private void Update() {
            if (Input.GetKeyDown(KeyCode.Escape)) {
                Tips.SetActive(!Tips.activeSelf);
            }
            if (Input.GetKeyDown(KeyCode.Alpha1)) {
                type = SettingType.Obstacle;
                isFirsetSearch = true;
                Clear();
                UpdateTextType(type);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2)) {
                type = SettingType.StartEnd;
                isFirsetSearch = true;
                Clear();
                UpdateTextType(type);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3)) {
                type = SettingType.Search;
                Clear();
                UpdateTextType(type);
            }

            switch (type) {
            case SettingType.Obstacle:
            case SettingType.StartEnd:
                BuildUpdate();
                break;
            case SettingType.Search:
                SearchUpdate();
                break;
            default:
                break;
            }
        }

        private void UpdateTextType(SettingType type) {
            typeText.text = type.ToString();
            switch (type) {
            case SettingType.Obstacle:
                tipText.text = "鼠标左键:设置障碍物或者取消障碍物";
                break;
            case SettingType.StartEnd:
                tipText.text = "鼠标左键:设置起点\n鼠标右键:设置终点";
                break;
            case SettingType.Search:
                tipText.text = "空格:分步寻找";
                break;
            }
        }

        private void BuildUpdate() {
            if (Input.GetMouseButtonDown(0)) {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
                if (hit.collider != null && hit.collider.TryGetComponent<AstarCell>(out AstarCell cell)) {
                    switch (type) {
                    case SettingType.Obstacle:
                        if (cell == start) {
                            start = null;
                        }
                        else if (cell == end) {
                            end = null;
                        }
                        cell.IsObstacle = !cell.IsObstacle;
                        break;
                    case SettingType.StartEnd:
                        if (start != null) {
                            start.SetNormal();
                        }
                        start = cell;
                        start.SetStart();
                        break;
                    }
                }
            }
            if (Input.GetMouseButtonDown(1)) {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
                if (hit.collider != null && hit.collider.TryGetComponent<AstarCell>(out AstarCell cell)) {
                    switch (type) {
                    case SettingType.StartEnd:
                        if (end != null) {
                            end.SetNormal();
                        }
                        end = cell;
                        end.SetEnd();
                        break;
                    }
                }
            }
        }

        bool isFirsetSearch = true;
        IEnumerator<bool> enumerator;
        private void SearchUpdate() {
            if (Input.GetKeyDown(KeyCode.Space)) {
                if (isFirsetSearch) {
                    Clear();
                    if (start == null || end == null) return;
                    enumerator = astarTest.IFindStep(start.Pos, end.Pos).GetEnumerator();
                    isFirsetSearch = false;
                    isClear = false;
                    if (!enumerator.MoveNext()) {
                        isFirsetSearch = true;
                    }
                }
                else {
                    isClear = false;
                    if (!enumerator.MoveNext()) {
                        isFirsetSearch = true;
                    }
                }
            }
        }

        bool isClear = true;

        public void SetPath(in Vector2Int pos) {
            cells[pos.x, pos.y].SetPath();
        }
        public void SetSelect(in Vector2Int pos) {
            cells[pos.x, pos.y].Select();
        }
        public void SetUnSelect(in Vector2Int pos) {
            cells[pos.x, pos.y].UnSelect();
        }

        public void SetOpen(in Vector2Int pos) {
            cells[pos.x, pos.y].SetOpenList();
        }

        public void SetClose (in Vector2Int pos) {
            cells[pos.x, pos.y].SetCloseList();
        }

        private void BuildMap() {
            cellFlag = new bool[mapSize.x, mapSize.y];
            cells = new AstarCell[mapSize.x, mapSize.y];

            var rt = cellPrefab.transform as RectTransform;
            Vector2 size = rt.sizeDelta;
            Vector2 pos = new Vector2(size.x / 2, size.y / 2);
            for (int i = 0; i < mapSize.x; i++) {
                for (int j = 0; j < mapSize.y; j++) {
                    GameObject go = Instantiate(cellPrefab, transform);
                    go.transform.localPosition = pos;
                    cells[i, j] = go.GetComponent<AstarCell>();
                    cells[i, j].Pos = new Vector2Int(i, j);
                    pos.y += size.y;
                }
                pos.x += size.x;
                pos.y = size.y / 2;
            }
        }

        /// <summary>
        /// 清除标色
        /// </summary>
        public void Clear() {
            if (isClear) return;
            isClear = true;
            for (int i = 0; i < mapSize.x; ++i) {
                for (int j = 0; j < mapSize.y; ++j) {
                    if (!cells[i, j].IsObstacle) {
                        cells[i, j].SetNormal();
                    }
                }
            }
            start?.SetStart();
            end?.SetEnd();
        }

        #region 网格接口
        public bool IsMoveable(in Vector2Int pos) {
            if (IsInMap(pos)) {
                return !cells[pos.x, pos.y].IsObstacle;
            }
            return false;
        }

        public bool IsObstacle(in Vector2Int pos) {
            if (IsInMap(pos)) {
                return cells[pos.x, pos.y].IsObstacle;
            }
            return true;
        }

        public List<Vector2Int> GetArroundCell(in Vector2Int pos) {
            List<Vector2Int> list = new List<Vector2Int>(8);
            for (int i = -1; i <= 1; ++i) {
                for (int j = -1; j <= 1; ++j) {
                    if (i == 0 && j == 0) continue;
                    list.Add(new Vector2Int(pos.x + i, pos.y + j));
                }
            }
            return list;
        }

        public bool IsInMap(int x, int y) {
            if (x >= 0 && x < mapSize.x && y >= 0 && y < mapSize.y) {
                return true;
            }
            return false;
        }

        public bool IsInMap(in Vector2Int pos) {
            return IsInMap(pos.x, pos.y);
        }

        #endregion
    }
}
