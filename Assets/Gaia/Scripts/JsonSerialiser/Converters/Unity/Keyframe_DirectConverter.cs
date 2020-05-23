#if !NO_UNITY
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gaia.FullSerializer
{
    partial class fsConverterRegistrar {
        public static Internal.DirectConverters.Keyframe_DirectConverter Register_Keyframe_DirectConverter;
    }
}

namespace Gaia.FullSerializer.Internal.DirectConverters
{
    public class Keyframe_DirectConverter : fsDirectConverter<Keyframe> {
        protected override fsResult DoSerialize(Keyframe model, Dictionary<string, fsData> serialized) {
            var result = fsResult.Success;

            result += SerializeMember(serialized, "time", model.time);
            result += SerializeMember(serialized, "value", model.value);

            #if !UNITY_2018_1_OR_NEWER
            result += SerializeMember(serialized, "tangentMode", model.tangentMode);
            #endif

            result += SerializeMember(serialized, "inTangent", model.inTangent);
            result += SerializeMember(serialized, "outTangent", model.outTangent);

            #if UNITY_2018_1_OR_NEWER
            result += SerializeMember(serialized, "weightedMode", model.weightedMode);
            result += SerializeMember(serialized, "inWeight", model.inWeight);
            result += SerializeMember(serialized, "outWeight", model.outWeight);
            #endif

            return result;
        }

        protected override fsResult DoDeserialize(Dictionary<string, fsData> data, ref Keyframe model) {
            var result = fsResult.Success;

            var t0 = model.time;
            result += DeserializeMember(data, "time", out t0);
            model.time = t0;

            var t1 = model.value;
            result += DeserializeMember(data, "value", out t1);
            model.value = t1;

            #if !UNITY_2018_1_OR_NEWER
            var t2 = model.tangentMode;
            result += DeserializeMember(data, "tangentMode", out t2);
            model.tangentMode = t2;
            #endif

            var t3 = model.inTangent;
            result += DeserializeMember(data, "inTangent", out t3);
            model.inTangent = t3;

            var t4 = model.outTangent;
            result += DeserializeMember(data, "outTangent", out t4);
            model.outTangent = t4;

            #if UNITY_2018_1_OR_NEWER
            var t5 = model.weightedMode;
            result += DeserializeMember(data, "weightedMode", out t5);
            model.weightedMode = t5;

            var t6 = model.inWeight;
            result += DeserializeMember(data, "inWeight", out t6);
            model.inWeight = t6;

            var t7 = model.outWeight;
            result += DeserializeMember(data, "outWeight", out t7);
            model.outWeight = t7;
            #endif

            return result;
        }

        public override object CreateInstance(fsData data, Type storageType) {
            return new Keyframe();
        }
    }
}
#endif