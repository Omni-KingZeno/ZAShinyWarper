using System.Globalization;
using System.Text;
using System.Text.Json;
using PKHeX.Core;
using PKHeX.Drawing.PokeSprite;
using ZAShinyWarper.helpers;
using ZAShinyWarper.Hunting;
using ZAShinyWarper.Injection;

namespace ZAShinyWarper
{
    public partial class ZAWarpWindow : Form
    {
        private readonly long[] jumpsPos = [0x41EC340, 0x248, 0x00, 0x138]; // [[[[main+41EC340]+248]+00]+138]+90
        private static IRAMReadWriter bot = default!;

        private List<Vector3> positions = [];
        private const string Config = "config.json";
        private static readonly JsonSerializerOptions jsonOptions = new() { WriteIndented = true };

        private ProgramConfig programConfig = new();

        private readonly ShinyHunter<PA9> shinyHunter = new();
        private readonly List<PictureBox> StashList;
        private readonly HttpClient httpClient = new();
        private readonly WarpProgressForm warpProgress = new();
        private ContextMenuStrip? shinyContextMenu;

        public ComboBox[] CBIVs = default!;

        // Bot
        private bool matchingShinyFound = false;
        private bool warping = false;
        private int currentWarps = 0;

        public ZAWarpWindow()
        {
            InitializeComponent();
            SetupListBox();
            CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
            Application.ApplicationExit += (s, e) => CleanUpBot();
            SpriteName.AllowShinySprite = true;
            StashList = [StashedShiny1, StashedShiny2, StashedShiny3, StashedShiny4, StashedShiny5, StashedShiny6, StashedShiny7, StashedShiny8, StashedShiny9, StashedShiny10];
        }

        private void LoadDefaults(object sender, EventArgs e)
        {
            // Load enums into comboboxes
            // WhenShinyFound
            foreach (var item in Enum.GetValues<ShinyFoundAction>())
                cBWhenShinyFound.Items.Add(item);
            cBWhenShinyFound.SelectedIndex = 0;

            // Forced Weather
            foreach (var item in Enum.GetValues<Weather>())
                cBForcedWeather.Items.Add(item);
            cBForcedWeather.SelectedIndex = 8;

            // Forced Time of Day
            foreach (var item in Enum.GetValues<TimeOfDay>())
                cBForcedTimeOfDay.Items.Add(item);
            cBForcedTimeOfDay.SelectedIndex = 4;

            // Species
            cBSpecies.Items.Add("Any");
            foreach (var item in ZAZukan.PokedexNumbersZA)
                cBSpecies.Items.Add($"{item.Value:D3} - {item.Key}");
            cBSpecies.SetItemChecked(0, true);

            // IVs
            CBIVs =
            [
                cBIVHP,
                cBIVAtk,
                cBIVDef,
                cBIVSpA,
                cBIVSpD,
                cBIVSpe
            ];

            foreach (var cb in CBIVs)
            {
                foreach (var item in Enum.GetValues<IVType>())
                    cb.Items.Add(item);
                cb.SelectedIndex = 0;
            }

            LoadConfig();
            LoadAllAndUpdateUI();
        }

        private ShinyFilter<PA9> GetFilter()
        {
            var filter = new ShinyFilter<PA9>();
            // Species - collect all checked species
            var checkedSpecies = new List<ushort>();
            for (int i = 0; i < cBSpecies.Items.Count; i++)
            {
                if (cBSpecies.GetItemChecked(i) && i > 0) // Skip "Any" at index 0
                {
                    var sel = cBSpecies.Items[i].ToString()!;
                    var spl = sel.Split(" - ");
                    checkedSpecies.Add(ushort.Parse(spl[0]));
                }
            }

            // If "Any" is checked or no species selected, leave filter.Species as null
            // Otherwise, set the list of species
            if (checkedSpecies.Count > 0)
                filter.SpeciesList = checkedSpecies;

            // IVs
            for (int i = 0; i < 6; i++)
            {
                filter.IVs[i] = (IVType)CBIVs[i].SelectedItem!;
            }

            // Size
            filter.SizeMinimum = (byte)nUDScaleMin.Value;
            filter.SizeMaximum = (byte)nUDScaleMax.Value;
            // Alpha
            filter.IsAlpha = cBIsAlpha.Checked;
            return filter;
        }

        private void OnClickConnect(object sender, EventArgs e)
        {
            if (bot != null && bot.Connected)
            {
                if (warping)
                {
                    warping = false;
                    SetFiltersEnableState(true);
                    btnWarp.Text = "Start Warping";
                }

                SetUIEnableState(true, false);
                CleanUpBot();
                ResetSprites();
                bot = default!;
            }
            else
            {
                try
                {
                    var botsys = new SysBot();
                    botsys.Connect(tB_IP.Text, 6000);
                    bot = botsys;

                    SetUIEnableState(true, true);
                    shinyHunter.LoadStashedShinies(bot);
                    DisplayStashedShinies();
                    DisplayStashedMessageBox(true);
                    bot.SendBytes(Encoding.ASCII.GetBytes("detachController\r\n"));
                    CleanUpBot();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void OnClickConnectUSB(object sender, EventArgs e)
        {
            if (bot != null && bot.Connected)
            {
                if (warping)
                {
                    warping = false;
                    SetFiltersEnableState(true);
                    btnWarp.Text = "Start Warping";
                }

                SetUIEnableState(false, false);
                CleanUpBot();
                ResetSprites();
                bot = default!;
            }
            else
            {
                try
                {
                    var botusb = new USBBot();
                    botusb.Connect();
                    bot = botusb;

                    SetUIEnableState(false, true);
                    shinyHunter.LoadStashedShinies(bot);
                    DisplayStashedShinies();
                    DisplayStashedMessageBox(true);
                    bot.SendBytes(Encoding.ASCII.GetBytes("detachController\r\n"));
                    CleanUpBot();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void DisplayStashedMessageBox(bool wifi)
        {
            var form = new Form
            {
                Text = "Connected",
                Width = 250,
                Height = 175,
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false,
                ControlBox = false
            };

            var headerLabel = new Label
            {
                Text = $"SysBot Connected\r\n({(wifi ? "Network" : "USB")})",
                Font = new Font("Segoe UI", 12),
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Top,
                Height = 65,
                Padding = new Padding(10)
            };
            var count = shinyHunter.StashedShinies.Count;
            var countLabel = new Label
            {
                Text = $"{(count == 0 ? "No Shinies found in your stash." : $"{count} Shin{(count == 1 ? "y" : "ies")} found in your stash!")}",
                Font = new Font("Segoe UI", 10),
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill
            };

            var okButton = new Button
            {
                Text = "OK",
                Width = 80,
                Height = 30,
                Dock = DockStyle.Bottom,
                Margin = new Padding(10)
            };
            okButton.Click += (s, e) => form.Close();

            form.Controls.Add(countLabel);
            form.Controls.Add(headerLabel);
            form.Controls.Add(okButton);
            form.ShowDialog();
        }

        private void OnClickScreenOn(object sender, EventArgs e)
        {
            if (bot != null && bot.Connected)
            {
                try
                {
                    bot.SendBytes(Encoding.ASCII.GetBytes("screenOn\r\n"));
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void OnClickScreenOff(object sender, EventArgs e)
        {
            if (bot != null && bot.Connected)
            {
                try
                {
                    bot.SendBytes(Encoding.ASCII.GetBytes("screenOff\r\n"));
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void OnClickForwards(object sender, EventArgs e)
        {
            MovePlayer(0, 1);
        }

        private void OnClickBackwards(object sender, EventArgs e)
        {
            MovePlayer(0, -1);
        }

        private void OnClickLeft(object sender, EventArgs e)
        {
            MovePlayer(1, 0);
        }

        private void OnClickRight(object sender, EventArgs e)
        {
            MovePlayer(-1, 0);
        }

        private void OnClickMoveItemUp(object sender, EventArgs e)
        {
            if (lBCoords.SelectedIndex > 0)
            {
                int index = lBCoords.SelectedIndex;
                (positions[index - 1], positions[index]) = (positions[index], positions[index - 1]);
                SaveAllAndUpdateUI();
                lBCoords.SelectedIndex = index - 1;
            }
        }

        private void OnClickMoveItemDown(object sender, EventArgs e)
        {
            if (lBCoords.SelectedIndex >= 0 &&
                lBCoords.SelectedIndex < lBCoords.Items.Count - 1)
            {
                int index = lBCoords.SelectedIndex;
                (positions[index + 1], positions[index]) = (positions[index], positions[index + 1]);
                SaveAllAndUpdateUI();
                lBCoords.SelectedIndex = index + 1;
            }
        }

        private void OnClickUp(object sender, EventArgs e)
        {
            MovePlayerZ();
        }

        private void OnClickSave(object sender, EventArgs e)
        {
            var pos = GetPlayerPosition();
            positions.Add(pos);
            SaveAllAndUpdateUI();

            lBCoords.SelectedIndex = lBCoords.Items.Count - 1;
        }

        private async void OnClickRestore(object sender, EventArgs e)
        {
            if (lBCoords.SelectedIndex > -1 && lBCoords.SelectedItem != null)
            {
                SetWarpingEnableState(false); // Disable warping inputs

                var toSend = (Vector3)lBCoords.SelectedItem;
                SetPlayerPosition(toSend.X, toSend.Y, toSend.Z);

                warpProgress.PerformSafely(() =>
                {
                    CenterFormOnParent(warpProgress);
                    warpProgress.Show();
                });
                await Task.Delay(250); // Let the form render correctly

                for (int i = 0; i < 15; ++i)
                {
                    int currentAttempt = i + 1;
                    if (GetPlayerPosition().Y >= toSend.Y - 0.02f && GetPlayerPosition().Y <= toSend.Y + 0.02f)
                        break;

                    if (i == 0)
                        await Task.Delay(4000); // we might be falling, wait longer first attempt
                    else
                        warpProgress.PerformSafely(() => warpProgress.SetText($"Reattempting to Warp ({currentAttempt}/15) Please wait."));

                    SetPlayerPosition(toSend.X, toSend.Y, toSend.Z);
                    await Task.Delay(1100).ConfigureAwait(false);

                    if (i == 14 && GetPlayerPosition().Y != toSend.Y)
                    {
                        warpProgress.PerformSafely(() => warpProgress.SetText("Warp failure. Please retry"));
                        await Task.Delay(1000).ConfigureAwait(false);
                    }
                }

                warpProgress.PerformSafely(() => warpProgress.Hide());
                warpProgress.PerformSafely(() => warpProgress.SetText("Warping..."));

                SetWarpingEnableState(true); // Enable warping inputs
            }
        }

        private void OnClickExport(object sender, EventArgs e)
        {
            if (lBCoords.Items.Count == 0)
            {
                MessageBox.Show("No items to export.", "Export",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using SaveFileDialog saveDialog = new();
            saveDialog.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
            saveDialog.DefaultExt = "txt";
            saveDialog.FileName = "coordinates.txt";

            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    using (StreamWriter writer = new(saveDialog.FileName))
                    {
                        foreach (var item in lBCoords.Items)
                        {
                            writer.WriteLine(item.ToString());
                        }
                    }
                    MessageBox.Show($"Successfully exported {lBCoords.Items.Count} items.",
                        "Export Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error exporting items: {ex.Message}",
                        "Export Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void OnClickImport(object sender, EventArgs e)
        {
            using OpenFileDialog openDialog = new();
            openDialog.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
            openDialog.DefaultExt = "txt";
            if (openDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    string[] lines = File.ReadAllLines(openDialog.FileName);

                    bool shouldClear = true; // Likely to always clear

                    // Only ask if there are existing items
                    if (positions.Count > 0)
                    {
                        DialogResult result = MessageBox.Show(this,
                            "Do you want to overwrite the current coordinates?\n\n" +
                            "Yes = Overwrite existing\n" +
                            "No = Add to existing\n",
                            "Import Options",
                            MessageBoxButtons.YesNoCancel,
                            MessageBoxIcon.Question);

                        if (result == DialogResult.Cancel)
                        {
                            return; // User cancelled
                        }

                        shouldClear = (result == DialogResult.Yes);
                    }

                    // If overwrite, clear existing data
                    if (shouldClear)
                    {
                        lBCoords.Items.Clear();
                        positions.Clear();
                    }

                    // Import the data
                    foreach (string line in lines)
                    {
                        if (!string.IsNullOrWhiteSpace(line))
                        {
                            Vector3 vector = Vector3.FromString(line);
                            lBCoords.Items.Add(vector);
                            positions.Add(vector);
                        }
                    }

                    string action = shouldClear ? "imported" : "added";
                    MessageBox.Show($"Successfully {action} {lines.Length} items.",
                        "Import Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error importing items: {ex.Message}",
                        "Import Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            SaveAllAndUpdateUI();
        }

        private void CenterFormOnParent(Form childForm)
        {
            childForm.StartPosition = FormStartPosition.Manual;
            int x = Location.X + (Width - childForm.Width) / 2;
            int y = Location.Y + (Height - childForm.Height) / 2;
            childForm.Location = new Point(x, y);
        }

        private void OnClickDelete(object sender, EventArgs e)
        {
            if (lBCoords.SelectedIndex > -1)
            {
                var removedIndex = lBCoords.SelectedIndex;
                positions.RemoveAt(lBCoords.SelectedIndex);
                SaveAllAndUpdateUI();

                if (removedIndex >= lBCoords.Items.Count)
                    lBCoords.SelectedIndex = lBCoords.Items.Count - 1;
                else
                    lBCoords.SelectedIndex = removedIndex;
            }
        }

        private void MovePlayer(float x, float y)
        {
            int stepOffset = (int)nUDDistance.Value;
            ulong ramOffset = GetPlayerCoordinatesOffset();

            var bytes = bot.ReadBytes(ramOffset, 12, RWMethod.Absolute);
            float xn = BitConverter.ToSingle(bytes, 0);
            float yn = BitConverter.ToSingle(bytes, 8);
            xn += (x * stepOffset); yn += (y * stepOffset);

            bot.WriteBytes(BitConverter.GetBytes(xn), ramOffset, RWMethod.Absolute);
            bot.WriteBytes(BitConverter.GetBytes(yn), ramOffset + 8, RWMethod.Absolute);
        }

        private void MovePlayerZ()
        {
            int stepOffset = (int)nUDDistance.Value;
            ulong ramOffset = GetPlayerCoordinatesOffset();

            var bytes = bot.ReadBytes(ramOffset, 12, RWMethod.Absolute);
            float zn = BitConverter.ToSingle(bytes, 4);
            zn += stepOffset;

            bot.WriteBytes(BitConverter.GetBytes(zn), ramOffset + 4, RWMethod.Absolute);
        }

        private Vector3 GetPlayerPosition()
        {
            ulong ramOffset = GetPlayerCoordinatesOffset();
            var bytes = bot.ReadBytes(ramOffset, 12, RWMethod.Absolute);

            float xn = BitConverter.ToSingle(bytes, 0);
            float yn = BitConverter.ToSingle(bytes, 4);
            float zn = BitConverter.ToSingle(bytes, 8);

            return new Vector3() { X = xn, Y = yn, Z = zn };
        }

        private void SetPlayerPosition(float x, float y, float z)
        {
            ulong ramOffset = GetPlayerCoordinatesOffset();

            byte[] xb = BitConverter.GetBytes(x);
            byte[] yb = BitConverter.GetBytes(y);
            byte[] zb = BitConverter.GetBytes(z);

            var bytes = xb.Concat(yb).Concat(zb);

            bot.WriteBytes(bytes.ToArray(), ramOffset, RWMethod.Absolute);
        }

        private ulong GetPlayerCoordinatesOffset()
        {
            return bot.FollowMainPointer(jumpsPos) + 0x90;
        }

        private void OnClickReset(object sender, EventArgs e)
        {
            // Uncheck all items
            for (int i = 0; i < cBSpecies.Items.Count; i++)
            {
                cBSpecies.SetItemChecked(i, false);
            }
            // Check "Any" (index 0)
            cBSpecies.SetItemChecked(0, true);
            SaveConfig();
        }

        private void OnClickResetFilters(object sender, EventArgs e)
        {
            // Reset all filters to default
            for (int i = 0; i < cBSpecies.Items.Count; i++)
            {
                cBSpecies.SetItemChecked(i, false);
            }
            cBSpecies.SetItemChecked(0, true);
            foreach (var cb in CBIVs)
            {
                cb.SelectedIndex = 0;
            }
            nUDScaleMin.Value = 0;
            nUDScaleMax.Value = 255;
            SaveConfig();
        }

        private void LoadAllAndUpdateUI()
        {
            LoadConfig();
            UpdateUI();
        }

        private void SaveAllAndUpdateUI()
        {
            SaveConfig();
            UpdateUI();
        }

        private void UpdateUI()
        {
            lBCoords.SelectedIndex = -1;
            lBCoords.Items.Clear();
            foreach (var pos in positions)
                lBCoords.Items.Add(pos);
        }

        private void OnConfigurationChange(object sender, EventArgs e)
        {
            // Save config when any configuration changes
            // Use BeginInvoke to ensure the change is applied before saving
            BeginInvoke(new Action(() => SaveConfig()));
        }

        private void OnAlphaCheckedChanged(object? sender, EventArgs e)
        {
            if (cBIsAlpha.Checked)
            {
                nUDScaleMin.Value = 255;
                nUDScaleMin.Enabled = false;
                nUDScaleMax.Value = 255;
                nUDScaleMax.Enabled = false;
            }
            else
            {
                nUDScaleMin.Enabled = true;
                nUDScaleMin.Value = 0;
                nUDScaleMax.Enabled = true;
                nUDScaleMax.Value = 255;
            }

            OnConfigurationChange(sender, e);
        }

        private void SaveConfig()
        {
            try
            {
                programConfig.IPAddress = tB_IP.Text;
                programConfig.Positions = [.. positions];
                programConfig.SpawnCheckTime = nUDCheckTime.Value;
                programConfig.CamMove = nUDCamMove.Value;
                programConfig.SaveFreq = nUDSaveFreq.Value;
                programConfig.WhenShinyFound = cBWhenShinyFound.SelectedIndex;
                programConfig.ForcedWeather = cBForcedWeather.SelectedIndex;
                programConfig.ForcedTimeOfDay = cBForcedTimeOfDay.SelectedIndex;
                programConfig.IVHP = cBIVHP.SelectedIndex;
                programConfig.IVAtk = cBIVAtk.SelectedIndex;
                programConfig.IVDef = cBIVDef.SelectedIndex;
                programConfig.IVSpA = cBIVSpA.SelectedIndex;
                programConfig.IVSpD = cBIVSpD.SelectedIndex;
                programConfig.IVSpe = cBIVSpe.SelectedIndex;
                programConfig.IsAlpha = cBIsAlpha.Checked;
                programConfig.ScaleMin = nUDScaleMin.Value;
                programConfig.ScaleMax = nUDScaleMax.Value;

                // Species
                programConfig.SpeciesIndices.Clear();
                for (int i = 0; i < cBSpecies.Items.Count; i++)
                {
                    if (cBSpecies.GetItemChecked(i))
                        programConfig.SpeciesIndices.Add(i);
                }


                var json = JsonSerializer.Serialize(programConfig, jsonOptions);
                File.WriteAllText(Config, json);
            }
            catch
            {
                // Silently fail if we can't save config
            }
        }

        private void LoadConfig()
        {
            try
            {
                if (!File.Exists(Config))
                {
                    if (File.Exists("positions.txt") || File.Exists("filter_config.txt"))
                        LegacyImport();
                    else
                        return;
                }

                var json = File.ReadAllText(Config);
                var config = JsonSerializer.Deserialize<ProgramConfig>(json);

                if (config == null)
                    return;

                programConfig = config;

                // Load all filter settings
                for (int i = 0; i < cBSpecies.Items.Count; i++)
                {
                    cBSpecies.SetItemChecked(i, false);
                }

                foreach (var idx in programConfig.SpeciesIndices)
                {
                    if (idx < cBSpecies.Items.Count)
                        cBSpecies.SetItemChecked(idx, true);
                }

                tB_IP.Text = programConfig.IPAddress;
                positions = [.. programConfig.Positions];
                cBWhenShinyFound.SelectedIndex = programConfig.WhenShinyFound;
                cBForcedWeather.SelectedIndex = programConfig.ForcedWeather;
                cBForcedTimeOfDay.SelectedIndex = programConfig.ForcedTimeOfDay;
                nUDCheckTime.Value = programConfig.SpawnCheckTime;
                nUDCamMove.Value = programConfig.CamMove;
                nUDSaveFreq.Value = programConfig.SaveFreq;
                cBIVHP.SelectedIndex = programConfig.IVHP;
                cBIVAtk.SelectedIndex = programConfig.IVAtk;
                cBIVDef.SelectedIndex = programConfig.IVDef;
                cBIVSpA.SelectedIndex = programConfig.IVSpA;
                cBIVSpD.SelectedIndex = programConfig.IVSpD;
                cBIVSpe.SelectedIndex = programConfig.IVSpe;
                nUDScaleMin.Value = programConfig.ScaleMin;
                nUDScaleMax.Value = programConfig.ScaleMax;
                cBIsAlpha.Checked = programConfig.IsAlpha;
            }
            catch
            {
                // If loading fails, just use defaults
            }
        }

        private void SetUIEnableState(bool wifi, bool enabled)
        {
            gBControls.Enabled = enabled;
            gBShinyHunt.Enabled = enabled;
            gBStashedShiny.Enabled = enabled;
            btnScreenOn.Enabled = enabled;
            btnScreenOff.Enabled = enabled;
            btnWebhookSettings.Enabled = enabled;
            btnWarp.Enabled = enabled;
            btnResetFilters.Enabled = enabled;
            btnExportSets.Enabled = enabled;

            if (wifi)
            {
                btnConnect.Text = enabled ? "Disconnect" : "Connect";
                btnConnect.Enabled = true;
                btnConnectUSB.Enabled = !enabled;
            }
            else
            {
                btnConnectUSB.Text = enabled ? "Disconnect USB" : "Connect USB";
                btnConnectUSB.Enabled = true;
                btnConnect.Enabled = !enabled;
            }
        }

        private void SetFiltersEnableState(bool enabled)
        {
            cBSpecies.PerformSafely(() => cBSpecies.Enabled = enabled);
            foreach (var cb in CBIVs)
                cb.PerformSafely(() => cb.Enabled = enabled);
            cBWhenShinyFound.PerformSafely(() => cBWhenShinyFound.Enabled = enabled);
            cBForcedWeather.PerformSafely(() => cBForcedWeather.Enabled = enabled);
            cBForcedTimeOfDay.PerformSafely(() => cBForcedTimeOfDay.Enabled = enabled);
            nUDCheckTime.PerformSafely(() => nUDCheckTime.Enabled = enabled);
            nUDCamMove.PerformSafely(() => nUDCamMove.Enabled = enabled);
            nUDSaveFreq.PerformSafely(() => nUDSaveFreq.Enabled = enabled);
            gBControls.PerformSafely(() => gBControls.Enabled = enabled);
            btnResetSpecies.PerformSafely(() => btnResetSpecies.Enabled = enabled);
            cBIsAlpha.PerformSafely(() => cBIsAlpha.Enabled = enabled);
            if (cBIsAlpha.Checked)
            {
                nUDScaleMin.PerformSafely(() => nUDScaleMin.Enabled = false);
                nUDScaleMax.PerformSafely(() => nUDScaleMax.Enabled = false);
            }
            else
            {
                nUDScaleMin.PerformSafely(() => nUDScaleMin.Enabled = enabled);
                nUDScaleMax.PerformSafely(() => nUDScaleMax.Enabled = enabled);
            }
            btnResetFilters.PerformSafely(() => btnResetFilters.Enabled = enabled);
        }

        private void SetWarpingEnableState(bool enabled)
        {
            btnRestore.PerformSafely(() => btnRestore.Enabled = enabled);
            btnSave.PerformSafely(() => btnSave.Enabled = enabled);
            btnDelete.PerformSafely(() => btnDelete.Enabled = enabled);
            btnWarp.PerformSafely(() => btnWarp.Enabled = enabled);
        }

        private void CleanUpBot()
        {
            if (bot != null && bot.Connected)
            {
                shinyHunter.SetWeather(bot, Weather.None);
                shinyHunter.SetTime(bot, TimeOfDay.None);
                bot.SendBytes(Encoding.ASCII.GetBytes("setStick RIGHT 0 0\r\n"));
                bot.SendBytes(Encoding.ASCII.GetBytes("detachController\r\n"));
            }
        }

        private static async Task SaveGame()
        {
            bot.SendBytes(Encoding.ASCII.GetBytes("click X\r\n"));
            await Task.Delay(1_000).ConfigureAwait(false);
            bot.SendBytes(Encoding.ASCII.GetBytes("click R\r\n"));
            await Task.Delay(1_000).ConfigureAwait(false);
            bot.SendBytes(Encoding.ASCII.GetBytes("click A\r\n"));
            await Task.Delay(5_000).ConfigureAwait(false); // wait for save
            bot.SendBytes(Encoding.ASCII.GetBytes("click B\r\n"));
            await Task.Delay(0_800).ConfigureAwait(false);
            bot.SendBytes(Encoding.ASCII.GetBytes("click B\r\n"));
            await Task.Delay(0_800).ConfigureAwait(false);
        }

        private async void OnClickWarp(object sender, EventArgs e)
        {
            if (warping)
            {
                warping = false;
                SetFiltersEnableState(true);
                btnWarp.PerformSafely(() => btnWarp.Text = "Start Warping");
                shinyHunter.UnlockWeather();
                shinyHunter.UnlockTime();
                CleanUpBot();
                return;
            }

            if (positions.Count < 2)
            {
                MessageBox.Show("A minimum of 2 warp positions is required!");
                return;
            }

            var filter = GetFilter();
            var warpInterval = (int)nUDCheckTime.Value;
            var camSpeed = (int)nUDCamMove.Value;
            var action = (ShinyFoundAction)cBWhenShinyFound.SelectedItem!;
            var saveFrequency = (int)nUDSaveFreq.Value;

            currentWarps = 0;

            warping = true;
            matchingShinyFound = false;
            SetFiltersEnableState(false);
            btnWarp.PerformSafely(() => btnWarp.Text = "Warping. Click to end.");

            // Set Time and Weather
            shinyHunter.SetTime(bot, (TimeOfDay)cBForcedTimeOfDay.SelectedItem!);
            shinyHunter.SetWeather(bot, (Weather)cBForcedWeather.SelectedItem!);

            // Rotate camera for spawns
            if (camSpeed != 0)
                bot.SendBytes(Encoding.ASCII.GetBytes($"setStick RIGHT {camSpeed} 0\r\n"));

            // Refresh stashed shinies
            _ = shinyHunter.LoadStashedShinies(bot);
            DisplayStashedShinies();

            while (warping)
            {
                currentWarps++;
                if (currentWarps % saveFrequency == 0)
                    await SaveGame().ConfigureAwait(false);

                // Check shinies first as a new one may have spawned before we move
                var newFound = shinyHunter.LoadStashedShinies(bot);
                var cacheIsFull = shinyHunter.StashedShinies.Count == 10;

                if (newFound)
                {
                    DisplayStashedShinies();
                    var newShinies = shinyHunter.DifferentShinies;
                    foreach (var pk in newShinies)
                    {
                        var matchesFilter = filter.MatchesFilter(pk.PKM);

                        if (matchesFilter)
                        {
                            matchingShinyFound = true;
                        }

                        var shouldStop = false;
                        var shouldSendEmbed = true;
                        var stopMessage = string.Empty;

                        if (matchesFilter && action == ShinyFoundAction.StopOnFound) // Found what we wanted. Stop warping, go catch it
                        {
                            shouldStop = true;
                            stopMessage = $"A Shiny {(pk.PKM.IsAlpha ? "Alpha " : "")}matching the filter has been found after {currentWarps} attempts!\r\nStopping warping.\r\n\r\n{pk}\r\n";
                        }
                        else if (matchesFilter && action == ShinyFoundAction.ClearCacheAndContinue) // Found what we wanted, keep going
                        {
                            MessageBox.Show($"We Found A Match after {currentWarps} attempts! Let's keep going to find more!\r\n\r\n{pk}\r\n", "Found!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                        else if (matchesFilter && cacheIsFull && action != ShinyFoundAction.StopOnFound) // Found what we wanted in this run and the cache is full
                        {
                            shouldStop = true;
                            stopMessage = $"A Shiny {(pk.PKM.IsAlpha ? "Alpha " : "")}matching the filter has been found after {currentWarps} attempts and your stash is now full!\r\nStopping warping.\r\n\r\n{pk}\r\n";
                        }
                        else if (!matchesFilter && action == ShinyFoundAction.ClearCacheAndContinue) // Does not match filter, clear it out
                        {
                            shouldSendEmbed = false;
                            int index = shinyHunter.StashedShinies.IndexOf(pk);
                            shinyHunter.RemoveShinyFromCache(bot, index); // keep removing until we have a whole cache of matches
                            shinyHunter.StashedShinies.RemoveAt(index);
                            StashList[index].PerformSafely(() => StashList[index].Image = null);
                            DisplayStashedShinies();
                        }
                        else if (!matchesFilter && cacheIsFull) // Does not match filter but the cache is full
                        {
                            if (action == ShinyFoundAction.StopAtFullCache) // Maybe something worth catching not being filtered for
                            {
                                shouldStop = true;
                                stopMessage = matchingShinyFound
                                    ? "A shiny matching your filter was found earlier in the hunt.\r\nYour shiny cache is now full!\r\nStopping warping to preserve filtered match."
                                    : "No shiny matching your filter was found.\r\nYour shiny cache is now full!\r\nStopping warping.";
                            }
                            else if (action == ShinyFoundAction.CacheAndContinue && matchingShinyFound) // Most recent does not match filter but the cache is now full and a filter match was found at an earlier point
                            {
                                shouldStop = true;
                                stopMessage = "A shiny matching your filter was found earlier in the hunt.\r\nYour shiny cache is now full!\r\nStopping to preserve your matching shiny.";
                            }
                            else // Cache is full but no match found. Keep going
                            {
                                ShowUnwantedShinyMessage(pk);
                            }
                        }
                        else if (!matchesFilter) // No match, still room in cache
                        {
                            ShowUnwantedShinyMessage(pk);
                        }

                        if (shouldStop)
                        {
                            StopWarping(stopMessage);
                        }

                        if (shouldSendEmbed)
                            await SendWebhook(pk.ToShowdownString(), pk.PKM);
                    }

                    // Helper methods
                    void StopWarping(string message)
                    {
                        warping = false;
                        CleanUpBot();
                        bot.SendBytes(Encoding.ASCII.GetBytes("click X\r\n"));
                        btnWarp.PerformSafely(() => btnWarp.Text = "Start Warping");
                        SetFiltersEnableState(true);
                        MessageBox.Show(message, cacheIsFull ? "Cache Full!" : "Found!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }

                    void ShowUnwantedShinyMessage(StashedShiny<PA9> pk)
                    {
                        CrossThreadExtensions.DoThreaded(() =>
                        {
                            MessageBox.Show($"The following Shiny {(pk.PKM.IsAlpha ? "Alpha " : "")} has been found, but does not match your filter.\r\nYou may wish to remove it such that it doesn't occupy one of your shiny stash slots.\r\n\r\n{pk}\r\n",
                                "Found something we don't want!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        });
                    }
                }

                foreach (var pos in positions)
                {
                    if (!warping)
                        break;
                    SetPlayerPosition(pos.X, pos.Y, pos.Z);
                    await Task.Delay(1_000).ConfigureAwait(false); // fall out and load species
                    // handle falling out
                    int tries = 25;
                    for (; tries > 0; --tries)
                    {
                        // check for less than 0.02 difference to avoid float precision issues. We only care about Y here as X/Z may vary due to terrain
                        if (GetPlayerPosition().Y >= pos.Y - 0.02f && GetPlayerPosition().Y <= pos.Y + 0.02f)
                            break;
                        SetPlayerPosition(pos.X, pos.Y + (tries > 20 ? 1 : 0), pos.Z);
                        await Task.Delay(1_200).ConfigureAwait(false);
                    }

                    if (tries == 0) // failed to load
                    {
                        warping = false;
                        CleanUpBot();
                        btnWarp.PerformSafely(() => btnWarp.Text = "Start Warping");
                        SetFiltersEnableState(true);
                        MessageBox.Show($"Warping has failed, please check the console!");
                        break;
                    }

                    if (pos.Flags.Contains("instant"))
                        continue;

                    if (pos.Flags.Contains("halfwait"))
                        await Task.Delay(warpInterval / 2).ConfigureAwait(false);
                    else
                        await Task.Delay(warpInterval).ConfigureAwait(false);
                }
            }
        }

        private void OnSpeciesSelectedIndexChange(object sender, EventArgs e)
        {
            // Select "Any" if none are selected
            bool anyChecked = false;
            for (int i = 0; i < cBSpecies.Items.Count; i++)
            {
                if (cBSpecies.GetItemChecked(i))
                {
                    anyChecked = true;
                    break;
                }
            }
            if (!anyChecked)
            {
                cBSpecies.SetItemChecked(0, true);
                return;
            }
            // Clear everything else if "Any" is selected
            if (cBSpecies.SelectedIndex == 0)
            {
                for (int i = 1; i < cBSpecies.Items.Count; i++)
                {
                    cBSpecies.SetItemChecked(i, false);
                }
            }
            else // Clear "Any" if anything else is selected
            {
                cBSpecies.SetItemChecked(0, false);
            }
        }

        private void OnMouseHover(object sender, EventArgs e)
        {
            PictureBox pb = (PictureBox)sender;
            pb.BorderStyle = BorderStyle.FixedSingle;
            ShinyInfo.Hide(pb);
            ShinyInfo.Active = false;
            ShinyInfo.Active = true;
            int index = StashList.IndexOf(pb);
            if (index >= 0 && index < shinyHunter.StashedShinies.Count)
            {
                pb.ContextMenuStrip = shinyContextMenu; // Enable context menu
                ShinyInfo.SetToolTip(pb, shinyHunter.StashedShinies[index].ToString());
            }
            else
            {
                pb.ContextMenuStrip = null; // Disable context menu for empty slots
                ShinyInfo.SetToolTip(pb, "No shiny data");
            }
        }

        private void ResetSprites()
        {
            foreach (var pb in StashList)
            {
                pb.PerformSafely(() => pb.Image = null);
            }
        }

        private void DisplayStashedShinies()
        {
            // Initialize context menu
            if (shinyContextMenu == null)
            {
                InitializeShinyContextMenu();
            }

            ResetSprites();
            for (int i = 0; i < shinyHunter.StashedShinies.Count; i++)
            {
                var pk = shinyHunter.StashedShinies[i].PKM;
                var img = pk.Sprite();
                StashList[i].PerformSafely(() => StashList[i].Image = img);
            }

            // Clear remaining slots if the count changed
            for (int i = shinyHunter.StashedShinies.Count; i < 10; i++)
            {
                StashList[i].PerformSafely(() => StashList[i].Image = null);
            }
        }

        private void InitializeShinyContextMenu()
        {
            shinyContextMenu = new ContextMenuStrip();
            var clearItem = new ToolStripMenuItem("Clear from Stash");
            clearItem.Click += ClearShinyFromStash;
            shinyContextMenu.Items.Add(clearItem);

            var clearAllItems = new ToolStripMenuItem("Clear ALL from Stash");
            clearAllItems.Click += ClearStash;
            shinyContextMenu.Items.Add(clearAllItems);

            // Attach the context menu to all picture boxes
            foreach (var pb in StashList)
            {
                pb.ContextMenuStrip = shinyContextMenu;
            }
        }

        private void ClearShinyFromStash(object? sender, EventArgs e)
        {
            // Get the picture box that was right-clicked
            if (sender is not ToolStripMenuItem menuItem)
                return;
            if (menuItem.Owner is not ContextMenuStrip contextMenu)
                return;
            if (contextMenu.SourceControl is not PictureBox pb)
                return;

            int index = StashList.IndexOf(pb);
            if (index >= 0 && index < shinyHunter.StashedShinies.Count)
            {
                var shiny = shinyHunter.StashedShinies[index];

                // Confirm with the user
                var result = MessageBox.Show(
                    $"Are you sure you want to clear {shiny.PKM.Nickname} from the stash?\n\nThis will despawn the Pokémon in-game.",
                    "Confirm Clear",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    try
                    {
                        // Call the despawn method
                        shinyHunter.RemoveShinyFromCache(bot, index);

                        // Reload the stashed shinies
                        shinyHunter.LoadStashedShinies(bot);

                        // Update the display
                        DisplayStashedShinies();

                        MessageBox.Show(
                            $"{shiny.PKM.Nickname} has been cleared from the stash.",
                            "Success",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(
                            $"Error clearing shiny from stash: {ex.Message}",
                            "Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void ClearStash(object? sender, EventArgs e)
        {
            // Get the picture box that was right-clicked
            if (sender is not ToolStripMenuItem menuItem)
                return;
            if (menuItem.Owner is not ContextMenuStrip contextMenu)
                return;
            if (contextMenu.SourceControl is not PictureBox)
                return;

            if (shinyHunter.StashedShinies.Count != 0)
            {
                // Confirm with the user
                var result = MessageBox.Show(
                    $"Are you sure you want to clear ALL stashed shinies?\n\nThis will despawn all current shinies in-game.",
                    "Confirm Clear",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    try
                    {
                        // Call the despawn method
                        shinyHunter.ClearAllFromStash(bot);

                        // Reload the stashed shinies
                        shinyHunter.LoadStashedShinies(bot);

                        // Update the display
                        DisplayStashedShinies();

                        MessageBox.Show(
                            $"All Pokémon have been cleared from the stash.",
                            "Success",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(
                            $"Error clearing shiny from stash: {ex.Message}",
                            "Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void OnClickExportSets(object sender, EventArgs e)
        {
            if (shinyHunter.StashedShinies.Count == 0)
            {
                MessageBox.Show("Nothing to export",
                                "Export",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
                return;
            }

            var result = MessageBox.Show("Export the currently stashed shiny Pokémon to Showdown format?",
                                         "Export Confirmation",
                                         MessageBoxButtons.YesNo,
                                         MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                var exportPath = Path.GetFullPath("Sets.txt");
                File.WriteAllText(exportPath, shinyHunter.GetShinyStashInfo(shinyHunter.StashedShinies));
                MessageBox.Show($"Export successful!\n\nSaved to:\n{exportPath}",
                                "Export Complete",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
            }
        }

        private async Task SendWebhook(string showdownSet, PA9 pk)
        {
            if (programConfig.Webhooks == null || programConfig.Webhooks.Count == 0)
                return;

            var strings = GameInfo.GetStrings("en");
            var species = strings.Species[pk.Species];
            var formName = ShowdownParsing.GetStringFromForm(pk.Form, strings, pk.Species, pk.Context);
            if (!string.IsNullOrEmpty(formName))
                species += $"-{formName}";
            string imageUrl = $"https://raw.githubusercontent.com/Omni-KingZeno/Pokemon-Sprites/refs/heads/main/Shiny/{species.ToLower().Replace("é", "e")}.png";

            var embed = new
            {
                title = $"{(pk.IsAlpha ? "Alpha " : "")}Shiny {species} Found!",
                description = showdownSet,
                image = new { url = imageUrl.Trim() }
            };

            var failedPosts = new List<string>();

            foreach (var webhook in programConfig.Webhooks)
            {
                if (!webhook.Enabled || string.IsNullOrWhiteSpace(webhook.WebhookAddress))
                    continue;

                try
                {
                    string json;

                    // Use custom message content if provided
                    if (!string.IsNullOrWhiteSpace(webhook.MessageContents))
                    {
                        var payloadWithContent = new
                        {
                            content = webhook.MessageContents,
                            embeds = new[] { embed }
                        };
                        json = JsonSerializer.Serialize(payloadWithContent);
                    }
                    else
                    {
                        var payloadNoContent = new
                        {
                            embeds = new[] { embed }
                        };
                        json = JsonSerializer.Serialize(payloadNoContent);
                    }

                    var response = await httpClient.PostAsync(webhook.WebhookAddress.Trim(),
                        new StringContent(json, Encoding.UTF8, "application/json"));
                    response.EnsureSuccessStatusCode();
                }
                catch (HttpRequestException ex)
                {
                    failedPosts.Add($"{webhook.WebhookAddress}: {ex.Message}");
                }
            }

            if (failedPosts.Count > 0)
            {
                MessageBox.Show($"Failed to send {failedPosts.Count} webhook(s):\n\n{string.Join("\n", failedPosts)}",
                                "Webhook Error",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
            }
        }

        private void LegacyImport()
        {
            try
            {
                const string legacyPositions = "positions.txt";
                const string legacyFilter = "filter_config.txt";

                if (File.Exists(legacyPositions))
                {
                    var lines = File.ReadAllLines(legacyPositions);
                    foreach (var line in lines)
                        positions.Add(Vector3.FromString(line));
                }

                if (File.Exists(legacyFilter))
                {
                    var importedIndices = new List<int>();
                    var lines = File.ReadAllLines(legacyFilter);
                    foreach (var line in lines)
                    {
                        if (line.StartsWith("SpeciesIndices="))
                        {
                            var indicesStr = line["SpeciesIndices=".Length..];
                            if (!string.IsNullOrEmpty(indicesStr))
                            {
                                importedIndices = [.. indicesStr.Split(',')
                                    .Select(s => int.TryParse(s.Trim(), out int idx) ? idx : -1)
                                    .Where(idx => idx >= 0)];
                            }
                            break;
                        }
                    }

                    if (importedIndices.Count > 0)
                    {
                        for (int i = 0; i < cBSpecies.Items.Count; i++)
                        {
                            cBSpecies.SetItemChecked(i, importedIndices.Contains(i));
                        }
                    }
                }

                UpdateUI();
            }
            catch
            {
                // Silently fail if we can't import
            }
        }

        private void OnClickWebhookSettings(object sender, EventArgs e)
        {
            var form = new WebhookForm(programConfig);
            CenterFormOnParent(form);
            if (form.ShowDialog() == DialogResult.OK)
            {
                SaveConfig();
            }
        }

        private void SetupListBox()
        {
            ContextMenuStrip contextMenu = new();

            ToolStripMenuItem copyItem = new("Copy");
            copyItem.Click += CopyLineItem;
            contextMenu.Items.Add(copyItem);

            ToolStripMenuItem pasteAboveItem = new("Insert Above");
            pasteAboveItem.Click += PasteAboveItem;
            contextMenu.Items.Add(pasteAboveItem);

            ToolStripMenuItem pasteBelowItem = new("Insert Below");
            pasteBelowItem.Click += PasteBelowItem;
            contextMenu.Items.Add(pasteBelowItem);

            lBCoords.ContextMenuStrip = contextMenu;
            lBCoords.MouseDown += CoordsRightClick;
        }

        private void CopyLineItem(object? sender, EventArgs e)
        {
            if (lBCoords.SelectedItem != null)
            {
                string? lineToCopy = lBCoords.SelectedItem.ToString();
                Clipboard.SetText(lineToCopy);
            }
        }

        private void PasteAboveItem(object? sender, EventArgs e)
        {
            if (Clipboard.ContainsText())
            {
                try
                {
                    string clipboardText = Clipboard.GetText().Trim();
                    Vector3 vector = Vector3.FromString(clipboardText);

                    int insertIndex = lBCoords.SelectedIndex;
                    lBCoords.Items.Insert(insertIndex, vector);
                    positions.Insert(insertIndex, vector);
                    SaveAllAndUpdateUI();
                    lBCoords.SelectedIndex = insertIndex;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error pasting item: {ex.Message}",
                        "Paste Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("No text in clipboard to paste.", "Paste",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void PasteBelowItem(object? sender, EventArgs e)
        {
            if (Clipboard.ContainsText())
            {
                try
                {
                    string clipboardText = Clipboard.GetText().Trim();
                    Vector3 vector = Vector3.FromString(clipboardText);

                    int insertIndex = lBCoords.SelectedIndex;
                    lBCoords.Items.Insert(insertIndex + 1, vector);
                    positions.Insert(insertIndex + 1, vector);
                    SaveAllAndUpdateUI();
                    lBCoords.SelectedIndex = insertIndex;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error pasting item: {ex.Message}",
                        "Paste Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("No text in clipboard to paste.", "Paste",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void CoordsRightClick(object? sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                int index = lBCoords.IndexFromPoint(e.Location);
                if (index >= 0 && index < lBCoords.Items.Count)
                {
                    lBCoords.SelectedIndex = index;
                }
            }
        }
    }

    public struct Vector3
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public string[] Flags { get; set; }

        public Vector3()
        {
            X = 0; Y = 0; Z = 0;
            Flags = [];
        }

        public override readonly string ToString()
        {
            return $"{X},{Y},{Z};{string.Join(',', Flags)}";
        }

        public static Vector3 FromString(string s)
        {
            var prePostFlags = s.Split(';');
            var spl = prePostFlags[0].Split(',');
            Vector3 v = new()
            {
                X = float.Parse(spl[0]),
                Y = float.Parse(spl[1]),
                Z = float.Parse(spl[2])
            };

            if (prePostFlags.Length > 1)
            {
                var nFlags = new List<string>();
                spl = prePostFlags[1].Split(',');
                foreach (var str in spl)
                    nFlags.Add(str);
                v.Flags = [.. nFlags];
            }

            return v;
        }
    }
}
