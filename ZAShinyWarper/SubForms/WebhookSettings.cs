using System.Net.Http.Json;
using System.Reflection;
using System.Text.Json;
using PKHeX.Core;

namespace ZAShinyWarper
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

        private void OnClickTest(object sender, EventArgs e)
        {
            if (dGWebhookSettings.CurrentRow != null && dGWebhookSettings.CurrentRow.Index >= 0)
            {
                string webhookAddr = dGWebhookSettings.CurrentRow.Cells["webhookAddress"].Value?.ToString() ?? "";
                string message = dGWebhookSettings.CurrentRow.Cells["messageContents"].Value?.ToString() ?? "";
                if (string.IsNullOrWhiteSpace(webhookAddr))
                {
                    MessageBox.Show("Please enter a valid webhook address to test.", "Invalid Webhook", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                try
                {
                    var pk = LoadPA9();
                    using HttpClient client = new();
                    string imageUrl = $"https://raw.githubusercontent.com/Omni-KingZeno/Pokemon-Sprites/refs/heads/main/Shiny/zygarde-10.png";

                    var embed = new
                    {
                        title = $"Shiny Alpha Zygarde Found!?!?",
                        description = ShowdownParsing.GetShowdownText(pk),
                        image = new { url = imageUrl.Trim() },
                        footer = new
                        {
                            text = "This is only a test",
                        }
                    };
                    var payload = new
                    {
                        content = message,
                        embeds = new[] { embed }
                    };
                    var json = JsonSerializer.Serialize(payload);


                    var response = client.PostAsJsonAsync(webhookAddr, payload).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        MessageBox.Show("Test webhook sent successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show($"Failed to send test webhook. Status Code: {response.StatusCode}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred while sending the test webhook:\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        public static PA9 LoadPA9()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "ZAShinyWarper.Resources.test.pa9";
            using Stream stream = assembly.GetManifestResourceStream(resourceName) ?? throw new FileNotFoundException($"Resource {resourceName} not found");
            byte[] data = new byte[stream.Length];
            stream.ReadExactly(data);
            return new PA9(data);
        }
    }
}