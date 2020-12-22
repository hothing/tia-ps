[System.Reflection.Assembly]::LoadWithPartialName("System.Xml")
[System.Reflection.Assembly]::LoadWithPartialName("System.Xml.XPath")

$xmlFile = "data/fx2.xml"

#$xDoc = New-Object System.Xml.XmlDocument
$xPathDoc = New-Object System.Xml.XPath.XPathDocument -ArgumentList $xmlFile 
$xPathNav = $xPathDoc.CreateNavigator()

$xPathNS = New-Object System.Xml.XmlNamespaceManager -ArgumentList $xPathNav.NameTable
$xPathNS.AddNamespace("xintf", "http://www.siemens.com/automation/Openness/SW/Interface/v3")
$xPathNS.AddNamespace("xcode", "http://www.siemens.com/automation/Openness/SW/NetworkSource/FlgNet/v3")

# /Document/SW.Blocks.FC/AttributeList/Interface/*[local-name()='Sections' and namespace-uri()='http://www.siemens.com/automation/Openness/SW/Interface/v3']
$xPathExp1 = $xPathNav.Compile("/Document/SW.Blocks.FC/AttributeList/Interface/xintf:Sections")
$xPathExp1.SetContext($xPathNS)

function Evaluate([System.Xml.XPath.XPathExpression] $expression, [System.Xml.XPath.XPathNavigator] $navigator)
{
	$res = "---"
	switch ($expression.ReturnType)
	{
		# [System.Xml.XPath.XPathResultType]::Number 
		"Number" { $res = $navigator.Evaluate($expression) }

		# [System.Xml.XPath.XPathResultType]::NodeSet 
		"NodeSet" {
		    $nodes = $navigator.Select($expression)
			$res = @($nodes.Count.ToString()) 
			
			while ($nodes.MoveNext())
			{
				$res += $nodes.Current.InnerXml
			}
		}

		# [System.Xml.XPath.XPathResultType]::Boolean 
		"Boolean" {
			if ($navigator.Evaluate($expression)) { $res = "True!" }
		}

		# [System.Xml.XPath.XPathResultType]::String 
		"String" {
			$res = $navigator.Evaluate($expression)
		}
		
		default {
			$res = ("Unexpected ReturnType: {0}" -f $expression.ReturnType)
		}
	}
	$res
}

Write-Host $xPathNav.GetType() 
Write-Host $xPathExp1.GetType()

$r = (Evaluate $xPathExp1 $xPathNav)
   
Write-Host $r.GetType()
Write-Host $r
