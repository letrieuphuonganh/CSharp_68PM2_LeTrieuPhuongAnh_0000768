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
            sinhvien.id = int.Parse(mSSV);
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
            List<tbl_sinhvien> dSSV = db.tbl_sinhviens.ToList();
            dgvSinhVien.DataSource = dSSV;
        }
        public void LoadDSLH4CBX() //load dữ liệu cho combobox
        {
            List<tbl_lophoc> dSLH = db.tbl_lophocs.ToList();
            cbMaLop.DataSource = dSLH;
            cbMaLop.DisplayMember = "tenlop";
            cbMaLop.ValueMember = "malop";
        }
    }
}
