using System;
using System.Data.SqlClient;
using System.Windows.Forms;
using User_Login_With_SQL.Connection;
using User_Login_With_SQL.Models;
using User_Login_With_SQL.Util;

namespace User_Login_With_SQL
{
    public partial class Login : Form
    {
        LoginModels LoginModels = new LoginModels();

        public Login()
        {
            InitializeComponent();

            controlDatabaseTable();
        }

        private void ButtonLogin_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textEmail.Text) || string.IsNullOrWhiteSpace(textPassword.Text))
            {
                MessageBox.Show("Please do not leave mandatory fields blank. ! ", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            using (SqlConnection connection = new SqlConnection(SqlCommands.SqlConnectionURL))
            {
                connection.Open();

                var encryptionPassword = Util.EncryptionDecryptionUtil.Encrypt(textPassword.Text);

                var command = "Select * from [User] where Email='" + textEmail.Text + "' and Password='" + encryptionPassword + "' ";

                SqlCommand cmd = new SqlCommand(command, connection);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        MessageBox.Show("Login Success", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        while (reader.Read())
                        {
                            LoginModels.ID = reader[0].ToString();
                            LoginModels.Name = reader[1].ToString();
                            LoginModels.Age = reader[2].ToString();
                            LoginModels.Email = reader[3].ToString();
                            LoginModels.Password = reader[4].ToString();
                            LoginModels.Country = reader[5].ToString();
                        }

                        using (var form = new UserSetting(LoginModels))
                        {
                            form.ShowDialog();
                        }

                    }
                    else
                    {
                        MessageBox.Show("No records found.", "No record", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    reader.Close();
                }

                connection.Close();
            }

        }

        private void LinkRegister_LinkClicked(object sender, EventArgs e)
        {
            using (var form = new Register())
            {
                form.ShowDialog();
            }
        }

        //If the database table does not exist, the code required to create a new one
        private void controlDatabaseTable()
        {
            using (SqlConnection connection = new SqlConnection("Server= localhost; Integrated Security= true"))
            {
                try
                {
                    connection.Open();

                    string checkDatabaseQuery = $"SELECT * FROM sys.databases WHERE Name = '{SqlCommands.DatabaseName}'";

                    using (SqlCommand command = new SqlCommand(checkDatabaseQuery, connection))
                    {
                        var result = command.ExecuteScalar();

                        if (result == null)
                        {
                            string createDatabaseQuery = $"CREATE DATABASE {SqlCommands.DatabaseName}";
                            using (SqlCommand createCommand = new SqlCommand(createDatabaseQuery, connection))
                            {
                                createCommand.ExecuteNonQuery();
                            }

                            string tableCreationQuery = $@"
                                                        USE {SqlCommands.DatabaseName};
                                                        CREATE TABLE [User] (
                                                            ID INT PRIMARY KEY IDENTITY,
                                                            Name NVARCHAR(50),
                                                            Age INT,
                                                            Email NVARCHAR(250),
                                                            Password NVARCHAR(50),
                                                            Country NVARCHAR(50)
                                                        )";

                            using (SqlCommand tableCommand = new SqlCommand(tableCreationQuery, connection))
                            {
                                tableCommand.ExecuteNonQuery();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    throw;
                }
            }
        }

        private void TextEMail_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(textEmail.Text))
            {
                if (!RegexUtil.isEMailRegex(textEmail.Text))
                {
                    toolTipEmail.Show("Please enter a valid email address!!", textEmail);
                    buttonLogin.Enabled = false;
                }
                else
                {
                    toolTipEmail.Hide(textEmail);
                    buttonLogin.Enabled = true;
                }
            }
            else
            {
                buttonLogin.Enabled = false;
            }
        }
    }
}

