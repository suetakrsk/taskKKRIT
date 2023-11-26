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
    public partial class admin_user : Form
    {
        private MySqlConnection connection;
        private string server;
        private string database;
        private string uid;
        private string password;
        public admin_user()
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
        private void admin_user_Load(object sender, EventArgs e)
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
                string query = "SELECT id as '№ пользователя', username as 'Имя пользователя', password as 'Пароль', role as 'Роль' FROM users";
                MySqlCommand cmd = new MySqlCommand(query, connection);
                

                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);

                DataTable dt = new DataTable();
                adapter.Fill(dt);

                // Убираем последний столбец
               

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

        private void button4_Click(object sender, EventArgs e)
        {
            if (int.TryParse(textBox1.Text, out int userId))
            {
                // Инициализируем подключение
                InitializeConnection();

                // Открываем подключение
                connection.Open();

                // SQL-запрос для удаления пользователя по ID
                string query = "DELETE FROM users WHERE id = @UserId";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    // Передаем параметр ID в SQL-запрос
                    command.Parameters.AddWithValue("@UserId", userId);

                    // Выполняем запрос
                    int rowsAffected = command.ExecuteNonQuery();

                    // Проверяем, были ли удалены строки
                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Пользователь удален успешно.");
                        // Очищаем textBox1 после успешного удаления
                        textBox1.Clear();
                        LoadData();
                    }
                    else
                    {
                        MessageBox.Show("Пользователь с указанным ID не найден.");
                    }
                }

                // Закрываем подключение после использования
                connection.Close();
            }
            else
            {
                MessageBox.Show("Введите корректное значение ID.");
            }
            panel1.Visible = false;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            panel1.Visible = true;
            panel2.Visible = false;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            // Инициализируем подключение
            InitializeConnection();

            // Открываем подключение
            connection.Open();

            // Получаем значения из элементов управления
            string username = textBox2.Text;
            string password = textBox3.Text;
            string role = comboBox1.Text;

            // Проверяем, что значения не являются пустыми
            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password) && !string.IsNullOrEmpty(role))
            {
                // SQL-запрос для вставки данных в таблицу users
                string query = "INSERT INTO users (username, password, role) VALUES (@Username, @Password, @Role)";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    // Передаем параметры в SQL-запрос
                    command.Parameters.AddWithValue("@Username", username);
                    command.Parameters.AddWithValue("@Password", password);
                    command.Parameters.AddWithValue("@Role", role);

                    // Выполняем запрос
                    int rowsAffected = command.ExecuteNonQuery();

                    // Проверяем, была ли вставка успешной
                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Пользователь добавлен успешно.");
                        // Очищаем элементы управления после успешной вставки
                        textBox2.Clear();
                        textBox3.Clear();
                        comboBox1.SelectedIndex = -1; // Сбрасываем выбор в комбобоксе
                        LoadData();
                    }
                    else
                    {
                        MessageBox.Show("Ошибка при добавлении пользователя.");
                    }
                }
            }
            else
            {
                MessageBox.Show("Заполните все поля перед добавлением пользователя.");
            }

            // Закрываем подключение после использования
            connection.Close();
            panel2.Visible = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            panel2.Visible = true;
            panel1.Visible = false;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            panel2.Visible = false;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            panel1.Visible = false;
        }
    }
}
