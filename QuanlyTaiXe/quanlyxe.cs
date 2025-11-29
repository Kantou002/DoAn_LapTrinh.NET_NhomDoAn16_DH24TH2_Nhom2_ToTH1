using System;
using System.Data;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using MENUQL;
using QuanlyTaiXe;


namespace QuanLyXe
{
    public class BaseForm : Form
    {
        public BaseForm()
        {
            // Use CenterParent so modal dialogs center relative to owner.
            // If no owner is provided, CenterParent falls back to screen-centering.
            StartPosition = FormStartPosition.CenterParent;
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

            // Ensure correct centering at show time: center to parent when owner set,
            // otherwise center to screen. Use try/catch to avoid designer-time issues.
            try
            {
                if (this.Owner != null)
                    CenterToParent();
                else
                    CenterToScreen();
            }
            catch
            {
                // ignore edge cases during startup / designer
            }
        }
    }
    public partial class quanlyxe : BaseForm
    {
        // Replace with your SQL Server connection string.
        private const string ConnectionString = "Server=LAPTOP-NABV6PHG\\KANTOU;Database=Quanlyxevalaixe;Trusted_Connection=True;Encrypt=True;TrustServerCertificate=True;";

        public quanlyxe()
        {
            InitializeComponent();
            this.Icon = System.Drawing.Icon.FromHandle(Quanlyxevalaixe.Properties.Resources.icons8_driver_48.GetHicon());
            HookEvents();

            // Ensure the exit button closes/hides the modal on a single click
            if (btnthoat != null)
            {
                btnthoat.DialogResult = DialogResult.Cancel;
                this.CancelButton = btnthoat;
            }
        }

        private void HookEvents()
        {
            // Note: btnthem is already wired to button1_Click in the designer.
            btnsua.Click += Btnsua_Click;
            btnluu.Click += Btnluu_Click;
            btnxoa.Click += Btnxoa_Click;
            btnhuy.Click += Btnhuy_Click;
            btnthoat.Click += Btnthoat_Click;
            dataGridView1.SelectionChanged += DataGridView1_SelectionChanged;

            // background click handler: reset inputs when clicking on the form background
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

        private void Form1_Load(object sender, EventArgs e)
        {
            SetInputsEnabled(true);
            LoadVehiclesIntoGrid();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // 'Thêm Xe' button behaviour: enable inputs for entering a new record,
            // then validate and insert immediately (same pattern as quanlytaixe).
            SetInputsEnabled(true);
            txtMaXB.Enabled = true;

            if (!ValidateInputs(out var msg))
            {
                MessageBox.Show(msg, "Lỗi nhập liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtMaXB.Focus();
                return;
            }

            if (AddVehicleToDb())
            {
                LoadVehiclesIntoGrid();
                ClearInputs();
                txtMaXB.Focus();
            }
        }

        private void Btnsua_Click(object? sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow?.DataBoundItem is not DataRowView drv)
            {
                MessageBox.Show("Vui lòng chọn xe để sửa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            LoadDataRowToInputs(drv.Row);
            SetInputsEnabled(true);
            txtMaXB.Enabled = false; // keep PK read-only when editing
        }

        private void Btnluu_Click(object? sender, EventArgs e)
        {
            if (!ValidateInputs(out var msg))
            {
                MessageBox.Show(msg, "Lỗi nhập liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (txtMaXB.Enabled)
            {
                if (AddVehicleToDb()) LoadVehiclesIntoGrid();
            }
            else
            {
                if (UpdateVehicleInDb()) LoadVehiclesIntoGrid();
            }
        }

        private void Btnxoa_Click(object? sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow?.DataBoundItem is not DataRowView drv)
            {
                MessageBox.Show("Vui lòng chọn xe để xóa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var ma = drv.Row.Field<string>("MaXB");
            if (ma == null)
            {
                MessageBox.Show("Selected vehicle has no `MaXB` value; cannot delete.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var confirm = MessageBox.Show($"Xóa xe mã {ma}?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes) return;

            if (DeleteVehicleFromDb(ma))
            {
                LoadVehiclesIntoGrid();
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
            this.Close();
        }

        private void DataGridView1_SelectionChanged(object? sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow?.DataBoundItem is DataRowView drv)
            {
                LoadDataRowToInputs(drv.Row);
                SetInputsEnabled(false);
            }
            else
            {
                ClearInputs();
            }
        }

        private void LoadDataRowToInputs(DataRow row)
        {
            txtMaXB.Text = row.Table.Columns.Contains("MaXB") ? row.Field<string>("MaXB") ?? "" : "";
            txtHangxe.Text = row.Table.Columns.Contains("HangXe") ? row.Field<string>("HangXe") ?? "" : "";
            cbbTT.Text = row.Table.Columns.Contains("TT") ? (row["TT"]?.ToString() ?? cbbTT.Items[0].ToString()) : cbbTT.Items[0].ToString();
            txtSoghe.Text = row.Table.Columns.Contains("SoGhe") ? row["SoGhe"]?.ToString() ?? "" : "";
            txtNSX.Text = row.Table.Columns.Contains("NSX") ? row["NSX"]?.ToString() ?? "" : "";
            txtBienso.Text = row.Table.Columns.Contains("BienSo") ? row.Field<string>("BienSo") ?? "" : "";
            txtMaBH.Text = row.Table.Columns.Contains("MaBH") ? row.Field<string>("MaBH") ?? "" : "";
        }

        private void ClearInputs()
        {
            txtMaXB.Text = "";
            txtHangxe.Text = "";
            cbbTT.SelectedIndex = 0;
            txtSoghe.Text = "";
            txtNSX.Text = "";
            txtBienso.Text = "";
            txtMaBH.Text = "";
            SetInputsEnabled(true);
        }

        // enable/disable all input controls consistently
        private void SetInputsEnabled(bool enabled)
        {
            txtMaXB.Enabled = enabled;
            txtHangxe.Enabled = enabled;
            cbbTT.Enabled = enabled;
            txtSoghe.Enabled = enabled;
            txtNSX.Enabled = enabled;
            txtBienso.Enabled = enabled;
            txtMaBH.Enabled = enabled;
        }

        private bool ValidateInputs(out string message)
        {
            if (string.IsNullOrWhiteSpace(txtMaXB.Text))
            {
                message = "Mã xe buýt không được để trống.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtBienso.Text))
            {
                message = "Biển số xe không được để trống.";
                return false;
            }

            if (cbbTT.SelectedItem == null || string.IsNullOrWhiteSpace(cbbTT.SelectedItem.ToString()))
            {
                message = "Vui lòng chọn tình trạng xe.";
                return false;
            }

            // optional: validate numeric fields
            if (!string.IsNullOrWhiteSpace(txtSoghe.Text) && !int.TryParse(txtSoghe.Text.Trim(), out _))
            {
                message = "Số ghế phải là số.";
                return false;
            }

            if (!string.IsNullOrWhiteSpace(txtNSX.Text) && !int.TryParse(txtNSX.Text.Trim(), out _))
            {
                message = "Năm sản xuất phải là số.";
                return false;
            }

            message = "";
            return true;
        }

        #region Database operations (ADO.NET)

        private DataTable GetAllVehicles()
        {
            var dt = new DataTable();
            try
            {
                using var cn = new SqlConnection(ConnectionString);
                using var cmd = new SqlCommand("SELECT MaXB, HangXe, TT, SoGhe, NSX, BienSo, MaBH FROM [dbo].[Xe] ORDER BY MaXB", cn);
                using var da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error querying `Xe` table.\n{ex.Message}", "DB Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return dt;
        }

        private void LoadVehiclesIntoGrid()
        {
            try
            {
                var dt = GetAllVehicles();
                if (dt == null)
                {
                    MessageBox.Show("Database returned no data (null). Check connection string and database.", "Debug", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                ConfigureDataGridView(dt);
                dataGridView1.ClearSelection();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Không thể tải danh sách xe.\n{ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Configure grid columns and bind the DataTable.
        /// Keeps column order, headers and formatting predictable regardless of database column ordering.
        /// </summary>
        private void ConfigureDataGridView(DataTable dt)
        {
            if (dt == null) return;

            dataGridView1.SuspendLayout();

            // Basic behaviours
            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.ReadOnly = true;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.MultiSelect = false;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // Clear any existing columns and add the ones we expect
            dataGridView1.Columns.Clear();

            // helper to add text column if column exists in the DataTable
            void AddTextColumn(string dataPropertyName, string headerText, int fillWeight = 100, DataGridViewContentAlignment align = DataGridViewContentAlignment.MiddleLeft)
            {
                if (!dt.Columns.Contains(dataPropertyName)) return;
                var col = new DataGridViewTextBoxColumn
                {
                    DataPropertyName = dataPropertyName,
                    HeaderText = headerText,
                    Name = dataPropertyName,
                    ReadOnly = true,
                    FillWeight = fillWeight,
                    DefaultCellStyle = { Alignment = align }
                };
                dataGridView1.Columns.Add(col);
            }

            AddTextColumn("MaXB", "Mã xe buýt", 80, DataGridViewContentAlignment.MiddleCenter);
            AddTextColumn("HangXe", "Hãng xe", 150);
            AddTextColumn("TT", "Tình trạng", 110, DataGridViewContentAlignment.MiddleCenter);
            AddTextColumn("SoGhe", "Số ghế", 70, DataGridViewContentAlignment.MiddleCenter);
            AddTextColumn("NSX", "Năm SX", 80, DataGridViewContentAlignment.MiddleCenter);
            AddTextColumn("BienSo", "Biển số", 120, DataGridViewContentAlignment.MiddleCenter);
            AddTextColumn("MaBH", "Mã BH", 100, DataGridViewContentAlignment.MiddleCenter);

            // Bind
            dataGridView1.DataSource = dt;

            // Optional: set specific column formatting where present
            if (dataGridView1.Columns.Contains("SoGhe"))
            {
                dataGridView1.Columns["SoGhe"].DefaultCellStyle.Format = "N0";
                dataGridView1.Columns["SoGhe"].SortMode = DataGridViewColumnSortMode.Automatic;
            }
            if (dataGridView1.Columns.Contains("NSX"))
            {
                dataGridView1.Columns["NSX"].DefaultCellStyle.Format = "N0";
                dataGridView1.Columns["NSX"].SortMode = DataGridViewColumnSortMode.Automatic;
            }

            // Make headers look consistent
            dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font(dataGridView1.Font, System.Drawing.FontStyle.Bold);

            dataGridView1.ResumeLayout();
        }

        private bool AddVehicleToDb()
        {
            const string sql = @"
INSERT INTO [dbo].[Xe] (MaXB, HangXe, TT, SoGhe, NSX, BienSo, MaBH)
VALUES (@MaXB, @HangXe, @TT, @SoGhe, @NSX, @BienSo, @MaBH)";
            try
            {
                using var cn = new SqlConnection(ConnectionString);
                using var cmd = new SqlCommand(sql, cn);
                cmd.Parameters.AddWithValue("@MaXB", txtMaXB.Text.Trim());
                cmd.Parameters.AddWithValue("@HangXe", txtHangxe.Text.Trim());
                cmd.Parameters.AddWithValue("@TT", cbbTT.SelectedItem?.ToString().Trim() ?? "");

                if (int.TryParse(txtSoghe.Text.Trim(), out var soghe))
                    cmd.Parameters.AddWithValue("@SoGhe", soghe);
                else
                    cmd.Parameters.AddWithValue("@SoGhe", DBNull.Value);

                if (int.TryParse(txtNSX.Text.Trim(), out var nsx))
                    cmd.Parameters.AddWithValue("@NSX", nsx);
                else
                    cmd.Parameters.AddWithValue("@NSX", DBNull.Value);

                cmd.Parameters.AddWithValue("@BienSo", txtBienso.Text.Trim());
                cmd.Parameters.AddWithValue("@MaBH", txtMaBH.Text.Trim());

                cn.Open();
                cmd.ExecuteNonQuery();
                return true;
            }
            catch (SqlException ex) when (ex.Number == 2627 || ex.Number == 2601)
            {
                MessageBox.Show("Mã xe buýt đã tồn tại.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi thêm xe.\n{ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private bool UpdateVehicleInDb()
        {
            const string sql = @"
UPDATE [dbo].[Xe]
SET HangXe = @HangXe, TT = @TT, SoGhe = @SoGhe, NSX = @NSX, BienSo = @BienSo, MaBH = @MaBH
WHERE MaXB = @MaXB";
            try
            {
                using var cn = new SqlConnection(ConnectionString);
                using var cmd = new SqlCommand(sql, cn);
                cmd.Parameters.AddWithValue("@MaXB", txtMaXB.Text.Trim());
                cmd.Parameters.AddWithValue("@HangXe", txtHangxe.Text.Trim());
                cmd.Parameters.AddWithValue("@TT", cbbTT.SelectedItem?.ToString().Trim() ?? "");

                if (int.TryParse(txtSoghe.Text.Trim(), out var soghe))
                    cmd.Parameters.AddWithValue("@SoGhe", soghe);
                else
                    cmd.Parameters.AddWithValue("@SoGhe", DBNull.Value);

                if (int.TryParse(txtNSX.Text.Trim(), out var nsx))
                    cmd.Parameters.AddWithValue("@NSX", nsx);
                else
                    cmd.Parameters.AddWithValue("@NSX", DBNull.Value);

                cmd.Parameters.AddWithValue("@BienSo", txtBienso.Text.Trim());
                cmd.Parameters.AddWithValue("@MaBH", txtMaBH.Text.Trim());

                cn.Open();
                var affected = cmd.ExecuteNonQuery();
                if (affected == 0)
                {
                    MessageBox.Show("Không tìm thấy xe để cập nhật.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi cập nhật xe.\n{ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private bool DeleteVehicleFromDb(string maXB)
        {
            const string sql = "DELETE FROM [dbo].[Xe] WHERE MaXB = @MaXB";
            try
            {
                using var cn = new SqlConnection(ConnectionString);
                using var cmd = new SqlCommand(sql, cn);
                cmd.Parameters.AddWithValue("@MaXB", maXB);
                cn.Open();
                var affected = cmd.ExecuteNonQuery();
                if (affected == 0)
                {
                    MessageBox.Show("Không tìm thấy xe để xóa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi xóa xe.\n{ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        #endregion

        private void btnReset_Click(object sender, EventArgs e)
        {
            // Clear the input controls so any search filters are removed
            ClearInputs();

            // Reload the full vehicle list from database into the DataGridView
            LoadVehiclesIntoGrid();

            // Ensure no row is selected after reload
            dataGridView1.ClearSelection();
        }

        private void btnTimkiem_Click(object sender, EventArgs e)
        {
            var ma = txtMaXB.Text.Trim();
            var hang = txtHangxe.Text.Trim();
            var ttText = cbbTT.SelectedIndex > 0 ? cbbTT.SelectedItem?.ToString().Trim() ?? "" : "";
            var sogheText = txtSoghe.Text.Trim();
            var nsxText = txtNSX.Text.Trim();
            var bienso = txtBienso.Text.Trim();
            var mabh = txtMaBH.Text.Trim();

            // Require at least one non-empty search field (per requirement)
            var anyCriteria = !string.IsNullOrWhiteSpace(ma)
                              || !string.IsNullOrWhiteSpace(hang)
                              || !string.IsNullOrWhiteSpace(ttText)
                              || !string.IsNullOrWhiteSpace(sogheText)
                              || !string.IsNullOrWhiteSpace(nsxText)
                              || !string.IsNullOrWhiteSpace(bienso)
                              || !string.IsNullOrWhiteSpace(mabh);

            if (!anyCriteria)
            {
                MessageBox.Show("Vui lòng nhập ít nhất một tiêu chí tìm kiếm (Mã, Hãng, Tình trạng, Số ghế, Năm SX, Biển số hoặc Mã BH).", "Tìm kiếm", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Parse numeric criteria if provided
            int? soghe = null;
            if (!string.IsNullOrWhiteSpace(sogheText))
            {
                if (int.TryParse(sogheText, out var parsedSoghe)) soghe = parsedSoghe;
                else
                {
                    MessageBox.Show("Số ghế phải là số.", "Tìm kiếm", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            int? nsx = null;
            if (!string.IsNullOrWhiteSpace(nsxText))
            {
                if (int.TryParse(nsxText, out var parsedNsx)) nsx = parsedNsx;
                else
                {
                    MessageBox.Show("Năm sản xuất phải là số.", "Tìm kiếm", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            var sql = "SELECT MaXB, HangXe, TT, SoGhe, NSX, BienSo, MaBH FROM [dbo].[Xe] WHERE 1=1";
            using var cn = new SqlConnection(ConnectionString);
            using var cmd = new SqlCommand();
            cmd.Connection = cn;

            if (!string.IsNullOrWhiteSpace(ma))
            {
                sql += " AND MaXB LIKE @MaXB";
                cmd.Parameters.AddWithValue("@MaXB", "%" + ma + "%");
            }

            if (!string.IsNullOrWhiteSpace(hang))
            {
                sql += " AND HangXe LIKE @HangXe";
                cmd.Parameters.AddWithValue("@HangXe", "%" + hang + "%");
            }

            if (!string.IsNullOrWhiteSpace(ttText))
            {
                sql += " AND TT = @TT";
                cmd.Parameters.AddWithValue("@TT", ttText);
            }

            if (soghe.HasValue)
            {
                sql += " AND SoGhe = @SoGhe";
                cmd.Parameters.AddWithValue("@SoGhe", soghe.Value);
            }

            if (nsx.HasValue)
            {
                sql += " AND NSX = @NSX";
                cmd.Parameters.AddWithValue("@NSX", nsx.Value);
            }

            if (!string.IsNullOrWhiteSpace(bienso))
            {
                sql += " AND BienSo LIKE @BienSo";
                cmd.Parameters.AddWithValue("@BienSo", "%" + bienso + "%");
            }

            if (!string.IsNullOrWhiteSpace(mabh))
            {
                sql += " AND MaBH LIKE @MaBH";
                cmd.Parameters.AddWithValue("@MaBH", "%" + mabh + "%");
            }

            cmd.CommandText = sql + " ORDER BY MaXB";

            try
            {
                var dt = new DataTable();
                using var da = new SqlDataAdapter(cmd);
                da.Fill(dt);

                ConfigureDataGridView(dt);
                dataGridView1.ClearSelection();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tìm kiếm.\n{ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}