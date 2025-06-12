using System;
using System.Drawing;
using System.Windows.Forms;

namespace OOP_Project {
    public partial class MainMenu : Form {
        public MainMenu() {
            InitializeComponent();
        }

        private void InitializeComponent() {
            this.Size = new Size(400, 300);
            this.Text = "Main Menu";
            this.StartPosition = FormStartPosition.CenterScreen;

            int yOffset = 50;
            Button btnArtworks = new Button();
            btnArtworks.Text = "ARTWORKS";
            btnArtworks.Size = new Size(200, 30);
            btnArtworks.Location = new Point(100, yOffset);
            btnArtworks.Click += (s, e) =>
            {
                DataViewer viewer = new DataViewer(this);
                viewer.SetDataObject("Artworks");
                viewer.Show();
                this.Visible = false;
            };
            this.Controls.Add(btnArtworks);

            yOffset += 50;

            Button btnArtists = new Button();
            btnArtists.Text = "ARTISTS";
            btnArtists.Size = new Size(200, 30);
            btnArtists.Location = new Point(100, yOffset);
            btnArtists.Click += (s, e) =>
            {
                DataViewer viewer = new DataViewer(this);
                viewer.SetDataObject("Artists");
                viewer.Show();
                this.Visible = false;
            };
            this.Controls.Add(btnArtists);

            yOffset += 50;

            Button btnExhibitions = new Button();
            btnExhibitions.Text = "EXHIBITIONS";
            btnExhibitions.Size = new Size(200, 30);
            btnExhibitions.Location = new Point(100, yOffset);
            btnExhibitions.Click += (s, e) =>
            {
                DataViewer viewer = new DataViewer(this);
                viewer.SetDataObject("Exhibitions");
                viewer.Show();
                this.Visible = false;
            };
            this.Controls.Add(btnExhibitions);

            yOffset += 50;

            Button btnExit = new Button();
            btnExit.Text = "EXIT";
            btnExit.Size = new Size(200, 30);
            btnExit.Location = new Point(100, yOffset);
            btnExit.Click += (s, e) => this.Close();
            this.Controls.Add(btnExit);
        }
    }

}