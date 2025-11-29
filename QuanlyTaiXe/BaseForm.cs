using System;
using System.Windows.Forms;

namespace QuanlyTaiXe
{
    // Base form that ensures every derived form opens centered on screen.
    // Add this file to the project and change your forms to inherit from BaseForm instead of Form.
    public class BaseForm : Form
    {
        public BaseForm()
        {
            // Ensure designer or other code does not override this before load.
            StartPosition = FormStartPosition.CenterScreen;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            // Extra safety: force center when the form is shown.
            try
            {
                CenterToScreen();
            }
            catch
            {
                // Ignore any edge-case failures during startup.
            }
        }
    }
}