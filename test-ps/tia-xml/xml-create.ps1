[xml]$doc = ""
$xd = $doc.CreateXmlDeclaration("1.0", "utf-8", "yes")
$doc.InsertBefore($xd, $doc.DocumentElement)

# Create the root element and add it to the document.
$doctype = $doc.CreateDocumentType("root", $null, $null, $null)
$doc.AppendChild($doctype)
$doc.AppendChild($doc.CreateElement("root"))

$rinf = $doc.CreateElement("root_info")
$rinf.SetAttribute("version", "1.0")
$doc.DocumentElement.AppendChild($rinf)

$newElem = $doc.CreateNode("element", "value", "")
$text = $doc.CreateTextNode("19.95")
$newElem.AppendChild($text)
#$newElem.AppendData("true")
$doc.DocumentElement.AppendChild($newElem)

$ss = $doc.CreateElement("structures")
$newElem = $doc.CreateElement("structure")
$ss.AppendChild($newElem)
$doc.DocumentElement.AppendChild($ss)

