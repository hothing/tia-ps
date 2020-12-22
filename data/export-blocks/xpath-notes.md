# TIA XML Xpath expressions

## Select all networks 

Xpath: `/Document/SW.Blocks.FC/ObjectList/child::SW.Blocks.CompileUnit/AttributeList/NetworkSource/*[local-name()='FlgNet' and namespace-uri()='http://www.siemens.com/automation/Openness/SW/NetworkSource/FlgNet/v3']`

## Select all parts from all networks 

Xpath : `/Document/SW.Blocks.FC/ObjectList/child::SW.Blocks.CompileUnit/AttributeList/NetworkSource/*[local-name()='FlgNet' and namespace-uri()='http://www.siemens.com/automation/Openness/SW/NetworkSource/FlgNet/v3']/*[local-name()='Parts' and namespace-uri()='http://www.siemens.com/automation/Openness/SW/NetworkSource/FlgNet/v3']`

## Select all function calls from all networks

Xpath : `/Document/SW.Blocks.FC/ObjectList/child::SW.Blocks.CompileUnit/AttributeList/NetworkSource/*[local-name()='FlgNet' and namespace-uri()='http://www.siemens.com/automation/Openness/SW/NetworkSource/FlgNet/v3']/*[local-name()='Parts' and namespace-uri()='http://www.siemens.com/automation/Openness/SW/NetworkSource/FlgNet/v3']/*[local-name()='Call' and namespace-uri()='http://www.siemens.com/automation/Openness/SW/NetworkSource/FlgNet/v3']`

## Select an interface of the function

XPath : `/Document/SW.Blocks.FC/AttributeList/Interface/*[local-name()='Sections' and namespace-uri()='http://www.siemens.com/automation/Openness/SW/Interface/v3']` 
