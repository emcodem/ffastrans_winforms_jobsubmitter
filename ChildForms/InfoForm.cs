using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FFAStrans_Concat_Submitter.ChildForms
{
    public partial class InfoForm : Form
    {
        public InfoForm()
        {
            InitializeComponent();
            textBox1.Text = "This program has no license, it can freely be used, distributed, modified without any restrictions" + Environment.NewLine +
                            "" + Environment.NewLine +
                            "Usage: Drag and Drop or Browse for files and add them to the list. Once you have a List of files (or a single file), select your FFAStrans Workflow and hit the start button." + Environment.NewLine +
                            Environment.NewLine +
                            "Configuration: In the same folder where this program (.exe) is, you should also find config.toml. Edit this with a text editor" + Environment.NewLine +
                            "You Can add as many workflows as you like by adding [[workflow]] and after this line, you add name and startproc" + Environment.NewLine +
                            "name cannot be empty, it is the name of your FFAStrans workflow. startproc can be empty (\"\") or a processor guid of your FFAStrans workflow that has multiple starting processors" +
                            "if you add more than just name and startproc under [[workflow]], it will be handed over as user variable. E.g. " + Environment.NewLine +
                            "s_outputpath = \"\\\\server\\path\\\"" + Environment.NewLine +
                            "" + Environment.NewLine +
                            "" + Environment.NewLine +
                            "" + Environment.NewLine;
            textBox1.Select(0, 0);
        }

   
    }
}
