using System;
using System.Data.SqlClient;
using System.Windows.Forms;
using User_Login_With_SQL.Connection;
using User_Login_With_SQL.Util;

namespace User_Login_With_SQL
{
    public partial class Register : Form
    {
        public Register()
        {
            InitializeComponent();
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
                    if (reader.HasRows == true)
                    {
                        MessageBox.Show("Email has already been received. Please enter a new email address.", "Email Registered", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    reader.Close();
                }

                var password = Util.EncryptionDecryptionUtil.Encrypt(textPassword.Text);

                SqlCommand cmd = new SqlCommand("Insert Into [User](Name,Age,Email,Password,Country) Values (@Name ,@Age ,@Email ,@Password ,@Country)", connection);

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
