using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManagerSQLProject
{
    class TaskSystem
    {
        int NextID { get; set; }

        public TaskSystem()
        {
            NextID = GetLastID();
            Menu();
        }

        void Menu()
        {
            //displays a menu to change view alls tasks or update and remove tasks...
        }

        void GetAllTasks()
        {
            //displays all tasks as a structured list in the console
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
            throw new NotImplementedException();
        }
    }
}
