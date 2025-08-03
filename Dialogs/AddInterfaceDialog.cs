using System;
using System.Drawing;
using System.Windows.Forms;
using PIInterfaceConfigUtility.Models;

namespace PIInterfaceConfigUtility.Dialogs
{
    public partial class AddInterfaceDialog : Form
    {
        // Make all UI controls nullable
        private Label? nameLabel;
        private TextBox? nameTextBox;
        private Label? typeLabel;
        private ComboBox? typeComboBox;
        private Label? descriptionLabel;
        private TextBox? descriptionTextBox;
        private Button? okButton;
        private Button? cancelButton;

        public PIInterface Interface { get; private set; } = new PIInterface();

        public AddInterfaceDialog()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Add New Interface";
            this.Size = new Size(400, 250);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            CreateControls();
            LayoutControls();
            SetupEventHandlers();
        }

        private void CreateControls()
        {
            nameLabel = new Label { Text = "Interface Name:" };
            nameTextBox = new TextBox();

            typeLabel = new Label { Text = "Interface Type:" };
            typeComboBox = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            typeComboBox.Items.AddRange(new object[]
            {
                InterfaceType.PIPing,
                InterfaceType.OPCDA,
                InterfaceType.OPCAE,
                InterfaceType.UFL,
                InterfaceType.RDBMS,
                InterfaceType.Perfmon,
                InterfaceType.UniInt,
                InterfaceType.Custom
            });
            typeComboBox.SelectedIndex = 0;

            descriptionLabel = new Label { Text = "Description:" };
            descriptionTextBox = new TextBox { Multiline = true };

            okButton = new Button
            {
                Text = "OK",
                DialogResult = DialogResult.OK
            };

            cancelButton = new Button
            {
                Text = "Cancel",
                DialogResult = DialogResult.Cancel
            };

            this.AcceptButton = okButton;
            this.CancelButton = cancelButton;
        }

        private void LayoutControls()
        {
            const int margin = 20;
            const int labelWidth = 100;
            const int controlWidth = 250;
            const int rowHeight = 35;
            int currentY = margin;

            // Interface Name
            nameLabel!.Location = new Point(margin, currentY);
            nameLabel.Size = new Size(labelWidth, 20);
            nameTextBox!.Location = new Point(margin + labelWidth + 10, currentY);
            nameTextBox.Size = new Size(controlWidth, 20);
            currentY += rowHeight;

            // Interface Type
            typeLabel!.Location = new Point(margin, currentY);
            typeLabel.Size = new Size(labelWidth, 20);
            typeComboBox!.Location = new Point(margin + labelWidth + 10, currentY);
            typeComboBox.Size = new Size(controlWidth, 20);
            currentY += rowHeight;

            // Description
            descriptionLabel!.Location = new Point(margin, currentY);
            descriptionLabel.Size = new Size(labelWidth, 20);
            descriptionTextBox!.Location = new Point(margin + labelWidth + 10, currentY);
            descriptionTextBox.Size = new Size(controlWidth, 60);
            currentY += 80;

            // Buttons
            okButton!.Location = new Point(margin + controlWidth - 80, currentY);
            okButton.Size = new Size(75, 25);
            cancelButton!.Location = new Point(margin + controlWidth + 5, currentY);
            cancelButton.Size = new Size(75, 25);

            // Add controls to form
            this.Controls.AddRange(new Control[]
            {
                nameLabel, nameTextBox,
                typeLabel, typeComboBox,
                descriptionLabel, descriptionTextBox,
                okButton, cancelButton
            });
        }

        private void SetupEventHandlers()
        {
            okButton!.Click += OkButton_Click;
        }

        private void OkButton_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(nameTextBox!.Text))
            {
                MessageBox.Show("Please enter an interface name.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                nameTextBox.Focus();
                return;
            }

            // Create the interface
            Interface = new PIInterface(nameTextBox.Text.Trim(), (InterfaceType)typeComboBox!.SelectedItem!)
            {
                Description = descriptionTextBox!.Text.Trim(),
                ServiceName = $"PI-{nameTextBox.Text.Trim()}",
                Status = InterfaceStatus.Stopped,
                IsEnabled = true,
                Version = "1.0.0"
            };

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
} 