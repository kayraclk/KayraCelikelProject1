using System;
using System.Configuration;
using System.Data;
using System.Data.SQLite;
using System.Windows.Forms;

namespace Form_App
{
    public partial class frmEmployee : Form
    {
        private int selectedEmployeeId = -1;

        public frmEmployee()
        {
            InitializeComponent();
            LoadEmployees();

            
            txtSalary.KeyPress += txtSalary_KeyPress;
        }

        private static string LoadConnectionString(string id = "Default")
        {
            return ConfigurationManager.ConnectionStrings[id].ConnectionString;
        }

        private void LoadEmployees()
        {
            using (SQLiteConnection conn = new SQLiteConnection(LoadConnectionString()))
            {
                conn.Open();
                string query = "SELECT * FROM Employees";
                SQLiteDataAdapter adapter = new SQLiteDataAdapter(query, conn);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                dgvEmployees.DataSource = dt;
            }
        }

        private void ClearForm()
        {
            txtName.Text = "";
            txtPosition.Text = "";
            txtSalary.Text = "";
            selectedEmployeeId = -1;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text) ||
                string.IsNullOrWhiteSpace(txtPosition.Text) ||
                string.IsNullOrWhiteSpace(txtSalary.Text))
            {
                MessageBox.Show("Please fill in all fields.");
                return;
            }

            using (SQLiteConnection conn = new SQLiteConnection(LoadConnectionString()))
            {
                conn.Open();
                string query = "INSERT INTO Employees (Name, Position, Salary) VALUES (@Name, @Position, @Salary)";
                using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Name", txtName.Text);
                    cmd.Parameters.AddWithValue("@Position", txtPosition.Text);
                    cmd.Parameters.AddWithValue("@Salary", Convert.ToDouble(txtSalary.Text));
                    cmd.ExecuteNonQuery();
                }
            }

            MessageBox.Show("Employee added successfully!");
            LoadEmployees();
            ClearForm();
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (selectedEmployeeId == -1)
            {
                MessageBox.Show("Please select an employee to edit.");
                return;
            }

            using (SQLiteConnection conn = new SQLiteConnection(LoadConnectionString()))
            {
                conn.Open();
                string query = "UPDATE Employees SET Name = @Name, Position = @Position, Salary = @Salary WHERE EmployeeID = @EmployeeID";
                using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Name", txtName.Text);
                    cmd.Parameters.AddWithValue("@Position", txtPosition.Text);
                    cmd.Parameters.AddWithValue("@Salary", Convert.ToDouble(txtSalary.Text));
                    cmd.Parameters.AddWithValue("@EmployeeID", selectedEmployeeId);
                    cmd.ExecuteNonQuery();
                }
            }

            MessageBox.Show("Employee updated successfully!");
            LoadEmployees();
            ClearForm();
        }

        private void button1_Click(object sender, EventArgs e) // Delete
        {
            if (selectedEmployeeId == -1)
            {
                MessageBox.Show("Please select an employee to delete.");
                return;
            }

            DialogResult confirm = MessageBox.Show("Are you sure you want to delete this employee?", "Confirm Delete", MessageBoxButtons.YesNo);
            if (confirm == DialogResult.Yes)
            {
                using (SQLiteConnection conn = new SQLiteConnection(LoadConnectionString()))
                {
                    conn.Open();
                    string query = "DELETE FROM Employees WHERE EmployeeID = @EmployeeID";
                    using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@EmployeeID", selectedEmployeeId);
                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Employee deleted successfully!");
                LoadEmployees();
                ClearForm();
            }
        }

        private void dgvEmployees_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvEmployees.Rows[e.RowIndex];
                selectedEmployeeId = Convert.ToInt32(row.Cells["EmployeeID"].Value);
                txtName.Text = row.Cells["Name"].Value.ToString();
                txtPosition.Text = row.Cells["Position"].Value.ToString();
                txtSalary.Text = row.Cells["Salary"].Value.ToString();
            }
        }

        private void txtSalary_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Allow digits, backspace, and one dot
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
            {
                e.Handled = true;
            }

            // Prevent more than one dot
            if (e.KeyChar == '.' && txtSalary.Text.Contains("."))
            {
                e.Handled = true;
            }
        }
    }
}
