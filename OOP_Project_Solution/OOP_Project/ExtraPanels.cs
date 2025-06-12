using System;
using System.Windows.Forms;

namespace OOP_Project {
    public enum FieldType { Text, Number, DateTime }

    public class DataViewerCreationButton : Button {
        public Type classType;
        public string[] extraProperties = new string[] { };
    }

}