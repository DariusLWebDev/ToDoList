using Amazon.Runtime.Internal;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace To_Do_List
{
    public partial class toDoList : Form
    {

        public static IMongoClient client = new MongoClient("mongodb://localhost:27017");
        public static IMongoDatabase db = client.GetDatabase("ToDoList");
        public static IMongoCollection<Entry> coll = db.GetCollection<Entry>("mycoll");

        public toDoList()
        {
            InitializeComponent();
        }

        DataTable todoList = new DataTable();
        bool isEditing = false;

        private void Form1_Load(object sender, EventArgs e)
        {
            //Create Columns
            todoList.Columns.Add("Title");
            todoList.Columns.Add("Description");

            //Point our datagridview to our datasource
            toDoListView.DataSource = todoList;

        }

        private void newButton_Click(object sender, EventArgs e)
        {
            // Add the new task to the DataTable
            todoList.Rows.Add(titleTextBox.Text, descriptionTextBox.Text);

            // Clear fields
            titleTextBox.Text = "";
            descriptionTextBox.Text = "";

            // Update the MongoDB collection with the changes
            UpdateMongoDB();

        }

        private void editButton_Click_1(object sender, EventArgs e)
        {
            if (toDoListView.SelectedRows.Count > 0)
            {
                // Get the index of the selected row
                int rowIndex = toDoListView.SelectedRows[0].Index;

                // Populate the text boxes with the data from the selected row for editing
                titleTextBox.Text = todoList.Rows[rowIndex]["Title"].ToString();
                descriptionTextBox.Text = todoList.Rows[rowIndex]["Description"].ToString();

                // Set the editing flag to true
                isEditing = true;
            }
            else
            {
                MessageBox.Show("Please select a task to edit.");
            }

        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            if (isEditing)
            {
                // Get the index of the selected row
                int rowIndex = toDoListView.SelectedRows[0].Index;

                // Update the current row in the DataTable
                todoList.Rows[rowIndex]["Title"] = titleTextBox.Text;
                todoList.Rows[rowIndex]["Description"] = descriptionTextBox.Text;

                // Clear fields
                titleTextBox.Text = "";
                descriptionTextBox.Text = "";

                // Reset editing flag
                isEditing = false;

                // Update the MongoDB collection with the changes
                UpdateMongoDB();
            }
            else
            {
                MessageBox.Show("Please select a task to edit.");
            }

        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            if (toDoListView.SelectedRows.Count > 0)
            {
                int rowIndex = toDoListView.SelectedRows[0].Index;
                todoList.Rows.RemoveAt(rowIndex); // Remove the selected row from the DataTable
                UpdateMongoDB(); // Update MongoDB after deleting the row
            }
            else
            {
                MessageBox.Show("Please select a task to delete.");
            }

        }

        

        private void label1_Click(object sender, EventArgs e)
        {

        }
        public class Entry
        {
            [BsonId]

            public ObjectId Id { get; set; }

            [BsonElement("titleTextBox")]

            public string titleTextBox { get; set; }

            [BsonElement("descriptionTextBox")]

            public string descriptionTextBox { get; set; }

            public Entry(string titleTextBox, string descriptionTextBox)
            {
                this.titleTextBox = titleTextBox;
                this.descriptionTextBox = descriptionTextBox;
            }
        }

        private void UpdateMongoDB()
        {
            // Clear the MongoDB collection
            coll.DeleteMany(new BsonDocument());

            // Insert each row from the DataTable into the MongoDB collection
            foreach (DataRow row in todoList.Rows)
            {
                Entry user = new Entry(row["Title"].ToString(), row["Description"].ToString());
                coll.InsertOne(user);
            }
        }
        private void titleTextBox_TextChanged(object sender, EventArgs e)
        {

        }

        
    }
}
