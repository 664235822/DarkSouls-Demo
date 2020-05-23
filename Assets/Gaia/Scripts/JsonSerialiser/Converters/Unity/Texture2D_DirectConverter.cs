#if !NO_UNITY
using System;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Gaia.FullSerializer
{
    partial class fsConverterRegistrar
    {
        public static Internal.DirectConverters.Texture2D_DirectConverter Register_Texture2D_DirectConverter;
    }
}

namespace Gaia.FullSerializer.Internal.DirectConverters
{
    public class Texture2D_DirectConverter : fsDirectConverter<Texture2D>
    {
        protected override fsResult DoSerialize(Texture2D model, Dictionary<string, fsData> serialized)
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
                result += SerializeMember(serialized, "width", model.width);
                result += SerializeMember(serialized, "height", model.height);
            }

            return result;
        }

        protected override fsResult DoDeserialize(Dictionary<string, fsData> data, ref Texture2D model)
        {
            var result = fsResult.Success;

            var present = false;
            result += DeserializeMember(data, "present", out present);

            if (present == true)
            {
                var path = "";
                result += DeserializeMember(data, "path", out path);
                if (!string.IsNullOrEmpty(path))
                {
                    model = GaiaUtils.GetAsset(path, typeof(Texture2D)) as Texture2D;
                    if (model == null)
                    {
                        Debug.LogWarning("Unable to locate asset : " + path);
                    }
                    else
                    {
                        var name = model.name;
                        result += DeserializeMember(data, "name", out name);
                        model.name = name;
                    }
                }
            }

            return result;
        }

        public override object CreateInstance(fsData data, Type storageType)
        {
            return new Texture2D(0,0);
        }
    }
}
#endif
