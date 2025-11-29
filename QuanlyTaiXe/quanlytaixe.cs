using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using MENUQL;

namespace QuanlyTaiXe
{
    public partial class quanlytaixe : BaseForm
    {
        // Replace with your SQL Server connection string.
        private const string ConnectionString = "Server=LAPTOP-NABV6PHG\\KANTOU;Database=Quanlyxevalaixe;Trusted_Connection=True;Encrypt=True;TrustServerCertificate=True;";

        public quanlytaixe()
        {
            InitializeComponent();
            this.Icon = System.Drawing.Icon.FromHandle(Quanlyxevalaixe.Properties.Resources.icons8_driver_48.GetHicon());
            HookEvents();
            datengaysinh.Value = DateTime.Today.AddYears(-25);
            LoadDriversIntoGrid();

            // Ensure the exit button closes/hides the modal on a single click
            if (btnthoat != null)
            {
                btnthoat.DialogResult = DialogResult.Cancel;
                this.CancelButton = btnthoat;
            }
        }

        private void HookEvents()
        {
            btnthem.Click += Btnthem_Click;
            btnsua.Click += Btnsua_Click;
            btnluu.Click += Btnluu_Click;
            btnxoa.Click += Btnxoa_Click;
            btnhuy.Click += Btnhuy_Click;
            btnthoat.Click += Btnthoat_Click;
            dataGridView1.SelectionChanged += DataGridView1_SelectionChanged;

            // Handle clicks on the form background (not on child controls)
            this.MouseClick += FormBackground_MouseClick;
        }

        private void FormBackground_MouseClick(object? sender, MouseEventArgs e)
        {
            // Only treat as "background click" when no child control exists at the click location.
            if (GetChildAtPoint(e.Location) == null)
            {
                ClearInputs();
                dataGridView1.ClearSelection();
            }
        }

        #region Button handlers

        private void Btnthem_Click(object? sender, EventArgs e)
        {
            // Treat this button as "insert now" — enable inputs so user can enter or edit it,
            // validate current inputs and insert to database immediately.
            SetInputsEnabled(true);
            txtMaTX.Enabled = true;

            if (!ValidateInputs(out var msg))
            {
                MessageBox.Show(msg, "Lỗi nhập liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtMaTX.Focus();
                return;
            }

            if (AddDriverToDb())
            {
                LoadDriversIntoGrid();
                ClearInputs();
                txtMaTX.Focus();
            }
        }

        private void Btnsua_Click(object? sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow?.DataBoundItem is not DataRowView drv)
            {
                MessageBox.Show("Vui lòng chọn tài xế để sửa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            LoadDataRowToInputs(drv.Row);
            // allow editing of all inputs for "Sửa", but keep MaTX read-only
            SetInputsEnabled(true);
            txtMaTX.Enabled = false;
        }

        private void Btnluu_Click(object? sender, EventArgs e)
        {
            if (!ValidateInputs(out var msg))
            {
                MessageBox.Show(msg, "Lỗi nhập liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (txtMaTX.Enabled)
            {
                if (AddDriverToDb()) LoadDriversIntoGrid();
            }
            else
            {
                if (UpdateDriverInDb()) LoadDriversIntoGrid();
            }
        }

        private void Btnxoa_Click(object? sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow?.DataBoundItem is not DataRowView drv)
            {
                MessageBox.Show("Vui lòng chọn tài xế để xóa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var ma = drv.Row.Field<string>("MaTX");
            if (ma == null)
            {
                MessageBox.Show("Selected driver has no `MaTX` value; cannot delete.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var confirm = MessageBox.Show($"Xóa tài xế mã {ma}?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes) return;

            if (DeleteDriverFromDb(ma))
            {
                LoadDriversIntoGrid();
                ClearInputs();
            }
        }

        private void Btnhuy_Click(object? sender, EventArgs e)
        {
            ClearInputs();
            SetInputsEnabled(true);
            dataGridView1.ClearSelection();
        }

        private void Btnthoat_Click(object? sender, EventArgs e)
        {
            Close();
        }

        private void BtnReset_Click(object? sender, EventArgs e)
        {
            // Clear the input controls so search filters are removed
            ClearInputs();

            // Reload the full driver list from database into the DataGridView
            LoadDriversIntoGrid();

            // Ensure no row is selected after reload
            dataGridView1.ClearSelection();
        }

        #endregion

        #region Grid + UI

        private void LoadDriversIntoGrid()
        {
            try
            {
                var dt = GetAllDrivers();
                if (dt == null)
                {
                    MessageBox.Show("Database returned no data (null). Check connection string and database.", "Debug", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                dataGridView1.DataSource = dt;
                dataGridView1.AutoGenerateColumns = true;
                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                dataGridView1.ClearSelection();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Không thể tải danh sách tài xế.\n{ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DataGridView1_SelectionChanged(object? sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow?.DataBoundItem is DataRowView drv)
            {
                LoadDataRowToInputs(drv.Row);
                // While selecting a grid row, keep inputs disabled (read-only view)
                SetInputsEnabled(false);
            }
            else
            {
                // no selection
                ClearInputs();
            }
        }

        private void LoadDataRowToInputs(DataRow row)
        {
            txtMaTX.Text = row.Field<string>("MaTX") ?? "";
            txtHolot.Text = row.Field<string>("Holot") ?? "";
            txtTen.Text = row.Field<string>("Ten") ?? "";
            var phai = ParsePhaiToBool(row["Phai"]);
            radiobtnNam.Checked = phai;
            radiobtnNu.Checked = !phai;
            datengaysinh.Value = row.Field<DateTime?>("NgaySinh") ?? DateTime.Today.AddYears(-25);
            txtBanglai.Text = row.Field<string>("BangLai") ?? "";
            txtSdt.Text = row.Field<string>("Sdt") ?? "";
        }

        // helper to enable/disable all input controls in the form consistently
        private void SetInputsEnabled(bool enabled)
        {
            // textboxes
            txtMaTX.Enabled = enabled;
            txtHolot.Enabled = enabled;
            txtTen.Enabled = enabled;
            txtSdt.Enabled = enabled;
            txtBanglai.Enabled = enabled;

            // radio buttons (Phai)
            radiobtnNam.Enabled = enabled;
            radiobtnNu.Enabled = enabled;

            // date picker (NgaySinh)
            datengaysinh.Enabled = enabled;
        }

        private void ClearInputs()
        {
            txtMaTX.Text = "";
            txtHolot.Text = "";
            txtTen.Text = "";
            radiobtnNam.Checked = true;
            datengaysinh.Value = DateTime.Today.AddYears(-25);
            txtBanglai.Text = "";
            txtSdt.Text = "";
            SetInputsEnabled(true);
        }

        #endregion

        #region Validation

        private bool ValidateInputs(out string message)
        {
            if (string.IsNullOrWhiteSpace(txtMaTX.Text))
            {
                message = "Mã tài xế không được để trống.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtTen.Text))
            {
                message = "Tên không được để trống.";
                return false;
            }

            message = "";
            return true;
        }

        #endregion

        #region Database operations (ADO.NET)

        private DataTable GetAllDrivers()
        {
            var dt = new DataTable();
            try
            {
                using var cn = new SqlConnection(ConnectionString);
                using var cmd = new SqlCommand("SELECT MaTX, Holot, Ten, Phai, NgaySinh, BangLai, Sdt FROM [dbo].[TaiXe] ORDER BY MaTX", cn);
                using var da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error querying `TaiXe` table.\n{ex.Message}", "DB Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return dt;
        }

        private bool AddDriverToDb()
        {
            const string sql = @"
INSERT INTO [dbo].[TaiXe] (MaTX, Holot, Ten, Phai, NgaySinh, BangLai, Sdt)
VALUES (@MaTX, @Holot, @Ten, @Phai, @NgaySinh, @BangLai, @Sdt)";
            try
            {
                using var cn = new SqlConnection(ConnectionString);
                using var cmd = new SqlCommand(sql, cn);
                cmd.Parameters.AddWithValue("@MaTX", txtMaTX.Text.Trim());
                cmd.Parameters.AddWithValue("@Holot", txtHolot.Text.Trim());
                cmd.Parameters.AddWithValue("@Ten", txtTen.Text.Trim());
                cmd.Parameters.AddWithValue("@Phai", BoolToPhaiString(radiobtnNam.Checked));
                cmd.Parameters.AddWithValue("@NgaySinh", datengaysinh.Value.Date);
                cmd.Parameters.AddWithValue("@BangLai", txtBanglai.Text.Trim());
                cmd.Parameters.AddWithValue("@Sdt", txtSdt.Text.Trim());

                cn.Open();
                cmd.ExecuteNonQuery();
                return true;
            }
            catch (SqlException ex) when (ex.Number == 2627 || ex.Number == 2601) // unique violation
            {
                MessageBox.Show("Mã tài xế đã tồn tại.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi thêm tài xế.\n{ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private bool UpdateDriverInDb()
        {
            const string sql = @"
UPDATE [dbo].[TaiXe]
SET Holot = @Holot, Ten = @Ten, Phai = @Phai, NgaySinh = @NgaySinh, BangLai = @BangLai, Sdt = @Sdt
WHERE MaTX = @MaTX";
            try
            {
                using var cn = new SqlConnection(ConnectionString);
                using var cmd = new SqlCommand(sql, cn);
                cmd.Parameters.AddWithValue("@MaTX", txtMaTX.Text.Trim());
                cmd.Parameters.AddWithValue("@Holot", txtHolot.Text.Trim());
                cmd.Parameters.AddWithValue("@Ten", txtTen.Text.Trim());
                cmd.Parameters.AddWithValue("@Phai", BoolToPhaiString(radiobtnNam.Checked));
                cmd.Parameters.AddWithValue("@NgaySinh", datengaysinh.Value.Date);
                cmd.Parameters.AddWithValue("@BangLai", txtBanglai.Text.Trim());
                cmd.Parameters.AddWithValue("@Sdt", txtSdt.Text.Trim());

                cn.Open();
                var affected = cmd.ExecuteNonQuery();
                if (affected == 0)
                {
                    MessageBox.Show("Không tìm thấy tài xế để cập nhật.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi cập nhật tài xế.\n{ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private bool DeleteDriverFromDb(string maTX)
        {
            const string sql = "DELETE FROM [dbo].[TaiXe] WHERE MaTX = @MaTX";
            try
            {
                using var cn = new SqlConnection(ConnectionString);
                using var cmd = new SqlCommand(sql, cn);
                cmd.Parameters.AddWithValue("@MaTX", maTX);
                cn.Open();
                var affected = cmd.ExecuteNonQuery();
                if (affected == 0)
                {
                    MessageBox.Show("Không tìm thấy tài xế để xóa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi xóa tài xế.\n{ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        #endregion

        private static bool ParsePhaiToBool(object? value)
        {
            if (value == null || value == DBNull.Value) return true;
            if (value is bool b) return b;
            var s = value.ToString()!.Trim().ToLowerInvariant();
            return s switch
            {
                "nam" => true,
                "n" => true,
                "m" => true,
                "male" => true,
                "1" => true,
                "true" => true,
                "nữ" => false,
                "nu" => false,
                "f" => false,
                "female" => false,
                "0" => false,
                "false" => false,
                _ => true
            };
        }

        private static string BoolToPhaiString(bool value) => value ? "Nam" : "Nữ";


        // Search button handler (wired to btnTimkiem in Designer)
        private void button1_Click(object sender, EventArgs e)
        {
            // Collect trimmed values from textboxes
            var ma = txtMaTX.Text.Trim();
            var holot = txtHolot.Text.Trim();
            var ten = txtTen.Text.Trim();
            var sdt = txtSdt.Text.Trim();
            var banglai = txtBanglai.Text.Trim();

            // Require at least one non-empty search field (per requirement)
            if (string.IsNullOrWhiteSpace(ma)
                && string.IsNullOrWhiteSpace(holot)
                && string.IsNullOrWhiteSpace(ten)
                && string.IsNullOrWhiteSpace(sdt)
                && string.IsNullOrWhiteSpace(banglai))
            {
                MessageBox.Show("Vui lòng nhập ít nhất một tiêu chí tìm kiếm (mã, họ lót, tên, số điện thoại hoặc bằng lái).", "Tìm kiếm", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var sql = "SELECT MaTX, Holot, Ten, Phai, NgaySinh, BangLai, Sdt FROM [dbo].[TaiXe] WHERE 1=1";
            using var cn = new SqlConnection(ConnectionString);
            using var cmd = new SqlCommand();
            cmd.Connection = cn;

            if (!string.IsNullOrWhiteSpace(ma))
            {
                sql += " AND MaTX LIKE @MaTX";
                cmd.Parameters.AddWithValue("@MaTX", "%" + ma + "%");
            }

            if (!string.IsNullOrWhiteSpace(holot))
            {
                sql += " AND Holot LIKE @Holot";
                cmd.Parameters.AddWithValue("@Holot", "%" + holot + "%");
            }

            if (!string.IsNullOrWhiteSpace(ten))
            {
                sql += " AND Ten LIKE @Ten";
                cmd.Parameters.AddWithValue("@Ten", "%" + ten + "%");
            }

            if (!string.IsNullOrWhiteSpace(sdt))
            {
                sql += " AND Sdt LIKE @Sdt";
                cmd.Parameters.AddWithValue("@Sdt", "%" + sdt + "%");
            }

            if (!string.IsNullOrWhiteSpace(banglai))
            {
                sql += " AND BangLai LIKE @BangLai";
                cmd.Parameters.AddWithValue("@BangLai", "%" + banglai + "%");
            }

            // Optional: include Phái as a filter if you want to always constrain by the selected gender.
            // If you don't want that behavior, remove the block below.
            if (radiobtnNam.Checked || radiobtnNu.Checked)
            {
                sql += " AND Phai = @Phai";
                cmd.Parameters.AddWithValue("@Phai", BoolToPhaiString(radiobtnNam.Checked));
            }

            cmd.CommandText = sql + " ORDER BY MaTX";

            try
            {
                var dt = new DataTable();
                using var da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                dataGridView1.DataSource = dt;
                dataGridView1.AutoGenerateColumns = true;
                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                dataGridView1.ClearSelection();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tìm kiếm.\n{ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void quanlytaixe_Load(object sender, EventArgs e)
        {

        }
    }
}
