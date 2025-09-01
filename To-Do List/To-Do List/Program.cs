using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace To_Do_List
{
    public class TaskItem
    {
        public int Id { get; set; }
        public string Description { get; set; } = string.Empty; // Null ataması engellendi
        public bool IsCompleted { get; set; }
    }

    public class ToDoList
    {
        private List<TaskItem> tasks = new();
        private int nextId = 1;
        private readonly string filePath = "tasks.json";

        // JsonSerializerOptions tek sefer oluşturuluyor
        private static readonly JsonSerializerOptions jsonOptions = new() { WriteIndented = true };

        public ToDoList()
        {
            Load();
        }

        public void AddTask(string description)
        {
            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentException("Görev açıklaması boş olamaz.");

            var task = new TaskItem
            {
                Id = nextId++,
                Description = description,
                IsCompleted = false
            };
            tasks.Add(task);
            Save();
        }

        public void ListTasks()
        {
            if (tasks.Count == 0)
            {
                Console.WriteLine("Görev listesi boş.");
                return;
            }

            foreach (var task in tasks)
            {
                Console.WriteLine($"{task.Id}. [{(task.IsCompleted ? "X" : " ")}] {task.Description}");
            }
        }

        public void DeleteTask(int id)
        {
            var task = tasks.Find(t => t.Id == id);
            if (task != null)
            {
                tasks.Remove(task);
                Save();
                Console.WriteLine("Görev silindi.");
            }
            else
            {
                Console.WriteLine("Görev bulunamadı.");
            }
        }

        public void CompleteTask(int id)
        {
            var task = tasks.Find(t => t.Id == id);
            if (task != null)
            {
                task.IsCompleted = true;
                Save();
                Console.WriteLine("Görev tamamlandı olarak işaretlendi.");
            }
            else
            {
                Console.WriteLine("Görev bulunamadı.");
            }
        }

        private void Save()
        {
            var json = JsonSerializer.Serialize(tasks, jsonOptions);
            File.WriteAllText(filePath, json);
        }

        private void Load()
        {
            if (File.Exists(filePath))
            {
                var json = File.ReadAllText(filePath);
                tasks = JsonSerializer.Deserialize<List<TaskItem>>(json, jsonOptions) ?? new List<TaskItem>();
                if (tasks.Count > 0)
                {
                    nextId = tasks[^1].Id + 1;
                }
            }
        }
    }

    class Program
    {
        static void Main()
        {
            var todoList = new ToDoList();

            while (true)
            {
                Console.WriteLine("\n--- To-Do List ---");
                Console.WriteLine("1. Görev Ekle");
                Console.WriteLine("2. Görevleri Listele");
                Console.WriteLine("3. Görev Sil");
                Console.WriteLine("4. Görevi Tamamlandı Olarak İşaretle");
                Console.WriteLine("5. Çıkış");
                Console.Write("Seçiminiz: ");

                var input = Console.ReadLine();

                switch (input)
                {
                    case "1":
                        Console.Write("Görev açıklaması: ");
                        var desc = Console.ReadLine();
                        if (!string.IsNullOrWhiteSpace(desc))
                        {
                            todoList.AddTask(desc);
                        }
                        else
                        {
                            Console.WriteLine("Geçersiz görev açıklaması.");
                        }
                        break;
                    case "2":
                        todoList.ListTasks();
                        break;
                    case "3":
                        Console.Write("Silinecek görev ID'si: ");
                        if (int.TryParse(Console.ReadLine(), out int deleteId))
                        {
                            todoList.DeleteTask(deleteId);
                        }
                        else
                        {
                            Console.WriteLine("Geçersiz ID.");
                        }
                        break;
                    case "4":
                        Console.Write("Tamamlanacak görev ID'si: ");
                        if (int.TryParse(Console.ReadLine(), out int completeId))
                        {
                            todoList.CompleteTask(completeId);
                        }
                        else
                        {
                            Console.WriteLine("Geçersiz ID.");
                        }
                        break;
                    case "5":
                        return;
                    default:
                        Console.WriteLine("Geçersiz seçim.");
                        break;
                }
            }
        }
    }
}
