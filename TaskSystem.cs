using Microsoft.Data.SqlClient;
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
            MethodHandler(GetAllTasks);
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

        void CreateNewTask()
        {
            //creates a new entry in the table and makes a new task
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
            return 0;
        }

    }
}