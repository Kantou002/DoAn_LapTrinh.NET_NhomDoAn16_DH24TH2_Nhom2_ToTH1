using System;
using System.Linq;
using System.Windows.Forms;
using QuanlyTaiXe;
using QuanLyXe;

namespace MENUQL
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
    public partial class Menu : BaseForm
    {
        public Menu()
        {
            InitializeComponent();
            this.Icon = System.Drawing.Icon.FromHandle(Quanlyxevalaixe.Properties.Resources.icons8_driver_48.GetHicon());
            // Make textboxes act like buttons (clickable)
            textBox1.Cursor = Cursors.Hand; // Quản Lý Xe Buýt
            textBox4.Cursor = Cursors.Hand; // Quản Lý Tài Xế
            textBox2.Cursor = Cursors.Hand; // Phân Công Chuyến Đi
            textBox3.Cursor = Cursors.Hand; // Thoát

            // Prevent double subscription: unsubscribe first, then subscribe.
            textBox1.Click -= TextBox1_Click;
            textBox4.Click -= TextBox4_Click;
            textBox2.Click -= TextBox2_Click;
            textBox3.Click -= TextBox3_Click;

            textBox1.Click += TextBox1_Click;
            textBox4.Click += TextBox4_Click;
            textBox2.Click += TextBox2_Click;
            textBox3.Click += TextBox3_Click;
        }

        private void TextBox1_Click(object? sender, EventArgs e)
        {
            // avoid duplicate window
            var existing = Application.OpenForms.OfType<quanlyxe>().FirstOrDefault();
            if (existing != null)
            {
                existing.Activate();
                return;
            }

            try
            {
                using var f = new quanlyxe { Owner = this };
                // hide menu while child is shown modally so focus returns reliably
                this.Hide();
                f.ShowDialog(this);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Cannot open Quản Lý Xe Buýt.\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // always restore and activate menu after child closed
                this.Show();
                this.Activate();
            }
        }

        private void TextBox4_Click(object? sender, EventArgs e)
        {
            var existing = Application.OpenForms.OfType<quanlytaixe>().FirstOrDefault();
            if (existing != null)
            {
                existing.Activate();
                return;
            }

            try
            {
                using var f = new quanlytaixe { Owner = this };
                this.Hide();
                f.ShowDialog(this);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Cannot open Quản Lý Tài Xế.\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this.Show();
                this.Activate();
            }
        }

        private void TextBox2_Click(object? sender, EventArgs e)
        {
            var existing = Application.OpenForms.OfType<quanlyphancong>().FirstOrDefault();
            if (existing != null)
            {
                existing.Activate();
                return;
            }

            try
            {
                using var f = new quanlyphancong { Owner = this };
                this.Hide();
                f.ShowDialog(this);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Cannot open Phân Công Chuyến Đi.\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this.Show();
                this.Activate();
            }
        }

        private void TextBox3_Click(object? sender, EventArgs e)
        {
            var result = MessageBox.Show("Bạn có chắc muốn thoát chương trình?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
                Application.Exit();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void Menu_Load(object sender, EventArgs e)
        {

        }
    }
}
