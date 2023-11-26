using System;
using NLog;
using System.IO;
using MySql.Data.MySqlClient;

namespace taskKKRIT
{
    public partial class Form1 : Form
    {
        private Form active;
        public Form1()
        {
            InitializeComponent();
        }
        
        private void PanelForm(Form fm)
        {
            if (active != null)
            {
                active.Close();

            }
            active = fm;
            fm.TopLevel = false;
            fm.FormBorderStyle = FormBorderStyle.None;
            fm.Dock = DockStyle.Bottom;
            this.panel1.Controls.Add(fm);
            this.panel1.Tag = fm;
            fm.BringToFront();
            fm.Show();
        }


        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            PanelForm(new task());
        }

        private void управлениеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PanelForm(new adminmenedger());
        }

        private void управлениеПользователямиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PanelForm(new admin_user());
        }

        private void button1_Click(object sender, EventArgs e)
        {
            bool foundUser = false;
            string role = "";
            string id = "";

            // Создаем подключение к базе данных
            MySqlConnection connection = new MySqlConnection("Server=localhost;Database=taskmenedger;Uid=root;Pwd=root;"); // Замените на свою строку подключения

            try
            {
                // Открываем соединение
                connection.Open();

                // Создаем команду SQL для проверки пользовательских данных и получения роли
                string query = "SELECT id, role FROM users WHERE username = @username AND password = @password";
                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@username", textBox1.Text);
                command.Parameters.AddWithValue("@password", textBox2.Text);

                // Выполняем запрос и получаем роль пользователя, если найден
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        
                        foundUser = true;
                        role = reader.GetString("role");
                        id = reader.GetString("id");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }
            finally
            {
                // Закрываем соединение
                connection.Close();
            }

            if (foundUser)
            {
                string deleteQuery = "DELETE FROM users_now";
                MySqlCommand deleteCommand = new MySqlCommand(deleteQuery, connection);

                try
                {
                    connection.Open();
                    deleteCommand.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при удалении данных: " + ex.Message);
                }
                finally
                {
                    connection.Close();
                }
                string insertQuery = "INSERT INTO users_now (user_id) VALUES (@id)";
                MySqlCommand insertCommand = new MySqlCommand(insertQuery, connection);
                insertCommand.Parameters.AddWithValue("@id", id);
                try
                {
                    connection.Open();
                    insertCommand.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при добавлении данных: " + ex.Message);
                }
                finally
                {
                    connection.Close();
                }
                if (role == "admin")
                {
                    PanelForm(new adminmenedger());
                    menuStrip1.Visible = true;
                    toolStripMenuItem1.Visible = false;

                    // Добавляем данные в таблицу my_table

                }
                else if (role == "user")
                {

                    PanelForm(new task());
                    
                }
                else
                {
                    MessageBox.Show("Что-то пошло не так");
                }
            }
            else
            {
                
                
                label4.Visible = true;
            }
        }
    }
}