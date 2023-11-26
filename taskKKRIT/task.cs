using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace taskKKRIT
{
    public partial class task : Form
    {
        private MySqlConnection connection;
        private string server;
        private string database;
        private string uid;
        private string password;

        public task()
        {
            InitializeComponent();
            server = "localhost";
            database = "taskmenedger";
            uid = "root";
            password = "root";

            InitializeConnection();
        }


        private void InitializeConnection()
        {
            string connectionString = $"Server={server};Database={database};Uid={uid};Pwd={password};";
            connection = new MySqlConnection(connectionString);
        }

        private void task_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }

                // Получаем user_id из таблицы users_now
                string getUserIdQuery = "SELECT user_id FROM users_now";
                MySqlCommand getUserIdCmd = new MySqlCommand(getUserIdQuery, connection);

                int userId = Convert.ToInt32(getUserIdCmd.ExecuteScalar());

                // Выбираем задачи только для текущего пользователя
                string query = "SELECT id as '№ задачи', task_text as 'Текст задачи', task_category as 'Категория', task_status as 'Статус', task_creation_date as 'Дата создания', task_completion_date as 'Дата завершения' FROM tasks WHERE user_id = @userId";
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@userId", userId);

                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);

                DataTable dt = new DataTable();
                adapter.Fill(dt);

             

                dataGridView1.DataSource = dt;

                // Опционально, вы можете настроить ширину столбцов
                for (int i = 0; i < dataGridView1.Columns.Count; i++)
                {
                    dataGridView1.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            panel1.Visible = true;
        }


        private void UpdateTaskStatus()
        {
            try
            {
                InitializeConnection(); // Подключение к базе данных

                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }

                // Получаем значения из textBox1 и comboBox1
                int taskId = Convert.ToInt32(textBox1.Text);
                string newStatus = comboBox1.Text;

                // Обновляем статус в таблице tasks
                string updateQuery = "UPDATE tasks SET task_status = @newStatus WHERE id = @taskId";
                MySqlCommand updateCmd = new MySqlCommand(updateQuery, connection);
                updateCmd.Parameters.AddWithValue("@newStatus", newStatus);
                updateCmd.Parameters.AddWithValue("@taskId", taskId);

                updateCmd.ExecuteNonQuery();

                MessageBox.Show("Статус успешно обновлен.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            panel1.Visible = false;
            UpdateTaskStatus();
            LoadData(); // Если вы хотите обновить отображение задач после изменения статуса
        }
    }
}
