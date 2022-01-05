using System;
using System.Management.Automation;
using Siemens.Engineering;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Xml;

namespace TiaCmdlet
{
    public enum MoveStaticVariables
    {
        InOut,
        Temp,
        Remove
    }

    [Cmdlet(VerbsData.Convert, "TiaFB2FC")]
    public class ConvertTiaFB2FC : PSCmdlet
    {
        #region Command parameters
        XmlDocument document;
        MoveStaticVariables _moveStaticVariables = MoveStaticVariables.InOut;

        /// <summary>
        /// Gets or sets the file name
        /// </summary>
        [Parameter(
        Position = 0,
        Mandatory = true,
        ValueFromPipeline = true,
        ValueFromPipelineByPropertyName = true)]
        [ValidateNotNullOrEmpty]
        [Alias("i")]
        public XmlDocument InputObject
        {
            get { return document; }
            set { 
                document = value;                
            }
        }


        /// <summary>
        /// Gets or sets a 'RemoveReturnMembers' mode
        /// </summary>
        [Parameter(Mandatory = false,
            HelpMessage = "Handling static parameters: move to In/Out, or move to Temp or remove it")]
        public MoveStaticVariables MoveStaticVariables
        {
            get { return _moveStaticVariables; }
            set { _moveStaticVariables = value; }
        }
        #endregion Command parameters

        #region Internal variable
        #endregion

        #region internal commands

        #endregion internal commands

        #region command code
        protected override void BeginProcessing()
        {
            base.BeginProcessing();
        }

        protected override void ProcessRecord()
        {
            base.ProcessRecord();
            XmlDocument new_document = document.Clone() as XmlDocument;
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(new_document.NameTable);
            nsmgr.AddNamespace("ns", @"http://www.siemens.com/automation/Openness/SW/Interface/v4");

            //select <SW.Blocks.FB> as "root"
            var swBlocksFb = new_document.SelectSingleNode("//SW.Blocks.FB");
            if ((swBlocksFb != null) && (swBlocksFb.HasChildNodes))
            {
                //remove <Section Name="Static"/> and move <Member>s to desired <Section>
                var sectionStatic = swBlocksFb.SelectSingleNode(".//ns:Section[@Name='Static']", nsmgr);
                if ((sectionStatic != null) && (sectionStatic.HasChildNodes))
                {
                    if (_moveStaticVariables == MoveStaticVariables.InOut)
                    {
                        var sectionInOut = swBlocksFb.SelectSingleNode(".//ns:Section[@Name='InOut']", nsmgr);
                        if (sectionInOut != null)
                        {
                            foreach (XmlNode member in sectionStatic.SelectNodes("./ns:Member", nsmgr))
                            {
                                sectionInOut.AppendChild(member);
                            }
                            WriteVerbose("The static variables have been marked as In/Out");
                        }                        
                    }
                    else if (_moveStaticVariables == MoveStaticVariables.Temp)
                    {
                        var sectionTemp = swBlocksFb.SelectSingleNode(".//ns:Section[@Name='Temp']", nsmgr);
                        if (sectionTemp != null)
                        {
                            foreach (XmlNode member in sectionStatic.SelectNodes("./ns:Member", nsmgr))
                            {
                                sectionTemp.AppendChild(member);
                            }
                            WriteVerbose("The static variables have been marked as temporary");
                        }                            
                    }

                    sectionStatic.ParentNode.RemoveChild(sectionStatic);
                }
                else
                {
                    WriteVerbose("There are not the static variables");
                }

                //if the block already has a "Ret_Val" defined
                var sectionOutput = swBlocksFb.SelectSingleNode(".//ns:Section[@Name='Output']", nsmgr);
                if (sectionOutput != null)
                {
                    foreach (XmlNode member in sectionOutput.SelectNodes("./ns:Member", nsmgr))
                    {
                        if (member.Attributes.GetNamedItem("Name").Value == "Ret_Val")
                        {
                            //create template string for <Section Name='Return'>
                            var sectionReturnString =
                                "<Section Name='Return'>" +
                                $"<Member Name='Ret_Val' Datatype='{member.Attributes.GetNamedItem("Datatype").Value}' Accessibility='Public' />" +
                                "</Section>";

                            //remove old "Ret_Val"
                            member.ParentNode.RemoveChild(member);

                            //create new <Section Name="Return"> node
                            var sectionReturn = new XmlDocument();
                            sectionReturn.LoadXml(sectionReturnString);

                            //insert new <Section Name="Return"> after <Section Name="Constant">
                            var sectionConstant = swBlocksFb.SelectSingleNode(".//ns:Section[@Name='Constant']", nsmgr);
                            if (sectionConstant?.ParentNode != null && swBlocksFb.OwnerDocument != null && sectionReturn.DocumentElement != null)
                            {
                                sectionConstant.ParentNode.InsertAfter(swBlocksFb.OwnerDocument.ImportNode(sectionReturn.DocumentElement, true), sectionConstant);
                            }
                            WriteVerbose("The Ret_Val variable has been translated");
                            break;
                        }                        
                    }
                }
                
                // remove all <StartValue> tags (except from Constant section) because FCs do not support start values
                var startValues = swBlocksFb.SelectNodes(".//ns:Section[@Name!='Constant']//ns:StartValue", nsmgr);
                if (startValues != null)
                {
                    foreach (XmlNode node in startValues)
                    {
                        node?.ParentNode?.RemoveChild(node);
                    }
                    WriteVerbose("All <StartValue> variables have been removed");
                }

                // remove all <SetPoint> attribute from <Section Name!="Temp">
                // remove all 'ExternalVisible' and 'ExternalWritable' attributes
                var setPoints = swBlocksFb.SelectNodes(".//ns:Section[@Name!='Temp']//ns:BooleanAttribute[@Name='SetPoint']", nsmgr);
                if (setPoints != null)
                {
                    foreach (XmlNode node in setPoints)
                    {
                        if (
                            (node.Attributes?.GetNamedItem("SetPoint") != null)
                            || (node.Attributes?.GetNamedItem("ExternalVisible") != null)
                            || (node.Attributes?.GetNamedItem("ExternalWritable") != null)
                            )
                        {
                            node.ParentNode?.RemoveChild(node);
                        }                        
                    }
                    WriteVerbose("All non FB attributes have been removed");
                }
                
                //remove <MemoryReserve>
                var memoryReserve = swBlocksFb.SelectSingleNode(".//MemoryReserve");
                if (memoryReserve != null)
                {
                    memoryReserve.ParentNode?.RemoveChild(memoryReserve);
                    WriteVerbose("All <MemoryReserve> variables have been removed");
                }                

                //create new <SW.Blocks.FC> node
                XmlNode swBlocksFc = new_document.CreateElement("SW.Blocks.FC");

                //add "ID" attribute to <SW.Blocks.FC>
                XmlNode attributeId = new_document.CreateAttribute("ID");
                attributeId.Value = swBlocksFb.SelectSingleNode("@ID").Value;
                swBlocksFc.Attributes.SetNamedItem(attributeId);

                //copy everything from <SW.Blocks.FB> to <SW.Blocks.FC> to switch the blocktype to FB
                foreach (XmlNode child in swBlocksFb.SelectNodes("./*"))
                {
                    swBlocksFc.AppendChild(child);
                }

                //replace <SW.Blocks.FB> with the new <SW.Blocks.FC>
                swBlocksFb.ParentNode.ReplaceChild(swBlocksFc, swBlocksFb);

                WriteObject(new_document);
            }
            else
            {
                WriteError(new ErrorRecord(new Exception("TIA SimaticML"), "The document does not contain a FB block", ErrorCategory.InvalidArgument, null));
            }
        }

        protected override void EndProcessing()
        {
            base.EndProcessing();
        }
        #endregion command code
    }
}
