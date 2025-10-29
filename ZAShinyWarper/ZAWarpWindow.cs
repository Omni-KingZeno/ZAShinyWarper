using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using NHSE.Injection;
using PKHeX.Core;
using PKHeX.Drawing.PokeSprite;
using PLAWarper;

namespace PLADumper
{
    public partial class ZAWarpWindow : Form
    {
        private readonly long[] jumpsPos = [0x41EC340, 0x248, 0x00, 0x138]; // [[[[main+41EC340]+248]+00]+138]+90
        private static IRAMReadWriter bot = default!;

        private List<Vector3> positions = [];
        private const string Config = "config.json";
        private static readonly JsonSerializerOptions jsonOptions = new() { WriteIndented = true };

        private ShinyHunter<PA9> shinyHunter = new();
        private readonly List<PictureBox> StashList;

        private LabelForm lf = new();

        public ComboBox[] CBIVs = default!;

        // Bot
        private bool warping = false;
        private int currentWarps = 0;

        public ZAWarpWindow()
        {
            InitializeComponent();
            CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
            Application.ApplicationExit += (s, e) => CleanUpBot();
            StashList = [StashedShiny1, StashedShiny2, StashedShiny3, StashedShiny4, StashedShiny5, StashedShiny6, StashedShiny7, StashedShiny8, StashedShiny9, StashedShiny10];
        }

        private void LoadDefaults(object sender, EventArgs e)
        {
            // Load enums into comboboxes
            // WhenShinyFound
            foreach (var item in Enum.GetValues<ShinyFoundAction>())
                cBWhenShinyFound.Items.Add(item);
            cBWhenShinyFound.SelectedIndex = 0;

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

        private ShinyHunter<PA9>.ShinyFilter<PA9> GetFilter()
        {
            var filter = new ShinyHunter<PA9>.ShinyFilter<PA9>();
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
                filter.IVs[i] = (IVType)CBIVs[i].SelectedItem;
            }

            // Size
            filter.SizeMinimum = (byte)nUDScaleMin.Value;
            filter.SizeMaximum = (byte)nUDScaleMax.Value;
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

                gBControls.Enabled = false;
                gBShinyHunt.Enabled = false;
                gBStashedShiny.Enabled = false;
                btnScreenOn.Enabled = false;
                btnScreenOff.Enabled = false;
                CleanUpBot();
                ResetSprites();
                bot = default!;
                btnConnect.Text = "Connect";
                btnConnectUSB.Enabled = true;
            }
            else
            {
                try
                {
                    var botsys = new SysBot();
                    botsys.Connect(tB_IP.Text, 6000);
                    bot = botsys;
                    btnConnect.Text = "Disconnect";
                    btnConnectUSB.Enabled = false;
                    gBControls.Enabled = true;
                    gBShinyHunt.Enabled = true;
                    gBStashedShiny.Enabled = true;
                    btnScreenOn.Enabled = true;
                    btnScreenOff.Enabled = true;
                    shinyHunter.LoadStashedShinies(bot, "sets.txt");
                    DisplayStashedShinies();
                    MessageBox.Show($"Connected to SysBot (network). \r\n{shinyHunter.GetShinyStashInfo([.. shinyHunter.StashedShinies.Reverse()])}");
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

                gBControls.Enabled = false;
                gBShinyHunt.Enabled = false;
                gBStashedShiny.Enabled = false;
                btnScreenOn.Enabled = false;
                btnScreenOff.Enabled = false;
                CleanUpBot();
                ResetSprites();
                bot = default!;
                btnConnectUSB.Text = "ConnectUSB";
                btnConnect.Enabled = true;
            }
            else
            {
                try
                {
                    var botusb = new USBBot();
                    botusb.Connect();
                    bot = botusb;
                    btnConnectUSB.Text = "DisconnectUSB";
                    btnConnect.Enabled = false;
                    gBControls.Enabled = true;
                    gBShinyHunt.Enabled = true;
                    btnScreenOn.Enabled = true;
                    btnScreenOff.Enabled = true;
                    shinyHunter.LoadStashedShinies(bot, "sets.txt");
                    DisplayStashedShinies();
                    MessageBox.Show($"Connected to UsbBot (USB). \r\n{shinyHunter.GetShinyStashInfo([.. shinyHunter.StashedShinies.Reverse()])}");
                    bot.SendBytes(Encoding.ASCII.GetBytes("detachController\r\n"));
                    CleanUpBot();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
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

        private void OnClickSave(object sender, EventArgs e)
        {
            SaveNewValue();
        }

        private void SaveNewValue()
        {
            var pos = GetPlayerPosition();
            positions.Add(pos);
            SaveAllAndUpdateUI();

            lBCoords.SelectedIndex = lBCoords.Items.Count - 1;
        }

        private async void OnClickRestore(object sender, EventArgs e)
        {
            if (lBCoords.SelectedIndex > -1)
            {
                var toSend = (Vector3)lBCoords.SelectedItem;
                SetPlayerPosition(toSend.X, toSend.Y, toSend.Z);
                Thread.Sleep(4000); // fall out

                int i;
                lf.PerformSafely(() => lf.SetText("Warping..."));
                lf.PerformSafely(() => lf.Show());

                for (i = 0; i < 15; ++i)
                {
                    lf.PerformSafely(() => lf.SetText($"Warping... ({i + 1}/15) Please wait."));
                    if (GetPlayerPosition().Y == toSend.Y)
                        break;
                    SetPlayerPosition(toSend.X, toSend.Y, toSend.Z);
                    await Task.Delay(1100).ConfigureAwait(false);
                }

                lf.PerformSafely(() => lf.Hide());
            }
        }

        private void OnClickDelete(object sender, EventArgs e)
        {
            if (lBCoords.SelectedIndex > -1)
            {
                positions.RemoveAt(lBCoords.SelectedIndex);
                SaveAllAndUpdateUI();
            }
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

        private void SaveConfig()
        {
            try
            {
                var config = new ProgramConfig
                {
                    // Control Settings
                    IPAddress = tB_IP.Text,
                    Positions = [.. positions],

                    // Filter settings
                    SpawnCheckTime = nUDCheckTime.Value,
                    CamMove = nUDCamMove.Value,
                    SaveFreq = nUDSaveFreq.Value,
                    WhenShinyFound = cBWhenShinyFound.SelectedIndex,
                    IVHP = cBIVHP.SelectedIndex,
                    IVAtk = cBIVAtk.SelectedIndex,
                    IVDef = cBIVDef.SelectedIndex,
                    IVSpA = cBIVSpA.SelectedIndex,
                    IVSpD = cBIVSpD.SelectedIndex,
                    IVSpe = cBIVSpe.SelectedIndex,
                    ScaleMin = nUDScaleMin.Value,
                    ScaleMax = nUDScaleMax.Value
                };

                // Species
                for (int i = 0; i < cBSpecies.Items.Count; i++)
                {
                    if (cBSpecies.GetItemChecked(i))
                        config.SpeciesIndices.Add(i);
                }

                var json = JsonSerializer.Serialize(config, jsonOptions);
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
                    return;

                var json = File.ReadAllText(Config);
                var config = JsonSerializer.Deserialize<ProgramConfig>(json);

                if (config == null)
                    return;

                // Load all filter settings
                for (int i = 0; i < cBSpecies.Items.Count; i++)
                {
                    cBSpecies.SetItemChecked(i, false);
                }

                foreach (var idx in config.SpeciesIndices)
                {
                    if (idx < cBSpecies.Items.Count)
                        cBSpecies.SetItemChecked(idx, true);
                }

                tB_IP.Text = config.IPAddress;
                positions = [.. config.Positions];
                nUDCheckTime.Value = config.SpawnCheckTime;
                nUDCamMove.Value = config.CamMove;
                nUDSaveFreq.Value = config.SaveFreq;
                nUDScaleMin.Value = config.ScaleMin;
                nUDScaleMax.Value = config.ScaleMax;
                cBWhenShinyFound.SelectedIndex = config.WhenShinyFound;
                cBIVHP.SelectedIndex = config.IVHP;
                cBIVAtk.SelectedIndex = config.IVAtk;
                cBIVDef.SelectedIndex = config.IVDef;
                cBIVSpA.SelectedIndex = config.IVSpA;
                cBIVSpD.SelectedIndex = config.IVSpD;
                cBIVSpe.SelectedIndex = config.IVSpe;
            }
            catch
            {
                // If loading fails, just use defaults
            }
        }

        private void SetFiltersEnableState(bool enabled)
        {
            cBSpecies.PerformSafely(() => cBSpecies.Enabled = enabled);
            foreach (var cb in CBIVs)
                cb.PerformSafely(() => cb.Enabled = enabled);
            nUDScaleMin.PerformSafely(() => nUDScaleMin.Enabled = enabled);
            nUDCheckTime.PerformSafely(() => nUDCheckTime.Enabled = enabled);
            cBWhenShinyFound.PerformSafely(() => cBWhenShinyFound.Enabled = enabled);
            nUDCamMove.PerformSafely(() => nUDCamMove.Enabled = enabled);
            nUDSaveFreq.PerformSafely(() => nUDSaveFreq.Enabled = enabled);
            gBControls.PerformSafely(() => gBControls.Enabled = enabled);
            btnResetSpecies.PerformSafely(() => btnResetSpecies.Enabled = enabled);
            nUDScaleMax.PerformSafely(() => nUDScaleMax.Enabled = enabled);
        }

        private void CleanUpBot()
        {
            if (bot != null && bot.Connected)
            {
                bot.SendBytes(Encoding.ASCII.GetBytes("setStick RIGHT 0 0\r\n"));
                bot.SendBytes(Encoding.ASCII.GetBytes("detachController\r\n"));
            }
        }

        private async Task SaveGame()
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
                CleanUpBot();
                return;
            }

            if (positions.Count < 2)
            {
                MessageBox.Show("Not enough postions have been set to warp to!");
                return;
            }

            var filter = GetFilter();
            var warpInterval = (int)nUDCheckTime.Value;
            var camSpeed = (int)nUDCamMove.Value;
            var action = (ShinyFoundAction)cBWhenShinyFound.SelectedItem!;
            var saveFrequency = (int)nUDSaveFreq.Value;

            int shiniesFound = 0;

            currentWarps = 0;

            warping = true;
            SetFiltersEnableState(false);
            btnWarp.PerformSafely(() => btnWarp.Text = "Warping. Click to end.");

            // Rotate camera for spawns
            if (camSpeed != 0)
                bot.SendBytes(Encoding.ASCII.GetBytes($"setStick RIGHT {camSpeed} 0\r\n"));

            // Refresh stashed shinies
            _ = shinyHunter.LoadStashedShinies(bot, "sets.txt");
            DisplayStashedShinies();

            while (warping)
            {
                currentWarps++;
                if (currentWarps % saveFrequency == 0)
                    await SaveGame().ConfigureAwait(false);

                // Check shinies first as a new one may have spawned before we move
                var newFound = shinyHunter.LoadStashedShinies(bot, "sets.txt");
                if (newFound)
                {
                    DisplayStashedShinies();
                    var newShinies = shinyHunter.DifferentShinies;
                    foreach (var pk in newShinies)
                    {
                        shiniesFound++;
                        if (filter.MatchesFilter(pk.PKM))
                        {
                            // Found one
                            switch (action)
                            {
                                case ShinyFoundAction.StopOnFound:
                                    warping = false;
                                    CleanUpBot();
                                    bot.SendBytes(Encoding.ASCII.GetBytes("click X\r\n"));
                                    btnWarp.PerformSafely(() => btnWarp.Text = "Start Warping");
                                    SetFiltersEnableState(true);
                                    MessageBox.Show($"A shiny matching the filter has been found after {currentWarps} attempts! Stopping warping.\r\n\r\n{pk}\r\n" +
                                                         (pk.PKM.Scale == 255 ? "This Pokemon is ALPHA!" : "This Pokemon is not an alpha"), "Found!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    break;
                            }
                        }
                        else // Notify the user anyway, but don't stop spawning/warping. The user should know that a shiny is found because it will occupy one of the shiny stash slots until it is removed
                        {
                            CrossThreadExtensions.DoThreaded(() =>
                            {
                                MessageBox.Show($"The following shiny has been found, but does not match your filter. You may wish to remove it such that it doesn't occupy one of your shiny stash slots.\r\n\r\n{pk}\r\n" +
                                                             (pk.PKM.Scale == 255 ? "This Pokemon is ALPHA!" : "This Pokemon is not an alpha"), "Found something we don't want!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            });
                        }
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
                ShinyInfo.SetToolTip(pb, shinyHunter.StashedShinies[index].ToString());
            }
            else
            {
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

        public Vector3(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
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

    public class ProgramConfig
    {
        public string IPAddress { get; set; } = "192.168.0.1";
        public List<Vector3> Positions { get; set; } = [];
        public List<int> SpeciesIndices { get; set; } = [];
        public decimal SpawnCheckTime { get; set; } = 2000;
        public decimal CamMove { get; set; } = 16000;
        public decimal SaveFreq { get; set; } = 3;
        public decimal ScaleMin { get; set; } = 0;
        public decimal ScaleMax { get; set; } = 255;
        public int WhenShinyFound { get; set; } = 0;
        public int IVHP { get; set; } = 0;
        public int IVAtk { get; set; } = 0;
        public int IVDef { get; set; } = 0;
        public int IVSpA { get; set; } = 0;
        public int IVSpD { get; set; } = 0;
        public int IVSpe { get; set; } = 0;
    }
}
