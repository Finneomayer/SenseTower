using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using Assets.Scripts.Space;
using Data;
using Mechanics.LoadSceneObjects.Models;
using Mechanics.Network.Scripts.SpaceObjectsService;
using UnityEngine;

namespace Assets.Scripts.Data
{
    public static class DataExtensions
    {
        public const string UserFullStatus = "IsFullFledgedUser";
        private const string RoomIdKey = "SPACE_ID";

        private const float BrowserDefaultSizeXZ = 4.03225f;
        private const float BrowserDefaultSizeY = 2.25044f;

        private static string[] availableNick =
            {"artem", "alena", "kovalevsky", "psilon2000", "sahanich_text", "sahanich", "Andrey", "andrey", "mironenko"};

        public static string GetSpaceID()
        {
            string _roomId = Environment.GetEnvironmentVariable(RoomIdKey);
            //_roomId = "466c4f30-6b7d-4155-878e-08f80e9b7324";
            return _roomId;
        }

        public static bool AvailableUsers(string nick)
        {
            for (int i = 0; i < availableNick.Length; i++)
            {
                if (availableNick[i].Equals(nick.ToLower()))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool AvailableAdmin(LocalSpace localSpace, string userId)
        {
            if (localSpace.AdminIds != null && localSpace.AdminIds.Count > 0)
            {
                if (localSpace.AdminIds.Contains(userId))
                {
                    return true;
                }
            }

            return false;
        }

        public static DateTime UnixToDateTime(this long unixTime)
        {
            DateTime unixStart = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            long unixTimeStampInTicks = unixTime * TimeSpan.TicksPerSecond;
            return new DateTime(unixStart.Ticks + unixTimeStampInTicks, System.DateTimeKind.Utc);
        }

        public static Vector3 StringToVector3(this string vector3Data)
        {
            string[] results = vector3Data.Split(':');

            if (results.Length < 2)
                return Vector3.zero;
            else
            {
                if (float.TryParse(results[0], NumberStyles.Float, CultureInfo.InvariantCulture, out float x) &&
                    float.TryParse(results[1], NumberStyles.Float, CultureInfo.InvariantCulture, out float y) &&
                    float.TryParse(results[2], NumberStyles.Float, CultureInfo.InvariantCulture, out float z))
                {
                    return new Vector3(x, y, z);
                }
            }

            return Vector3.zero;
        }

        public static Quaternion StringToQuaternion(this string vector3Data)
        {
            string[] results = vector3Data.Split(':');
            if (results.Length < 3)
                return Quaternion.identity;
            else
            {
                if (float.TryParse(results[0], out float x) &&
                    float.TryParse(results[1], out float y) &&
                    float.TryParse(results[2], out float z)
                   )
                {
                    return Quaternion.Euler(x,y,z);
                }
            }

            return Quaternion.identity;
        }

        public static Vector3 VectorComponentsToVector3(this VectorsComponent vectorsComponent)
        {
            if (vectorsComponent == null)
                return Vector3.zero;

            return new Vector3(vectorsComponent.X, vectorsComponent.Y, vectorsComponent.Z);
        }

        public static VectorsComponent Vector3ToVectorComponents(this Vector3 vectorsComponent)
        {
            VectorsComponent tempVectorsComponent = new VectorsComponent();

            tempVectorsComponent.X = vectorsComponent.x;
            tempVectorsComponent.Y = vectorsComponent.y;
            tempVectorsComponent.Z = vectorsComponent.z;

            return tempVectorsComponent;
        }

        public static string Vector3ToCustomString(this Vector3 vector3Data)
        {
            string result = $"{vector3Data.x}:{vector3Data.y}:{vector3Data.z}";
            result = result.Replace(',', '.');
            return result;
        }

        public static string QuaternionToCustomString(this Quaternion quaternionData)
        {
            string result = $"{quaternionData.x}:{quaternionData.y}:{quaternionData.z}:{quaternionData.w}";
            result = result.Replace(',', '.');
            return result;
        }

        public static string StringConvertionToGalleryDescription(this string stringData)
        {
            if (string.IsNullOrEmpty(stringData))
                return stringData;

            string pattern = @"(')?[+](?(')^$)";
            string substitution = @"";
            Regex regex = new Regex(pattern);
            string result = regex.Replace(stringData, substitution);
            result = result.Replace("'+'", "+", StringComparison.Ordinal);

            if (result.Length > 120)
            {
                result = result.Remove(115);
                result = $"{result}.....";
            }

            return result;
        }

        public static List<string> StringsConvertionToYandexSpeach(this IEnumerable<string> textBlocks)
        {
            List<string> resultStrings = new();
            foreach (string block in textBlocks)
            {
                resultStrings.Add(StringConvertionToYandexSpeach(block));
            }

            return resultStrings;
        }

        public static string StringConvertionToYandexSpeach(this string textData)
        {
            string result = textData.Replace("'+'", "плюс", StringComparison.Ordinal);
            return result;
        }

        public static Vector3 CalculateMiddlePosition(StaticObject staticObject)
        {
            Vector3 centerPos = Vector3.zero;

            if (ScallableCalculate(staticObject))
            {
                centerPos = (staticObject.Vectors.LeftTop.VectorComponentsToVector3() +
                             staticObject.Vectors.RightDown.VectorComponentsToVector3()) / 2;
            }
            else
            {
                centerPos = new Vector3(staticObject.LeftTopX + staticObject.RightDownX,
                    staticObject.LeftTopY + staticObject.RightDownY,
                    staticObject.LeftTopZ + staticObject.RightDownZ) / 2;
            }

            return centerPos;
        }

        public static Vector3 CalculateBlackboardScale(StaticObject staticObject)
        {
            float CurrentDistXZ = 0;
            float CurrentDistY = 0;

            if (ScallableCalculate(staticObject))
            {
                Vector3 LeftTop = staticObject.Vectors.LeftTop.VectorComponentsToVector3();
                Vector3 RightDown = staticObject.Vectors.RightDown.VectorComponentsToVector3();

                var ball1XZPosition = new Vector3(LeftTop.x, 0, LeftTop.z);
                var ball2XZPosition = new Vector3(RightDown.x, 0, RightDown.z);

                CurrentDistXZ = Vector3.Distance(ball1XZPosition, ball2XZPosition) / 10;
                CurrentDistY = (LeftTop.y - RightDown.y) / 10;
            }
            else
            {
                CurrentDistY = Mathf.Abs(staticObject.LeftTopY - staticObject.RightDownY) / 10;
                CurrentDistXZ = Mathf.Abs(staticObject.LeftTopZ - staticObject.RightDownZ) / 10;
                if (CurrentDistXZ <= 0.001f && CurrentDistXZ >= -0.001f)
                {
                    CurrentDistY = Mathf.Abs(staticObject.LeftTopY - staticObject.RightDownY) / 10;
                    CurrentDistXZ = Mathf.Abs(staticObject.LeftTopX - staticObject.RightDownX) / 10;
                }
            }

            return new Vector3(CurrentDistY, 1, CurrentDistXZ);
        }

        public static Vector3 CalculateBrowserScale(StaticObject staticObject)
        {
            Vector3 result = CalculateScale(staticObject);
            result.x = result.z / BrowserDefaultSizeXZ;
            result.y = result.x;
            result.z = result.x;
            
            if (result.x < 0.3f)
            {
                result.x = result.z;
            }
            return result;
        }

        public static Vector3 CalculateScale(StaticObject staticObject)
        {
            float scaleX = 0;
            float CurrentDistXZ = 0;
            float CurrentDistY = 0;
            if (ScallableCalculate(staticObject))
            {
                Vector3 LeftTop = staticObject.Vectors.LeftTop.VectorComponentsToVector3();
                Vector3 RightDown = staticObject.Vectors.RightDown.VectorComponentsToVector3();
                var ball1XZPosition = new Vector3(LeftTop.x, 0, LeftTop.z);
                var ball2XZPosition = new Vector3(RightDown.x, 0, RightDown.z);

                CurrentDistXZ = Vector3.Distance(ball1XZPosition, ball2XZPosition);
                CurrentDistY = (LeftTop.y - RightDown.y);
                scaleX = Mathf.Abs(LeftTop.x - RightDown.x);
            }
            else
            {
                scaleX = Mathf.Abs(staticObject.LeftTopX - staticObject.RightDownX);
                CurrentDistY = Mathf.Abs(staticObject.LeftTopY - staticObject.RightDownY);
                CurrentDistXZ = Mathf.Abs(staticObject.LeftTopZ - staticObject.RightDownZ);
            }

            if (CurrentDistXZ <= 0.001f)
            {
                CurrentDistXZ = scaleX;
                scaleX = 1;
            }

            return new Vector3(scaleX, CurrentDistY, CurrentDistXZ);
        }

        public static Quaternion CalculateRotation(Transform target, StaticObject staticObject)
        {
            Vector3 LeftTopCorner = Vector3.zero;
            Vector3 RightDownCorner = Vector3.zero;

            if (ScallableCalculate(staticObject))
            {
                LeftTopCorner = staticObject.Vectors.LeftTop.VectorComponentsToVector3();
                RightDownCorner = staticObject.Vectors.RightDown.VectorComponentsToVector3();
            }
            else
            {
                LeftTopCorner = new Vector3(staticObject.LeftTopX, staticObject.LeftTopY, staticObject.LeftTopZ);
                RightDownCorner = new Vector3(staticObject.RightDownX, staticObject.RightDownY,
                    staticObject.RightDownZ);
            }

            Vector3 dir = LeftTopCorner - RightDownCorner;

            //Quaternion rotation = Quaternion.LookRotation(dir, Vector3.up);
            Vector3 rotationInEulerAngle = Quaternion.LookRotation(dir, Vector3.forward).eulerAngles;
            Vector3 targetRotationEulerAngle = target.transform.rotation.eulerAngles;

            rotationInEulerAngle.x = targetRotationEulerAngle.x;
            rotationInEulerAngle.z = targetRotationEulerAngle.z;
            return Quaternion.Euler(rotationInEulerAngle);
        }

        public static StaticObject SpaceObjectToStaticObject(SpaceObject spaceObject)
        {
            var result = new StaticObject();

            result.TowerObjectId = spaceObject.TowerObjectId;
            result.RemoteObjectType = Enumenators.RemoteContentType.NetworkPrefab;
            if (spaceObject.TowerObject != null && spaceObject.TowerObject.ObjectClass != null)
            {
                result.PrefabObjectType = spaceObject.TowerObject.ObjectClass.PrefabObjectType;
                if (spaceObject.TowerObject.ObjectClass.RemoteObjectTypeInfo != null)
                {
                    result.ObjectKey = spaceObject.TowerObject.ObjectClass.RemoteObjectTypeInfo.ObjectKey ?? "";
                    result.RepositoryUrl = spaceObject.TowerObject.ObjectClass.RemoteObjectTypeInfo.ObjectRepositoryUrl ?? "";
                }
                else
                {
                    result.ObjectKey = "";
                    result.RepositoryUrl = "";
                }
            }
            result.Vectors = spaceObject.Vectors;

            if (spaceObject.Vectors.LeftTop != null && spaceObject.Vectors.RightDown != null)
            {
                result.LeftTopX = spaceObject.Vectors.LeftTop.X;
                result.LeftTopY = spaceObject.Vectors.LeftTop.Y;
                result.LeftTopZ = spaceObject.Vectors.LeftTop.Z;
                result.RightDownX = spaceObject.Vectors.RightDown.X;
                result.RightDownY = spaceObject.Vectors.RightDown.Y;
                result.RightDownZ = spaceObject.Vectors.RightDown.Z;
            }
            result.TempRelatedObjectId = spaceObject.TempRelatedObjectId ?? "";
            result.HelpContent = spaceObject.HelpContent ?? "";
            result.IsActive = spaceObject.IsActive;

            return result;
        }

        private static bool ScallableCalculate(StaticObject staticobject)
        {
            bool CheckNullLeftAndRightCorner = staticobject.Vectors == null || staticobject.Vectors.LeftTop == null ||
                                               staticobject.Vectors.RightDown == null;

            if (!CheckNullLeftAndRightCorner)
            {
                return staticobject.Vectors.LeftTop.VectorComponentsToVector3() != Vector3.zero &&
                       staticobject.Vectors.RightDown.VectorComponentsToVector3() != Vector3.zero;
            }

            return !CheckNullLeftAndRightCorner;
        }
    }
}