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

        private void ����������ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PanelForm(new adminmenedger());
        }

        private void ������������������������ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PanelForm(new admin_user());
        }

        private void button1_Click(object sender, EventArgs e)
        {
            bool foundUser = false;
            string role = "";
            string id = "";

            // ������� ����������� � ���� ������
            MySqlConnection connection = new MySqlConnection("Server=localhost;Database=taskmenedger;Uid=root;Pwd=root;"); // �������� �� ���� ������ �����������

            try
            {
                // ��������� ����������
                connection.Open();

                // ������� ������� SQL ��� �������� ���������������� ������ � ��������� ����
                string query = "SELECT id, role FROM users WHERE username = @username AND password = @password";
                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@username", textBox1.Text);
                command.Parameters.AddWithValue("@password", textBox2.Text);

                // ��������� ������ � �������� ���� ������������, ���� ������
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
                MessageBox.Show("������: " + ex.Message);
            }
            finally
            {
                // ��������� ����������
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
                    MessageBox.Show("������ ��� �������� ������: " + ex.Message);
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
                    MessageBox.Show("������ ��� ���������� ������: " + ex.Message);
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

                    // ��������� ������ � ������� my_table

                }
                else if (role == "user")
                {

                    PanelForm(new task());
                    
                }
                else
                {
                    MessageBox.Show("���-�� ����� �� ���");
                }
            }
            else
            {
                
                
                label4.Visible = true;
            }
        }
    }
}