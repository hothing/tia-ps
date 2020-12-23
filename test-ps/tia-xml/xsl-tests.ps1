<#
The powershell script for testing XML/XSLT processing
#>

$xsl1 = [xml]@"
<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0"
			xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:output method="xml" indent="yes"/>
<xsl:template match="Document/SW.Blocks.FC/ObjectList/SW.Blocks.CompileUnit/AttributeList/NetworkSource">
    Network
    <xsl:apply-templates />
</xsl:template>
</xsl:stylesheet>
"@

<#
$XSLInputElement = New-Object System.Xml.Xsl.XslCompiledTransform;
$XSLInputElement.Load($XSLFileInput)

$XMLInputDoc = Get-Content -Path $XMLInputFile

$reader = [System.Xml.XmlReader]::Create($XMLInputFile)
$writter = [System.Xml.XmlTextWriter]::Create($XMLOutputFile)

$XSLInputElement.Transform($XMLInputDoc, $writter)
#>

function TransformXML{
    param ($xml, $xsl, $output)

    if (-not $xml -or -not $xsl -or -not $output)
    {
        Write-Host "& .\xslt.ps1 [-xml] xml-input [-xsl] xsl-input [-output] transform-output"
        return 0;
    }

    Try
    {
        $xslt_settings = New-Object System.Xml.Xsl.XsltSettings;
        $xslt_settings.EnableScript = 1;

        $xslt = New-Object System.Xml.Xsl.XslCompiledTransform;
        $XmlUrlResolver = New-Object System.Xml.XmlUrlResolver;
        
        $xslt.Load($xsl,$xslt_settings,$XmlUrlResolver);
        $xslt.Transform($xml, $output);
    }

    Catch
    {
        $ErrorMessage = $_.Exception.Message
        $FailedItem = $_.Exception.ItemName
        Write-Host  'Error'$ErrorMessage':'$FailedItem':' $_.Exception;
        return 0
    }
    return 1

}

$a = [System.Text.Encoding]::UTF8.GetBytes($xsl1)
$b = New-Object System.IO.MemoryStream -ArgumentList $a.Count