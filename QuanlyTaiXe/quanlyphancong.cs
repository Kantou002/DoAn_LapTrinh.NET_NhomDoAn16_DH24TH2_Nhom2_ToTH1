using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using MENUQL;


namespace QuanlyTaiXe
{
    public partial class quanlyphancong : BaseForm
    {
        // Keep same connection string as other forms
        private const string ConnectionString = "Server=LAPTOP-NABV6PHG\\KANTOU;Database=Quanlyxevalaixe;Trusted_Connection=True;Encrypt=True;TrustServerCertificate=True;";

        // Centralize known vehicle status used by the code (adjust to match DB allowed values)
        private const string VehicleStatusUnavailable = "Unavailable";
        private const string VehicleStatusOnTheTrip = "onthetrip";

        // Track editing state: when non-null we are editing the given MaPC
        private string? _editingMaPC;
        // Keep the original MaXB for the row being edited to detect vehicle changes
        private string? _originalMaXB;

        // Timer used to check scheduled assignments and update vehicle TT to "onthetrip"
        private readonly System.Windows.Forms.Timer _statusTimer;
        // How tolerant we are when comparing scheduled time to "now"
        private readonly TimeSpan _statusCheckTolerance = TimeSpan.FromMinutes(1);

        public quanlyphancong()
        {
            InitializeComponent();
            // wire button handlers
            this.Icon = System.Drawing.Icon.FromHandle(Quanlyxevalaixe.Properties.Resources.icons8_driver_48.GetHicon());
            btnThem.Click += BtnThem_Click;
            btnXoa.Click += BtnKetthuc_Click;

            // wire new button handlers (similar behaviour to quanlytaixe)
            if (btnLuu != null) btnLuu.Click += BtnLuu_Click;
            if (btnHuy != null) btnHuy.Click += BtnHuy_Click;
            if (btnTimkiem != null) btnTimkiem.Click += BtnTimkiem_Click;
            if (btnReset != null) btnReset.Click += BtnReset_Click;

            // configure grid behaviour before loading data
            ConfigureGridForSelection();

            // also respond to selection changes (load row into inputs or clear)
            if (dataGridView1 != null)
            {
                dataGridView1.SelectionChanged -= DataGridView1_SelectionChanged;
                dataGridView1.SelectionChanged += DataGridView1_SelectionChanged;
            }

            // Handle clicks on the form background (not on child controls)
            this.MouseClick -= FormBackground_MouseClick;
            this.MouseClick += FormBackground_MouseClick;

            PopulateDriverCombo();
            PopulateVehicleCombo();
            LoadAssignmentsIntoGrid();

            // Set default time for timekhoihanh to current system time
            if (timekhoihanh != null)
            {
                try
                {
                    timekhoihanh.Value = DateTime.Now;
                }
                catch { /* ignore designer/timepicker edge cases */ }
            }

            // Configure and start background timer to check assignment times
            _statusTimer = new System.Windows.Forms.Timer();
            _statusTimer.Interval = 60_000; // check every 60 seconds
            _statusTimer.Tick += StatusTimer_Tick;
            _statusTimer.Start();

            // Do an immediate check on startup to catch assignments due now
            _ = Task.Run(() => UpdateToOnTheTripIfDue());

            // Ensure the exit button closes the modal on a single click
            if (btnThoat != null)
            {
                btnThoat.DialogResult = DialogResult.Cancel;
                this.CancelButton = btnThoat;
            }
        }

        // Timer tick — run update in background to avoid blocking UI
        private async void StatusTimer_Tick(object? sender, EventArgs e)
        {
            _statusTimer.Stop();
            try
            {
                await Task.Run(() => UpdateToOnTheTripIfDue());
            }
            finally
            {
                if (!IsDisposed)
                {
                    _statusTimer.Start();
                }
            }
        }

        /// <summary>
        /// Find assignments whose scheduled datetime is the same or before "now" (with tolerance)
        /// and set both:
        ///   - quanlyphancong.Trangthai = "onthetrip" (for matching MaPC)
        ///   - dbo.Xe.TT = "onthetrip" (for vehicles with matching MaXB)
        ///
        /// Matches when:
        ///   CAST(Ngayphancong AS date) &lt; today
        ///   OR (CAST(Ngayphancong AS date) = today AND Giokhoihanh <= now + tolerance)
        /// Only rows where Trangthai is not already "onthetrip" are selected.
        /// </summary>
        private void UpdateToOnTheTripIfDue()
        {
            try
            {
                var now = DateTime.Now;
                var today = now.Date;
                var nowPlusTol = now.TimeOfDay + _statusCheckTolerance;
                if (nowPlusTol > TimeSpan.FromDays(1).Subtract(TimeSpan.FromTicks(1)))
                    nowPlusTol = TimeSpan.FromDays(1).Subtract(TimeSpan.FromTicks(1));

                var due = new DataTable();

                using (var cn = new SqlConnection(ConnectionString))
                using (var cmd = new SqlCommand(@"
SELECT MaPC, MaXB
FROM dbo.quanlyphancong
WHERE ISNULL(Trangthai,'') <> @Status
  AND (
        CAST(Ngayphancong AS date) < @Today
     OR (CAST(Ngayphancong AS date) = @Today AND Giokhoihanh <= @NowTime)
  )", cn))
                {
                    cmd.Parameters.AddWithValue("@Status", VehicleStatusOnTheTrip);
                    cmd.Parameters.AddWithValue("@Today", today);
                    cmd.Parameters.AddWithValue("@NowTime", nowPlusTol);

                    using var da = new SqlDataAdapter(cmd);
                    da.Fill(due);
                }

                if (due.Rows.Count == 0) return;

                var anyUpdated = false;

                using (var cn = new SqlConnection(ConnectionString))
                {
                    cn.Open();

                    foreach (DataRow r in due.Rows)
                    {
                        var maPC = (r["MaPC"] as string) ?? "";
                        var maXB = (r["MaXB"] as string) ?? "";
                        if (string.IsNullOrWhiteSpace(maPC)) continue;

                        using var tran = cn.BeginTransaction();
                        try
                        {
                            // update assignment's Trangthai
                            using var updAssign = new SqlCommand("UPDATE dbo.quanlyphancong SET Trangthai = @Status WHERE MaPC = @MaPC", cn, tran);
                            updAssign.Parameters.AddWithValue("@Status", VehicleStatusOnTheTrip);
                            updAssign.Parameters.AddWithValue("@MaPC", maPC);
                            var a1 = updAssign.ExecuteNonQuery();

                            // update vehicle status if MaXB present
                            var a2 = 0;
                            if (!string.IsNullOrWhiteSpace(maXB))
                            {
                                using var updXe = new SqlCommand("UPDATE dbo.Xe SET TT = @Status WHERE MaXB = @MaXB", cn, tran);
                                updXe.Parameters.AddWithValue("@Status", VehicleStatusOnTheTrip);
                                updXe.Parameters.AddWithValue("@MaXB", maXB);
                                try
                                {
                                    a2 = updXe.ExecuteNonQuery();
                                }
                                catch (SqlException sqlEx)
                                {
                                    // if DB enforces a CHECK constraint that does not allow this value,
                                    // surface a message once on UI thread and continue.
                                    if (sqlEx.Message.Contains("CK_Xe_TT_Check", StringComparison.OrdinalIgnoreCase) && !IsDisposed)
                                    {
                                        BeginInvoke(new Action(() =>
                                        {
                                            MessageBox.Show("Không thể đặt TT = \"onthetrip\" vì ràng buộc cơ sở dữ liệu. Thêm giá trị này vào danh sách TT hợp lệ hoặc chọn giá trị khác.", "Lỗi cập nhật trạng thái xe", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                        }));
                                    }
                                }
                            }

                            tran.Commit();

                            if (a1 > 0 || a2 > 0) anyUpdated = true;
                        }
                        catch
                        {
                            try { tran.Rollback(); } catch { /* ignore */ }
                        }
                    }
                }

                if (anyUpdated && !IsDisposed)
                {
                    BeginInvoke(new Action(() =>
                    {
                        LoadAssignmentsIntoGrid();
                        PopulateVehicleCombo();
                    }));
                }
            }
            catch
            {
                // swallow errors in background check to keep timer running
            }
        }

        // configure DataGridView so user can only choose existing rows (no editing, single-row select)
        private void ConfigureGridForSelection()
        {
            if (dataGridView1 == null) return;

            dataGridView1.SuspendLayout();

            dataGridView1.ReadOnly = true;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.AllowUserToResizeRows = false;
            dataGridView1.RowHeadersVisible = false;

            // user selects whole row; only one row at a time
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.MultiSelect = false;

            // prevent any in-place editing
            dataGridView1.EditMode = DataGridViewEditMode.EditProgrammatically;

            // make column auto sizing at bind time (LoadAssignmentsIntoGrid sets DataSource)
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // Ensure clicks select the row reliably
            dataGridView1.CellClick -= DataGridView1_CellClick;
            dataGridView1.CellClick += DataGridView1_CellClick;

            dataGridView1.ResumeLayout();
        }

        // ensure any cell click selects the entire row (defensive)
        private void DataGridView1_CellClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView1 == null) return;
            if (e.RowIndex < 0 || e.RowIndex >= dataGridView1.Rows.Count) return;

            dataGridView1.ClearSelection();
            dataGridView1.Rows[e.RowIndex].Selected = true;
            dataGridView1.CurrentCell = dataGridView1.Rows[e.RowIndex].Cells[0];
        }

        // Handle clicks on the form background (not on child controls)
        private void FormBackground_MouseClick(object? sender, MouseEventArgs e)
        {
            // Only treat as "background click" when no child control exists at the click location.
            if (GetChildAtPoint(e.Location) == null)
            {
                ClearInputs();
                dataGridView1.ClearSelection();
            }
        }

        // existing PopulateDriverCombo from earlier work (keeps driver combo functionality)
        private void PopulateDriverCombo()
        {
            if (cbbTaixe == null) return;

            try
            {
                var dt = new DataTable();
                using var cn = new SqlConnection(ConnectionString);
                using var cmd = new SqlCommand("SELECT MaTX, Holot, Ten FROM dbo.TaiXe ORDER BY MaTX", cn);
                using var da = new SqlDataAdapter(cmd);
                da.Fill(dt);

                if (dt.Rows.Count == 0)
                {
                    cbbTaixe.DataSource = null;
                    cbbTaixe.Items.Clear();
                    cbbTaixe.Items.Add("Không có tài xế sẵn sàng");
                    cbbTaixe.SelectedIndex = 0;
                    cbbTaixe.Enabled = false;
                    return;
                }

                var source = new DataTable();
                source.Columns.Add("MaTX", typeof(string));
                source.Columns.Add("Display", typeof(string));

                foreach (DataRow row in dt.Rows)
                {
                    var ma = (row["MaTX"] as string) ?? "";
                    var holot = (row["Holot"] as string) ?? "";
                    var ten = (row["Ten"] as string) ?? "";
                    var display = $"{ma}-{holot}{ten}";
                    source.Rows.Add(ma, display);
                }

                cbbTaixe.DataSource = source;
                cbbTaixe.DisplayMember = "Display";
                cbbTaixe.ValueMember = "MaTX";
                cbbTaixe.Enabled = true;
                cbbTaixe.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                cbbTaixe.DataSource = null;
                cbbTaixe.Items.Clear();
                cbbTaixe.Items.Add("Không thể tải danh sách tài xế");
                cbbTaixe.SelectedIndex = 0;
                cbbTaixe.Enabled = false;
                MessageBox.Show($"Lỗi khi tải tài xế:\n{ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Populate `cbbXe` with vehicles that have an "available" status.
        /// Items appear as "MaXB-BienSo".
        /// If no available vehicles exist, show "Không có xe sẵn sàng" and disable the combo.
        /// The query treats several common variants of "available". (Vietnamese + English)
        /// Adjust the IN-list if your TT values differ.
        /// </summary>
        private void PopulateVehicleCombo()
        {
            if (cbbXe == null) return;

            try
            {
                var dt = new DataTable();
                using var cn = new SqlConnection(ConnectionString);

                // Filter TT values that commonly mean "available". Adjust this list to match your data.
                var sql = @"
SELECT MaXB, BienSo
FROM dbo.Xe
WHERE LOWER(ISNULL(TT, '')) IN (
    N'sẵn sàng', N'san sang', N'có sẵn', N'co san', N'available'
)
ORDER BY MaXB";

                using var cmd = new SqlCommand(sql, cn);
                using var da = new SqlDataAdapter(cmd);
                da.Fill(dt);

                if (dt.Rows.Count == 0)
                {
                    cbbXe.DataSource = null;
                    cbbXe.Items.Clear();
                    cbbXe.Items.Add("Không có xe sẵn sàng");
                    cbbXe.SelectedIndex = 0;
                    cbbXe.Enabled = false;
                    return;
                }

                var source = new DataTable();
                source.Columns.Add("MaXB", typeof(string));
                source.Columns.Add("Display", typeof(string));

                foreach (DataRow row in dt.Rows)
                {
                    var ma = (row["MaXB"] as string) ?? "";
                    var bienSo = (row["BienSo"] as string) ?? "";
                    var display = $"{ma}-{bienSo}";
                    source.Rows.Add(ma, display);
                }

                cbbXe.DataSource = source;
                cbbXe.DisplayMember = "Display";
                cbbXe.ValueMember = "MaXB";
                cbbXe.Enabled = true;
                cbbXe.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                cbbXe.DataSource = null;
                cbbXe.Items.Clear();
                cbbXe.Items.Add("Không thể tải danh sách xe");
                cbbXe.SelectedIndex = 0;
                cbbXe.Enabled = false;
                MessageBox.Show($"Lỗi khi tải xe:\n{ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Loads all assignment records from dbo.quanlyphancong and displays them in dataGridView1.
        /// </summary>
        private void LoadAssignmentsIntoGrid()
        {
            if (dataGridView1 == null) return;

            try
            {
                var dt = new DataTable();
                using var cn = new SqlConnection(ConnectionString);
                using var cmd = new SqlCommand(@"
SELECT MaPC, MaTX, MaXB, Ngayphancong, Giokhoihanh, Trangthai, Diadiemden
FROM dbo.quanlyphancong
ORDER BY Ngayphancong DESC, Giokhoihanh DESC", cn);
                using var da = new SqlDataAdapter(cmd);
                da.Fill(dt);

                dataGridView1.DataSource = dt;

                // formatting (null-safe)
                if (dataGridView1.Columns.Contains("Ngayphancong"))
                {
                    var col = dataGridView1.Columns["Ngayphancong"];
                    if (col?.DefaultCellStyle != null) col.DefaultCellStyle.Format = "yyyy-MM-dd";
                }
                if (dataGridView1.Columns.Contains("Giokhoihanh"))
                {
                    var col = dataGridView1.Columns["Giokhoihanh"];
                    if (col?.DefaultCellStyle != null) col.DefaultCellStyle.Format = @"hh\:mm";
                }

                dataGridView1.ClearSelection();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải danh sách phân công.\n{ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Click handler for the "Phân công mới" button.
        /// Generates a unique MaPC ("PC0{n}"), validates inputs, inserts a new row into dbo.quanlyphancong,
        /// refreshes the grid and the vehicle combo (so assigned vehicle can be removed from available list).
        /// </summary>
        private void BtnThem_Click(object? sender, EventArgs e)
        {
            // Basic validation
            var maTX = GetSelectedComboValue(cbbTaixe);
            var maXB = GetSelectedComboValue(cbbXe);
            if (string.IsNullOrWhiteSpace(maTX))
            {
                MessageBox.Show("Vui lòng chọn tài xế.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(maXB))
            {
                MessageBox.Show("Vui lòng chọn xe.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var ngay = datengayphancong.Value.Date;
            var gio = timekhoihanh.Value.TimeOfDay; // maps to SQL TIME
            var diadiem = txtDiadiem?.Text.Trim() ?? "";

            // generate next MaPC
            var maPC = GenerateNextMaPC();

            const string insertSql = @"
INSERT INTO dbo.quanlyphancong (MaPC, MaTX, MaXB, Ngayphancong, Giokhoihanh, Trangthai, Diadiemden)
VALUES (@MaPC, @MaTX, @MaXB, @Ngayphancong, @Giokhoihanh, @Trangthai, @Diadiemden)";

            const string updateVehicleSql = @"
UPDATE dbo.Xe
SET TT = @TT
WHERE MaXB = @MaXB";

            try
            {
                using var cn = new SqlConnection(ConnectionString);
                cn.Open();

                using var tran = cn.BeginTransaction();

                try
                {
                    // Insert assignment
                    using var insertCmd = new SqlCommand(insertSql, cn, tran);
                    insertCmd.Parameters.AddWithValue("@MaPC", maPC);
                    insertCmd.Parameters.AddWithValue("@MaTX", maTX);
                    insertCmd.Parameters.AddWithValue("@MaXB", maXB);
                    insertCmd.Parameters.AddWithValue("@Ngayphancong", ngay);
                    insertCmd.Parameters.AddWithValue("@Giokhoihanh", gio);
                    insertCmd.Parameters.AddWithValue("@Trangthai", "Đang chờ");
                    insertCmd.Parameters.AddWithValue("@Diadiemden", string.IsNullOrEmpty(diadiem) ? (object)DBNull.Value : diadiem);
                    insertCmd.ExecuteNonQuery();

                    // Mark vehicle as unavailable (update TT).
                    using var updCmd = new SqlCommand(updateVehicleSql, cn, tran);

                    // Use a centralized value that must match the DB CHECK constraint.
                    // Update this constant to one of the values allowed by CK_Xe_TT_Check.
                    updCmd.Parameters.AddWithValue("@TT", VehicleStatusUnavailable);
                    updCmd.Parameters.AddWithValue("@MaXB", maXB);
                    var affected = updCmd.ExecuteNonQuery();

                    if (affected == 0)
                    {
                        MessageBox.Show($"Cảnh báo: không tìm thấy xe với MaXB = {maXB} để cập nhật trạng thái.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }

                    tran.Commit();
                }
                catch (SqlException sqlEx)
                {
                    tran.Rollback();

                    // Detect CHECK constraint violation and show a clearer message
                    if (sqlEx.Message.Contains("CK_Xe_TT_Check", StringComparison.OrdinalIgnoreCase))
                    {
                        MessageBox.Show("Không thể cập nhật trạng thái xe: giá trị trạng thái không hợp lệ theo ràng buộc cơ sở dữ liệu. Kiểm tra các giá trị TT hợp lệ trong bảng `Xe` và đồng bộ với mã nguồn.", "Lỗi cập nhật trạng thái", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // rethrow other SQL exceptions
                    throw;
                }
                catch
                {
                    tran.Rollback();
                    throw;
                }
            }
            catch (SqlException ex) when (ex.Number == 2627 || ex.Number == 2601)
            {
                MessageBox.Show("Mã phân công đã tồn tại. Vui lòng thử lại.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi thêm phân công hoặc cập nhật trạng thái xe.\n{ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            MessageBox.Show($"Thêm phân công thành công (MaPC = {maPC}).", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);

            // Refresh UI
            LoadAssignmentsIntoGrid();
            PopulateVehicleCombo(); // vehicle may no longer be available
            // Optionally clear inputs after insert
            cbbTaixe.SelectedIndex = -1;
            cbbXe.SelectedIndex = -1;
            if (txtDiadiem != null)
            {
                txtDiadiem.Text = "";
            }
        }

        /// <summary>
        /// Click handler for the "Kết thúc chuyến" button.
        /// Deletes the selected assignment (MaPC) and sets vehicle(s) with the same BienSo to "available".
        /// Shows "The trip is done" on success and refreshes UI.
        /// </summary>
        private void BtnKetthuc_Click(object? sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow?.DataBoundItem is not DataRowView drv)
            {
                MessageBox.Show("Vui lòng chọn chuyến để kết thúc.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var row = drv.Row;
            var maPC = row.Table.Columns.Contains("MaPC") ? row.Field<string>("MaPC") ?? "" : row["MaPC"]?.ToString() ?? "";
            var maXB = row.Table.Columns.Contains("MaXB") ? row.Field<string>("MaXB") ?? "" : row["MaXB"]?.ToString() ?? "";

            if (string.IsNullOrWhiteSpace(maPC))
            {
                MessageBox.Show("Không thể xác định MaPC cho chuyến đã chọn.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                using var cn = new SqlConnection(ConnectionString);
                cn.Open();
                using var tran = cn.BeginTransaction();

                try
                {
                    // retrieve BienSo for the selected MaXB (if any)
                    string? bienSo = null;
                    if (!string.IsNullOrWhiteSpace(maXB))
                    {
                        using var cmdGet = new SqlCommand("SELECT BienSo FROM dbo.Xe WHERE MaXB = @MaXB", cn, tran);
                        cmdGet.Parameters.AddWithValue("@MaXB", maXB);
                        var obj = cmdGet.ExecuteScalar();
                        bienSo = obj?.ToString();
                    }

                    // delete assignment
                    using var delCmd = new SqlCommand("DELETE FROM dbo.quanlyphancong WHERE MaPC = @MaPC", cn, tran);
                    delCmd.Parameters.AddWithValue("@MaPC", maPC);
                    var delAffected = delCmd.ExecuteNonQuery();

                    if (delAffected == 0)
                    {
                        MessageBox.Show("Không tìm thấy chuyến để xóa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        tran.Rollback();
                        return;
                    }

                    // update vehicle(s) TT => "available" by BienSo if found, otherwise by MaXB
                    int updAffected = 0;
                    if (!string.IsNullOrWhiteSpace(bienSo))
                    {
                        using var updByBienSo = new SqlCommand("UPDATE dbo.Xe SET TT = @TT WHERE BienSo = @BienSo", cn, tran);
                        updByBienSo.Parameters.AddWithValue("@TT", "available");
                        updByBienSo.Parameters.AddWithValue("@BienSo", bienSo);
                        updAffected = updByBienSo.ExecuteNonQuery();
                    }
                    else if (!string.IsNullOrWhiteSpace(maXB))
                    {
                        using var updByMaXB = new SqlCommand("UPDATE dbo.Xe SET TT = @TT WHERE MaXB = @MaXB", cn, tran);
                        updByMaXB.Parameters.AddWithValue("@TT", "available");
                        updByMaXB.Parameters.AddWithValue("@MaXB", maXB);
                        updAffected = updByMaXB.ExecuteNonQuery();
                    }

                    tran.Commit();

                    // Announce and refresh UI
                    MessageBox.Show("The trip is done", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadAssignmentsIntoGrid();
                    PopulateVehicleCombo();
                }
                catch
                {
                    tran.Rollback();
                    throw;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi kết thúc chuyến hoặc cập nhật trạng thái xe.\n{ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // helper to return selected value (string) from a combo box which uses ValueMember,
        // or empty string when no valid selection.
        private static string GetSelectedComboValue(ComboBox combo)
        {
            if (combo == null || !combo.Enabled) return "";
            if (combo.SelectedValue != null)
                return combo.SelectedValue.ToString() ?? "";
            if (combo.SelectedItem is DataRowView drv)
            {
                // try common column names
                if (drv.Row.Table.Columns.Contains("MaTX")) return drv.Row.Field<string>("MaTX") ?? "";
                if (drv.Row.Table.Columns.Contains("MaXB")) return drv.Row.Field<string>("MaXB") ?? "";
            }
            if (combo.SelectedItem is string s) return s;
            return "";
        }

        // Generate next MaPC in the pattern "PC0{n}" where n increments from existing values.
        private string GenerateNextMaPC()
        {
            try
            {
                using var cn = new SqlConnection(ConnectionString);
                using var cmd = new SqlCommand("SELECT MaPC FROM dbo.quanlyphancong WHERE MaPC LIKE 'PC0%'", cn);
                using var da = new SqlDataAdapter(cmd);
                var dt = new DataTable();
                da.Fill(dt);

                var max = 0;
                foreach (DataRow r in dt.Rows)
                {
                    var m = r["MaPC"]?.ToString() ?? "";
                    if (m.StartsWith("PC0", StringComparison.OrdinalIgnoreCase))
                    {
                        var suffix = m.Substring(3); // after "PC0"
                        if (int.TryParse(suffix, out var v))
                        {
                            if (v > max) max = v;
                        }
                    }
                }

                var next = max + 1;
                return $"PC0{next}";
            }
            catch
            {
                // on error fallback to timestamp-based id to avoid blocking user
                return $"PC0{DateTime.UtcNow:yyyyMMddHHmmss}";
            }
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            // Close this form to return control to the Menu (owner)
            Close();
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow?.DataBoundItem is not DataRowView drv)
            {
                MessageBox.Show("Vui lòng chọn phân công để sửa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // record editing state so BtnLuu knows we're updating an existing MaPC
            var row = drv.Row;
            _editingMaPC = row.Table.Columns.Contains("MaPC") ? row.Field<string>("MaPC") ?? "" : row["MaPC"]?.ToString() ?? "";
            _originalMaXB = row.Table.Columns.Contains("MaXB") ? row.Field<string>("MaXB") ?? "" : row["MaXB"]?.ToString() ?? "";

            LoadDataRowToInputs(drv.Row);
            SetInputsEnabled(true);
        }

        private void LoadDataRowToInputs(DataRow row)
        {
            cbbTaixe.SelectedValue = row.Field<string>("MaTX") ?? "";
            cbbXe.SelectedValue = row.Field<string>("MaXB") ?? "";
            datengayphancong.Value = row.Field<DateTime?>("Ngayphancong") ?? DateTime.Now;
            timekhoihanh.Value = DateTime.Today.Add(row.Field<TimeSpan?>("Giokhoihanh") ?? TimeSpan.Zero);
            txtDiadiem.Text = row.Field<string>("Diadiemden") ?? "";
        }
        private void SetInputsEnabled(bool enabled)
        {
            cbbTaixe.Enabled = enabled;
            cbbXe.Enabled = enabled;
            datengayphancong.Enabled = enabled;
            timekhoihanh.Enabled = enabled;
            txtDiadiem.Enabled = enabled;
        }

        #region New button implementations (Lưu, Hủy, Tìm kiếm, Reset)

        private void BtnLuu_Click(object? sender, EventArgs e)
        {
            // Validate inputs
            var maTX = GetSelectedComboValue(cbbTaixe);
            var maXB = GetSelectedComboValue(cbbXe);
            if (string.IsNullOrWhiteSpace(maTX))
            {
                MessageBox.Show("Vui lòng chọn tài xế.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(maXB))
            {
                MessageBox.Show("Vui lòng chọn xe.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var ngay = datengayphancong.Value.Date;
            var gio = timekhoihanh.Value.TimeOfDay;
            var diadiem = txtDiadiem?.Text.Trim() ?? "";

            // If _editingMaPC is null or empty => insert; otherwise update existing
            if (string.IsNullOrWhiteSpace(_editingMaPC))
            {
                // Insert path (reuse the logic from BtnThem_Click)
                try
                {
                    var maPC = GenerateNextMaPC();
                    const string insertSql = @"
INSERT INTO dbo.quanlyphancong (MaPC, MaTX, MaXB, Ngayphancong, Giokhoihanh, Trangthai, Diadiemden)
VALUES (@MaPC, @MaTX, @MaXB, @Ngayphancong, @Giokhoihanh, @Trangthai, @Diadiemden)";

                    const string updateVehicleSql = @"
UPDATE dbo.Xe
SET TT = @TT
WHERE MaXB = @MaXB";

                    using var cn = new SqlConnection(ConnectionString);
                    cn.Open();
                    using var tran = cn.BeginTransaction();
                    try
                    {
                        using var insertCmd = new SqlCommand(insertSql, cn, tran);
                        insertCmd.Parameters.AddWithValue("@MaPC", maPC);
                        insertCmd.Parameters.AddWithValue("@MaTX", maTX);
                        insertCmd.Parameters.AddWithValue("@MaXB", maXB);
                        insertCmd.Parameters.AddWithValue("@Ngayphancong", ngay);
                        insertCmd.Parameters.AddWithValue("@Giokhoihanh", gio);
                        insertCmd.Parameters.AddWithValue("@Trangthai", "Đang chờ");
                        insertCmd.Parameters.AddWithValue("@Diadiemden", string.IsNullOrEmpty(diadiem) ? (object)DBNull.Value : diadiem);
                        insertCmd.ExecuteNonQuery();

                        using var updCmd = new SqlCommand(updateVehicleSql, cn, tran);
                        updCmd.Parameters.AddWithValue("@TT", VehicleStatusUnavailable);
                        updCmd.Parameters.AddWithValue("@MaXB", maXB);
                        updCmd.ExecuteNonQuery();

                        tran.Commit();
                    }
                    catch
                    {
                        tran.Rollback();
                        throw;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi lưu phân công mới.\n{ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                MessageBox.Show("Thêm phân công thành công.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                // Update path
                try
                {
                    using var cn = new SqlConnection(ConnectionString);
                    cn.Open();
                    using var tran = cn.BeginTransaction();
                    try
                    {
                        // Update assignment record
                        const string updateAssignSql = @"
UPDATE dbo.quanlyphancong
SET MaTX = @MaTX, MaXB = @MaXB, Ngayphancong = @Ngayphancong, Giokhoihanh = @Giokhoihanh, Diadiemden = @Diademden
WHERE MaPC = @MaPC";
                        using var updAssign = new SqlCommand(updateAssignSql, cn, tran);
                        updAssign.Parameters.AddWithValue("@MaTX", maTX);
                        updAssign.Parameters.AddWithValue("@MaXB", maXB);
                        updAssign.Parameters.AddWithValue("@Ngayphancong", ngay);
                        updAssign.Parameters.AddWithValue("@Giokhoihanh", gio);
                        updAssign.Parameters.AddWithValue("@Diadiemden", string.IsNullOrEmpty(diadiem) ? (object)DBNull.Value : diadiem);
                        updAssign.Parameters.AddWithValue("@MaPC", _editingMaPC);
                        var affectedAssign = updAssign.ExecuteNonQuery();
                        if (affectedAssign == 0)
                        {
                            MessageBox.Show("Không tìm thấy phân công để cập nhật.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            tran.Rollback();
                            return;
                        }

                        // If MaXB changed, mark new MaXB unavailable and original MaXB available (by BienSo if possible)
                        if (!string.Equals(_originalMaXB ?? "", maXB, StringComparison.OrdinalIgnoreCase))
                        {
                            // mark new MaXB unavailable
                            using var updNew = new SqlCommand("UPDATE dbo.Xe SET TT = @TT WHERE MaXB = @MaXB", cn, tran);
                            updNew.Parameters.AddWithValue("@TT", VehicleStatusUnavailable);
                            updNew.Parameters.AddWithValue("@MaXB", maXB);
                            updNew.ExecuteNonQuery();

                            // try to set original vehicle(s) to available by BienSo first
                            if (!string.IsNullOrWhiteSpace(_originalMaXB))
                            {
                                string? origBienSo = null;
                                using (var cmdGet = new SqlCommand("SELECT BienSo FROM dbo.Xe WHERE MaXB = @MaXB", cn, tran))
                                {
                                    cmdGet.Parameters.AddWithValue("@MaXB", _originalMaXB);
                                    var obj = cmdGet.ExecuteScalar();
                                    origBienSo = obj?.ToString();
                                }

                                if (!string.IsNullOrWhiteSpace(origBienSo))
                                {
                                    using var updByBienSo = new SqlCommand("UPDATE dbo.Xe SET TT = @TT WHERE BienSo = @BienSo", cn, tran);
                                    updByBienSo.Parameters.AddWithValue("@TT", "available");
                                    updByBienSo.Parameters.AddWithValue("@BienSo", origBienSo);
                                    updByBienSo.ExecuteNonQuery();
                                }
                                else
                                {
                                    using var updByMaXB = new SqlCommand("UPDATE dbo.Xe SET TT = @TT WHERE MaXB = @MaXB", cn, tran);
                                    updByMaXB.Parameters.AddWithValue("@TT", "available");
                                    updByMaXB.Parameters.AddWithValue("@MaXB", _originalMaXB);
                                    updByMaXB.ExecuteNonQuery();
                                }
                            }
                        }

                        tran.Commit();
                    }
                    catch
                    {
                        tran.Rollback();
                        throw;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi cập nhật phân công.\n{ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                MessageBox.Show("Cập nhật phân công thành công.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            // Reset editing state and refresh UI
            _editingMaPC = null;
            _originalMaXB = null;
            LoadAssignmentsIntoGrid();
            PopulateVehicleCombo();
            ClearInputs();
        }

        private void BtnHuy_Click(object? sender, EventArgs e)
        {
            // Cancel current edit/insert operation
            _editingMaPC = null;
            _originalMaXB = null;
            ClearInputs();
            SetInputsEnabled(true);
            dataGridView1.ClearSelection();
        }

        private void BtnTimkiem_Click(object? sender, EventArgs e)
        {
            // Build search based on selected driver/vehicle and/or destination.
            var maTX = GetSelectedComboValue(cbbTaixe);
            var maXB = GetSelectedComboValue(cbbXe);
            var diadiem = txtDiadiem?.Text.Trim() ?? "";

            if (string.IsNullOrWhiteSpace(maTX) && string.IsNullOrWhiteSpace(maXB) && string.IsNullOrWhiteSpace(diadiem))
            {
                MessageBox.Show("Vui lòng nhập ít nhất một tiêu chí tìm kiếm (tài xế, xe hoặc địa điểm).", "Tìm kiếm", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var sql = @"SELECT MaPC, MaTX, MaXB, Ngayphancong, Giokhoihanh, Trangthai, Diadiemden FROM dbo.quanlyphancong WHERE 1=1";
            using var cn = new SqlConnection(ConnectionString);
            using var cmd = new SqlCommand();
            cmd.Connection = cn;

            if (!string.IsNullOrWhiteSpace(maTX))
            {
                sql += " AND MaTX LIKE @MaTX";
                cmd.Parameters.AddWithValue("@MaTX", "%" + maTX + "%");
            }

            if (!string.IsNullOrWhiteSpace(maXB))
            {
                sql += " AND MaXB LIKE @MaXB";
                cmd.Parameters.AddWithValue("@MaXB", "%" + maXB + "%");
            }

            if (!string.IsNullOrWhiteSpace(diadiem))
            {
                sql += " AND Diadiemden LIKE @Diadiem";
                cmd.Parameters.AddWithValue("@Diadiem", "%" + diadiem + "%");
            }

            cmd.CommandText = sql + " ORDER BY Ngayphancong DESC, Giokhoihanh DESC";

            try
            {
                var dt = new DataTable();
                using var da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                dataGridView1.DataSource = dt;
                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                dataGridView1.ClearSelection();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tìm kiếm phân công.\n{ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnReset_Click(object? sender, EventArgs e)
        {
            // Clear inputs and reload full assignment list
            ClearInputs();
            LoadAssignmentsIntoGrid();
            dataGridView1.ClearSelection();
        }

        private void ClearInputs()
        {
            if (cbbTaixe != null) cbbTaixe.SelectedIndex = -1;
            if (cbbXe != null) cbbXe.SelectedIndex = -1;
            if (datengayphancong != null) datengayphancong.Value = DateTime.Now;
            if (timekhoihanh != null) timekhoihanh.Value = DateTime.Today;
            if (txtDiadiem != null) txtDiadiem.Text = "";
            SetInputsEnabled(true);
        }

        private void DataGridView1_SelectionChanged(object? sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow?.DataBoundItem is DataRowView drv)
            {
                LoadDataRowToInputs(drv.Row);
                // Selecting a row should show read-only view until user clicks Sửa
                SetInputsEnabled(false);
            }
            else
            {
                ClearInputs();
            }
        }

        #endregion

        private void quanlyphancong_Load(object sender, EventArgs e)
        {

        }

        private void btnExcel_Click(object sender, EventArgs e)
        {
            try
            {
                // Prefer exporting the underlying DataTable (preserves types)
                if (dataGridView1?.DataSource is DataTable dt && dt.Rows.Count > 0)
                {
                    ExcelExporter.PromptAndExport(dt, this);
                    return;
                }

                // Fallback to exporting visible DataGridView content
                if (dataGridView1 != null && dataGridView1.Rows.Count > 0)
                {
                    ExcelExporter.PromptAndExport(dataGridView1, this);
                    return;
                }

                MessageBox.Show("Không có dữ liệu để xuất.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xuất file Excel.\n{ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
