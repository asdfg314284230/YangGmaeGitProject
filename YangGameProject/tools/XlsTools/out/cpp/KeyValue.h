#ifndef __KEYVALUE_H__
#define __KEYVALUE_H__

#include <vector>
#include <string>
#include <stdint.h>
#include "ProtoBuffer.h"
#include "ProtoStream.h"
#include "ProtoType.h"
#include "MessageBase.h"
#include "ConfigBase.h"
// 键值对
class KeyValue : public MessageBase
{
private:
    typedef MessageBase Super;
public:
    // Key
    int32_t Key;
    // Value
    int32_t Value;

    void Decode(ProtoStream *stream) override {
        Super::Decode(stream);
        int tagAndType = 0;
        int fieldCount = stream->ReadFixedShort();

        while ( fieldCount-- > 0 ) {
            tagAndType = stream->ReadInt();

            switch ((tagAndType >> TAG_TYPE_BITS)) {
                case 1: {
                    Key = stream->ReadInt();
                    break;
                }
                case 2: {
                    Value = stream->ReadInt();
                    break;
                }
                default:{
                    stream->ReadUnknow(tagAndType);
                    break;
                }
            }
        }
    }

    void Encode(ProtoStream *stream) override {
        Super::Encode(stream);
        int fieldCount = 0;
        int pos = stream->GetPosition();

        stream->WriteFixedShort(0);

        fieldCount += stream->Write(1, Key);
        fieldCount += stream->Write(2, Value);

        if(fieldCount > 0)
            stream->WriteFixedShort(fieldCount, pos);
    }
};

#endif //__KEYVALUE_H__
