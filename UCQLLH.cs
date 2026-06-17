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
        List<tbl_lophoc> _allData;
        int _currentPage = 1;
        int _pageSize = 3;
        int _totalPages = 1;

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
            //List<tbl_lophoc> dSLH = db.tbl_lophocs.ToList();
            //dgvLopHoc.DataSource = dSLH;
            try
            {
                _allData = db.tbl_lophocs
                             .OrderBy(lh => lh.malop)
                             .ToList();
                ApplyPaging();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi kết nối CSDL:\n" + ex.Message, "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ApplyPaging()
        {
            _totalPages = Math.Max(1, (int)Math.Ceiling(_allData.Count / (double)_pageSize));
            if (_currentPage < 1) _currentPage = 1;
            if (_currentPage > _totalPages) _currentPage = _totalPages;

            dgvLopHoc.DataSource = _allData
                .Skip((_currentPage - 1) * _pageSize)
                .Take(_pageSize)
                .Select(lh => new
                {
                    lh.id,
                    lh.malop,
                    lh.tenlop,
                    lh.ghichu
                })
                .ToList();

            lb_trang.Text = _currentPage + "/" + _totalPages;
            lb_soBanGhi.Text = _allData.Count + " bản ghi";
        }

        private void LoadLopHocTheoTu(string tuKhoa)
        {
            string tk = tuKhoa.Trim();
            _allData = db.tbl_lophocs
                          .Where(lh =>
                              lh.id.ToString().Contains(tk) ||
                              lh.malop.Contains(tk) ||
                              lh.tenlop.Contains(tk))
                          .OrderBy(lh => lh.malop)
                          .ToList();
            _currentPage = 1;
            ApplyPaging();
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
        private void dgv_DSLH_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
            {
                return;
            }
            var row = dgvLopHoc.Rows[e.RowIndex];

            _selectedId = Convert.ToInt32(row.Cells["id"].Value);
            txtMaID.Text = _selectedId.ToString();
            txtMaLop.Text = row.Cells["malop"].Value?.ToString();
            txtTenLop.Text = row.Cells["tenlop"].Value?.ToString();
            txtGhichu.Text = row.Cells["ghichu"].Value?.ToString();

            txtMaID.Enabled = false;
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
            LoadLopHocTheoTu(txtTimKiem.Text);
        }

        private void btn_xemDanhSach_Click(List<SinhVien> danhSachSinhVien)
        {
            using (Form formDanhSach = new Form())
            {
                Label lblTieuDe = new Label
                {
                    Dock = DockStyle.Top,
                    Height = 36,
                    Padding = new Padding(10, 0, 0, 0),
                    TextAlign = ContentAlignment.MiddleLeft,
                    Text = $"Lop {maLopDangChon} - {danhSachSinhVien.Count} sinh vien"
                };

                DataGridView dgvSinhVien = new DataGridView
                {
                    AllowUserToAddRows = false,
                    AllowUserToDeleteRows = false,
                    AutoGenerateColumns = false,
                    AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                    ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize,
                    Dock = DockStyle.Fill,
                    MultiSelect = false,
                    ReadOnly = true,
                    SelectionMode = DataGridViewSelectionMode.FullRowSelect
                };

                dgvSinhVien.Columns.Add(new DataGridViewTextBoxColumn
                {
                    DataPropertyName = "MaSV",
                    HeaderText = "Ma SV",
                    Name = "colMaSV"
                });
                dgvSinhVien.Columns.Add(new DataGridViewTextBoxColumn
                {
                    DataPropertyName = "HoTen",
                    HeaderText = "Ho ten",
                    Name = "colHoTen"
                });
                dgvSinhVien.Columns.Add(new DataGridViewTextBoxColumn
                {
                    DataPropertyName = "GioiTinh",
                    HeaderText = "Gioi tinh",
                    Name = "colGioiTinh"
                });
                dgvSinhVien.Columns.Add(new DataGridViewTextBoxColumn
                {
                    DataPropertyName = "NgaySinh",
                    HeaderText = "Ngay sinh",
                    Name = "colNgaySinh",
                    DefaultCellStyle = { Format = "dd/MM/yyyy" }
                });
                dgvSinhVien.Columns.Add(new DataGridViewTextBoxColumn
                {
                    DataPropertyName = "Lop",
                    HeaderText = "Lop",
                    Name = "colLop"
                });
                dgvSinhVien.DataSource = danhSachSinhVien;

                formDanhSach.Text = "Danh sach sinh vien";
                formDanhSach.StartPosition = FormStartPosition.CenterParent;
                formDanhSach.Size = new Size(760, 420);
                formDanhSach.MinimizeBox = false;
                formDanhSach.Controls.Add(dgvSinhVien);
                formDanhSach.Controls.Add(lblTieuDe);
                formDanhSach.ShowDialog(this);
            }
        }

        private void btn_trangDau_Click(object sender, EventArgs e)
        {
            _currentPage = 1;
            ApplyPaging();
        }

        private void btn_trangTruoc_Click(object sender, EventArgs e)
        {
            _currentPage--;
            ApplyPaging();
        }

        private void btn_trangSau_Click(object sender, EventArgs e)
        {
            _currentPage++;
            ApplyPaging();
        }

        private void btn_trangCuoi_Click(object sender, EventArgs e)
        {
            _currentPage = _totalPages;
            ApplyPaging();
        }
    }
}
