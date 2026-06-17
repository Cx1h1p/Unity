using System;
using System.Collections.Generic;
using UnityEngine;

public class TaskManager : MonoBehaviour
{
    public static TaskManager Instance;

   
    private readonly Dictionary<string, bool> tasks = new Dictionary<string, bool>();

    
    public event Action<string, bool> OnTaskChanged;

    
    public event Action OnTasksInitialized;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

       
        RegisterTask("Blueprint");
       
        RegisterTask("Code");
        RegisterTask("Puzzle");

        
        OnTasksInitialized?.Invoke();
    }

    public void RegisterTask(string taskId)
    {
        if (string.IsNullOrWhiteSpace(taskId)) return;
        if (tasks.ContainsKey(taskId)) return;

        tasks.Add(taskId, false);
    }

    public bool IsCompleted(string taskId)
    {
        return tasks.TryGetValue(taskId, out bool done) && done;
    }

    public void CompleteTask(string taskId)
    {
        SetCompleted(taskId, true);
    }

    public void SetCompleted(string taskId, bool completed)
    {
        if (string.IsNullOrWhiteSpace(taskId)) return;

        
        if (!tasks.ContainsKey(taskId))
            tasks.Add(taskId, false);

        
        if (tasks[taskId] == completed) return;

        tasks[taskId] = completed;

        Debug.Log($"TaskManager: {taskId} -> {(completed ? "COMPLETED" : "NOT COMPLETED")}");

        OnTaskChanged?.Invoke(taskId, completed);
    }

    public IEnumerable<KeyValuePair<string, bool>> GetAllTasks()
    {
        return tasks;
    }
}