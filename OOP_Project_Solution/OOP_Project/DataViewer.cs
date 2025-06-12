using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Diagnostics;
using OOP_Project.Models;

namespace OOP_Project {
    public partial class DataViewer : Form {
        public MainMenu mainMenu;
        public MuseumCatalog catalog;
        private string dataObject;
        private Dictionary<string, FieldType> dataObjectTypeMap;
        private DataGridView dataGrid;
        private ComboBox searchCombo;
        private TextBox searchTextEntry;
        private Button searchButton;
        private Button deleteButton;
        private Button returnButton;
        private Panel panel1;
        private Panel panel2;
        private TableLayoutPanel layoutPanel;

        public DataViewer(MainMenu menu) {
            mainMenu = menu;
            catalog = new MuseumCatalog();
            dataObjectTypeMap = new Dictionary<string, FieldType>();
            InitializeComponent();
        }

        private void InitializeComponent() {
            this.Size = new Size(800, 600);
            this.Text = "Data Viewer";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormClosed += DataViewer_FormClosed;

            // Main TableLayoutPanel for layout management
            layoutPanel = new TableLayoutPanel();
            layoutPanel.Dock = DockStyle.Fill;
            layoutPanel.RowCount = 3;
            layoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 60)); // Top panel height
            layoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100)); // DataGridView takes remaining space
            layoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 60)); // Bottom panel height
            this.Controls.Add(layoutPanel);

            // Top panel for search controls
            panel1 = new Panel();
            panel1.Dock = DockStyle.Fill;
            panel1.Padding = new Padding(0, 0, 0, 5); // Padding at the bottom to separate from grid
            layoutPanel.Controls.Add(panel1, 0, 0);

            Label label1 = new Label();
            label1.Text = "Search by:";
            label1.AutoSize = true;
            label1.Location = new Point(10, 15);
            panel1.Controls.Add(label1);

            searchCombo = new ComboBox();
            searchCombo.Location = new Point(80, 10);
            searchCombo.Size = new Size(150, 20);
            searchCombo.SelectedIndexChanged += searchComboBox_SelectedIndexChanged;
            panel1.Controls.Add(searchCombo);

            searchTextEntry = new TextBox();
            searchTextEntry.Location = new Point(240, 10);
            searchTextEntry.Size = new Size(200, 20);
            panel1.Controls.Add(searchTextEntry);

            searchButton = new Button();
            searchButton.Text = "Search";
            searchButton.Location = new Point(450, 10);
            searchButton.Size = new Size(80, 25);
            searchButton.Click += buttonSearch_Click;
            panel1.Controls.Add(searchButton);

            deleteButton = new Button();
            deleteButton.Text = "Delete";
            deleteButton.Location = new Point(540, 10);
            deleteButton.Size = new Size(80, 25);
            deleteButton.Click += buttonDelete_Click;
            panel1.Controls.Add(deleteButton);

            returnButton = new Button();
            returnButton.Text = "Return";
            returnButton.Location = new Point(630, 10);
            returnButton.Size = new Size(80, 25);
            returnButton.Click += buttonReturn_Click;
            panel1.Controls.Add(returnButton);

            // DataGridView
            dataGrid = new DataGridView();
            dataGrid.Dock = DockStyle.Fill;
            dataGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGrid.ReadOnly = true;
            dataGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGrid.ColumnHeadersVisible = true;
            layoutPanel.Controls.Add(dataGrid, 0, 1);

            // Bottom panel for creation buttons
            panel2 = new Panel();
            panel2.Dock = DockStyle.Fill;
            panel2.Padding = new Padding(0, 5, 0, 0); // Padding at the top to separate from grid
            layoutPanel.Controls.Add(panel2, 0, 2);

            Debug.WriteLine($"Panel1 Height: {panel1.Height}, DataGrid Height: {dataGrid.Height}, Panel2 Height: {panel2.Height}");
        }

        public string GetDataObject() {
            return dataObject;
        }

        private void DefineObjectProperty(string name, FieldType fieldType, bool primary) {
            searchCombo.Items.Add(name);

            string columnName = name.Replace(" ", "");
            Debug.WriteLine($"Defining column: HeaderText={name}, Name={columnName}, DataPropertyName={columnName}");
            DataGridViewColumn column = new DataGridViewColumn();
            column.HeaderText = name;
            column.Name = columnName;
            column.DataPropertyName = columnName;
            column.CellTemplate = new DataGridViewTextBoxCell();
            dataGrid.Columns.Add(column);

            dataObjectTypeMap[name] = fieldType;
        }

        public void DeployCreationButton(string name, Type classType, string[] extraProperties) {
            var button = new DataViewerCreationButton();
            button.Text = $"Create {name}";
            button.Parent = panel2;
            button.Dock = DockStyle.Left;
            button.AutoSize = true;
            button.extraProperties = extraProperties;
            button.classType = classType;
            button.Click += (sender, e) => InitCreationForm((DataViewerCreationButton)sender);
            panel2.Controls.Add(button);
        }

        public void LoadObjects(string objectName) {
            Debug.WriteLine($"Loading objects for: {objectName}");
            catalog.LoadData();
            int existingRows = dataGrid.Rows.Count - (dataGrid.ColumnHeadersVisible ? 1 : 0); // Subtract header row if visible
            Debug.WriteLine($"Existing rows (excluding header): {existingRows}");
            if (existingRows > 0) {
                dataGrid.Rows.Clear();
            }
            Debug.WriteLine($"Grid columns: {string.Join(", ", dataGrid.Columns.Cast<DataGridViewColumn>().Select(c => c.Name))}");
            switch (objectName) {
                case "Artworks":
                    Debug.WriteLine($"Found {catalog.Artworks.Count} artworks");
                    if (catalog.Artworks.Count > 0) {
                        foreach (var artwork in catalog.Artworks) {
                            int index = dataGrid.Rows.Add();
                            DataGridViewRow row = dataGrid.Rows[index];
                            row.Cells["Title"].Value = artwork.Title ?? "";
                            row.Cells["Year"].Value = artwork.Year.ToString();
                            row.Cells["Medium"].Value = artwork.Medium ?? "";
                            row.Cells["Weight"].Value = artwork.Weight ?? "";
                            row.Cells["Location"].Value = artwork.Location ?? "";
                            row.Cells["Artist"].Value = artwork.Artist?.Name ?? "";
                            if (artwork is Painting painting)
                                row.Cells["FrameType"].Value = painting.FrameType ?? "";
                            else if (artwork is Sculpture sculpture)
                                row.Cells["Material"].Value = sculpture.Material ?? "";
                            Debug.WriteLine($"Added artwork: Title={artwork.Title}, Year={artwork.Year}, Artist={artwork.Artist?.Name}");
                        }
                    }
                    break;
                case "Artists":
                    Debug.WriteLine($"Found {catalog.Artists.Count} artists");
                    if (catalog.Artists.Count > 0) {
                        foreach (var artist in catalog.Artists) {
                            int index = dataGrid.Rows.Add();
                            DataGridViewRow row = dataGrid.Rows[index];
                            row.Cells["Name"].Value = artist.Name ?? "";
                            row.Cells["BirthYear"].Value = artist.BirthYear.ToString();
                            row.Cells["Nationality"].Value = artist.Nationality ?? "";
                            row.Cells["DeathYear"].Value = artist.DeathYear?.ToString() ?? "";
                            Debug.WriteLine($"Added artist: Name={artist.Name}, BirthYear={artist.BirthYear}, Nationality={artist.Nationality}, DeathYear={artist.DeathYear}");
                        }
                    }
                    break;
                case "Exhibitions":
                    Debug.WriteLine($"Found {catalog.Exhibitions.Count} exhibitions");
                    if (catalog.Exhibitions.Count > 0) {
                        foreach (var exhibition in catalog.Exhibitions) {
                            int index = dataGrid.Rows.Add();
                            DataGridViewRow row = dataGrid.Rows[index];
                            row.Cells["Title"].Value = exhibition.Title ?? "";
                            row.Cells["StartDate"].Value = exhibition.StartDate.ToShortDateString();
                            row.Cells["EndDate"].Value = exhibition.EndDate.ToShortDateString();
                            row.Cells["ArtworkCount"].Value = exhibition.Artworks.Count.ToString();
                            Debug.WriteLine($"Added exhibition: Title={exhibition.Title}, StartDate={exhibition.StartDate}, EndDate={exhibition.EndDate}");
                        }
                    }
                    break;
            }
            dataGrid.Refresh();
            dataGrid.Update(); // Force UI update
            Debug.WriteLine($"Grid refreshed with {dataGrid.Rows.Count} rows");
        }

        public void SetDataObject(string objectName) {
            this.Text = objectName;
            dataObject = objectName;
            dataGrid.Columns.Clear();
            searchCombo.Items.Clear();
            dataObjectTypeMap.Clear();
            panel2.Controls.Clear();

            switch (objectName) {
                case "Artworks":
                    DefineObjectProperty("Title", FieldType.Text, true);
                    DefineObjectProperty("Year", FieldType.Number, true);
                    DefineObjectProperty("Medium", FieldType.Text, true);
                    DefineObjectProperty("Weight", FieldType.Text, true);
                    DefineObjectProperty("Location", FieldType.Text, true);
                    DefineObjectProperty("Artist", FieldType.Text, true);
                    DefineObjectProperty("Frame Type", FieldType.Text, false);
                    DefineObjectProperty("Material", FieldType.Text, false);

                    DeployCreationButton("Painting", typeof(Painting), new string[] { "Frame Type" });
                    DeployCreationButton("Sculpture", typeof(Sculpture), new string[] { "Material" });
                    break;
                case "Artists":
                    DefineObjectProperty("Name", FieldType.Text, true);
                    DefineObjectProperty("Birth Year", FieldType.Number, true);
                    DefineObjectProperty("Nationality", FieldType.Text, true);
                    DefineObjectProperty("Death Year", FieldType.Number, true);

                    var artistButton = new DataViewerCreationButton();
                    artistButton.Text = "Create Artist";
                    artistButton.Parent = panel2;
                    artistButton.Dock = DockStyle.Left;
                    artistButton.AutoSize = true;
                    artistButton.Click += (sender, e) =>
                    {
                        CreationArtist artist = new CreationArtist(this);
                        artist.FormClosed += (s, args) =>
                        {
                            Debug.WriteLine("CreationArtist closed, refreshing DataViewer");
                            LoadObjects(objectName);
                        };
                        artist.ShowDialog();
                    };
                    panel2.Controls.Add(artistButton);
                    break;
                case "Exhibitions":
                    DefineObjectProperty("Title", FieldType.Text, true);
                    DefineObjectProperty("Start Date", FieldType.DateTime, true);
                    DefineObjectProperty("End Date", FieldType.DateTime, true);
                    DefineObjectProperty("Artwork Count", FieldType.Number, false);

                    var exhibitionButton = new DataViewerCreationButton();
                    exhibitionButton.Text = "Create Exhibition";
                    exhibitionButton.Parent = panel2;
                    exhibitionButton.Dock = DockStyle.Left;
                    exhibitionButton.AutoSize = true;
                    exhibitionButton.Click += (sender, e) => InitExhibitionForm();
                    panel2.Controls.Add(exhibitionButton);
                    break;
                default:
                    throw new Exception("Invalid object");
            }

            LoadObjects(objectName);
        }

        private void InitExhibitionForm() {
            Form form = new Form();
            form.Size = new Size(500, 300); // Начальная высота будет скорректирована ниже
            form.Text = "Create Exhibition";
            form.Padding = new Padding(10);
            form.StartPosition = FormStartPosition.CenterScreen;
            int yOffset = 20;

            Label titleLabel = new Label();
            titleLabel.Text = "Title";
            titleLabel.AutoSize = true;
            titleLabel.Location = new Point(10, yOffset);
            form.Controls.Add(titleLabel);
            yOffset += 20;

            TextBox titleBox = new TextBox();
            titleBox.Location = new Point(10, yOffset);
            titleBox.Size = new Size(460, 20);
            form.Controls.Add(titleBox);
            yOffset += 30;

            Label startDateLabel = new Label();
            startDateLabel.Text = "Start Date (YYYY-MM-DD)";
            startDateLabel.AutoSize = true;
            startDateLabel.Location = new Point(10, yOffset);
            form.Controls.Add(startDateLabel);
            yOffset += 20;

            TextBox startDateBox = new TextBox();
            startDateBox.Location = new Point(10, yOffset);
            startDateBox.Size = new Size(460, 20);
            form.Controls.Add(startDateBox);
            yOffset += 30;

            Label endDateLabel = new Label();
            endDateLabel.Text = "End Date (YYYY-MM-DD)";
            endDateLabel.AutoSize = true;
            endDateLabel.Location = new Point(10, yOffset);
            form.Controls.Add(endDateLabel);
            yOffset += 20;

            TextBox endDateBox = new TextBox();
            endDateBox.Location = new Point(10, yOffset);
            endDateBox.Size = new Size(460, 20);
            form.Controls.Add(endDateBox);
            yOffset += 30;

            Button btnCreate = new Button();
            btnCreate.Text = "Create";
            btnCreate.Size = new Size(100, 30);
            btnCreate.Location = new Point(10, yOffset);
            btnCreate.Click += (s, e) =>
            {
                try {
                    string title = titleBox.Text.Trim();
                    if (string.IsNullOrEmpty(title)) { MessageBox.Show("Title is required"); return; }

                    if (!DateTime.TryParse(startDateBox.Text, out DateTime startDate)) {
                        MessageBox.Show("Invalid start date format (use YYYY-MM-DD)");
                        return;
                    }

                    if (!DateTime.TryParse(endDateBox.Text, out DateTime endDate)) {
                        MessageBox.Show("Invalid end date format (use YYYY-MM-DD)");
                        return;
                    }

                    if (endDate < startDate) {
                        MessageBox.Show("End date must be after start date");
                        return;
                    }

                    var exhibition = new Exhibition(title, startDate, endDate);
                    catalog.Exhibitions.Add(exhibition);
                    catalog.SaveData();
                    LoadObjects(dataObject);
                    form.Close();
                } catch (Exception ex) {
                    MessageBox.Show($"Error creating exhibition: {ex.Message}");
                    Debug.WriteLine($"Error creating exhibition: {ex.Message}");
                }
            };
            form.Controls.Add(btnCreate);

            // Dynamically set form height based on yOffset
            yOffset += 40; // Add space for the button and some padding
            form.Size = new Size(500, yOffset + 40); // Add extra 20 pixels for aesthetics
            form.ShowDialog();

        }

        private void InitCreationForm(DataViewerCreationButton sender) {
            Form form = new Form();
            form.Size = new Size(500, 300); // Начальная высота будет скорректирована ниже
            form.Text = $"Create {sender.Text.Replace("Create ", "")}";
            form.Padding = new Padding(10);
            form.StartPosition = FormStartPosition.CenterScreen;

            List<string> properties = new List<string> { "Title", "Year", "Medium", "Weight", "Location", "Artist" };
            properties.AddRange(sender.extraProperties);
            TextBox[] textBoxes = new TextBox[properties.Count - 1]; // Exclude Artist (ComboBox)
            ComboBox artistCombo = null;
            int yOffset = 20;

            // Create fields in correct order (top to bottom: FrameType/Material, Artist, Location, Weight, Medium, Year, Title)
            for (int i = 0; i < properties.Count; i++) {
                string name = properties[i];
                Label label = new Label();
                label.Text = name;
                label.AutoSize = true;
                label.Location = new Point(10, yOffset);
                form.Controls.Add(label);
                yOffset += 20;

                if (name == "Artist") {
                    artistCombo = new ComboBox();
                    artistCombo.Location = new Point(10, yOffset);
                    artistCombo.Size = new Size(460, 20);
                    foreach (var artist in catalog.Artists)
                        artistCombo.Items.Add(artist.Name);
                    artistCombo.DropDownStyle = ComboBoxStyle.DropDownList;
                    form.Controls.Add(artistCombo);
                    yOffset += 30;
                } else {
                    int textBoxIndex = name == "Title" ? 0 :
                                      name == "Year" ? 1 :
                                      name == "Medium" ? 2 :
                                      name == "Weight" ? 3 :
                                      name == "Location" ? 4 : 5; // FrameType or Material
                    textBoxes[textBoxIndex] = new TextBox();
                    textBoxes[textBoxIndex].Location = new Point(10, yOffset);
                    textBoxes[textBoxIndex].Size = new Size(460, 20);
                    if (name == "Year") {
                        textBoxes[textBoxIndex].KeyPress += (s, e) =>
                        {
                            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                                e.Handled = true;
                        };
                        textBoxes[textBoxIndex].TextChanged += (s, e) =>
                        {
                            TextBox tb = (TextBox)s;
                            if (!string.IsNullOrEmpty(tb.Text) && !int.TryParse(tb.Text, out _))
                                tb.Text = new string(tb.Text.Where(char.IsDigit).ToArray());
                        };
                    }
                    form.Controls.Add(textBoxes[textBoxIndex]);
                    yOffset += 30;
                }
            }

            Button btnCreate = new Button();
            btnCreate.Text = "Create";
            btnCreate.Size = new Size(100, 30);
            btnCreate.Location = new Point(10, yOffset);
            btnCreate.Click += (s, e) =>
            {
                try {
                    string title = textBoxes[0].Text.Trim();
                    if (string.IsNullOrEmpty(title)) { MessageBox.Show("Title is required"); return; }

                    string yearInput = textBoxes[1].Text.Trim();
                    Debug.WriteLine($"Year input: '{yearInput}'");
                    if (string.IsNullOrEmpty(yearInput)) { MessageBox.Show("Year is required"); return; }
                    if (!int.TryParse(yearInput, out int year)) {
                        MessageBox.Show("Invalid year: Please enter a valid number");
                        return;
                    }
                    if (year < 1 || year > DateTime.Now.Year) {
                        MessageBox.Show($"Year must be between 1 and {DateTime.Now.Year}");
                        return;
                    }

                    string medium = textBoxes[2].Text.Trim();
                    if (string.IsNullOrEmpty(medium)) { MessageBox.Show("Medium is required"); return; }

                    string weight = textBoxes[3].Text.Trim();
                    if (string.IsNullOrEmpty(weight)) { MessageBox.Show("Weight is required"); return; }

                    string location = textBoxes[4].Text.Trim();
                    if (string.IsNullOrEmpty(location)) { MessageBox.Show("Location is required"); return; }

                    Artist artist = catalog.Artists.FirstOrDefault(a => a.Name == (string)artistCombo.SelectedItem);
                    if (artist == null) { MessageBox.Show("Please select an artist"); return; }

                    Artwork newArtwork;
                    if (sender.classType == typeof(Painting)) {
                        string frameType = textBoxes[5].Text.Trim();
                        if (string.IsNullOrEmpty(frameType)) { MessageBox.Show("Frame Type is required"); return; }
                        newArtwork = new Painting(title, year, medium, weight, location, artist, frameType);
                    } else // Sculpture
                      {
                        string material = textBoxes[5].Text.Trim();
                        if (string.IsNullOrEmpty(material)) { MessageBox.Show("Material is required"); return; }
                        newArtwork = new Sculpture(title, year, medium, weight, location, artist, material);
                    }

                    catalog.Artworks.Add(newArtwork);
                    artist.Artworks.Add(newArtwork);
                    catalog.SaveData();
                    LoadObjects(dataObject);
                    Debug.WriteLine($"Created artwork: {newArtwork.Title}");
                    form.Close();
                } catch (Exception ex) {
                    MessageBox.Show($"Error creating artwork: {ex.Message}");
                    Debug.WriteLine($"Error creating artwork: {ex.Message}");
                }
            };
            form.Controls.Add(btnCreate);

            // Dynamically set form height based on yOffset
            yOffset += 40; // Add space for the button and some padding
            form.Size = new Size(500, yOffset + 60); // Add extra 20 pixels for aesthetics
            form.ShowDialog();

        }

        private void DataViewer_FormClosed(object sender, FormClosedEventArgs e) {
            mainMenu.Visible = true;
        }

        private void buttonSearch_Click(object sender, EventArgs e) {
            if (string.IsNullOrEmpty(searchCombo.Text) || string.IsNullOrEmpty(searchTextEntry.Text)) {
                LoadObjects(dataObject);
                return;
            }

            string criteria = searchCombo.Text.Replace(" ", "");
            string searchText = searchTextEntry.Text.Trim().ToLower();
            dataGrid.Rows.Clear();

            if (dataObject == "Artists") {
                foreach (var artist in catalog.Artists) {
                    bool matches = false;
                    switch (criteria) {
                        case "Name":
                            matches = artist.Name.ToLower().Contains(searchText);
                            break;
                        case "Nationality":
                            matches = artist.Nationality.ToLower().Contains(searchText);
                            break;
                        case "BirthYear":
                            if (int.TryParse(searchText, out int year))
                                matches = artist.BirthYear == year;
                            break;
                        case "DeathYear":
                            if (int.TryParse(searchText, out int deathYear))
                                matches = artist.DeathYear.HasValue && artist.DeathYear.Value == deathYear;
                            break;
                    }

                    if (matches) {
                        int index = dataGrid.Rows.Add();
                        DataGridViewRow row = dataGrid.Rows[index];
                        row.Cells["Name"].Value = artist.Name ?? "";
                        row.Cells["BirthYear"].Value = artist.BirthYear.ToString();
                        row.Cells["Nationality"].Value = artist.Nationality ?? "";
                        row.Cells["DeathYear"].Value = artist.DeathYear?.ToString() ?? "";
                    }
                }
            } else if (dataObject == "Artworks") {
                foreach (var artwork in catalog.Artworks) {
                    bool matches = false;
                    switch (criteria) {
                        case "Title":
                            matches = artwork.Title.ToLower().Contains(searchText);
                            break;
                        case "Year":
                            if (int.TryParse(searchText, out int year))
                                matches = artwork.Year == year;
                            break;
                        case "Medium":
                            matches = artwork.Medium.ToLower().Contains(searchText);
                            break;
                        case "Weight":
                            matches = artwork.Weight.ToLower().Contains(searchText);
                            break;
                        case "Location":
                            matches = artwork.Location.ToLower().Contains(searchText);
                            break;
                        case "Artist":
                            matches = artwork.Artist?.Name.ToLower().Contains(searchText) ?? false;
                            break;
                        case "FrameType":
                            matches = artwork is Painting painting && painting.FrameType.ToLower().Contains(searchText);
                            break;
                        case "Material":
                            matches = artwork is Sculpture sculpture && sculpture.Material.ToLower().Contains(searchText);
                            break;
                    }

                    if (matches) {
                        int index = dataGrid.Rows.Add();
                        DataGridViewRow row = dataGrid.Rows[index];
                        row.Cells["Title"].Value = artwork.Title ?? "";
                        row.Cells["Year"].Value = artwork.Year.ToString();
                        row.Cells["Medium"].Value = artwork.Medium ?? "";
                        row.Cells["Weight"].Value = artwork.Weight ?? "";
                        row.Cells["Location"].Value = artwork.Location ?? "";
                        row.Cells["Artist"].Value = artwork.Artist?.Name ?? "";
                        if (artwork is Painting painting)
                            row.Cells["FrameType"].Value = painting.FrameType ?? "";
                        else if (artwork is Sculpture sculpture)
                            row.Cells["Material"].Value = sculpture.Material ?? "";
                    }
                }
            } else if (dataObject == "Exhibitions") {
                foreach (var exhibition in catalog.Exhibitions) {
                    bool matches = false;
                    switch (criteria) {
                        case "Title":
                            matches = exhibition.Title.ToLower().Contains(searchText);
                            break;
                        case "StartDate":
                            if (DateTime.TryParse(searchText, out DateTime startDate))
                                matches = exhibition.StartDate.Date == startDate.Date;
                            break;
                        case "EndDate":
                            if (DateTime.TryParse(searchText, out DateTime endDate))
                                matches = exhibition.EndDate.Date == endDate.Date;
                            break;
                        case "ArtworkCount":
                            if (int.TryParse(searchText, out int count))
                                matches = exhibition.Artworks.Count == count;
                            break;
                    }

                    if (matches) {
                        int index = dataGrid.Rows.Add();
                        DataGridViewRow row = dataGrid.Rows[index];
                        row.Cells["Title"].Value = exhibition.Title ?? "";
                        row.Cells["StartDate"].Value = exhibition.StartDate.ToShortDateString();
                        row.Cells["EndDate"].Value = exhibition.EndDate.ToShortDateString();
                        row.Cells["ArtworkCount"].Value = exhibition.Artworks.Count.ToString();
                    }
                }
            }
            dataGrid.Refresh();
            dataGrid.Update();
        }

        private void buttonDelete_Click(object sender, EventArgs e) {
            if (dataGrid.SelectedRows.Count == 0) {
                MessageBox.Show("Please select a row to delete");
                return;
            }

            if (MessageBox.Show("Are you sure you want to delete the selected item?", "Confirm Delete", MessageBoxButtons.YesNo) == DialogResult.No)
                return;

            try {
                int selectedIndex = dataGrid.SelectedRows[0].Index;
                if (dataObject == "Artworks") {
                    string title = (string)dataGrid.Rows[selectedIndex].Cells["Title"].Value;
                    var artwork = catalog.Artworks.FirstOrDefault(a => a.Title == title);
                    if (artwork != null) {
                        catalog.Artworks.Remove(artwork);
                        if (artwork.Artist != null)
                            artwork.Artist.Artworks.Remove(artwork);
                        foreach (var exhibition in catalog.Exhibitions)
                            exhibition.Artworks.Remove(artwork);
                    }
                } else if (dataObject == "Artists") {
                    string name = (string)dataGrid.Rows[selectedIndex].Cells["Name"].Value;
                    var artist = catalog.Artists.FirstOrDefault(a => a.Name == name);
                    if (artist != null) {
                        if (artist.Artworks.Any()) {
                            MessageBox.Show("Cannot delete artist with associated artworks");
                            return;
                        }
                        catalog.Artists.Remove(artist);
                    }
                } else if (dataObject == "Exhibitions") {
                    string title = (string)dataGrid.Rows[selectedIndex].Cells["Title"].Value;
                    var exhibition = catalog.Exhibitions.FirstOrDefault(ex => ex.Title == title);
                    if (exhibition != null)
                        catalog.Exhibitions.Remove(exhibition);
                }

                catalog.SaveData();
                LoadObjects(dataObject);
            } catch (Exception ex) {
                MessageBox.Show($"Error deleting item: {ex.Message}");
                Debug.WriteLine($"Error deleting item: {ex.Message}");
            }
        }

        private void searchComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            searchTextEntry.Clear();
        }

        private void buttonReturn_Click(object sender, EventArgs e) {
            this.Close();
        }
    }

}