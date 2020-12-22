# $xf = Get-Content "data/fx1.xml"

$xDoc = New-Object System.Xml.XmlDocument
$xDoc.Load("data/fx2.xml")
$xRoot = $xDoc.DocumentElement
$xEs = $xRoot.'SW.Blocks.FC'.'AttributeList'.'Interface'.'Sections'
if ($xEs -ne $null) 
{
	$xIntf = $xEs.ChildNodes
	foreach($xnode in $xIntf) 
	{
		Write-Host "[BeginNode]"
		Write-Host ("Name: {0}" -f $xnode.Name)
		#Write-Host ("Node Inner: {0}" -f $xnode.InnerXml)
		# получаем атрибут name
		if($xnode.IsEmpty)	{
			Write-Host "Empty"
		} 
		else  {
			# обходим все дочерние узлы элемента user
			foreach($cnode in $xnode.ChildNodes)
			{
				Write-Host ("Element: {0}" -f $cnode.Name)
			}
		}
		Write-Host "[EndNode]"
    }
}
else
{
	Write-Host "An error occurred: the given XML document is not TIA Document"
	Write-Host $_
}
