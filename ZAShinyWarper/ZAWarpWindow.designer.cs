using System.Drawing;
using System.Windows.Forms;

namespace ZAWarper
{
    partial class ZAWarpWindow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ZAWarpWindow));
            lbl_IP = new Label();
            tB_IP = new TextBox();
            btnConnect = new Button();
            gBControls = new GroupBox();
            btnDelete = new Button();
            btnRestore = new Button();
            btnSave = new Button();
            lBCoords = new ListBox();
            lblFreeWarp = new Label();
            nUDDistance = new NumericUpDown();
            btnLeft = new Button();
            btnRight = new Button();
            btnBack = new Button();
            btnForw = new Button();
            lblThanks = new Label();
            btnConnectUSB = new Button();
            gBShinyHunt = new GroupBox();
            cBIsAlpha = new CheckBox();
            pBAlpha = new PictureBox();
            btnResetFilters = new Button();
            nUDSaveFreq = new NumericUpDown();
            lblSaveFreq = new Label();
            nUDCamMove = new NumericUpDown();
            lblCam = new Label();
            nUDCheckTime = new NumericUpDown();
            label3 = new Label();
            nUDScaleMin = new NumericUpDown();
            nUDScaleMax = new NumericUpDown();
            lblScaleMin = new Label();
            lblScaleMax = new Label();
            lblIVSpe = new Label();
            lblIVSpD = new Label();
            lblIVSpA = new Label();
            cBIVSpe = new ComboBox();
            cBIVSpD = new ComboBox();
            cBIVSpA = new ComboBox();
            lblIVDef = new Label();
            lblIVAtk = new Label();
            lblIVHP = new Label();
            cBIVDef = new ComboBox();
            cBIVAtk = new ComboBox();
            cBIVHP = new ComboBox();
            lblIV = new Label();
            cBSpecies = new CheckedListBox();
            btnResetSpecies = new Button();
            lblSpecies = new Label();
            lblFilter = new Label();
            cBWhenShinyFound = new ComboBox();
            lblShinyFound = new Label();
            btnWarp = new Button();
            gBStashedShiny = new GroupBox();
            StashedShiny1 = new PictureBox();
            StashedShiny2 = new PictureBox();
            StashedShiny3 = new PictureBox();
            StashedShiny4 = new PictureBox();
            StashedShiny5 = new PictureBox();
            StashedShiny6 = new PictureBox();
            StashedShiny7 = new PictureBox();
            StashedShiny8 = new PictureBox();
            StashedShiny9 = new PictureBox();
            StashedShiny10 = new PictureBox();
            btnScreenOn = new Button();
            btnScreenOff = new Button();
            ShinyInfo = new ToolTip(components);
            btnExport = new Button();
            btnWebhookSettings = new Button();
            gBControls.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)nUDDistance).BeginInit();
            gBShinyHunt.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pBAlpha).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nUDSaveFreq).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nUDCamMove).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nUDCheckTime).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nUDScaleMin).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nUDScaleMax).BeginInit();
            gBStashedShiny.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)StashedShiny1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)StashedShiny2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)StashedShiny3).BeginInit();
            ((System.ComponentModel.ISupportInitialize)StashedShiny4).BeginInit();
            ((System.ComponentModel.ISupportInitialize)StashedShiny5).BeginInit();
            ((System.ComponentModel.ISupportInitialize)StashedShiny6).BeginInit();
            ((System.ComponentModel.ISupportInitialize)StashedShiny7).BeginInit();
            ((System.ComponentModel.ISupportInitialize)StashedShiny8).BeginInit();
            ((System.ComponentModel.ISupportInitialize)StashedShiny9).BeginInit();
            ((System.ComponentModel.ISupportInitialize)StashedShiny10).BeginInit();
            SuspendLayout();
            // 
            // lbl_IP
            // 
            lbl_IP.AutoSize = true;
            lbl_IP.Location = new Point(15, 15);
            lbl_IP.Margin = new Padding(4, 0, 4, 0);
            lbl_IP.Name = "lbl_IP";
            lbl_IP.Size = new Size(20, 15);
            lbl_IP.TabIndex = 0;
            lbl_IP.Text = "IP:";
            // 
            // tB_IP
            // 
            tB_IP.Location = new Point(46, 12);
            tB_IP.Margin = new Padding(4, 3, 4, 3);
            tB_IP.Name = "tB_IP";
            tB_IP.Size = new Size(272, 23);
            tB_IP.TabIndex = 1;
            tB_IP.Text = "192.168.0.1";
            tB_IP.TextChanged += OnConfigurationChange;
            // 
            // btnConnect
            // 
            btnConnect.Location = new Point(14, 42);
            btnConnect.Margin = new Padding(4, 3, 4, 3);
            btnConnect.Name = "btnConnect";
            btnConnect.Size = new Size(148, 36);
            btnConnect.TabIndex = 2;
            btnConnect.Text = "Connect";
            btnConnect.UseVisualStyleBackColor = true;
            btnConnect.Click += OnClickConnect;
            // 
            // gBControls
            // 
            gBControls.Controls.Add(btnDelete);
            gBControls.Controls.Add(btnRestore);
            gBControls.Controls.Add(btnSave);
            gBControls.Controls.Add(lBCoords);
            gBControls.Controls.Add(lblFreeWarp);
            gBControls.Controls.Add(nUDDistance);
            gBControls.Controls.Add(btnLeft);
            gBControls.Controls.Add(btnRight);
            gBControls.Controls.Add(btnBack);
            gBControls.Controls.Add(btnForw);
            gBControls.Enabled = false;
            gBControls.Location = new Point(15, 165);
            gBControls.Margin = new Padding(4, 3, 4, 3);
            gBControls.Name = "gBControls";
            gBControls.Padding = new Padding(4, 3, 4, 3);
            gBControls.Size = new Size(303, 381);
            gBControls.TabIndex = 3;
            gBControls.TabStop = false;
            gBControls.Text = "Editing";
            // 
            // btnDelete
            // 
            btnDelete.Location = new Point(204, 166);
            btnDelete.Margin = new Padding(4, 3, 4, 3);
            btnDelete.Name = "btnDelete";
            btnDelete.Size = new Size(88, 27);
            btnDelete.TabIndex = 17;
            btnDelete.Text = "Delete Pos";
            btnDelete.UseVisualStyleBackColor = true;
            btnDelete.Click += OnClickDelete;
            // 
            // btnRestore
            // 
            btnRestore.Location = new Point(108, 166);
            btnRestore.Margin = new Padding(4, 3, 4, 3);
            btnRestore.Name = "btnRestore";
            btnRestore.Size = new Size(88, 27);
            btnRestore.TabIndex = 16;
            btnRestore.Text = "Restore Pos";
            btnRestore.UseVisualStyleBackColor = true;
            btnRestore.Click += OnClickRestore;
            // 
            // btnSave
            // 
            btnSave.Location = new Point(12, 166);
            btnSave.Margin = new Padding(4, 3, 4, 3);
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(88, 27);
            btnSave.TabIndex = 15;
            btnSave.Text = "Save Pos";
            btnSave.UseVisualStyleBackColor = true;
            btnSave.Click += OnClickSave;
            // 
            // lBCoords
            // 
            lBCoords.FormattingEnabled = true;
            lBCoords.Location = new Point(10, 22);
            lBCoords.Margin = new Padding(4, 3, 4, 3);
            lBCoords.Name = "lBCoords";
            lBCoords.Size = new Size(285, 139);
            lBCoords.TabIndex = 14;
            // 
            // lblFreeWarp
            // 
            lblFreeWarp.AutoSize = true;
            lblFreeWarp.Location = new Point(12, 248);
            lblFreeWarp.Margin = new Padding(4, 0, 4, 0);
            lblFreeWarp.Name = "lblFreeWarp";
            lblFreeWarp.Size = new Size(61, 15);
            lblFreeWarp.TabIndex = 13;
            lblFreeWarp.Text = "Free warp:";
            // 
            // nUDDistance
            // 
            nUDDistance.Location = new Point(216, 340);
            nUDDistance.Margin = new Padding(4, 3, 4, 3);
            nUDDistance.Name = "nUDDistance";
            nUDDistance.Size = new Size(76, 23);
            nUDDistance.TabIndex = 12;
            nUDDistance.Value = new decimal(new int[] { 3, 0, 0, 0 });
            // 
            // btnLeft
            // 
            btnLeft.Location = new Point(12, 285);
            btnLeft.Margin = new Padding(4, 3, 4, 3);
            btnLeft.Name = "btnLeft";
            btnLeft.Size = new Size(88, 27);
            btnLeft.TabIndex = 11;
            btnLeft.Text = "←";
            btnLeft.UseVisualStyleBackColor = true;
            btnLeft.Click += OnClickLeft;
            // 
            // btnRight
            // 
            btnRight.Location = new Point(204, 285);
            btnRight.Margin = new Padding(4, 3, 4, 3);
            btnRight.Name = "btnRight";
            btnRight.Size = new Size(88, 27);
            btnRight.TabIndex = 10;
            btnRight.Text = "→";
            btnRight.UseVisualStyleBackColor = true;
            btnRight.Click += OnClickRight;
            // 
            // btnBack
            // 
            btnBack.Location = new Point(108, 324);
            btnBack.Margin = new Padding(4, 3, 4, 3);
            btnBack.Name = "btnBack";
            btnBack.Size = new Size(88, 27);
            btnBack.TabIndex = 9;
            btnBack.Text = "↓";
            btnBack.UseVisualStyleBackColor = true;
            btnBack.Click += OnClickBackwards;
            // 
            // btnForw
            // 
            btnForw.Location = new Point(108, 248);
            btnForw.Margin = new Padding(4, 3, 4, 3);
            btnForw.Name = "btnForw";
            btnForw.Size = new Size(88, 27);
            btnForw.TabIndex = 8;
            btnForw.Text = "↑";
            btnForw.UseVisualStyleBackColor = true;
            btnForw.Click += OnClickForwards;
            // 
            // lblThanks
            // 
            lblThanks.AutoSize = true;
            lblThanks.Location = new Point(55, 555);
            lblThanks.Margin = new Padding(4, 0, 4, 0);
            lblThanks.Name = "lblThanks";
            lblThanks.Size = new Size(216, 30);
            lblThanks.TabIndex = 4;
            lblThanks.Text = "Many thanks to Kurt for PKHeX and\r\nAnubis for the Z-A shiny stash research.";
            lblThanks.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // btnConnectUSB
            // 
            btnConnectUSB.Location = new Point(170, 42);
            btnConnectUSB.Margin = new Padding(4, 3, 4, 3);
            btnConnectUSB.Name = "btnConnectUSB";
            btnConnectUSB.Size = new Size(148, 36);
            btnConnectUSB.TabIndex = 5;
            btnConnectUSB.Text = "ConnectUSB";
            btnConnectUSB.UseVisualStyleBackColor = true;
            btnConnectUSB.Click += OnClickConnectUSB;
            // 
            // gBShinyHunt
            // 
            gBShinyHunt.Controls.Add(cBIsAlpha);
            gBShinyHunt.Controls.Add(pBAlpha);
            gBShinyHunt.Controls.Add(btnResetFilters);
            gBShinyHunt.Controls.Add(nUDSaveFreq);
            gBShinyHunt.Controls.Add(lblSaveFreq);
            gBShinyHunt.Controls.Add(nUDCamMove);
            gBShinyHunt.Controls.Add(lblCam);
            gBShinyHunt.Controls.Add(nUDCheckTime);
            gBShinyHunt.Controls.Add(label3);
            gBShinyHunt.Controls.Add(nUDScaleMin);
            gBShinyHunt.Controls.Add(nUDScaleMax);
            gBShinyHunt.Controls.Add(lblScaleMin);
            gBShinyHunt.Controls.Add(lblScaleMax);
            gBShinyHunt.Controls.Add(lblIVSpe);
            gBShinyHunt.Controls.Add(lblIVSpD);
            gBShinyHunt.Controls.Add(lblIVSpA);
            gBShinyHunt.Controls.Add(cBIVSpe);
            gBShinyHunt.Controls.Add(cBIVSpD);
            gBShinyHunt.Controls.Add(cBIVSpA);
            gBShinyHunt.Controls.Add(lblIVDef);
            gBShinyHunt.Controls.Add(lblIVAtk);
            gBShinyHunt.Controls.Add(lblIVHP);
            gBShinyHunt.Controls.Add(cBIVDef);
            gBShinyHunt.Controls.Add(cBIVAtk);
            gBShinyHunt.Controls.Add(cBIVHP);
            gBShinyHunt.Controls.Add(lblIV);
            gBShinyHunt.Controls.Add(cBSpecies);
            gBShinyHunt.Controls.Add(btnResetSpecies);
            gBShinyHunt.Controls.Add(lblSpecies);
            gBShinyHunt.Controls.Add(lblFilter);
            gBShinyHunt.Controls.Add(cBWhenShinyFound);
            gBShinyHunt.Controls.Add(lblShinyFound);
            gBShinyHunt.Enabled = false;
            gBShinyHunt.Location = new Point(327, 15);
            gBShinyHunt.Margin = new Padding(4, 3, 4, 3);
            gBShinyHunt.Name = "gBShinyHunt";
            gBShinyHunt.Padding = new Padding(4, 3, 4, 3);
            gBShinyHunt.Size = new Size(212, 567);
            gBShinyHunt.TabIndex = 6;
            gBShinyHunt.TabStop = false;
            gBShinyHunt.Text = "Shiny Hunting";
            // 
            // cBIsAlpha
            // 
            cBIsAlpha.AutoSize = true;
            cBIsAlpha.Location = new Point(51, 493);
            cBIsAlpha.Name = "cBIsAlpha";
            cBIsAlpha.Size = new Size(15, 14);
            cBIsAlpha.TabIndex = 32;
            cBIsAlpha.UseVisualStyleBackColor = true;
            cBIsAlpha.CheckedChanged += OnAlphaCheckedChanged;
            cBIsAlpha.CheckedChanged += OnConfigurationChange;
            // 
            // pBAlpha
            // 
            pBAlpha.BackgroundImageLayout = ImageLayout.None;
            pBAlpha.Image = ZAShinyWarper.Properties.Resources.alpha;
            pBAlpha.Location = new Point(14, 481);
            pBAlpha.Name = "pBAlpha";
            pBAlpha.Size = new Size(31, 32);
            pBAlpha.SizeMode = PictureBoxSizeMode.StretchImage;
            pBAlpha.TabIndex = 31;
            pBAlpha.TabStop = false;
            // 
            // btnResetFilters
            // 
            btnResetFilters.Enabled = false;
            btnResetFilters.Location = new Point(14, 533);
            btnResetFilters.Name = "btnResetFilters";
            btnResetFilters.Size = new Size(184, 27);
            btnResetFilters.TabIndex = 30;
            btnResetFilters.Text = "Reset Filters";
            btnResetFilters.UseVisualStyleBackColor = true;
            btnResetFilters.Click += OnClickResetFilters;
            // 
            // nUDSaveFreq
            // 
            nUDSaveFreq.Location = new Point(140, 150);
            nUDSaveFreq.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            nUDSaveFreq.Name = "nUDSaveFreq";
            nUDSaveFreq.Size = new Size(63, 23);
            nUDSaveFreq.TabIndex = 28;
            nUDSaveFreq.Value = new decimal(new int[] { 3, 0, 0, 0 });
            nUDSaveFreq.ValueChanged += OnConfigurationChange;
            // 
            // lblSaveFreq
            // 
            lblSaveFreq.AutoSize = true;
            lblSaveFreq.Location = new Point(10, 152);
            lblSaveFreq.Name = "lblSaveFreq";
            lblSaveFreq.Size = new Size(90, 15);
            lblSaveFreq.TabIndex = 27;
            lblSaveFreq.Text = "Save frequency:";
            // 
            // nUDCamMove
            // 
            nUDCamMove.Increment = new decimal(new int[] { 1000, 0, 0, 0 });
            nUDCamMove.Location = new Point(10, 126);
            nUDCamMove.Maximum = new decimal(new int[] { 32000, 0, 0, 0 });
            nUDCamMove.Minimum = new decimal(new int[] { 32000, 0, 0, int.MinValue });
            nUDCamMove.Name = "nUDCamMove";
            nUDCamMove.Size = new Size(193, 23);
            nUDCamMove.TabIndex = 26;
            nUDCamMove.Value = new decimal(new int[] { 16000, 0, 0, 0 });
            nUDCamMove.ValueChanged += OnConfigurationChange;
            // 
            // lblCam
            // 
            lblCam.AutoSize = true;
            lblCam.Location = new Point(8, 109);
            lblCam.Name = "lblCam";
            lblCam.RightToLeft = RightToLeft.No;
            lblCam.Size = new Size(192, 15);
            lblCam.TabIndex = 25;
            lblCam.Text = "Cam move speed (-32000 to 32000)";
            // 
            // nUDCheckTime
            // 
            nUDCheckTime.Increment = new decimal(new int[] { 1000, 0, 0, 0 });
            nUDCheckTime.Location = new Point(8, 83);
            nUDCheckTime.Maximum = new decimal(new int[] { 20000, 0, 0, 0 });
            nUDCheckTime.Name = "nUDCheckTime";
            nUDCheckTime.Size = new Size(193, 23);
            nUDCheckTime.TabIndex = 24;
            nUDCheckTime.Value = new decimal(new int[] { 2000, 0, 0, 0 });
            nUDCheckTime.ValueChanged += OnConfigurationChange;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(8, 66);
            label3.Name = "label3";
            label3.Size = new Size(133, 15);
            label3.TabIndex = 23;
            label3.Text = "Spawn check time (ms):";
            // 
            // nUDScaleMin
            // 
            nUDScaleMin.Location = new Point(140, 474);
            nUDScaleMin.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
            nUDScaleMin.Name = "nUDScaleMin";
            nUDScaleMin.Size = new Size(57, 23);
            nUDScaleMin.TabIndex = 22;
            nUDScaleMin.ValueChanged += OnConfigurationChange;
            // 
            // nUDScaleMax
            // 
            nUDScaleMax.Location = new Point(140, 501);
            nUDScaleMax.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
            nUDScaleMax.Name = "nUDScaleMax";
            nUDScaleMax.Size = new Size(57, 23);
            nUDScaleMax.TabIndex = 22;
            nUDScaleMax.Value = new decimal(new int[] { 255, 0, 0, 0 });
            nUDScaleMax.ValueChanged += OnConfigurationChange;
            // 
            // lblScaleMin
            // 
            lblScaleMin.AutoSize = true;
            lblScaleMin.Location = new Point(72, 480);
            lblScaleMin.Name = "lblScaleMin";
            lblScaleMin.Size = new Size(60, 15);
            lblScaleMin.TabIndex = 21;
            lblScaleMin.Text = "Min scale:";
            // 
            // lblScaleMax
            // 
            lblScaleMax.AutoSize = true;
            lblScaleMax.Location = new Point(72, 503);
            lblScaleMax.Name = "lblScaleMax";
            lblScaleMax.Size = new Size(62, 15);
            lblScaleMax.TabIndex = 21;
            lblScaleMax.Text = "Max scale:";
            // 
            // lblIVSpe
            // 
            lblIVSpe.AutoSize = true;
            lblIVSpe.Location = new Point(140, 430);
            lblIVSpe.Name = "lblIVSpe";
            lblIVSpe.Size = new Size(26, 15);
            lblIVSpe.TabIndex = 19;
            lblIVSpe.Text = "Spe";
            // 
            // lblIVSpD
            // 
            lblIVSpD.AutoSize = true;
            lblIVSpD.Location = new Point(77, 430);
            lblIVSpD.Name = "lblIVSpD";
            lblIVSpD.Size = new Size(28, 15);
            lblIVSpD.TabIndex = 18;
            lblIVSpD.Text = "SpD";
            // 
            // lblIVSpA
            // 
            lblIVSpA.AutoSize = true;
            lblIVSpA.Location = new Point(14, 430);
            lblIVSpA.Name = "lblIVSpA";
            lblIVSpA.Size = new Size(28, 15);
            lblIVSpA.TabIndex = 17;
            lblIVSpA.Text = "SpA";
            // 
            // cBIVSpe
            // 
            cBIVSpe.FormattingEnabled = true;
            cBIVSpe.Location = new Point(140, 448);
            cBIVSpe.Name = "cBIVSpe";
            cBIVSpe.Size = new Size(57, 23);
            cBIVSpe.TabIndex = 16;
            // 
            // cBIVSpD
            // 
            cBIVSpD.FormattingEnabled = true;
            cBIVSpD.Location = new Point(77, 448);
            cBIVSpD.Name = "cBIVSpD";
            cBIVSpD.Size = new Size(57, 23);
            cBIVSpD.TabIndex = 15;
            cBIVSpD.SelectedIndexChanged += OnConfigurationChange;
            // 
            // cBIVSpA
            // 
            cBIVSpA.FormattingEnabled = true;
            cBIVSpA.Location = new Point(14, 448);
            cBIVSpA.Name = "cBIVSpA";
            cBIVSpA.Size = new Size(57, 23);
            cBIVSpA.TabIndex = 14;
            cBIVSpA.SelectedIndexChanged += OnConfigurationChange;
            // 
            // lblIVDef
            // 
            lblIVDef.AutoSize = true;
            lblIVDef.Location = new Point(140, 378);
            lblIVDef.Name = "lblIVDef";
            lblIVDef.Size = new Size(25, 15);
            lblIVDef.TabIndex = 13;
            lblIVDef.Text = "Def";
            // 
            // lblIVAtk
            // 
            lblIVAtk.AutoSize = true;
            lblIVAtk.Location = new Point(77, 378);
            lblIVAtk.Name = "lblIVAtk";
            lblIVAtk.Size = new Size(25, 15);
            lblIVAtk.TabIndex = 12;
            lblIVAtk.Text = "Atk";
            // 
            // lblIVHP
            // 
            lblIVHP.AutoSize = true;
            lblIVHP.Location = new Point(14, 378);
            lblIVHP.Name = "lblIVHP";
            lblIVHP.Size = new Size(23, 15);
            lblIVHP.TabIndex = 11;
            lblIVHP.Text = "HP";
            // 
            // cBIVDef
            // 
            cBIVDef.FormattingEnabled = true;
            cBIVDef.Location = new Point(140, 396);
            cBIVDef.Name = "cBIVDef";
            cBIVDef.Size = new Size(57, 23);
            cBIVDef.TabIndex = 10;
            cBIVDef.SelectedIndexChanged += OnConfigurationChange;
            // 
            // cBIVAtk
            // 
            cBIVAtk.FormattingEnabled = true;
            cBIVAtk.Location = new Point(77, 396);
            cBIVAtk.Name = "cBIVAtk";
            cBIVAtk.Size = new Size(57, 23);
            cBIVAtk.TabIndex = 9;
            cBIVAtk.SelectedIndexChanged += OnConfigurationChange;
            // 
            // cBIVHP
            // 
            cBIVHP.FormattingEnabled = true;
            cBIVHP.Location = new Point(14, 396);
            cBIVHP.Name = "cBIVHP";
            cBIVHP.Size = new Size(57, 23);
            cBIVHP.TabIndex = 8;
            cBIVHP.SelectedIndexChanged += OnConfigurationChange;
            // 
            // lblIV
            // 
            lblIV.AutoSize = true;
            lblIV.Location = new Point(14, 361);
            lblIV.Name = "lblIV";
            lblIV.Size = new Size(25, 15);
            lblIV.TabIndex = 7;
            lblIV.Text = "IVs:";
            // 
            // cBSpecies
            // 
            cBSpecies.CheckOnClick = true;
            cBSpecies.FormattingEnabled = true;
            cBSpecies.IntegralHeight = false;
            cBSpecies.Location = new Point(10, 213);
            cBSpecies.Name = "cBSpecies";
            cBSpecies.Size = new Size(193, 142);
            cBSpecies.TabIndex = 6;
            cBSpecies.ItemCheck += OnConfigurationChange;
            cBSpecies.SelectedIndexChanged += OnSpeciesSelectedIndexChange;
            // 
            // btnResetSpecies
            // 
            btnResetSpecies.Location = new Point(140, 193);
            btnResetSpecies.Name = "btnResetSpecies";
            btnResetSpecies.Size = new Size(64, 23);
            btnResetSpecies.TabIndex = 29;
            btnResetSpecies.Text = "Reset";
            btnResetSpecies.UseVisualStyleBackColor = true;
            btnResetSpecies.Click += OnClickReset;
            // 
            // lblSpecies
            // 
            lblSpecies.AutoSize = true;
            lblSpecies.Location = new Point(10, 196);
            lblSpecies.Name = "lblSpecies";
            lblSpecies.Size = new Size(46, 15);
            lblSpecies.TabIndex = 5;
            lblSpecies.Text = "Species";
            // 
            // lblFilter
            // 
            lblFilter.AutoSize = true;
            lblFilter.Location = new Point(10, 181);
            lblFilter.Name = "lblFilter";
            lblFilter.Size = new Size(33, 15);
            lblFilter.TabIndex = 4;
            lblFilter.Text = "Filter";
            // 
            // cBWhenShinyFound
            // 
            cBWhenShinyFound.FormattingEnabled = true;
            cBWhenShinyFound.Location = new Point(8, 40);
            cBWhenShinyFound.Margin = new Padding(4, 3, 4, 3);
            cBWhenShinyFound.Name = "cBWhenShinyFound";
            cBWhenShinyFound.Size = new Size(193, 23);
            cBWhenShinyFound.TabIndex = 3;
            cBWhenShinyFound.SelectedIndexChanged += OnConfigurationChange;
            // 
            // lblShinyFound
            // 
            lblShinyFound.AutoSize = true;
            lblShinyFound.Location = new Point(8, 20);
            lblShinyFound.Margin = new Padding(4, 0, 4, 0);
            lblShinyFound.Name = "lblShinyFound";
            lblShinyFound.Size = new Size(107, 15);
            lblShinyFound.TabIndex = 2;
            lblShinyFound.Text = "When shiny found:";
            // 
            // btnWarp
            // 
            btnWarp.Enabled = false;
            btnWarp.Location = new Point(170, 126);
            btnWarp.Margin = new Padding(4, 3, 4, 3);
            btnWarp.Name = "btnWarp";
            btnWarp.Size = new Size(147, 36);
            btnWarp.TabIndex = 1;
            btnWarp.Text = "Begin Warping";
            btnWarp.UseVisualStyleBackColor = true;
            btnWarp.Click += OnClickWarp;
            // 
            // gBStashedShiny
            // 
            gBStashedShiny.Controls.Add(StashedShiny1);
            gBStashedShiny.Controls.Add(StashedShiny2);
            gBStashedShiny.Controls.Add(StashedShiny3);
            gBStashedShiny.Controls.Add(StashedShiny4);
            gBStashedShiny.Controls.Add(StashedShiny5);
            gBStashedShiny.Controls.Add(StashedShiny6);
            gBStashedShiny.Controls.Add(StashedShiny7);
            gBStashedShiny.Controls.Add(StashedShiny8);
            gBStashedShiny.Controls.Add(StashedShiny9);
            gBStashedShiny.Controls.Add(StashedShiny10);
            gBStashedShiny.Enabled = false;
            gBStashedShiny.Location = new Point(546, 15);
            gBStashedShiny.Name = "gBStashedShiny";
            gBStashedShiny.Size = new Size(210, 567);
            gBStashedShiny.TabIndex = 7;
            gBStashedShiny.TabStop = false;
            gBStashedShiny.Text = "Stashed Shinies";
            // 
            // StashedShiny1
            // 
            StashedShiny1.BorderStyle = BorderStyle.FixedSingle;
            StashedShiny1.Location = new Point(6, 20);
            StashedShiny1.Name = "StashedShiny1";
            StashedShiny1.Size = new Size(96, 96);
            StashedShiny1.SizeMode = PictureBoxSizeMode.Zoom;
            StashedShiny1.TabIndex = 0;
            StashedShiny1.TabStop = false;
            StashedShiny1.MouseEnter += OnMouseHover;
            // 
            // StashedShiny2
            // 
            StashedShiny2.BorderStyle = BorderStyle.FixedSingle;
            StashedShiny2.Location = new Point(108, 20);
            StashedShiny2.Name = "StashedShiny2";
            StashedShiny2.Size = new Size(96, 96);
            StashedShiny2.SizeMode = PictureBoxSizeMode.Zoom;
            StashedShiny2.TabIndex = 1;
            StashedShiny2.TabStop = false;
            StashedShiny2.MouseEnter += OnMouseHover;
            // 
            // StashedShiny3
            // 
            StashedShiny3.BorderStyle = BorderStyle.FixedSingle;
            StashedShiny3.Location = new Point(6, 122);
            StashedShiny3.Name = "StashedShiny3";
            StashedShiny3.Size = new Size(96, 96);
            StashedShiny3.SizeMode = PictureBoxSizeMode.Zoom;
            StashedShiny3.TabIndex = 3;
            StashedShiny3.TabStop = false;
            StashedShiny3.MouseEnter += OnMouseHover;
            // 
            // StashedShiny4
            // 
            StashedShiny4.BorderStyle = BorderStyle.FixedSingle;
            StashedShiny4.Location = new Point(108, 122);
            StashedShiny4.Name = "StashedShiny4";
            StashedShiny4.Size = new Size(96, 96);
            StashedShiny4.SizeMode = PictureBoxSizeMode.Zoom;
            StashedShiny4.TabIndex = 4;
            StashedShiny4.TabStop = false;
            StashedShiny4.MouseEnter += OnMouseHover;
            // 
            // StashedShiny5
            // 
            StashedShiny5.BorderStyle = BorderStyle.FixedSingle;
            StashedShiny5.Location = new Point(6, 224);
            StashedShiny5.Name = "StashedShiny5";
            StashedShiny5.Size = new Size(96, 96);
            StashedShiny5.SizeMode = PictureBoxSizeMode.Zoom;
            StashedShiny5.TabIndex = 6;
            StashedShiny5.TabStop = false;
            StashedShiny5.MouseEnter += OnMouseHover;
            // 
            // StashedShiny6
            // 
            StashedShiny6.BorderStyle = BorderStyle.FixedSingle;
            StashedShiny6.Location = new Point(108, 224);
            StashedShiny6.Name = "StashedShiny6";
            StashedShiny6.Size = new Size(96, 96);
            StashedShiny6.SizeMode = PictureBoxSizeMode.Zoom;
            StashedShiny6.TabIndex = 5;
            StashedShiny6.TabStop = false;
            StashedShiny6.MouseEnter += OnMouseHover;
            // 
            // StashedShiny7
            // 
            StashedShiny7.BorderStyle = BorderStyle.FixedSingle;
            StashedShiny7.Location = new Point(6, 326);
            StashedShiny7.Name = "StashedShiny7";
            StashedShiny7.Size = new Size(96, 96);
            StashedShiny7.SizeMode = PictureBoxSizeMode.Zoom;
            StashedShiny7.TabIndex = 8;
            StashedShiny7.TabStop = false;
            StashedShiny7.MouseEnter += OnMouseHover;
            // 
            // StashedShiny8
            // 
            StashedShiny8.BorderStyle = BorderStyle.FixedSingle;
            StashedShiny8.Location = new Point(108, 326);
            StashedShiny8.Name = "StashedShiny8";
            StashedShiny8.Size = new Size(96, 96);
            StashedShiny8.SizeMode = PictureBoxSizeMode.Zoom;
            StashedShiny8.TabIndex = 2;
            StashedShiny8.TabStop = false;
            StashedShiny8.MouseEnter += OnMouseHover;
            // 
            // StashedShiny9
            // 
            StashedShiny9.BorderStyle = BorderStyle.FixedSingle;
            StashedShiny9.Location = new Point(6, 428);
            StashedShiny9.Name = "StashedShiny9";
            StashedShiny9.Size = new Size(96, 96);
            StashedShiny9.SizeMode = PictureBoxSizeMode.Zoom;
            StashedShiny9.TabIndex = 7;
            StashedShiny9.TabStop = false;
            StashedShiny9.MouseEnter += OnMouseHover;
            // 
            // StashedShiny10
            // 
            StashedShiny10.BorderStyle = BorderStyle.FixedSingle;
            StashedShiny10.Location = new Point(108, 428);
            StashedShiny10.Name = "StashedShiny10";
            StashedShiny10.Size = new Size(96, 96);
            StashedShiny10.SizeMode = PictureBoxSizeMode.Zoom;
            StashedShiny10.TabIndex = 9;
            StashedShiny10.TabStop = false;
            StashedShiny10.MouseEnter += OnMouseHover;
            // 
            // btnScreenOn
            // 
            btnScreenOn.Enabled = false;
            btnScreenOn.Location = new Point(15, 84);
            btnScreenOn.Name = "btnScreenOn";
            btnScreenOn.Size = new Size(148, 36);
            btnScreenOn.TabIndex = 8;
            btnScreenOn.Text = "Screen On";
            btnScreenOn.UseVisualStyleBackColor = true;
            btnScreenOn.Click += OnClickScreenOn;
            // 
            // btnScreenOff
            // 
            btnScreenOff.Enabled = false;
            btnScreenOff.Location = new Point(169, 84);
            btnScreenOff.Name = "btnScreenOff";
            btnScreenOff.Size = new Size(148, 36);
            btnScreenOff.TabIndex = 9;
            btnScreenOff.Text = "Screen Off";
            btnScreenOff.UseVisualStyleBackColor = true;
            btnScreenOff.Click += OnClickScreenOff;
            // 
            // ShinyInfo
            // 
            ShinyInfo.AutoPopDelay = 30000;
            ShinyInfo.InitialDelay = 750;
            ShinyInfo.ReshowDelay = 1000;
            // 
            // btnExport
            // 
            btnExport.Enabled = false;
            btnExport.Location = new Point(552, 549);
            btnExport.Name = "btnExport";
            btnExport.Size = new Size(198, 27);
            btnExport.TabIndex = 14;
            btnExport.Text = "Export Sets";
            btnExport.UseVisualStyleBackColor = true;
            btnExport.Click += OnClickExport;
            // 
            // btnWebhookSettings
            // 
            btnWebhookSettings.Enabled = false;
            btnWebhookSettings.Location = new Point(15, 126);
            btnWebhookSettings.Name = "btnWebhookSettings";
            btnWebhookSettings.Size = new Size(148, 36);
            btnWebhookSettings.TabIndex = 15;
            btnWebhookSettings.Text = "Webhook Settings";
            btnWebhookSettings.UseVisualStyleBackColor = true;
            btnWebhookSettings.Click += OnClickWebhookSettings;
            // 
            // ZAWarpWindow
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(768, 594);
            Controls.Add(btnWebhookSettings);
            Controls.Add(btnExport);
            Controls.Add(btnScreenOff);
            Controls.Add(btnScreenOn);
            Controls.Add(gBStashedShiny);
            Controls.Add(gBShinyHunt);
            Controls.Add(btnConnectUSB);
            Controls.Add(lblThanks);
            Controls.Add(gBControls);
            Controls.Add(btnConnect);
            Controls.Add(tB_IP);
            Controls.Add(lbl_IP);
            Controls.Add(btnWarp);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(4, 3, 4, 3);
            MaximizeBox = false;
            Name = "ZAWarpWindow";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Z-A Shiny Warper by Berichan";
            Load += LoadDefaults;
            gBControls.ResumeLayout(false);
            gBControls.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)nUDDistance).EndInit();
            gBShinyHunt.ResumeLayout(false);
            gBShinyHunt.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pBAlpha).EndInit();
            ((System.ComponentModel.ISupportInitialize)nUDSaveFreq).EndInit();
            ((System.ComponentModel.ISupportInitialize)nUDCamMove).EndInit();
            ((System.ComponentModel.ISupportInitialize)nUDCheckTime).EndInit();
            ((System.ComponentModel.ISupportInitialize)nUDScaleMin).EndInit();
            ((System.ComponentModel.ISupportInitialize)nUDScaleMax).EndInit();
            gBStashedShiny.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)StashedShiny1).EndInit();
            ((System.ComponentModel.ISupportInitialize)StashedShiny2).EndInit();
            ((System.ComponentModel.ISupportInitialize)StashedShiny3).EndInit();
            ((System.ComponentModel.ISupportInitialize)StashedShiny4).EndInit();
            ((System.ComponentModel.ISupportInitialize)StashedShiny5).EndInit();
            ((System.ComponentModel.ISupportInitialize)StashedShiny6).EndInit();
            ((System.ComponentModel.ISupportInitialize)StashedShiny7).EndInit();
            ((System.ComponentModel.ISupportInitialize)StashedShiny8).EndInit();
            ((System.ComponentModel.ISupportInitialize)StashedShiny9).EndInit();
            ((System.ComponentModel.ISupportInitialize)StashedShiny10).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label lbl_IP;
        private System.Windows.Forms.TextBox tB_IP;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.GroupBox gBControls;
        private System.Windows.Forms.Label lblThanks;
        private System.Windows.Forms.Button btnConnectUSB;
        private System.Windows.Forms.Button btnLeft;
        private System.Windows.Forms.Button btnRight;
        private System.Windows.Forms.Button btnBack;
        private System.Windows.Forms.Button btnForw;
        private System.Windows.Forms.NumericUpDown nUDDistance;
        private System.Windows.Forms.Label lblFreeWarp;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnRestore;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.ListBox lBCoords;
        private System.Windows.Forms.GroupBox gBShinyHunt;
        private System.Windows.Forms.ComboBox cBWhenShinyFound;
        private System.Windows.Forms.Label lblShinyFound;
        private System.Windows.Forms.Button btnWarp;
        private System.Windows.Forms.CheckedListBox cBSpecies;
        private System.Windows.Forms.Button btnResetSpecies;
        private System.Windows.Forms.Label lblSpecies;
        private System.Windows.Forms.Label lblFilter;
        private System.Windows.Forms.Label lblIVSpe;
        private System.Windows.Forms.Label lblIVSpD;
        private System.Windows.Forms.Label lblIVSpA;
        private System.Windows.Forms.ComboBox cBIVSpe;
        private System.Windows.Forms.ComboBox cBIVSpD;
        private System.Windows.Forms.ComboBox cBIVSpA;
        private System.Windows.Forms.Label lblIVDef;
        private System.Windows.Forms.Label lblIVAtk;
        private System.Windows.Forms.Label lblIVHP;
        private System.Windows.Forms.ComboBox cBIVDef;
        private System.Windows.Forms.ComboBox cBIVAtk;
        private System.Windows.Forms.ComboBox cBIVHP;
        private System.Windows.Forms.Label lblIV;
        private System.Windows.Forms.NumericUpDown nUDScaleMin;
        private System.Windows.Forms.NumericUpDown nUDScaleMax;
        private System.Windows.Forms.Label lblScaleMin;
        private System.Windows.Forms.Label lblScaleMax;
        private System.Windows.Forms.NumericUpDown nUDCheckTime;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown nUDCamMove;
        private System.Windows.Forms.Label lblCam;
        private System.Windows.Forms.NumericUpDown nUDSaveFreq;
        private System.Windows.Forms.Label lblSaveFreq;
        private GroupBox gBStashedShiny;
        private PictureBox StashedShiny1;
        private PictureBox StashedShiny3;
        private PictureBox StashedShiny5;
        private PictureBox StashedShiny7;
        private PictureBox StashedShiny9;
        private PictureBox StashedShiny2;
        private PictureBox StashedShiny4;
        private PictureBox StashedShiny6;
        private PictureBox StashedShiny8;
        private PictureBox StashedShiny10;
        private Button btnScreenOn;
        private Button btnScreenOff;
        private ToolTip ShinyInfo;
        private Button btnExport;
        private Button btnWebhookSettings;
        private Button btnResetFilters;
        private PictureBox pBAlpha;
        private CheckBox cBIsAlpha;
    }
}

