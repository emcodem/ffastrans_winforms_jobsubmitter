# ffastrans_winforms_jobsubmitter

Local Windows Forms Application for submitting Files to a FFAStrans Job. 
Special Files in project root:

ffastrans_workflow_emcodem_guisubmit_concat_different_formats.json
A FFAStrans workflow that shows how to stitch multiple source files

config_example.toml
a configuration file example, place it next to the FFAStrans_Job_Submitter.exe and name it config.toml

Configuration file minimal contents (toml formatting):

[ffastrans]
api = "http://localhost:65445/api/json/v2"


[[workflow]]
name = "emcodem_guisubmit_concat_different_formats"
startproc = "20221022-1124-2650-58ea-85fbe7da8009"
i_file_count = "%count%"
s_final_deliver_extension = "mxf"
s_final_deliver_path = "\\\\localhost\\c$\\temp"

There can be multiple workflow sections.
name and startproc is mandatory.  
name is the name of a FFAStrans workflow.
startproc value is optional, if your workflow only has one then you can specify "".
You can use %count% in variable values, it will be replaced by the application by the selected file count.

How it works from FFAStrans Perspective:
s_source will be populated with an array of the selected source files, e.g. ["path\\file1","path\\file2"]
All other entries than name and startproc under [[workflow]] will be submitted as user_variables.