using System;
using System.Drawing;
using System.Windows.Forms;

namespace PIInterfaceConfigUtility
{
    public partial class AboutDialog : Form
    {
        public AboutDialog()
        {
            InitializeComponent();
        }
        
        private void InitializeComponent()
        {
            this.SuspendLayout();
            
            // Form properties
            this.Size = new Size(450, 300);
            this.Text = "About PI Interface Configuration Utility";
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;
            this.ShowIcon = false;
            this.BackColor = Color.White;
            
            // Title
            var titleLabel = new Label
            {
                Text = "PI Interface Configuration Utility",
                Location = new Point(20, 20),
                Size = new Size(400, 30),
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 120, 215)
            };
            
            // Version
            var versionLabel = new Label
            {
                Text = "Version 1.0.0",
                Location = new Point(20, 55),
                Size = new Size(400, 20),
                Font = new Font("Segoe UI", 10F)
            };
            
            // Description
            var descriptionLabel = new Label
            {
                Text = "A comprehensive desktop application for configuring and managing\nPI interfaces and data collection systems.",
                Location = new Point(20, 85),
                Size = new Size(400, 40),
                Font = new Font("Segoe UI", 9F)
            };
            
            // Features
            var featuresLabel = new Label
            {
                Text = "Features:\n• PI Server connection management\n• Interface configuration and control\n• PI Points management\n• Service monitoring and diagnostics\n• Configuration import/export\n• Real-time logging and troubleshooting",
                Location = new Point(20, 135),
                Size = new Size(400, 100),
                Font = new Font("Segoe UI", 9F)
            };
            
            // Copyright
            var copyrightLabel = new Label
            {
                Text = "© 2024 Custom PI Tools. All rights reserved.",
                Location = new Point(20, 200),
                Size = new Size(400, 20),
                Font = new Font("Segoe UI", 8F),
                ForeColor = Color.Gray
            };
            
            // Technology
            var techLabel = new Label
            {
                Text = "Built with .NET 6.0 and Windows Forms",
                Location = new Point(20, 220),
                Size = new Size(400, 20),
                Font = new Font("Segoe UI", 8F),
                ForeColor = Color.Gray
            };
            
            // OK Button
            var okButton = new Button
            {
                Text = "OK",
                Location = new Point(350, 230),
                Size = new Size(75, 25),
                DialogResult = DialogResult.OK,
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            
            this.Controls.AddRange(new Control[]
            {
                titleLabel, versionLabel, descriptionLabel,
                featuresLabel, copyrightLabel, techLabel, okButton
            });
            
            this.AcceptButton = okButton;
            
            this.ResumeLayout(false);
        }
    }
} 