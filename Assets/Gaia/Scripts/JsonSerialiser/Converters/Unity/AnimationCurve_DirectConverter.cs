#if !NO_UNITY
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gaia.FullSerializer
{
    partial class fsConverterRegistrar {
        public static Internal.DirectConverters.AnimationCurve_DirectConverter Register_AnimationCurve_DirectConverter;
    }
}

namespace Gaia.FullSerializer.Internal.DirectConverters
{
    public class AnimationCurve_DirectConverter : fsDirectConverter<AnimationCurve> {
        protected override fsResult DoSerialize(AnimationCurve model, Dictionary<string, fsData> serialized) {
            var result = fsResult.Success;

            result += SerializeMember(serialized, "keys", model.keys);
            result += SerializeMember(serialized, "preWrapMode", model.preWrapMode);
            result += SerializeMember(serialized, "postWrapMode", model.postWrapMode);

            return result;
        }

        protected override fsResult DoDeserialize(Dictionary<string, fsData> data, ref AnimationCurve model) {
            var result = fsResult.Success;

            // Legacy keyframe data
#if UNITY_2018_1_OR_NEWER
            //List<int> tangentModes = new List<int>();

            if (data.ContainsKey("keys"))
            {
                var keysList = data["keys"].AsList;

                for (int i = 0; i < keysList.Count; i++)
                {
                    var dict = keysList[i].AsDictionary;
                    if (!dict.ContainsKey("tangentMode"))
                    {
                        // Not a legacy key
                        break;
                    }

                    //tangentModes.Add((int)dict["tangentMode"].AsUInt64);

                    SerializeMember(dict, "weightedMode", WeightedMode.None);
                    SerializeMember(dict, "inWeight", 0f);
                    SerializeMember(dict, "outWeight", 0f);
                }
            }
#endif

            var t0 = model.keys;
            result += DeserializeMember(data, "keys", out t0);
            model.keys = t0;

            //#if UNITY_2018_1_OR_NEWER
            //// Let's see if we have any legacy tangent modes
            //for (int i = 0; i < tangentModes.Count; i++)
            //{
            //    //UnityEditor.AnimationUtility.SetKeyLeftTangentMode(model, i, tangentMode: (UnityEditor.AnimationUtility.TangentMode)tangentModes[i]);
            //    //UnityEditor.AnimationUtility.SetKeyRightTangentMode(model, i, tangentMode: (UnityEditor.AnimationUtility.TangentMode)tangentModes[i]);
            //}
            //#endif

            var t1 = model.preWrapMode;
            result += DeserializeMember(data, "preWrapMode", out t1);
            model.preWrapMode = t1;

            var t2 = model.postWrapMode;
            result += DeserializeMember(data, "postWrapMode", out t2);
            model.postWrapMode = t2;

            return result;
        }

        public override object CreateInstance(fsData data, Type storageType) {
            return new AnimationCurve();
        }
    }
}
#endif