import openpyxl
import os

from const import source_path
from const import target_path
from const import configList_path

from creatOne import creatXml_List

def creatXml():

    print("Begin 逐个生成所有表格的解析文件-------------------------------------------------------");
    #遍历所有xlsx
    creatXml_List(os.listdir(source_path))
    print("End 逐个生成所有表格的解析文件-------------------------------------------------------");
    
    
def creatConfigList():
    print("Begin 生成所有表格的归纳文档-------------------------------------------------------");
    #遍历所有xlsx
    fileContent = ''
    fileContent += '<?xml version="1.0" encoding="utf-8"?>'
    fileContent += '\n<configlist>\n'
    fileContent += '<!--<config name="配置名" meta="结构名称" excel="excel文件名" sheet="数据sheet" desc="描述" type="0所有，1前台 2后台" client="前台配置需要的字段，用，隔开"/>-->\n\n' 
    for dir in os.listdir(source_path):
        child = os.path.join(source_path, dir)
        if(child.find('~') != -1): continue
        if os.path.isfile(child):
            #打开一个workbook
            workbook = openpyxl.load_workbook(child)
            #print(dir.split(".")[0])
            #拿到所有sheet
            sheets = workbook.sheetnames
            #print(sheets)
            
            for i in range(len(sheets)):
                #针对每一个sheet做操作
                if(sheets[i].find('#') != -1):
                    continue
                sheet = workbook[sheets[i]]
                fileContent += '  <config name="' + sheets[i] + 'Cfg' + '" ' + 'meta="' + sheets[i] + 'Cfg' + '" ' + 'excel="' + dir + '" ' + 'sheet="' + sheets[i]  + '" />\n'
                
    fileContent += '\n</configlist>'
    fo = open(configList_path, "wb")
    fo.write(fileContent.encode('utf-8'))
    fo.close()
    print("End 生成所有表格的归纳文档-------------------------------------------------------");
    


