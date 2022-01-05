using System;
using System.Management.Automation;
using Siemens.Engineering;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Xml;

namespace TiaCmdlet
{
    
    [Cmdlet(VerbsData.Convert, "TiaFC2FB")]
    public class ConvertTiaFC2FB : PSCmdlet
    {
        #region Command parameters
        string _filePath;
        bool _removeReturnMembers = false;
        
        /// <summary>
        /// Gets or sets the file name
        /// </summary>
        [Parameter(Mandatory = true,
            HelpMessage = "XML-file name")]
        [Alias("f")]
        public string FileName
        {
            get { return _filePath; }
            set { _filePath = value; }
        }

        /// <summary>
        /// Gets or sets a 'RemoveReturnMembers' mode
        /// </summary>
        [Parameter(Mandatory = false,
            HelpMessage = "Remove return parameters")]
        public SwitchParameter RemoveReturnMembers
        {
            get { return _removeReturnMembers; }
            set { _removeReturnMembers = value; }
        }
        
        #endregion Command parameters

        #region internal commands

        public static bool FCtoFB(XmlDocument document, bool removeReturnMembers)
        {
            try
            {
                if ((document != null) && (document.HasChildNodes))
                {
                    //select <SW.Blocks.FC> as "root"
                    var swBlocksFc = document.SelectSingleNode("//SW.Blocks.FC");

                    //set XML namespaces
                    var nsmgr = new XmlNamespaceManager(document.NameTable);
                    nsmgr.AddNamespace("ns", @"http://www.siemens.com/automation/Openness/SW/Interface/v4");

                    //remove <Section Name="Return"/> and move <Member>s to <Section Name="Output">
                    var sectionReturn = swBlocksFc.SelectSingleNode(".//ns:Section[@Name='Return']", nsmgr);
                    if (removeReturnMembers == false)
                    {
                        var sectionOutput = swBlocksFc.SelectSingleNode(".//ns:Section[@Name='Output']", nsmgr);

                        var members = sectionReturn.SelectNodes("./ns:Member", nsmgr);
                        foreach (XmlNode member in members)
                        {
                            if (member.Attributes.GetNamedItem("Datatype").Value != "Void")
                            {
                                sectionOutput.AppendChild(member);
                            }
                        }
                    }

                    sectionReturn.ParentNode.RemoveChild(sectionReturn);

                    //create new <SW.Blocks.FB> node
                    XmlNode swBlocksFb = document.CreateElement("SW.Blocks.FB");

                    //add "ID" attribute to <SW.Blocks.FB>
                    XmlNode attributeId = document.CreateAttribute("ID");
                    attributeId.Value = swBlocksFc.SelectSingleNode("@ID").Value;
                    swBlocksFb.Attributes.SetNamedItem(attributeId);

                    //copy everything from <SW.Blocks.FC> to <SW.Blocks.FB> to switch the blocktype to FB
                    foreach (XmlNode child in swBlocksFc.SelectNodes("./*"))
                    {
                        swBlocksFb.AppendChild(child);
                    }

                    //replace <SW.Blocks.FB> with the new <SW.Blocks.FC>
                    swBlocksFc.ParentNode.ReplaceChild(swBlocksFb, swBlocksFc);

                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("Exception during XML editing:" + Environment.NewLine + ex);
                return false;
            }
        }

        #endregion internal commands

        #region command code
        protected override void BeginProcessing()
        {
            base.BeginProcessing();
        }

        protected override void ProcessRecord()
        {
            base.ProcessRecord();
        }

        protected override void EndProcessing()
        {
            base.EndProcessing();
        }
        #endregion command code
    }
}
