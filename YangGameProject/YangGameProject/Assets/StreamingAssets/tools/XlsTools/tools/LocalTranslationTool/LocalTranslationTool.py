import os
import sys
import time
try:
    import opencc
except ImportError:
    os.system('pip install opencc')
    import opencc

if __name__ == '__main__':
    converter = opencc.OpenCC('s2twp.json')
    if len(sys.argv) > 2:
        convertType = sys.argv[1]
        if convertType == '1':
            # 转换单句
            originStr = sys.argv[2]
            convertStr = converter.convert(originStr)
            print(convertStr)
        elif convertType == '2':
            # 转换txt文件
            originFilePath = sys.argv[2]
            originFilePathSplitextList = os.path.splitext(originFilePath)
            convertFilePath = None
            if len(originFilePathSplitextList) == 1:
                convertFilePath = originFilePath+'_Convert_' + \
                    time.strftime("%Y%m%d%H%M%S", time.localtime())
            elif len(originFilePathSplitextList) == 2:
                originFilePathWithoutExt = originFilePathSplitextList[0]
                ext = originFilePathSplitextList[1]
                convertFilePath = originFilePathWithoutExt+'_Convert_' + \
                    time.strftime("%Y%m%d%H%M%S", time.localtime())

            if convertFilePath is not None:
                if os.path.exists(originFilePath):
                    with open(originFilePath, encoding='utf8') as f:
                        originStr = f.read()
                        convertStr = converter.convert(originStr)
                        print(convertStr)
                        with open(convertFilePath, 'w', encoding='utf8') as wf:
                            wf.write(convertStr)
