using System;
using System.Windows.Forms;

namespace Form_App
{
    public partial class frmMain : Form
    {
        private string currentUser;

        public frmMain(string username)
        {
            InitializeComponent();
            currentUser = username;
            lblWelcome.Text = $"Welcome, {currentUser}";
        }

        private void btnManageEmployees_Click(object sender, EventArgs e)
        {
            frmEmployee employeeForm = new frmEmployee();
            employeeForm.ShowDialog();
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Are you sure you want to logout?", "Confirm Logout", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                this.Hide();
                login loginForm = new login(); 
                loginForm.Show();
            }
        }
    }
}
