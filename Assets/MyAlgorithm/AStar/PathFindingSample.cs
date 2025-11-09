using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AStar
{

    public class PathFindingSample : MonoBehaviour
    {
        [SerializeField]
        private List<Vector2Int> obstacles = new List<Vector2Int>();

        [Header("地图")] [SerializeField] private Texture2D _mapTexture;

        [Header("地图宽")] public int mapWidth;
        [Header("地图高")] public int mapHeight;

        [Header("材质")] public Material edgeMaterial;
        public Material obstacleMaterial;
        public Material backgroundMaterial;
        public Material playerMaterial;

        public Vector2Int StartPos;
        public Vector2Int EndPos;

        public void GenerateMapAndObstacles()
        {
            if (_mapTexture != null)
            {
                // 设置地图宽度和高度
                mapWidth = _mapTexture.width;
                mapHeight = _mapTexture.height;

                // 清空之前的障碍物
                obstacles.Clear();

                // 遍历图片的每个像素，设置障碍物
                for (int y = 0; y < mapHeight; y++)
                {
                    for (int x = 0; x < mapWidth; x++)
                    {
                        Color pixelColor = _mapTexture.GetPixel(x, y);
                        if (pixelColor.a > 0.5f) // 假设不透明的像素是障碍物
                        {
                            obstacles.Add(new Vector2Int(x, y));
                        }
                    }
                }

                StartPos = new Vector2Int(0, 0);
                EndPos = new Vector2Int(_mapTexture.width - 1, _mapTexture.height - 1);
            }
        }

        private List<Vector2Int> ReconstructPath(Node endNode)
        {
            List<Vector2Int> path = new List<Vector2Int>();
            Node currentNode = endNode;

            while (currentNode != null)
            {
                path.Add(currentNode.Position);
                currentNode = currentNode.Parent;
            }

            path.Reverse();
            return path;
        }

        private List<Vector2Int> GetNeighbors(Vector2Int position)
        {
            List<Vector2Int> neighbors = new List<Vector2Int>();
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if (x == 0 && y == 0) continue;
                    Vector2Int newPos = new Vector2Int(position.x + x, position.y + y);
                    if (IsValidPosition(newPos))
                    {
                        neighbors.Add(newPos);
                    }
                }
            }
            return neighbors;
        }

        private bool IsValidPosition(Vector2Int pos)
        {
            // 检查是否在地图范围内,是否是可通过的格子
            return pos.x >= 0 && pos.x < mapWidth &&
                   pos.y >= 0 && pos.y < mapHeight &&
                   !obstacles.Contains(pos);
        }

        public bool TryFindPath(Vector2Int startPosition, Vector2Int goalPosition, out List<Vector2Int> returnPath)
        {
            List<Node> openList = new List<Node>();
            HashSet<Vector2Int> closedSet = new HashSet<Vector2Int>();

            Node startNode = new Node(startPosition);
            openList.Add(startNode);
            while (openList.Count > 0)
            {
                Node currentNode = openList.OrderBy(n => n.F).First();

                if (currentNode.Position == goalPosition)
                {
                    returnPath = ReconstructPath(currentNode);
                    return true;
                }

                openList.Remove(currentNode);
                closedSet.Add(currentNode.Position);

                foreach (Vector2Int neighbor in GetNeighbors(currentNode.Position))
                {
                    if (closedSet.Contains(neighbor)) continue;

                    float tentativeG = currentNode.G + Vector2Int.Distance(currentNode.Position, neighbor);

                    Node neighborNode = openList.Find(n => n.Position == neighbor);
                    if (neighborNode == null)
                    {
                        neighborNode = new Node(neighbor, currentNode);
                        openList.Add(neighborNode);
                    }
                    else if (tentativeG >= neighborNode.G)
                    {
                        continue;
                    }

                    neighborNode.Parent = currentNode;
                    neighborNode.G = tentativeG;
                    neighborNode.H = Vector2Int.Distance(neighbor, goalPosition);
                    neighborNode.F = neighborNode.G + neighborNode.H;
                }
            }

            returnPath = null;
            return false;
        }

        public void VisualizeMap()
        {
            // 清除之前的可视化
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                var child = transform.GetChild(i);
                DestroyImmediate(child.gameObject);
            }

            // 可视化地图边界
            for (int x = -1; x <= mapWidth; x++)
            {
                CreateCube(new Vector3(x, 0, -1), edgeMaterial);
                CreateCube(new Vector3(x, 0, mapHeight), edgeMaterial);
            }

            for (int y = -1; y <= mapHeight; y++)
            {
                CreateCube(new Vector3(-1, 0, y), edgeMaterial);
                CreateCube(new Vector3(mapWidth, 0, y), edgeMaterial);
            }

            // 可视化地图和障碍物
            for (int y = 0; y < mapHeight; y++)
            {
                for (int x = 0; x < mapWidth; x++)
                {
                    Vector3 position = new Vector3(x, 0, y);
                    if (obstacles.Contains(new Vector2Int(x, y)))
                    {
                        CreateCube(position, obstacleMaterial);
                    }
                    else
                    {
                        CreatePlane(position, backgroundMaterial);
                    }
                }
            }
        }

        private void CreateCube(Vector3 position, Material material)
        {
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.position = position;
            cube.transform.parent = transform;
            cube.GetComponent<Renderer>().material = material;
        }

        private void CreatePlane(Vector3 position, Material material)
        {
            GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
            plane.transform.position = position;
            plane.transform.parent = transform;
            plane.GetComponent<Renderer>().material = material;
        }

        public IEnumerator AnimatePath(List<Vector2Int> path)
        {
            if (path == null || path.Count == 0) yield break;

            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.position = new Vector3(path[0].x, 0, path[0].y);
            cube.transform.parent = transform;
            cube.GetComponent<Renderer>().material = playerMaterial;

            foreach (Vector2Int pos in path)
            {
                cube.transform.position = new Vector3(pos.x, 0, pos.y);
                yield return new WaitForSeconds(0.5f); // 固定速度移动
            }

            Destroy(cube);
        }
    }
}