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

            while(true)
            {
                Console.WriteLine("1. Show All Tasks.");
                Console.WriteLine("2. Create a new Task.");
                Console.WriteLine("3. Delete a Task.");
                Console.WriteLine("4. Edit a Task.");
                Console.Write("Input:");

                var result = int.TryParse(Console.ReadLine(), out int input);

                if (result)
                {
                    switch (input)
                    {
                        case 1:
                            Console.Clear();
                            MethodHandler(GetAllTasks);
                            break;
                        case 2:
                            Console.Clear();
                            MethodHandler(CreateNewTask);
                            break;
                        case 3:
                            Console.Clear();
                            MethodHandler(DeleteTask);
                            break;
                        case 4:
                            Console.Clear();
                            MethodHandler(UpdateTask);
                            break;
                    }
                }
            }
        }

        void GetAllTasks(SqlConnection connection)
        {
            tasksList.Clear();

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

            Console.WriteLine("---TASKS----");

            Console.WriteLine("TaskID\t\tTaskName");
            //display all data from the list 
            foreach (var task in tasksList)
            {
                Console.WriteLine(task.TaskId + "\t\t" + task.TaskName);
            }

            Console.WriteLine("---TASKS----");
        }

        void CreateNewTask(SqlConnection connection)
        {
            NextID = GetLastID();
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

        void DeleteTask(SqlConnection connection)
        {
            //deletes a task and searched it by id

            GetAllTasks(connection);

            Console.Write("ID From Task to delete:");

            var result = int.TryParse(Console.ReadLine(), out int taskId);

            if(result)
            {
                string deleteTask = $"DELETE FROM Task WHERE Task_Id = {taskId}";

                try
                {
                    using (SqlCommand deleteTaskCommand = new SqlCommand(deleteTask, connection))
                    {
                        if(deleteTaskCommand.ExecuteNonQuery() != 0)
                        {
                            deleteTaskCommand.ExecuteNonQuery();
                        }
                        else if(deleteTaskCommand.ExecuteNonQuery() == 0)
                        {
                            Console.WriteLine("No Task was deleted.");
                        }
                    }
                }
                catch (SqlException sqlException)
                {
                    Console.WriteLine("SQL ERROR:" + sqlException);
                }
            }
        }

        void UpdateTask(SqlConnection connection)
        {
            //update a task and search it by id

            GetAllTasks(connection);

            Console.Write("ID From Task to delete:");

            var result = int.TryParse(Console.ReadLine(), out int taskId);

            if (result)
            {
                Console.Write("New Task Name: ");
                string taskName = Console.ReadLine();

                if(!string.IsNullOrEmpty(taskName))
                {
                    string updateTask = $"UPDATE Task SET Task_Name = '{taskName}' WHERE Task_Id = {taskId}";

                    using(SqlCommand updateTaskCommand = new SqlCommand(updateTask, connection))
                    {
                        updateTaskCommand.ExecuteNonQuery();
                    }
                }
            }
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