﻿using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManagerSQLProject
{
    class TaskSystem
    {
        string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        readonly string connectionString;

        int NextID { get; set; }

        List<Task> tasksList = new();

        public TaskSystem()
        {
            string connectionStringText = File.ReadAllText(Path.Combine(desktopPath + Path.DirectorySeparatorChar + "connectionstring.txt"));
            connectionString = connectionStringText;
            NextID = GetLastID();
            Menu();
        }

        void MethodHandler(Action<SqlConnection> action)
        {
            using (SqlConnection conn = new(connectionString))
            {
                conn.Open();

                if (conn.State == ConnectionState.Open)
                {
                    action?.Invoke(conn);
                }

                conn.Close();
            }
        }

        void Menu()
        {

            //displays a menu to change view alls tasks or update and remove tasks...

            Console.WriteLine("1. Show All Tasks.");
            Console.WriteLine("2. Create a new Task.");
            Console.WriteLine("3. Delete a Task.");
            Console.WriteLine("4. Edit a Task.");

            var result = int.TryParse(Console.ReadLine(), out int input);

            if(result)
            {
                switch (input)
                {
                    case 1:
                        MethodHandler(GetAllTasks);
                        break;
                }
            }
        }

        void GetAllTasks(SqlConnection connection)
        {
            //displays all tasks as a structured list in the console
            string getAllTasks = "SELECT * FROM Task";
            SqlCommand getAllTasksCommand = new SqlCommand(getAllTasks, connection);
            SqlDataReader readAllTasks = getAllTasksCommand.ExecuteReader();

            //read all rows and add them to the List<task> 
            while (readAllTasks.Read())
            {
                int id = readAllTasks.GetInt32(0);
                string taskName = readAllTasks.GetString(1);

                tasksList.Add(new Task { TaskId = id, TaskName = taskName });
            }
            
            //display all data from the list 
            foreach (var task in tasksList)
            {
                Console.WriteLine("TaskID\t\tTaskName");
                Console.WriteLine(task.TaskId + "\t\t" + task.TaskName);
            }
        }

        void CreateNewTask(SqlConnection connection)
        {
            //creates a new entry in the table and makes a new task
            Console.Write("New TaskName:");

            string taskName = Console.ReadLine();

            if(!string.IsNullOrEmpty(taskName))
            {
                string createNewTask = $"INSERT INTO Task VALUES({++NextID},'{taskName}')";

                using (SqlCommand createNewTaskCommand = new SqlCommand(createNewTask, connection))
                {
                    createNewTaskCommand.ExecuteNonQuery();
                }
            }
            else
            {
                Console.WriteLine("Error: No Task Name");
                CreateNewTask(connection);
            }
        }

        void DeleteTask()
        {
            //deletes a task and searched it by id
        }

        void UpdateTask()
        {
            //update a task and search it by id
        }

        //implement it
        int GetLastID()
        {
            //gets a list of ids and returns the lastid it was ending with
            //throw new NotImplementedException();

            List<int> listOfIds = new();

            string getLastID = "SELECT Task_Id FROM Task";
            using(SqlConnection conn = new(connectionString))
            {
                conn.Open();

                using(SqlCommand cmd = new SqlCommand(getLastID,conn))
                {
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        int id = reader.GetInt32(0);

                        listOfIds.Add(id);
                    }
                }

                conn.Close();
            }

            return listOfIds.Last();
        }

    }
}