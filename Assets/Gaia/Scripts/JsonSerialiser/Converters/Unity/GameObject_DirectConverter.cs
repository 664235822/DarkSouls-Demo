#if !NO_UNITY
using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Gaia.FullSerializer
{
    partial class fsConverterRegistrar
    {
        public static Internal.DirectConverters.GameObject_DirectConverter Register_GameObject_DirectConverter;
    }
}

namespace Gaia.FullSerializer.Internal.DirectConverters
{
    public class GameObject_DirectConverter : fsDirectConverter<GameObject>
    {
        protected override fsResult DoSerialize(GameObject model, Dictionary<string, fsData> serialized)
        {
            var result = fsResult.Success;

            if (model == null)
            {
                result += SerializeMember(serialized, "present", false);
            }
            else
            {
                result += SerializeMember(serialized, "present", true);
                result += SerializeMember(serialized, "name", model.name);
                #if UNITY_EDITOR
                result += SerializeMember(serialized, "path", AssetDatabase.GetAssetPath(model));
                #else
                result += SerializeMember(serialized, "path", "");
                #endif
            }
            return result;
        }

        protected override fsResult DoDeserialize(Dictionary<string, fsData> data, ref GameObject model)
        {
            var result = fsResult.Success;

            var present = false;
            result += DeserializeMember(data, "present", out present);

            if (present == true)
            {
                var name = model.name;
                result += DeserializeMember(data, "name", out name);
                model.name = name;

                var path = "";
                result += DeserializeMember(data, "path", out path);
                if (!string.IsNullOrEmpty(path))
                {
                    #if UNITY_EDITOR
                    model = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                    #endif
                }
            }

            return result;
        }

        public override object CreateInstance(fsData data, Type storageType)
        {
            return new Texture2D(1024, 1024);
        }
    }
}
#endif