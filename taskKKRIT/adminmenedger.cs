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
    public partial class adminmenedger : Form

    {
        private MySqlConnection connection;
        private string server;
        private string database;
        private string uid;
        private string password;
        public adminmenedger()
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

        private void adminmenedger_Load(object sender, EventArgs e)
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

                

                // Выбираем задачи только для текущего пользователя
                string query = "SELECT id as '№ задачи', task_text as 'Текст задачи', task_category as 'Категория', task_status as 'Статус', task_creation_date as 'Дата создания', task_completion_date as 'Дата завершения', user_id as '№ Пользователя' FROM tasks";
                MySqlCommand cmd = new MySqlCommand(query, connection);
              

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
        private void LoadDataById()
        {
            try
            {
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }

                // Проверяем, что введенное значение в textBox1 является числом
                if (int.TryParse(textBox1.Text, out int userId))
                {
                    // Выбираем задачи только для указанного пользователя
                    string query = "SELECT id as '№ задачи', task_text as 'Текст задачи', task_category as 'Категория', task_status as 'Статус', task_creation_date as 'Дата создания', task_completion_date as 'Дата завершения', user_id as '№ Пользователя' FROM tasks WHERE user_id = @UserId";
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@UserId", userId);

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
                else
                {
                    // Если textBox1 пуст, вызываем LoadData
                    LoadData();
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
        private void button6_Click(object sender, EventArgs e)
        {
            LoadDataById();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            panel1.Visible = true;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            panel1.Visible = false;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                // Проверяем, что номер исполнителя (textBox5) представляет собой корректное число
                if (!int.TryParse(textBox5.Text, out int userId))
                {
                    MessageBox.Show("Введите корректный номер исполнителя.");
                    return;
                }

                // Инициализируем подключение
                InitializeConnection();

                // Открываем подключение
                connection.Open();

                // SQL-запрос для добавления данных в таблицу tasks
                string query = "INSERT INTO tasks (task_text, task_category, task_status, task_creation_date, task_completion_date, user_id) " +
                               "VALUES (@TaskText, @TaskCategory, @TaskStatus, @TaskCreationDate, @TaskCompletionDate, @UserId)";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    // Передаем параметры в SQL-запрос
                    command.Parameters.AddWithValue("@TaskText", textBox2.Text);
                    command.Parameters.AddWithValue("@TaskCategory", textBox3.Text);
                    command.Parameters.AddWithValue("@TaskStatus", comboBox1.Text);
                    command.Parameters.AddWithValue("@TaskCreationDate", dateTimePicker1.Value.ToString("yyyy-MM-dd")); // Формат даты для MySQL
                    command.Parameters.AddWithValue("@TaskCompletionDate", DBNull.Value); // Для начала, устанавливаем значение null для даты завершения
                    command.Parameters.AddWithValue("@UserId", userId);

                    // Выполняем запрос
                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Задача добавлена успешно.");
                        // Очищаем элементы управления после успешного добавления
                        textBox2.Clear();
                        textBox3.Clear();
                        comboBox1.SelectedIndex = 0; // Сбрасываем выбор в комбобоксе
                        dateTimePicker1.Value = DateTime.Now; // Сбрасываем дату на текущую
                        textBox5.Clear();
                        panel1.Visible = false;
                        LoadData();
                        
                    }
                    else
                    {
                        MessageBox.Show("Ошибка при добавлении задачи.");
                    }
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

        private void button5_Click(object sender, EventArgs e)
        {
            panel2.Visible = false;
        }

        private void button7_Click(object sender, EventArgs e)
        {
            try
            {
                // Проверяем, что введенное значение в textBox4 является числом
                if (int.TryParse(textBox4.Text, out int taskId))
                {
                    // Инициализируем подключение
                    InitializeConnection();

                    // Открываем подключение
                    connection.Open();

                    // SQL-запрос для удаления задачи по ID
                    string query = "DELETE FROM tasks WHERE id = @TaskId";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        // Передаем параметр ID в SQL-запрос
                        command.Parameters.AddWithValue("@TaskId", taskId);

                        // Выполняем запрос
                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Задача удалена успешно.");
                            // Очищаем textBox4 после успешного удаления
                            textBox4.Clear();
                            panel2.Visible = false;
                            LoadData();
                        }
                        else
                        {
                            MessageBox.Show("Задача с указанным ID не найдена.");
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Введите корректное значение ID.");
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

        private void button2_Click(object sender, EventArgs e)
        {
            panel2.Visible = true;
            panel1.Visible = false;
        }
    }
}
