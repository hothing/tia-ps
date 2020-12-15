function Create-TiaFunctionBlock {
    [CmdletBinding()]
    param(
        [Parameter(Mandatory=$true, ValueFromPipeline=$true)][string]$Name,
        [string]$Author,
        [string]$Family,
        [string]$Vesrion
    )

    $doc = New-Object System.Xml.XmlDocument
    $x = $doc.InsertBefore($doc.CreateXmlDeclaration("1.0", "utf-8", "yes"), $doc.DocumentElement)

    # Create the root element and add it to the document.
    $x = $doc.AppendChild($doc.CreateElement("Document"))
    
    # Engineering node
    $e_eng = $doc.CreateElement("Engineering")
    $e_eng.SetAttribute("version", "V15")
    $x = $doc.DocumentElement.AppendChild($e_eng)

    # DocumentInfo node
    $e_info = $doc.CreateElement("DocumentInfo")
    $x = $doc.DocumentElement.AppendChild($e_info)
    
    # SW.Blocks.FB node
    $e_fb = $doc.CreateElement("SW.Blocks.FB")
    $x = $e_fb.SetAttribute("ID", "0")
    $x = $doc.DocumentElement.AppendChild($e_fb)

    # //SW.Blocks.FB/AttributeList node
    $e_al = $doc.CreateElement("AttributeList")
    $x = $e_fb.AppendChild($e_al)
    # //SW.Blocks.FB/ObjectList node
    $e_ol = $doc.CreateElement("ObjectList")
    $x = $e_fb.AppendChild($e_ol)

    # //SW.Blocks.FB/AttributeList/HeaderName node
    $e_ha = $doc.CreateElement("HeaderName")
    $x = $e_ha.AppendChild($doc.CreateTextNode($Name))
    $x = $e_al.AppendChild($e_ha)

    # //SW.Blocks.FB/AttributeList/HeaderAuthor node
    $e_ha = $doc.CreateElement("HeaderAuthor")
    if (($Author -ne $null) -and ($Author -ne "")) {
        $x = $e_ha.AppendChild($doc.CreateTextNode($Author))
    }
    $x = $e_al.AppendChild($e_ha)

    # //SW.Blocks.FB/AttributeList/HeaderFamily node
    $e_ha = $doc.CreateElement("HeaderFamily")
    if (($Family -ne $null) -and ($Family -ne "")) {
        $x = $e_ha.AppendChild($doc.CreateTextNode($Family))
    }
    $x = $e_al.AppendChild($e_ha)

    # //SW.Blocks.FB/AttributeList/HeaderVersion node
    $e_ha = $doc.CreateElement("HeaderVersion")
    if (($Vesrion -ne $null) -and ($Vesrion -ne "")) {
        $x = $e_ha.AppendChild($doc.CreateTextNode($Vesrion))
    }
    $x = $e_al.AppendChild($e_ha)
    
    # //SW.Blocks.FB/AttributeList/Interface node
    $e_intf = $doc.CreateElement("Interface")
    $x = $e_al.AppendChild($e_intf)

    # //SW.Blocks.FB/AttributeList/Interface/Sections node
    $e_isc = $doc.CreateElement("Sections", "http://www.siemens.com/automation/Openness/SW/Interface/v4")
    $x = $e_intf.AppendChild($e_isc)
    
    # //SW.Blocks.FB/AttributeList/Interface/Sections/Section[Name="Input"] node
    $e_sc = $doc.CreateElement("Section")
    $x = $e_sc.SetAttribute("Name", "Input")
    $x = $e_isc.AppendChild($e_sc)
    
    # //SW.Blocks.FB/AttributeList/Interface/Sections/Section[Name="Output"] node
    $e_sc = $doc.CreateElement("Section")
    $x = $e_sc.SetAttribute("Name", "Output")
    $x = $e_isc.AppendChild($e_sc)

    # //SW.Blocks.FB/AttributeList/Interface/Sections/Section[Name="InOut"] node
    $e_sc = $doc.CreateElement("Section")
    $x = $e_sc.SetAttribute("Name", "InOut")
    $x = $e_isc.AppendChild($e_sc)

    # //SW.Blocks.FB/AttributeList/Interface/Sections/Section[Name="Temp"] node
    $e_sc = $doc.CreateElement("Section")
    $x = $e_sc.SetAttribute("Name", "Temp")
    $x = $e_isc.AppendChild($e_sc)

    # //SW.Blocks.FB/AttributeList/Interface/Sections/Section[Name="Constant"] node
    $e_sc = $doc.CreateElement("Section")
    $x = $e_sc.SetAttribute("Name", "Constant")
    $x = $e_isc.AppendChild($e_sc)

    $doc
}

function Insert-TiaBlockInput {
    [CmdletBinding()]
    param(
        [Parameter(Mandatory=$true, ValueFromPipeline=$true)][xml]$doc,
        [Parameter(Mandatory=$true)][string]$Name,
        [Parameter(Mandatory=$true)][string]$Type
    )
    # Check the section
    $xna = $doc.CreateNavigator()
    $manager = New-Object System.Xml.XmlNamespaceManager -ArgumentList $xna.NameTable
    $x = $manager.AddNamespace("sc_v4", "http://www.siemens.com/automation/Openness/SW/Interface/v4")
    $query = $xna.Compile("/Document/SW.Blocks.FB[1]/AttributeList/Interface/sc_v4:Sections/Section")
    $x = $query.SetContext($manager)
    $e_sec = $xna.Select($query)
    
    if ($e_sec -ne $null) {
        $e_sx = $e_sec.Current.GetNode()
        $e_in = $e_sx.ChildNodes[0].ChildNodes | where-object -Property Name -EQ 'Input'
        # <Member Name="wl" Datatype="Bool" Remanence="NonRetain" Accessibility="Public">
        $e_m = $doc.CreateElement("Member")
        $x = $e_m.SetAttribute("Name", $Name)
        $x = $e_m.SetAttribute("Datatype", $Type)
        $x = $e_in.AppendChild($e_m)
    } else {
        Write-Error "XML structure is wrong"
    }
    # Insert an input definition
    $doc
}

$fb = Create-TiaFunctionBlock -Name "TestFB" -Vesrion "0.1"
$fb | Insert-TiaBlockInput -Name Input_1 -Type Bool

