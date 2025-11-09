using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace HeapSort
{
    [System.Serializable]
    public class Task
    {
        public string taskName;
        public int priority;
    }

    public class TaskManager : MonoBehaviour
    {
        public Text taskListText; // 用于显示任务列表的Text组件
        public Button sortButton; // 用于触发排序的Button组件

        public List<Task> tasks; // 在Inspector面板上设置的任务列表

        private PriorityQueue<string> taskQueue;
        private List<string> sortedTasks;

        void Start()
        {
            // 初始化任务队列
            taskQueue = new PriorityQueue<string>();
            sortedTasks = new List<string>();

            // 将任务添加到优先队列中
            foreach (var task in tasks)
            {
                taskQueue.Enqueue(task.taskName, task.priority);
            }

            // 绑定按钮点击事件
            sortButton.onClick.AddListener(SortTasks);

            // 初始显示任务列表
            UpdateTaskList();
        }

        void SortTasks()
        {
            // 清空任务列表
            sortedTasks.Clear();

            // 按优先级从高到低取出任务
            while (taskQueue.Count > 0)
            {
                sortedTasks.Add(taskQueue.Dequeue());
            }

            // 更新任务列表显示
            UpdateTaskList();
        }

        void UpdateTaskList()
        {
            taskListText.text = "任务列表:\n";
            foreach (var task in sortedTasks)
            {
                taskListText.text += task + "\n";
            }
        }
    }
}