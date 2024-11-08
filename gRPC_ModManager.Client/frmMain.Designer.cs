namespace gRPC_ModManager.Client;

partial class frmMain
{
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    ///  Clean up any resources being used.
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
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        btnStart = new Button();
        txtPath = new TextBox();
        progressBar1 = new ProgressBar();
        btnOpenFolder = new Button();
        SuspendLayout();
        // 
        // btnStart
        // 
        btnStart.Location = new Point(94, 89);
        btnStart.Name = "btnStart";
        btnStart.Size = new Size(198, 28);
        btnStart.TabIndex = 0;
        btnStart.Text = "Start";
        btnStart.UseVisualStyleBackColor = true;
        btnStart.Click += btnStart_Click;
        // 
        // txtPath
        // 
        txtPath.Location = new Point(24, 43);
        txtPath.Name = "txtPath";
        txtPath.Size = new Size(342, 23);
        txtPath.TabIndex = 1;
        // 
        // progressBar1
        // 
        progressBar1.Location = new Point(47, 153);
        progressBar1.Name = "progressBar1";
        progressBar1.Size = new Size(298, 8);
        progressBar1.TabIndex = 2;
        // 
        // btnOpenFolder
        // 
        btnOpenFolder.Location = new Point(365, 42);
        btnOpenFolder.Name = "btnOpenFolder";
        btnOpenFolder.Size = new Size(31, 25);
        btnOpenFolder.TabIndex = 0;
        btnOpenFolder.Text = "...";
        btnOpenFolder.UseVisualStyleBackColor = true;
        btnOpenFolder.Click += btnOpenFolder_Click;
        // 
        // frmMain
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(408, 193);
        Controls.Add(progressBar1);
        Controls.Add(txtPath);
        Controls.Add(btnOpenFolder);
        Controls.Add(btnStart);
        Name = "frmMain";
        Text = "Form1";
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private Button btnStart;
    private TextBox txtPath;
    private ProgressBar progressBar1;
    private Button btnOpenFolder;
}