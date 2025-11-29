namespace QuanlyTaiXe
{
    partial class quanlyphancong
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            label4 = new Label();
            label5 = new Label();
            label8 = new Label();
            sqlCommand1 = new Microsoft.Data.SqlClient.SqlCommand();
            timekhoihanh = new DateTimePicker();
            cbbTaixe = new ComboBox();
            cbbXe = new ComboBox();
            datengayphancong = new DateTimePicker();
            txtDiadiem = new TextBox();
            dataGridView1 = new DataGridView();
            label6 = new Label();
            btnThem = new Button();
            btnSua = new Button();
            btnXoa = new Button();
            btnTimkiem = new Button();
            btnThoat = new Button();
            btnReset = new Button();
            btnLuu = new Button();
            btnHuy = new Button();
            btnExcel = new Button();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            SuspendLayout();
            // 
            // label1
            // 
            label1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            label1.AutoSize = true;
            label1.Font = new Font("Times New Roman", 18F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label1.ForeColor = Color.Purple;
            label1.Location = new Point(286, 9);
            label1.Name = "label1";
            label1.Size = new Size(345, 35);
            label1.TabIndex = 0;
            label1.Text = "QUẢN LÝ PHÂN CÔNG";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.ForeColor = Color.DodgerBlue;
            label2.Location = new Point(12, 80);
            label2.Name = "label2";
            label2.Size = new Size(86, 20);
            label2.TabIndex = 1;
            label2.Text = "Chọn tài xế:";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.ForeColor = Color.DodgerBlue;
            label3.Location = new Point(279, 80);
            label3.Name = "label3";
            label3.Size = new Size(65, 20);
            label3.TabIndex = 2;
            label3.Text = "Chọn xe:";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.ForeColor = Color.DodgerBlue;
            label4.Location = new Point(286, 132);
            label4.Name = "label4";
            label4.Size = new Size(102, 20);
            label4.TabIndex = 3;
            label4.Text = "Địa điểm đến:";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.ForeColor = Color.DodgerBlue;
            label5.Location = new Point(12, 132);
            label5.Name = "label5";
            label5.Size = new Size(103, 20);
            label5.TabIndex = 4;
            label5.Text = "Giờ khởi hành:";
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.BackColor = SystemColors.ControlLightLight;
            label8.ForeColor = Color.DodgerBlue;
            label8.Location = new Point(525, 80);
            label8.Name = "label8";
            label8.Size = new Size(121, 20);
            label8.TabIndex = 7;
            label8.Text = "Ngày phân công:";
            // 
            // sqlCommand1
            // 
            sqlCommand1.CommandTimeout = 30;
            sqlCommand1.EnableOptimizedParameterBinding = false;
            // 
            // timekhoihanh
            // 
            timekhoihanh.CustomFormat = "hh:mm tt";
            timekhoihanh.Format = DateTimePickerFormat.Time;
            timekhoihanh.Location = new Point(121, 127);
            timekhoihanh.Name = "timekhoihanh";
            timekhoihanh.ShowUpDown = true;
            timekhoihanh.Size = new Size(152, 27);
            timekhoihanh.TabIndex = 8;
            // 
            // cbbTaixe
            // 
            cbbTaixe.FormattingEnabled = true;
            cbbTaixe.Location = new Point(104, 77);
            cbbTaixe.Name = "cbbTaixe";
            cbbTaixe.Size = new Size(169, 28);
            cbbTaixe.TabIndex = 10;
            // 
            // cbbXe
            // 
            cbbXe.FormattingEnabled = true;
            cbbXe.Location = new Point(350, 77);
            cbbXe.Name = "cbbXe";
            cbbXe.Size = new Size(169, 28);
            cbbXe.TabIndex = 11;
            // 
            // datengayphancong
            // 
            datengayphancong.Location = new Point(652, 77);
            datengayphancong.Name = "datengayphancong";
            datengayphancong.Size = new Size(250, 27);
            datengayphancong.TabIndex = 12;
            // 
            // txtDiadiem
            // 
            txtDiadiem.Location = new Point(394, 125);
            txtDiadiem.Name = "txtDiadiem";
            txtDiadiem.Size = new Size(252, 27);
            txtDiadiem.TabIndex = 13;
            // 
            // dataGridView1
            // 
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Location = new Point(12, 253);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.RowHeadersWidth = 51;
            dataGridView1.Size = new Size(890, 231);
            dataGridView1.TabIndex = 14;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label6.ForeColor = Color.Purple;
            label6.Location = new Point(12, 210);
            label6.Name = "label6";
            label6.Size = new Size(189, 28);
            label6.TabIndex = 15;
            label6.Text = "Danh sách chuyến:";
            // 
            // btnThem
            // 
            btnThem.BackColor = Color.Lime;
            btnThem.ForeColor = SystemColors.ControlText;
            btnThem.Location = new Point(12, 175);
            btnThem.Name = "btnThem";
            btnThem.Size = new Size(123, 29);
            btnThem.TabIndex = 16;
            btnThem.Text = "Phân công mới";
            btnThem.UseVisualStyleBackColor = false;
            // 
            // btnSua
            // 
            btnSua.BackColor = Color.Yellow;
            btnSua.Location = new Point(145, 175);
            btnSua.Name = "btnSua";
            btnSua.Size = new Size(128, 29);
            btnSua.TabIndex = 17;
            btnSua.Text = "Sửa chuyến";
            btnSua.UseVisualStyleBackColor = false;
            btnSua.Click += btnSua_Click;
            // 
            // btnXoa
            // 
            btnXoa.BackColor = Color.FromArgb(255, 128, 128);
            btnXoa.Location = new Point(539, 175);
            btnXoa.Name = "btnXoa";
            btnXoa.Size = new Size(123, 29);
            btnXoa.TabIndex = 18;
            btnXoa.Text = "Kết thúc chuyến";
            btnXoa.UseVisualStyleBackColor = false;
            // 
            // btnTimkiem
            // 
            btnTimkiem.BackColor = Color.FromArgb(255, 192, 128);
            btnTimkiem.Location = new Point(687, 175);
            btnTimkiem.Name = "btnTimkiem";
            btnTimkiem.Size = new Size(94, 29);
            btnTimkiem.TabIndex = 19;
            btnTimkiem.Text = "Tìm kiếm";
            btnTimkiem.UseVisualStyleBackColor = false;
            // 
            // btnThoat
            // 
            btnThoat.BackColor = Color.Red;
            btnThoat.Location = new Point(808, 175);
            btnThoat.Name = "btnThoat";
            btnThoat.Size = new Size(94, 29);
            btnThoat.TabIndex = 20;
            btnThoat.Text = "Thoát";
            btnThoat.UseVisualStyleBackColor = false;
            btnThoat.Click += btnThoat_Click;
            // 
            // btnReset
            // 
            btnReset.Location = new Point(205, 210);
            btnReset.Name = "btnReset";
            btnReset.Size = new Size(94, 28);
            btnReset.TabIndex = 21;
            btnReset.Text = "Reset";
            btnReset.UseVisualStyleBackColor = true;
            // 
            // btnLuu
            // 
            btnLuu.BackColor = Color.CornflowerBlue;
            btnLuu.Location = new Point(294, 175);
            btnLuu.Name = "btnLuu";
            btnLuu.Size = new Size(94, 29);
            btnLuu.TabIndex = 22;
            btnLuu.Text = "Lưu";
            btnLuu.UseVisualStyleBackColor = false;
            // 
            // btnHuy
            // 
            btnHuy.BackColor = SystemColors.ControlLight;
            btnHuy.Location = new Point(413, 175);
            btnHuy.Name = "btnHuy";
            btnHuy.Size = new Size(94, 29);
            btnHuy.TabIndex = 23;
            btnHuy.Text = "Hủy";
            btnHuy.UseVisualStyleBackColor = false;
            // 
            // btnExcel
            // 
            btnExcel.Location = new Point(808, 213);
            btnExcel.Name = "btnExcel";
            btnExcel.Size = new Size(94, 29);
            btnExcel.TabIndex = 24;
            btnExcel.Text = "Xuất excel";
            btnExcel.UseVisualStyleBackColor = true;
            btnExcel.Click += btnExcel_Click;
            // 
            // quanlyphancong
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.ControlLightLight;
            ClientSize = new Size(928, 496);
            Controls.Add(btnExcel);
            Controls.Add(btnHuy);
            Controls.Add(btnLuu);
            Controls.Add(btnReset);
            Controls.Add(btnThoat);
            Controls.Add(btnTimkiem);
            Controls.Add(btnXoa);
            Controls.Add(btnSua);
            Controls.Add(btnThem);
            Controls.Add(label6);
            Controls.Add(dataGridView1);
            Controls.Add(txtDiadiem);
            Controls.Add(datengayphancong);
            Controls.Add(cbbXe);
            Controls.Add(cbbTaixe);
            Controls.Add(timekhoihanh);
            Controls.Add(label8);
            Controls.Add(label5);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(label1);
            Name = "quanlyphancong";
            Text = "Form2";
            Load += quanlyphancong_Load;
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private Label label2;
        private Label label3;
        private Label label4;
        private Label label5;
        private Label label8;
        private Microsoft.Data.SqlClient.SqlCommand sqlCommand1;
        private DateTimePicker timekhoihanh;
        private ComboBox cbbTaixe;
        private ComboBox cbbXe;
        private DateTimePicker datengayphancong;
        private TextBox txtDiadiem;
        private DataGridView dataGridView1;
        private Label label6;
        private Button btnThem;
        private Button btnSua;
        private Button btnXoa;
        private Button btnTimkiem;
        private Button btnThoat;
        private Button btnReset;
        private Button btnLuu;
        private Button btnHuy;
        private Button btnExcel;
    }
}