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
    public partial class UCQLSV : UserControl
    {
        databaseDataContext db = new databaseDataContext(); //khai báo để dùng cho Load để đẩy dữa liệu lên
        string _selectedMaSV;
        List<tbl_sinhvien> _allData;
        int _currentPage = 1;
        int _pageSize = 2;
        int _totalPages = 1;
        public UCQLSV()
        {
            InitializeComponent();
        }
        //đẩy data lên giao diện
        private void UCQLSV_Load(object sender, EventArgs e)
        {
            LoadData();
            LoadDSLH4CBX();

        }
        //thao tác với nút thêm mới
        private void btnThem_Click(object sender, EventArgs e)
        {
            //lấy về dữ liệu
            /*string mSSV = txtMaSV.Text;
            string hoTen = txtHoTen.Text;
            string gioiTinh = cbGioiTinh.Text;
            string ngaySinh = dtNgaySinh.Text;
            tbl_sinhvien sinhvien = new tbl_sinhvien();
            sinhvien.masv = mSSV;
            sinhvien.hoten = hoTen;
            sinhvien.gioitinh = gioiTinh;
            sinhvien.ngaysinh = DateTime.Parse(ngaySinh);*/
            
            //code thay thế đoạn trên - ngắn gọn hơn do lồng ghép
            tbl_sinhvien sinhvien = new tbl_sinhvien();
            sinhvien.id = int.Parse(txtMaSV.Text);
            sinhvien.hoten = txtHoTen.Text;
            sinhvien.gioitinh = cbGioiTinh.Text;
            sinhvien.ngaysinh = DateTime.Parse(dtNgaySinh.Text);
            sinhvien.malop = cbMaLop.SelectedValue.ToString();
            try
            {
                //thêm mới 1 bản ghi
                db.tbl_sinhviens.InsertOnSubmit(sinhvien);
                db.SubmitChanges();
                MessageBox.Show("Thêm mới sinh viên thành công");
                //load lại dữ liệu vueaf thêm lên giao diện luôn
                LoadData();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        public void LoadData()
        {
            //List<tbl_sinhvien> dSSV = db.tbl_sinhviens.ToList();
            //dgvSinhVien.DataSource = dSSV;
            try
            {
                _allData = db.tbl_sinhviens
                             .OrderBy(sv => sv.id)
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

            dgvSinhVien.DataSource = _allData
                .Skip((_currentPage - 1) * _pageSize)
                .Take(_pageSize)
                .Select(sv => new
                {
                    sv.id,
                    sv.hoten,
                    sv.gioitinh,
                    sv.ngaysinh,
                    sv.malop
                })
                .ToList();

            lb_trang.Text = _currentPage + "/" + _totalPages;
            lb_soBanGhi.Text = _allData.Count + " bản ghi";
        }

        private void LoadSinhVienTheoTu(string tuKhoa)
        {
            string tk = tuKhoa.Trim();
            _allData = db.tbl_sinhviens
                          .Where(sv =>
                              sv.id.ToString().Contains(tk) ||
                              sv.hoten.Contains(tk) ||
                              sv.malop.Contains(tk))
                          .OrderBy(sv => sv.id)
                          .ToList();
            _currentPage = 1;
            ApplyPaging();
        }
        public void LoadDSLH4CBX() //load dữ liệu cho combobox
        {
            List<tbl_lophoc> dSLH = db.tbl_lophocs.ToList();
            cbMaLop.DataSource = dSLH;
            cbMaLop.DisplayMember = "tenlop";
            cbMaLop.ValueMember = "malop";
        }

        private void ClearForm()
        {
            txtMaSV.Clear();
            txtHoTen.Clear();
            dtNgaySinh.Value = DateTime.Now;
            cbGioiTinh.SelectedIndex = -1;
            cbMaLop.SelectedIndex = -1;

            _selectedMaSV = "";
            txtHoTen.Focus();
        }

        private void dgvSinhVien_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
            {
                return;
            }
            var row = dgvSinhVien.Rows[e.RowIndex];

            _selectedMaSV = row.Cells["id"].Value.ToString();
            txtMaSV.Text = _selectedMaSV;
            txtHoTen.Text = row.Cells["hoten"].Value.ToString();
            cbGioiTinh.Text = row.Cells["gioitinh"].Value.ToString();

            txtMaSV.Enabled = false;

            string malop = row.Cells["malop"].Value?.ToString().Trim();
            if (!string.IsNullOrEmpty(malop))
                cbMaLop.SelectedValue = malop;
            else if (cbMaLop.Items.Count > 0)
                cbMaLop.SelectedIndex = 0;

            if (row.Cells["ngaysinh"].Value is DateTime dt)
                dtNgaySinh.Value = dt;
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_selectedMaSV))
            {
                MessageBox.Show("Vui lòng chọn sinh viên cần sửa!", "Cảnh báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var sv = db.tbl_sinhviens.FirstOrDefault(x => x.id == Convert.ToInt32(_selectedMaSV));
            if (sv == null) 
            { 
                MessageBox.Show("Không tìm thấy sinh viên!"); 
                return; 
            }

            sv.hoten = txtHoTen.Text.Trim();
            sv.ngaysinh = dtNgaySinh.Value.Date;
            sv.gioitinh = cbGioiTinh.Text;
            sv.malop = cbMaLop.SelectedValue?.ToString()?.Trim();

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

        private void btnLamMoi_Click(object sender, EventArgs e)
        {
            ClearForm();
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_selectedMaSV))
            {
                MessageBox.Show("Vui lòng chọn sinh viên cần xóa!", "Cảnh báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var result = MessageBox.Show(
                "Bạn có chắc muốn xóa sinh viên '" + txtHoTen.Text + "'?",
                "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result != DialogResult.Yes) return;

            var sv = db.tbl_sinhviens.FirstOrDefault(x => x.id == Convert.ToInt32(_selectedMaSV));
            if (sv == null) { MessageBox.Show("Không tìm thấy sinh viên!"); return; }

            try
            {
                db.tbl_sinhviens.DeleteOnSubmit(sv);
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

        private void btnTim_Click(object sender, EventArgs e)
        {
            LoadSinhVienTheoTu(txtTimKiem.Text);
        }
    }
}

