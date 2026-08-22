class DeathScreenForm : Form
{
    Image monsterHeadImg;
    Image deathHandImg;
    Image placeholderIconImg;
    Bitmap desktopScreenshot;
    Panel flashPanel;
    System.Windows.Forms.Timer lightTimer;
    System.Windows.Forms.Timer glitchTimer;
    Random rand = new Random();
    
    int flashCount = 0;
    int closeAttempts = 0;
    string deathText = "Y O U   A R E   N O T   F R E E";

    Color glassColor = Color.FromArgb(220, 10, 100, 200);
    Color borderColor = Color.FromArgb(150, 150, 150);

    public DeathScreenForm(Image head, Image hand, Image placeholder)
    {
        this.monsterHeadImg = head;
        this.deathHandImg = hand;
        this.placeholderIconImg = placeholder;
        
        desktopScreenshot = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
        using (Graphics g = Graphics.FromImage(desktopScreenshot))
        {
            g.CopyFromScreen(0, 0, 0, 0, desktopScreenshot.Size);
        }

        this.FormBorderStyle = FormBorderStyle.None;
        this.StartPosition = FormStartPosition.CenterScreen;
        this.Size = Screen.PrimaryScreen.Bounds.Size;
        this.BackColor = Color.Black;
        this.DoubleBuffered = true;
        this.TopMost = true;

        flashPanel = new Panel();
        flashPanel.Size = this.Size;
        flashPanel.Location = new Point(0, 0);
        flashPanel.BackColor = Color.Transparent;
        flashPanel.SendToBack();
        this.Controls.Add(flashPanel);

        // Fast lighting pulse
        lightTimer = new System.Windows.Forms.Timer();
        lightTimer.Interval = 60;
        lightTimer.Tick += PulsateLighting;
        lightTimer.Start();

        // New aggressive jitter timer (shakes the entire death screen randomly)
        glitchTimer = new System.Windows.Forms.Timer();
        glitchTimer.Interval = 50;
        glitchTimer.Tick += (s, e) => {
            if (rand.Next(0, 3) == 0)
            {
                this.Location = new Point(
                    (Screen.PrimaryScreen.Bounds.Width - this.Width) / 2 + rand.Next(-15, 15),
                    (Screen.PrimaryScreen.Bounds.Height - this.Height) / 2 + rand.Next(-15, 15)
                );
            }
        };
        glitchTimer.Start();

        AddPlaceholderButton();
    }

    private void AddPlaceholderButton()
    {
        Button specialButton = new Button();
        specialButton.Size = new Size(64, 64);
        specialButton.Location = new Point((this.Width / 2) - 32, (this.Height / 2) + 120);
        specialButton.FlatStyle = FlatStyle.Flat;
        specialButton.FlatAppearance.BorderSize = 1;
        specialButton.FlatAppearance.BorderColor = Color.DarkGray;
        if (placeholderIconImg != null)
        {
            specialButton.Image = new Bitmap(placeholderIconImg, 48, 48);
        }
        specialButton.Cursor = Cursors.Hand;
        specialButton.Click += (s, e) => {
            new Thread(() => Console.Beep(rand.Next(2000, 6000), 40)).Start();
            MessageBox.Show("ACCESS DENIED", "", MessageBoxButtons.OK, MessageBoxIcon.Stop);
        };
        this.Controls.Add(specialButton);
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        if (e.CloseReason == CloseReason.UserClosing)
        {
            closeAttempts++;
            e.Cancel = true; // Still locks them in completely

            string[] taunts = { "N I C E   T R Y", "T H E R E   I S   N O   E S C A P E", "Y O U   C A N T   C L O S E   M E", "E R R O R :   L O C K E D" };
            deathText = taunts[rand.Next(taunts.Length)];
            
            flashPanel.BackColor = Color.FromArgb(rand.Next(100, 220), Color.DarkRed);
            new Thread(() => Console.Beep(rand.Next(200, 900), 150)).Start();
            this.Invalidate();
        }
    }

    private void PulsateLighting(object sender, EventArgs e)
    {
        flashCount++;
        int alpha = rand.Next(80, 180);
        if (rand.Next(0, 3) == 0) flashPanel.BackColor = Color.FromArgb(alpha, Color.DarkRed);
        else flashPanel.BackColor = Color.Transparent;
        this.Invalidate();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        Graphics g = e.Graphics;

        if (desktopScreenshot != null) g.DrawImage(desktopScreenshot, this.ClientRectangle);

        int winWidth = 600;
        int winHeight = 350;
        int winX = (this.Width - winWidth) / 2;
        int winY = (this.Height - winHeight) / 2;

        using (LinearGradientBrush glassBrush = new LinearGradientBrush(new Point(winX, winY), new Point(winX, winY + 30), glassColor, glassColor))
        {
            g.FillRectangle(glassBrush, winX, winY, winWidth, 30);
        }
        using (Pen framePen = new Pen(borderColor, 2))
        {
            framePen.Alignment = PenAlignment.Inset;
            g.DrawRectangle(framePen, winX, winY, winWidth, winHeight);
            g.FillRectangle(new SolidBrush(Color.FromArgb(210, 0, 0, 0)), winX, winY + 30, winWidth, winHeight - 30);
        }

        g.DrawString("Amr010920 - Fatal System Exception", new Font("Tahoma", 10, FontStyle.Bold), Brushes.White, winX + 5, winY + 8);
        g.DrawString("X", new Font("Arial", 12, FontStyle.Bold), Brushes.DarkRed, winX + winWidth - 25, winY + 6);

        // Draw flickering death text
        g.DrawString(deathText, new Font("Tahoma", 16, FontStyle.Bold), Brushes.Red, winX + 60 + rand.Next(-3, 4), winY + 120);

        if (deathHandImg != null)
        {
            g.DrawImage(deathHandImg, winX - 100, winY + 30, 120, 250);
            Image flippedHand = (Image)deathHandImg.Clone();
            flippedHand.RotateFlip(RotateFlipType.RotateNoneFlipX);
            g.DrawImage(flippedHand, winX + winWidth - 20, winY + 30, 120, 250);
        }

        if (monsterHeadImg != null)
        {
            g.DrawImage(monsterHeadImg, (this.Width / 2) - 100, winY - 110, 200, 150);
            if (placeholderIconImg != null)
            {
                g.DrawImage(placeholderIconImg, (this.Width / 2) - 24, winY - 80, 48, 48);
            }
        }
    }
}