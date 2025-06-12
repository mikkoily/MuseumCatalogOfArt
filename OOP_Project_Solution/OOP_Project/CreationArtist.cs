using System;
using System.Drawing;
using System.Windows.Forms;
using OOP_Project.Models;

namespace OOP_Project {
    public partial class CreationArtist : Form {
        private DataViewer dataViewer;
        public CreationArtist(DataViewer viewer) {
            dataViewer = viewer;
            InitializeComponent();
        }

        private void InitializeComponent() {
            this.Size = new Size(500, 300);
            this.Text = "Create Artist";
            this.Padding = new Padding(10);
            this.StartPosition = FormStartPosition.CenterScreen;

            int yOffset = 20;

            Label nameLabel = new Label();
            nameLabel.Text = "Name";
            nameLabel.AutoSize = true;
            nameLabel.Location = new Point(10, yOffset);
            this.Controls.Add(nameLabel);
            yOffset += 20;

            TextBox nameBox = new TextBox();
            nameBox.Location = new Point(10, yOffset);
            nameBox.Size = new Size(460, 20);
            this.Controls.Add(nameBox);
            yOffset += 30;

            Label birthYearLabel = new Label();
            birthYearLabel.Text = "Birth Year";
            birthYearLabel.AutoSize = true;
            birthYearLabel.Location = new Point(10, yOffset);
            this.Controls.Add(birthYearLabel);
            yOffset += 20;

            TextBox birthYearBox = new TextBox();
            birthYearBox.Location = new Point(10, yOffset);
            birthYearBox.Size = new Size(460, 20);
            birthYearBox.KeyPress += (s, e) =>
            {
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                    e.Handled = true;
            };
            this.Controls.Add(birthYearBox);
            yOffset += 30;

            Label nationalityLabel = new Label();
            nationalityLabel.Text = "Nationality";
            nationalityLabel.AutoSize = true;
            nationalityLabel.Location = new Point(10, yOffset);
            this.Controls.Add(nationalityLabel);
            yOffset += 20;

            TextBox nationalityBox = new TextBox();
            nationalityBox.Location = new Point(10, yOffset);
            nationalityBox.Size = new Size(460, 20);
            this.Controls.Add(nationalityBox);
            yOffset += 30;

            Label deathYearLabel = new Label();
            deathYearLabel.Text = "Death Year (optional)";
            deathYearLabel.AutoSize = true;
            deathYearLabel.Location = new Point(10, yOffset);
            this.Controls.Add(deathYearLabel);
            yOffset += 20;

            TextBox deathYearBox = new TextBox();
            deathYearBox.Location = new Point(10, yOffset);
            deathYearBox.Size = new Size(460, 20);
            deathYearBox.KeyPress += (s, e) =>
            {
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                    e.Handled = true;
            };
            this.Controls.Add(deathYearBox);
            yOffset += 30;

            Button btnCreate = new Button();
            btnCreate.Text = "Create";
            btnCreate.Size = new Size(100, 30);
            btnCreate.Location = new Point(10, yOffset);
            btnCreate.Click += (s, e) =>
            {
                try {
                    string name = nameBox.Text.Trim();
                    if (string.IsNullOrEmpty(name)) { MessageBox.Show("Name is required"); return; }

                    string birthYearInput = birthYearBox.Text.Trim();
                    if (string.IsNullOrEmpty(birthYearInput)) { MessageBox.Show("Birth Year is required"); return; }
                    if (!int.TryParse(birthYearInput, out int birthYear)) {
                        MessageBox.Show("Invalid Birth Year: Please enter a valid number");
                        return;
                    }
                    if (birthYear < 1 || birthYear > DateTime.Now.Year) {
                        MessageBox.Show($"Birth Year must be between 1 and {DateTime.Now.Year}");
                        return;
                    }

                    string nationality = nationalityBox.Text.Trim();
                    if (string.IsNullOrEmpty(nationality)) { MessageBox.Show("Nationality is required"); return; }

                    int? deathYear = null;
                    string deathYearInput = deathYearBox.Text.Trim();
                    if (!string.IsNullOrEmpty(deathYearInput)) {
                        if (!int.TryParse(deathYearInput, out int dy)) {
                            MessageBox.Show("Invalid Death Year: Please enter a valid number");
                            return;
                        }
                        if (dy < birthYear || dy > DateTime.Now.Year) {
                            MessageBox.Show($"Death Year must be between {birthYear} and {DateTime.Now.Year}");
                            return;
                        }
                        deathYear = dy;
                    }

                    var artist = new Artist(name, birthYear, nationality, deathYear);
                    dataViewer.catalog.Artists.Add(artist);
                    dataViewer.catalog.SaveData();
                    this.Close();
                } catch (Exception ex) {
                    MessageBox.Show($"Error creating artist: {ex.Message}");
                }
            };
            this.Controls.Add(btnCreate);

            yOffset += 40;
            this.Size = new Size(500, yOffset + 40);
        }
    }

}