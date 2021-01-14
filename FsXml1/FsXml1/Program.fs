// Learn more about F# at http://fsharp.org

open System
open System.IO
open System.Xml
open FSharp.Data

let loadXml strFileName =
    if File.Exists strFileName then
        use fs = new FileStream(strFileName, FileMode.Open, FileAccess.Read)
        let xd : XmlDocument = new XmlDocument()
        xd.Load strFileName
        fs.Close()
        Some(xd)
    else
        None

type FxSection = XmlProvider<ResolutionFolder = "C:\Program Files\Siemens\Automation\Portal V16\PublicAPI\V16\Schemas", Schema = "SW.InterfaceSections_v4.xsd">

let selectFxInterface (xd : XmlDocument) =
    //let nt : NameTable = new NameTable()
    let nsmgr : XmlNamespaceManager = new XmlNamespaceManager(xd.NameTable)
    nsmgr.AddNamespace("sbi", "http://www.siemens.com/automation/Openness/SW/Interface/v4")
    //let xpath = "Document/SW.Blocks.FB/AttributeList/Interface"
    let xpath = "//sbi:Section[@Name != 'None']"
    let xnlIntf : XmlNodeList = xd.SelectNodes(xpath, nsmgr)
    xnlIntf


[<EntryPoint>]
let main argv =
    printfn "Hello World from F#!"
    let bdir =  Environment.GetEnvironmentVariable("USERPROFILE");
    let fpath = bdir + "/Documents/tia-ps/data/export-blocks-2/FB_anamonitor.xml"
    printfn "The document %s is loading" fpath
    let xd = loadXml fpath
    let res = match xd with
              | Some(d) -> 
                let xin = selectFxInterface d
                printfn "%A" xin.Count
                for n in xin do
                    printfn "Node name = %A" n.Name
                    printfn "%A" (FxSection.Parse(n.OuterXml).Comment)
                    if n.NodeType = XmlNodeType.Element then
                        let ne : XmlElement =  downcast n
                        printfn "Attribute 'Name'= %A" (ne.GetAttribute("Name"))                        
                0
              | _ -> (-1);
    res
    // return an integer exit code
