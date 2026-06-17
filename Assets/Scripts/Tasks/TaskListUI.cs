using System.Collections.Generic;
using UnityEngine;

public class TaskListUI : MonoBehaviour
{
    [SerializeField] private Transform rowsParent;     
    [SerializeField] private TaskRowUI rowTemplate;     

    private Dictionary<string, TaskRowUI> rows =
        new Dictionary<string, TaskRowUI>();

    private void Start()
    {
        var tm = TaskManager.Instance;
        if (tm == null)
        {
            Debug.LogError("TaskManager not found in scene.");
            return;
        }

        
        BuildAll(tm);

        
        tm.OnTasksInitialized += () => BuildAll(tm);

        
        tm.OnTaskChanged += OnTaskChanged;
    }

    private void BuildAll(TaskManager tm)
    {
        foreach (var kv in tm.GetAllTasks())
            CreateOrUpdate(kv.Key, kv.Value);
    }

    private void OnDestroy()
    {
        if (TaskManager.Instance != null)
            TaskManager.Instance.OnTaskChanged -= OnTaskChanged;
    }

    private void OnTaskChanged(string taskId, bool completed)
    {
        CreateOrUpdate(taskId, completed);
    }

    private void CreateOrUpdate(string taskId, bool completed)
    {
        if (!rows.TryGetValue(taskId, out var row))
        {
            row = Instantiate(rowTemplate, rowsParent);
            row.gameObject.SetActive(true);
            rows[taskId] = row;

            row.Init(taskId, completed);
        }
        else
        {
            row.SetCompleted(completed);
        }
    }
}