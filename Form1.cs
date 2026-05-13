using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Quanlysinhvien
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private class Student
        {
            public string Email { get; set; }
            public string MSSV { get; set; }
        }

        // Small in-memory list of students for demo purposes
        private readonly List<Student> students = new List<Student>
        {
            new Student { Email = "0000768@st.huce.edu.vn", MSSV = "0000768" }
        };

        private void btnLogin_Click(object sender, EventArgs e)
        {
            var username = txtUsername.Text.Trim();
            var password = txtPassword.Text; // password equals MSSV in this simple example

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Vui lòng nhập tên đăng nhập và mật khẩu", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var found = students.FirstOrDefault(s => string.Equals(s.Email, username, StringComparison.OrdinalIgnoreCase)
                                                     && s.MSSV == password);

            if (found != null)
            {
                MessageBox.Show("Đăng nhập thành công", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Đăng nhập thất bại", "Thất bại", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
