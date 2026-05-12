using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using Mechanics.Network.Scripts.SpaceObjectsService;
using Mechanics.VideoService.Models;

public class TowerObjectConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return typeof(TowerObject).IsAssignableFrom(objectType);
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        var jsonObject = JObject.Load(reader);
        var prefabName = jsonObject["ObjectClass"]?["PrefabName"]?.ToString();

        // Если PrefabName == "Picture", то десериализуем как PictureTowerObject
        if (prefabName == "Picture")
        {
            var pictureTowerObject = new PictureTowerObject();
            serializer.Populate(jsonObject.CreateReader(), pictureTowerObject);
            return pictureTowerObject;
        }else if (prefabName == "VideoPlayer")
        {
            var videoTowerObject = new VideoTowerObject();
            serializer.Populate(jsonObject.CreateReader(), videoTowerObject);
            return videoTowerObject;
        }


        // По умолчанию возвращаем TowerObject
        var towerObject = new TowerObject();
        serializer.Populate(jsonObject.CreateReader(), towerObject);
        return towerObject;
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        serializer.Serialize(writer, value);
    }
}


