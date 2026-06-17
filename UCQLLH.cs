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
    public partial class UCQLLH : UserControl
    {
        databaseDataContext db = new databaseDataContext();
        int _selectedId = 0;
        public UCQLLH()
        {
            InitializeComponent();
        }

        private void UCQLLH_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        public void LoadData()
        {
            List<tbl_lophoc> dSLH = db.tbl_lophocs.ToList();
            dgvLopHoc.DataSource = dSLH;
        }

        private void ClearForm()
        {
            txtMaID.Clear();
            txtMaLop.Clear();
            txtTenLop.Clear();
            txtGhichu.Clear();

            _selectedId = 0;
            txtMaLop.Focus();
        }
        private void btnThem_Click(object sender, EventArgs e)
        {
            tbl_lophoc lophoc = new tbl_lophoc();
            lophoc.malop = txtMaLop.Text.Trim();
            lophoc.tenlop = txtTenLop.Text.Trim();
            lophoc.ghichu = txtGhichu.Text.Trim();

            try
            {
                db.tbl_lophocs.InsertOnSubmit(lophoc);
                db.SubmitChanges();
                MessageBox.Show("Thêm mới lớp học thành công");
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void btnSua_Click(object sender, EventArgs e)
        {
            if (_selectedId == 0)
            {
                MessageBox.Show("Vui lòng chọn lớp học cần sửa!", "Cảnh báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var lh = db.tbl_lophocs.FirstOrDefault(x => x.id == _selectedId);
            if (lh == null) { MessageBox.Show("Không tìm thấy lớp học!"); return; }

            lh.malop = txtMaLop.Text.Trim();
            lh.tenlop = txtTenLop.Text.Trim();
            lh.ghichu = txtGhichu.Text.Trim();

            try
            {
                db.SubmitChanges();
                MessageBox.Show("Cập nhật thành công!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadData();
                ClearForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi sửa:\n" + ex.Message, "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (_selectedId == 0)
            {
                MessageBox.Show("Vui lòng chọn lớp học cần xóa!", "Cảnh báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var result = MessageBox.Show(
                "Bạn có chắc muốn xóa lớp học '" + txtTenLop.Text + "'?",
                "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result != DialogResult.Yes) return;

            var lh = db.tbl_lophocs.FirstOrDefault(x => x.id == _selectedId);
            if (lh == null) { MessageBox.Show("Không tìm thấy lớp học!"); return; }

            try
            {
                db.tbl_lophocs.DeleteOnSubmit(lh);
                db.SubmitChanges();
                MessageBox.Show("Xóa thành công!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadData();
                ClearForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi xóa:\n" + ex.Message, "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void btnLamMoi_Click(object sender, EventArgs e)
        {
            ClearForm();
        }

        private void btnTim_Click(object sender, EventArgs e)
        {

        }

        private void btn_xemDanhSach_Click(object sender, EventArgs e)
        {

        }
    }
}
