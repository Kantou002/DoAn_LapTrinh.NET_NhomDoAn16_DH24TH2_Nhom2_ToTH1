namespace MENUQL
{
    partial class Menu
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
            label1 = new Label();
            textBox1 = new TextBox();
            textBox2 = new TextBox();
            textBox3 = new TextBox();
            textBox4 = new TextBox();
            pictureBox1 = new PictureBox();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.BackColor = SystemColors.ButtonHighlight;
            label1.Font = new Font("Segoe UI", 19.8000011F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label1.ForeColor = Color.Blue;
            label1.Location = new Point(324, 9);
            label1.Name = "label1";
            label1.Size = new Size(279, 46);
            label1.TabIndex = 0;
            label1.Text = "MENU QUẢN LÝ";
            // 
            // textBox1
            // 
            textBox1.BackColor = Color.FromArgb(255, 192, 192);
            textBox1.Cursor = Cursors.Hand;
            textBox1.Font = new Font("Segoe UI", 18F, FontStyle.Bold, GraphicsUnit.Point, 0);
            textBox1.ForeColor = Color.Red;
            textBox1.Location = new Point(324, 100);
            textBox1.Name = "textBox1";
            textBox1.ReadOnly = true;
            textBox1.Size = new Size(279, 47);
            textBox1.TabIndex = 1;
            textBox1.Text = "Quản Lý Xe Buýt";
            textBox1.TextAlign = HorizontalAlignment.Center;
            textBox1.Click += TextBox1_Click;
            // 
            // textBox2
            // 
            textBox2.BackColor = Color.FromArgb(255, 192, 255);
            textBox2.Cursor = Cursors.Hand;
            textBox2.Font = new Font("Segoe UI", 16.2F, FontStyle.Bold, GraphicsUnit.Point, 0);
            textBox2.ForeColor = Color.Purple;
            textBox2.Location = new Point(324, 278);
            textBox2.Name = "textBox2";
            textBox2.ReadOnly = true;
            textBox2.Size = new Size(279, 43);
            textBox2.TabIndex = 2;
            textBox2.Text = "Phân Công Chuyến Đi";
            textBox2.TextAlign = HorizontalAlignment.Center;
            textBox2.Click += TextBox2_Click;
            // 
            // textBox3
            // 
            textBox3.BackColor = Color.Red;
            textBox3.Cursor = Cursors.Hand;
            textBox3.Font = new Font("Segoe UI", 13.8F, FontStyle.Regular, GraphicsUnit.Point, 0);
            textBox3.ForeColor = SystemColors.InactiveBorder;
            textBox3.Location = new Point(406, 362);
            textBox3.Name = "textBox3";
            textBox3.ReadOnly = true;
            textBox3.Size = new Size(125, 38);
            textBox3.TabIndex = 3;
            textBox3.Text = "Thoát";
            textBox3.TextAlign = HorizontalAlignment.Center;
            textBox3.Click += TextBox3_Click;
            // 
            // textBox4
            // 
            textBox4.BackColor = Color.SkyBlue;
            textBox4.Cursor = Cursors.Hand;
            textBox4.Font = new Font("Segoe UI", 18F, FontStyle.Bold, GraphicsUnit.Point, 0);
            textBox4.ForeColor = Color.Blue;
            textBox4.Location = new Point(324, 187);
            textBox4.Name = "textBox4";
            textBox4.ReadOnly = true;
            textBox4.Size = new Size(279, 47);
            textBox4.TabIndex = 4;
            textBox4.Text = "Quản Lý Tài Xế ";
            textBox4.TextAlign = HorizontalAlignment.Center;
            textBox4.Click += TextBox4_Click;
            // 
            // pictureBox1
            // 
            pictureBox1.Image = Quanlyxevalaixe.Properties.Resources.icons8_driver_48;
            pictureBox1.Location = new Point(270, 9);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(48, 48);
            pictureBox1.TabIndex = 5;
            pictureBox1.TabStop = false;
            // 
            // Menu
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(925, 497);
            Controls.Add(pictureBox1);
            Controls.Add(textBox4);
            Controls.Add(textBox3);
            Controls.Add(textBox2);
            Controls.Add(textBox1);
            Controls.Add(label1);
            Name = "Menu";
            Text = "MENU";
            Load += Menu_Load;
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private TextBox textBox1;
        private TextBox textBox2;
        private TextBox textBox3;
        private TextBox textBox4;
        private PictureBox pictureBox1;
    }
}
