using System;
using System.Drawing;
using System.Windows.Forms;
using PIInterfaceConfigUtility.Models;

namespace PIInterfaceConfigUtility
{
    public partial class AddInterfaceDialog : Form
    {
        private TextBox nameTextBox, descriptionTextBox;
        private ComboBox typeComboBox;
        private Button okButton, cancelButton;
        
        public string InterfaceName => nameTextBox.Text;
        public string Description => descriptionTextBox.Text;
        public InterfaceType InterfaceType => (InterfaceType)typeComboBox.SelectedItem;
        
        public AddInterfaceDialog()
        {
            InitializeComponent();
        }
        
        private void InitializeComponent()
        {
            this.SuspendLayout();
            
            this.Size = new Size(400, 250);
            this.Text = "Add New Interface";
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;
            this.ShowIcon = false;
            
            // Name
            var nameLabel = new Label
            {
                Text = "Interface Name:",
                Location = new Point(15, 20),
                Size = new Size(100, 23)
            };
            
            nameTextBox = new TextBox
            {
                Location = new Point(125, 17),
                Size = new Size(240, 23)
            };
            
            // Type
            var typeLabel = new Label
            {
                Text = "Interface Type:",
                Location = new Point(15, 50),
                Size = new Size(100, 23)
            };
            
            typeComboBox = new ComboBox
            {
                Location = new Point(125, 47),
                Size = new Size(240, 23),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            
            foreach (InterfaceType type in Enum.GetValues<InterfaceType>())
            {
                typeComboBox.Items.Add(type);
            }
            typeComboBox.SelectedIndex = 0;
            
            // Description
            var descLabel = new Label
            {
                Text = "Description:",
                Location = new Point(15, 80),
                Size = new Size(100, 23)
            };
            
            descriptionTextBox = new TextBox
            {
                Location = new Point(125, 77),
                Size = new Size(240, 60),
                Multiline = true,
                ScrollBars = ScrollBars.Vertical
            };
            
            // Buttons
            okButton = new Button
            {
                Text = "Add",
                Location = new Point(210, 170),
                Size = new Size(75, 30),
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                DialogResult = DialogResult.OK
            };
            
            cancelButton = new Button
            {
                Text = "Cancel",
                Location = new Point(295, 170),
                Size = new Size(75, 30),
                BackColor = Color.FromArgb(108, 117, 125),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                DialogResult = DialogResult.Cancel
            };
            
            this.Controls.AddRange(new Control[]
            {
                nameLabel, nameTextBox, typeLabel, typeComboBox,
                descLabel, descriptionTextBox, okButton, cancelButton
            });
            
            this.AcceptButton = okButton;
            this.CancelButton = cancelButton;
            
            okButton.Click += OkButton_Click;
            
            this.ResumeLayout(false);
        }
        
        private void OkButton_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(nameTextBox.Text))
            {
                MessageBox.Show("Please enter an interface name.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.DialogResult = DialogResult.None;
                return;
            }
        }
    }
} 