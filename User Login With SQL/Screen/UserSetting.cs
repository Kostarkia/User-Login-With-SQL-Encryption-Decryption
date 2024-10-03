using System;
using System.Data.SqlClient;
using System.Windows.Forms;
using User_Login_With_SQL.Connection;
using User_Login_With_SQL.Models;
using User_Login_With_SQL.Util;

namespace User_Login_With_SQL
{
    public partial class UserSetting : Form
    {
        private LoginModels _model;

        public UserSetting(LoginModels model)
        {
            InitializeComponent();
            _model = model;
        }

        string ID;

        private void UserSetting_Load(object sender, EventArgs e)
        {
            var password = Util.EncryptionDecryptionUtil.Decrypt(_model.Password);
             
            ID = _model.ID;
            textName.Text = _model.Name;
            numericUpAge.Text = _model.Age;
            textEmail.Text = _model.Email;
            textPassword.Text = password;
            textCountry.Text = _model.Country;
        }

        private void ButtonSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textName.Text) || string.IsNullOrWhiteSpace(numericUpAge.Text) || string.IsNullOrWhiteSpace(textEmail.Text) || string.IsNullOrWhiteSpace(textPassword.Text))
            {
                MessageBox.Show("Please do not leave mandatory fields blank. ! ", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }



            using (SqlConnection connection = new SqlConnection(SqlCommands.SqlConnectionURL))
            {
                connection.Open();

                SqlCommand filterCmd = new SqlCommand("Select Email From [User] where Email='" + textEmail.Text + "'", connection);

                using (SqlDataReader reader = filterCmd.ExecuteReader())
                {
                    if (textEmail.Text != _model.Email)
                    {
                        if (reader.HasRows == true)
                        {
                            MessageBox.Show("Email has already been received. Please enter a new email address.", "Email Registered", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }

                    reader.Close();
                }

                var password = Util.EncryptionDecryptionUtil.Encrypt(textPassword.Text);

                SqlCommand cmd = new SqlCommand("UPDATE [User] set Name=@Name , Age= @Age , Email= @Email , Password= @Password , Country= @Country where  ID =" + ID, connection);

                cmd.Parameters.AddWithValue("@Name", textName.Text);
                cmd.Parameters.AddWithValue("@Age", numericUpAge.Text);
                cmd.Parameters.AddWithValue("@Email", textEmail.Text);
                cmd.Parameters.AddWithValue("@Password", password);
                cmd.Parameters.AddWithValue("@Country", textCountry.Text);

                cmd.ExecuteNonQuery();

                MessageBox.Show("Your information has been successfully saved.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                connection.Close();

                Close();

            }
        }

        private void TextEmail_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(textEmail.Text))
            {
                if (!RegexUtil.isEMailRegex(textEmail.Text))
                {
                    toolTipEmail.Show("Please enter a valid email address!!", textEmail);
                    buttonSave.Enabled = false;
                }
                else
                {
                    toolTipEmail.Hide(textEmail);
                    buttonSave.Enabled = true;
                }
            }
            else
            {
                buttonSave.Enabled = false;
            }
        }
    }
}
