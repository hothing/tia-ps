#!/bin/sh

XPATH="/Document/SW.Blocks.FC/ObjectList/child::SW.Blocks.CompileUnit/AttributeList/NetworkSource/*[local-name()='FlgNet' and namespace-uri()='http://www.siemens.com/automation/Openness/SW/NetworkSource/FlgNet/v3']/*[local-name()='Parts' and namespace-uri()='http://www.siemens.com/automation/Openness/SW/NetworkSource/FlgNet/v3']/*[local-name()='Call' and namespace-uri()='http://www.siemens.com/automation/Openness/SW/NetworkSource/FlgNet/v3']"
echo "XPath: $XPATH"
echo "Result: "
xmlstarlet sel -t -c "$XPATH" $@
