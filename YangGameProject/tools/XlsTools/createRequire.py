from creatOne import creatXml_List
import xml.etree.ElementTree as ET
import os

if __name__ == "__main__":
    configListFilePath = './tools/xproto/ConfigList.xml'
    requireSheetNameListFilePath = './tools/xproto/requireSheetNameListForXProto2.txt'

    requireSheetNameList = []
    if os.path.exists(requireSheetNameListFilePath) and os.path.isfile(requireSheetNameListFilePath):
        with open(requireSheetNameListFilePath) as f:
            lines = f.readlines()
            for line in lines:
                line = line.replace('\n', '')
                requireSheetNameList.append(line)
            pass

    requireXlsList = []
    if os.path.exists(configListFilePath) and os.path.isfile(configListFilePath):
        tree = ET.parse(configListFilePath)
        root = tree.getroot()
        for child in root:
            if child.attrib['name'].replace('Cfg', '') in requireSheetNameList:
                xlsName = child.attrib['excel']
                if xlsName not in requireXlsList:
                    requireXlsList.append(xlsName)

    creatXml_List(requireXlsList)