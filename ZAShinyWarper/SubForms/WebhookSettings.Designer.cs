using System.Windows.Forms;
using ZAShinyWarper;

namespace ZAShinyWarper
{
    partial class WebhookForm
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
            components = new System.ComponentModel.Container();
            programConfigBindingSource = new BindingSource(components);
            dGWebhookSettings = new DataGridView();
            btnSave = new Button();
            btnCancel = new Button();
            btnRemoveRow = new Button();
            btnAddRow = new Button();
            ((System.ComponentModel.ISupportInitialize)programConfigBindingSource).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dGWebhookSettings).BeginInit();
            SuspendLayout();
            // 
            // programConfigBindingSource
            // 
            programConfigBindingSource.DataSource = typeof(ProgramConfig);
            // 
            // dGWebhookSettings
            // 
            dGWebhookSettings.AllowUserToAddRows = false;
            dGWebhookSettings.AllowUserToResizeColumns = false;
            dGWebhookSettings.AllowUserToResizeRows = false;
            dGWebhookSettings.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dGWebhookSettings.Dock = DockStyle.Top;
            dGWebhookSettings.Location = new System.Drawing.Point(0, 0);
            dGWebhookSettings.MultiSelect = false;
            dGWebhookSettings.Name = "dGWebhookSettings";
            dGWebhookSettings.RowHeadersVisible = false;
            dGWebhookSettings.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            dGWebhookSettings.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dGWebhookSettings.Size = new System.Drawing.Size(534, 233);
            dGWebhookSettings.TabIndex = 0;
            // 
            // btnSave
            // 
            btnSave.Location = new System.Drawing.Point(366, 251);
            btnSave.Name = "btnSave";
            btnSave.Size = new System.Drawing.Size(75, 23);
            btnSave.TabIndex = 1;
            btnSave.Text = "Save";
            btnSave.UseVisualStyleBackColor = true;
            btnSave.Click += OnClickSave;
            // 
            // btnCancel
            // 
            btnCancel.Location = new System.Drawing.Point(447, 251);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new System.Drawing.Size(75, 23);
            btnCancel.TabIndex = 2;
            btnCancel.Text = "Cancel";
            btnCancel.UseVisualStyleBackColor = true;
            btnCancel.Click += OnClickCancel;
            // 
            // btnRemoveRow
            // 
            btnRemoveRow.Location = new System.Drawing.Point(84, 239);
            btnRemoveRow.Name = "btnRemoveRow";
            btnRemoveRow.Size = new System.Drawing.Size(75, 23);
            btnRemoveRow.TabIndex = 4;
            btnRemoveRow.Text = "Remove";
            btnRemoveRow.UseVisualStyleBackColor = true;
            btnRemoveRow.Click += OnClickRemove;
            // 
            // btnAddRow
            // 
            btnAddRow.Location = new System.Drawing.Point(3, 239);
            btnAddRow.Name = "btnAddRow";
            btnAddRow.Size = new System.Drawing.Size(75, 23);
            btnAddRow.TabIndex = 3;
            btnAddRow.Text = "Add";
            btnAddRow.UseVisualStyleBackColor = true;
            btnAddRow.Click += OnClickAdd;
            // 
            // WebhookForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(534, 290);
            Controls.Add(btnRemoveRow);
            Controls.Add(btnAddRow);
            Controls.Add(btnCancel);
            Controls.Add(btnSave);
            Controls.Add(dGWebhookSettings);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Name = "WebhookForm";
            Text = "Webhook Settings";
            ((System.ComponentModel.ISupportInitialize)programConfigBindingSource).EndInit();
            ((System.ComponentModel.ISupportInitialize)dGWebhookSettings).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.BindingSource programConfigBindingSource;
        private System.Windows.Forms.DataGridView dGWebhookSettings;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnRemoveRow;
        private System.Windows.Forms.Button btnAddRow;
    }
}