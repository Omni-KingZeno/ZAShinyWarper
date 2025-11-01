using System;
using System.Windows.Forms;
using ZAWarper;

namespace ZAWarper
{
    public partial class WebhookForm : Form
    {
        private readonly ProgramConfig _config;

        public WebhookForm(ProgramConfig config)
        {
            InitializeComponent();
            _config = config;
            SetupWebhookView();
            LoadWebhooks();
        }

        private void SetupWebhookView()
        {
            // Start fresh
            dGWebhookSettings.Columns.Clear();
            dGWebhookSettings.Rows.Clear();

            // Create our layout
            DataGridViewTextBoxColumn webhookColumn = new()
            {
                Name = "webhookAddress",
                HeaderText = "Webhook Address",
                Width = 265,
                Resizable = DataGridViewTriState.False
            };
            dGWebhookSettings.Columns.Add(webhookColumn);

            DataGridViewTextBoxColumn messageColumn = new()
            {
                Name = "messageContents",
                HeaderText = "Message Contents",
                Width = 215,
                Resizable = DataGridViewTriState.False
            };
            dGWebhookSettings.Columns.Add(messageColumn);

            DataGridViewCheckBoxColumn checkColumn = new()
            {
                Name = "enabled",
                HeaderText = "Enabled",                
                Width = 50,
                Resizable = DataGridViewTriState.False
            };
            checkColumn.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dGWebhookSettings.Columns.Add(checkColumn);

            // Set and lock row height
            dGWebhookSettings.RowTemplate.Height = 25;
            dGWebhookSettings.RowTemplate.Resizable = DataGridViewTriState.False;
        }

        private void LoadWebhooks()
        {
            dGWebhookSettings.Rows.Clear();

            // Load config
            if (_config.Webhooks != null && _config.Webhooks.Count > 0)
            {
                foreach (var webhook in _config.Webhooks)
                {
                    int rowIndex = dGWebhookSettings.Rows.Add(
                        webhook.WebhookAddress,
                        webhook.MessageContents,
                        webhook.Enabled
                    );

                    // Lock the row's height
                    dGWebhookSettings.Rows[rowIndex].Height = 25;
                    dGWebhookSettings.Rows[rowIndex].Resizable = DataGridViewTriState.False;
                }
            }
            else
            {
                // Add 3 default empty rows if no webhooks exist
                for (int i = 0; i < 3; i++)
                {
                    int rowIndex = dGWebhookSettings.Rows.Add("", "", false);
                    dGWebhookSettings.Rows[rowIndex].Height = 25;
                    dGWebhookSettings.Rows[rowIndex].Resizable = DataGridViewTriState.False;
                }
            }
        }

        private void SaveWebhooks()
        {
            _config.Webhooks.Clear();

            foreach (DataGridViewRow row in dGWebhookSettings.Rows)
            {
                string webhookAddr = row.Cells["webhookAddress"].Value?.ToString() ?? "";
                string message = row.Cells["messageContents"].Value?.ToString() ?? "";
                bool enabled = row.Cells["enabled"].Value as bool? ?? false;

                // Only save rows that have at least a webhook address
                if (!string.IsNullOrWhiteSpace(webhookAddr))
                {
                    _config.Webhooks.Add(new WebhookData
                    {
                        WebhookAddress = webhookAddr,
                        MessageContents = message,
                        Enabled = enabled
                    });
                }
            }
        }

        private void OnClickAdd(object sender, EventArgs e)
        {
            int newRowIndex = dGWebhookSettings.Rows.Add("", "", false);

            // Lock the new row's height
            dGWebhookSettings.Rows[newRowIndex].Height = 25;
            dGWebhookSettings.Rows[newRowIndex].Resizable = DataGridViewTriState.False;
        }

        private void OnClickRemove(object sender, EventArgs e)
        {
            if (dGWebhookSettings.CurrentRow != null && dGWebhookSettings.CurrentRow.Index >= 0)
            {
                dGWebhookSettings.Rows.RemoveAt(dGWebhookSettings.CurrentRow.Index);
            }
        }

        private void OnClickSave(object sender, EventArgs e)
        {
            SaveWebhooks();
            DialogResult = DialogResult.OK;
            Close();
        }

        private void OnClickCancel(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}