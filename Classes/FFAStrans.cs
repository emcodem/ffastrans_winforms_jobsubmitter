using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Encodings.Web;

namespace FFAStrans_Concat_Submitter.Classes
{
    internal class FFAStrans
    {
        private string _api;

        public FFAStrans(string api) { 
            this._api = api;
        }

        private JsonObject constructJobJson() {
            var ffastransJob = new JsonObject
            {
                ["wf_id"] = "",
                ["start_proc"] = "",
                ["inputfile"] = "",
                ["variables"] = new JsonArray()
            };
            return ffastransJob;
        }

        public async Task startWorkflow(ComboBoxWorkflowItem selectedWf, JsonArray fileList) 
        {
            
            //get wf_list from api for translating wf name to id
            HttpClient req = new HttpClient();
            var content = await req.GetAsync(this._api + "/workflows");
            if (content.StatusCode != System.Net.HttpStatusCode.OK) {
                try
                {
                    string errbody = await content.Content.ReadAsStringAsync();
                    MessageBox.Show("Error reading Workflows from "+ this._api + ". \nHTTP Status code was: " + content.StatusCode +"\n"+ "Response Body:" + errbody);
                }
                catch (Exception ex) { 
                }
            }
            string body = await content.Content.ReadAsStringAsync();
            JsonDocument document = JsonDocument.Parse(body);
            JsonElement root = document.RootElement;
            JsonElement wf_array = root.GetProperty("workflows");

            foreach (var wf in wf_array.EnumerateArray()) {
                string wf_name = wf.GetProperty("wf_name").ToString();
                string wf_id = wf.GetProperty("wf_id").ToString();

                if (selectedWf.Name.ToLower() == wf_name.ToLower()) {
                    //we successfully parsed wf_name into wf_guid, kick off a ffastrans job

                    var jobjson = constructJobJson();
                    jobjson["wf_id"] = wf.GetProperty("wf_id").ToString();
                    jobjson["start_proc"] = selectedWf.StartProcGUID;
                    jobjson["inputfile"] = JsonSerializer.Serialize(fileList);
                    

                    //add variables if any
                    foreach (var user_var in selectedWf.Variables) {
                        var variableValue = user_var.Value.Replace("%count%", fileList.Count.ToString());
                        var _variableObj = new JsonObject { ["name"] = user_var.Key, ["data"] = variableValue };
                        jobjson["variables"].AsArray().Add(_variableObj);
                    }

                    string jobjsonstring = JsonSerializer.Serialize(jobjson);

                    // Post Request
                    
                    HttpContent payload = new StringContent(jobjsonstring);
                    var responseObj2 = await req.PostAsync(_api + "/jobs", payload);
                    var responseBody2 = await responseObj2.Content.ReadAsStringAsync();
                    if (responseObj2.StatusCode == System.Net.HttpStatusCode.Accepted) //ffastrans returns accepted if all was ok
                    {
                        MessageBox.Show("Success submitting job.");
                    }
                    else {
                        MessageBox.Show("Error, job was not submitted. There is something wrong in the config.toml or in FFAStrans regarding the selected target workflow." + "Response code was " + responseObj2.StatusCode);
                    }
                    return;

                }
                else
                {
                    continue;
                }
            }

            //if we come here, something is wrong, we did not find the selected workflow in ffastrans api...
            MessageBox.Show("Workflow name " + selectedWf.Name + " was not found on FFAStrans " + this._api);

        }
    }
}
