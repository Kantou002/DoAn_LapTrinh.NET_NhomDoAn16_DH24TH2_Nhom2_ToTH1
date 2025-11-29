namespace QuanLyXe
{
    partial class quanlyxe
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            txtMaXB = new TextBox();
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            label4 = new Label();
            label5 = new Label();
            label6 = new Label();
            label7 = new Label();
            label8 = new Label();
            label9 = new Label();
            txtNSX = new TextBox();
            txtSoghe = new TextBox();
            txtBienso = new TextBox();
            txtHangxe = new TextBox();
            cbbTT = new ComboBox();
            txtMaBH = new TextBox();
            label10 = new Label();
            backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            dataGridView1 = new DataGridView();
            btnthem = new Button();
            btnxoa = new Button();
            btnluu = new Button();
            btnsua = new Button();
            btnhuy = new Button();
            btnthoat = new Button();
            label11 = new Label();
            btnReset = new Button();
            btnTimkiem = new Button();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            SuspendLayout();
            // 
            // txtMaXB
            // 
            txtMaXB.Location = new Point(103, 87);
            txtMaXB.Name = "txtMaXB";
            txtMaXB.Size = new Size(125, 27);
            txtMaXB.TabIndex = 0;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.BackColor = SystemColors.HighlightText;
            label1.Font = new Font("Times New Roman", 18F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label1.ForeColor = Color.Red;
            label1.Location = new Point(283, 9);
            label1.Name = "label1";
            label1.Size = new Size(295, 35);
            label1.TabIndex = 3;
            label1.Text = "QUẢN LÝ XE BUÝT";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(47, 141);
            label2.Name = "label2";
            label2.Size = new Size(0, 20);
            label2.TabIndex = 4;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(12, 91);
            label3.Name = "label3";
            label3.Size = new Size(0, 20);
            label3.TabIndex = 5;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.ForeColor = SystemColors.HotTrack;
            label4.Location = new Point(12, 141);
            label4.Name = "label4";
            label4.Size = new Size(69, 20);
            label4.TabIndex = 6;
            label4.Text = "Hãng Xe:";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.ForeColor = SystemColors.HotTrack;
            label5.Location = new Point(12, 187);
            label5.Name = "label5";
            label5.Size = new Size(81, 20);
            label5.TabIndex = 7;
            label5.Text = "Tình Trạng:";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.ForeColor = SystemColors.HotTrack;
            label6.Location = new Point(539, 94);
            label6.Name = "label6";
            label6.Size = new Size(103, 20);
            label6.TabIndex = 8;
            label6.Text = "Mã Bão Hiểm:";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.ForeColor = SystemColors.Highlight;
            label7.Location = new Point(283, 138);
            label7.Name = "label7";
            label7.Size = new Size(59, 20);
            label7.TabIndex = 9;
            label7.Text = "Số Ghế:";
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.ForeColor = SystemColors.HotTrack;
            label8.Location = new Point(260, 187);
            label8.Name = "label8";
            label8.Size = new Size(106, 20);
            label8.TabIndex = 10;
            label8.Text = "Năm Sản Xuất:";
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.ForeColor = SystemColors.Highlight;
            label9.Location = new Point(283, 90);
            label9.Name = "label9";
            label9.Size = new Size(83, 20);
            label9.TabIndex = 11;
            label9.Text = "Biển Số Xe:";
            // 
            // txtNSX
            // 
            txtNSX.Location = new Point(372, 184);
            txtNSX.Name = "txtNSX";
            txtNSX.Size = new Size(125, 27);
            txtNSX.TabIndex = 12;
            // 
            // txtSoghe
            // 
            txtSoghe.Location = new Point(372, 138);
            txtSoghe.Name = "txtSoghe";
            txtSoghe.Size = new Size(125, 27);
            txtSoghe.TabIndex = 13;
            // 
            // txtBienso
            // 
            txtBienso.Location = new Point(372, 88);
            txtBienso.Name = "txtBienso";
            txtBienso.Size = new Size(125, 27);
            txtBienso.TabIndex = 14;
            // 
            // txtHangxe
            // 
            txtHangxe.Location = new Point(103, 134);
            txtHangxe.Name = "txtHangxe";
            txtHangxe.Size = new Size(125, 27);
            txtHangxe.TabIndex = 15;
            // 
            // cbbTT
            // 
            cbbTT.DropDownStyle = ComboBoxStyle.DropDownList;
            cbbTT.Items.AddRange(new object[] { "available", "maintainance", "onthetrip", "unvailable" });
            cbbTT.Location = new Point(103, 184);
            cbbTT.Name = "cbbTT";
            cbbTT.Size = new Size(125, 28);
            cbbTT.TabIndex = 16;
            // 
            // txtMaBH
            // 
            txtMaBH.Location = new Point(648, 88);
            txtMaBH.Name = "txtMaBH";
            txtMaBH.Size = new Size(125, 27);
            txtMaBH.TabIndex = 17;
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.ForeColor = SystemColors.Highlight;
            label10.Location = new Point(12, 91);
            label10.Name = "label10";
            label10.Size = new Size(87, 20);
            label10.TabIndex = 18;
            label10.Text = "Mã Xe Buýt:";
            // 
            // dataGridView1
            // 
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Location = new Point(7, 263);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.RowHeadersWidth = 51;
            dataGridView1.Size = new Size(834, 234);
            dataGridView1.TabIndex = 19;
            // 
            // btnthem
            // 
            btnthem.BackColor = Color.Chartreuse;
            btnthem.ForeColor = Color.Black;
            btnthem.Location = new Point(539, 136);
            btnthem.Name = "btnthem";
            btnthem.Size = new Size(94, 29);
            btnthem.TabIndex = 20;
            btnthem.Text = "Thêm Xe";
            btnthem.UseVisualStyleBackColor = false;
            btnthem.Click += button1_Click;
            // 
            // btnxoa
            // 
            btnxoa.BackColor = Color.Fuchsia;
            btnxoa.Location = new Point(539, 187);
            btnxoa.Name = "btnxoa";
            btnxoa.Size = new Size(94, 29);
            btnxoa.TabIndex = 21;
            btnxoa.Text = "Xóa ";
            btnxoa.UseVisualStyleBackColor = false;
            // 
            // btnluu
            // 
            btnluu.BackColor = Color.Blue;
            btnluu.ForeColor = SystemColors.ControlLightLight;
            btnluu.Location = new Point(648, 134);
            btnluu.Name = "btnluu";
            btnluu.Size = new Size(94, 29);
            btnluu.TabIndex = 22;
            btnluu.Text = "Lưu";
            btnluu.UseVisualStyleBackColor = false;
            // 
            // btnsua
            // 
            btnsua.BackColor = Color.FromArgb(128, 255, 255);
            btnsua.Location = new Point(647, 187);
            btnsua.Name = "btnsua";
            btnsua.Size = new Size(94, 29);
            btnsua.TabIndex = 23;
            btnsua.Text = "Sửa";
            btnsua.UseVisualStyleBackColor = false;
            // 
            // btnhuy
            // 
            btnhuy.Location = new Point(752, 136);
            btnhuy.Name = "btnhuy";
            btnhuy.Size = new Size(94, 29);
            btnhuy.TabIndex = 24;
            btnhuy.Text = "Hủy";
            btnhuy.UseVisualStyleBackColor = true;
            // 
            // btnthoat
            // 
            btnthoat.BackColor = Color.Crimson;
            btnthoat.Location = new Point(752, 187);
            btnthoat.Name = "btnthoat";
            btnthoat.Size = new Size(94, 29);
            btnthoat.TabIndex = 25;
            btnthoat.Text = "Thoát";
            btnthoat.UseVisualStyleBackColor = false;
            // 
            // label11
            // 
            label11.AutoSize = true;
            label11.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label11.ForeColor = Color.Red;
            label11.Location = new Point(7, 232);
            label11.Name = "label11";
            label11.Size = new Size(197, 28);
            label11.TabIndex = 26;
            label11.Text = "Danh Sách Xe Buýt:";
            // 
            // btnReset
            // 
            btnReset.Location = new Point(210, 231);
            btnReset.Name = "btnReset";
            btnReset.Size = new Size(94, 29);
            btnReset.TabIndex = 27;
            btnReset.Text = "Reset";
            btnReset.UseVisualStyleBackColor = true;
            btnReset.Click += btnReset_Click;
            // 
            // btnTimkiem
            // 
            btnTimkiem.Location = new Point(323, 231);
            btnTimkiem.Name = "btnTimkiem";
            btnTimkiem.Size = new Size(94, 29);
            btnTimkiem.TabIndex = 28;
            btnTimkiem.Text = "Tìm Kiếm";
            btnTimkiem.UseVisualStyleBackColor = true;
            btnTimkiem.Click += btnTimkiem_Click;
            // 
            // quanlyxe
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.ControlLightLight;
            ClientSize = new Size(853, 509);
            Controls.Add(btnTimkiem);
            Controls.Add(btnReset);
            Controls.Add(label11);
            Controls.Add(btnthoat);
            Controls.Add(btnhuy);
            Controls.Add(btnsua);
            Controls.Add(btnluu);
            Controls.Add(btnxoa);
            Controls.Add(btnthem);
            Controls.Add(dataGridView1);
            Controls.Add(label10);
            Controls.Add(txtMaBH);
            Controls.Add(cbbTT);
            Controls.Add(txtHangxe);
            Controls.Add(txtBienso);
            Controls.Add(txtSoghe);
            Controls.Add(txtNSX);
            Controls.Add(label9);
            Controls.Add(label8);
            Controls.Add(label7);
            Controls.Add(label6);
            Controls.Add(label5);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(txtMaXB);
            Name = "quanlyxe";
            Text = "frmPCchuyendi";
            Load += Form1_Load;
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox txtMaXB;
        private Label label1;
        private Label label2;
        private Label label3;
        private Label label4;
        private Label label5;
        private Label label6;
        private Label label7;
        private Label label8;
        private Label label9;
        private TextBox txtNSX;
        private TextBox txtSoghe;
        private TextBox txtBienso;
        private TextBox txtHangxe;
        private ComboBox cbbTT;
        private TextBox txtMaBH;
        private Label label10;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private DataGridView dataGridView1;
        private Button btnthem;
        private Button btnxoa;
        private Button btnluu;
        private Button btnsua;
        private Button btnhuy;
        private Button btnthoat;
        private Label label11;
        private Button btnReset;
        private Button btnTimkiem;
    }
}
