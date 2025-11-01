namespace ZAWarper
{
    partial class WarpProgressForm
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
            lblWarping = new System.Windows.Forms.Label();
            SuspendLayout();
            // 
            // lblWarping
            // 
            lblWarping.Location = new System.Drawing.Point(12, 9);
            lblWarping.Name = "lblWarping";
            lblWarping.Size = new System.Drawing.Size(308, 89);
            lblWarping.TabIndex = 0;
            lblWarping.Text = "Warping...";
            lblWarping.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // WarpProgressForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(332, 107);
            ControlBox = false;
            Controls.Add(lblWarping);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            Name = "WarpProgressForm";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            TopMost = true;
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Label lblWarping;
    }
}