#!/bin/sh

XPATH="/Document/SW.Blocks.FC/ObjectList/child::SW.Blocks.CompileUnit/AttributeList/NetworkSource/*[local-name()='FlgNet' and namespace-uri()='http://www.siemens.com/automation/Openness/SW/NetworkSource/FlgNet/v3']/*[local-name()='Wires' and namespace-uri()='http://www.siemens.com/automation/Openness/SW/NetworkSource/FlgNet/v3']/*[local-name()='Wire' and namespace-uri()='http://www.siemens.com/automation/Openness/SW/NetworkSource/FlgNet/v3' and attribute('UId')='30']"
echo "XPath: $XPATH"
echo "Result: "
xmlstarlet sel -t -c "$XPATH" $@
