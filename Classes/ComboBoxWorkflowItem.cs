using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFAStrans_Concat_Submitter.Classes
{

    public class ComboBoxWorkflowItem
    {
        public string Name { get; set; }
        //public WorkflowConfig Config { get; set; }
        public string WorkflowGUID;
        public string StartProcGUID;
        public Dictionary<string, string>  Variables;

        public ComboBoxWorkflowItem(Dictionary<string, object> wf_config)
        {
            //WorkflowConfig c = new WorkflowConfig();
            Variables = new Dictionary<string, string> ();
            foreach (var kv in wf_config) {
                if (kv.Key == "name") 
                    this.Name = (string)kv.Value;
                else if (kv.Key == "startproc")
                    this.StartProcGUID = (string)kv.Value;
                else
                {
                    //generic variable
                    Variables.Add(kv.Key, (string)kv.Value);
                    //this.Name = (string)kv.Value;
                }
                    
            }
            
        }


    }
}
