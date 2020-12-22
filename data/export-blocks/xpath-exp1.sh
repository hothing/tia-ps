#!/bin/sh

XPATH="/Document/SW.Blocks.FC/AttributeList/Interface/*[local-name()='Sections' and namespace-uri()='http://www.siemens.com/automation/Openness/SW/Interface/v3']"
echo "XPath: $XPATH"
echo "Result: "
xmlstarlet sel -t -c "$XPATH" $@
