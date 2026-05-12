using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using Data;
using Mechanics.Network.Scripts.SpaceObjectsService;
using Mechanics.VideoService.Models;
using UnityEngine;

public class SpaceObjectConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return typeof(SpaceObject).IsAssignableFrom(objectType);
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        // Загружаем JSON вручную, чтобы избежать рекурсивного вызова
        var jsonObject = JObject.Load(reader);
        var prefabName = jsonObject["PrefabName"]?.ToString();
        var name = jsonObject["Name"]?.ToString();
        // Если PrefabName == "Picture", то десериализуем как PictureSpaceObject
        if (prefabName == "Picture")
        {
            var pictureSpaceObject = new PictureSpaceObject();
            serializer.Populate(jsonObject.CreateReader(), pictureSpaceObject);
            return pictureSpaceObject;
        }else if (prefabName == "VideoPlayer")
        {
            var videoSpaceObject = new VideoSpaceObject();
            serializer.Populate(jsonObject.CreateReader(), videoSpaceObject);
            return videoSpaceObject;
        }


        // По умолчанию возвращаем SpaceObject
        var spaceObject = new SpaceObject();
        serializer.Populate(jsonObject.CreateReader(), spaceObject);
        return spaceObject;
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        serializer.Serialize(writer, value);
    }
}
