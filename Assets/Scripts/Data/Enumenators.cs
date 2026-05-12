using System;

namespace Data
{
    public class Enumenators
    {
        public enum PanelType
        {
            Unknown,
            LoginPanel,
            ScenesPanel,
            SettingsPanel,
            FriendsPanel,
            InventoryPanel,
            EventsPanel,
            WalletPanel,
            HelperPanel,
            PreparePanel,
            LocationsPanel,
            VisitorsPanel,
            StatisticsPanel,
            RegistrationPanel,
            TipsPanel
        }

        public enum SpaceModeType
        {
            Unknown,
            Help,
            Normal
        }

        public enum SkinType
        {
            Unknown = 0,
            Material = 1,
            Mesh = 2,
            Script = 3
        }

        public enum ApartmentType
        {
            Unknown,
            CommonApartment,
            MyApartment
        }

        public enum RemoteContentType
        {
            Unknown = 0,
            Scene = 1,
            ClientModel = 2,
            NetworkPrefab = 3
        }

        public enum PrefabObjectType
        {
            Unknown = 0,
            Picture = 1,
            Levitation = 2,
            Blackboard = 3,
            BrowserAdminPlace = 4,
            BrowserPlace = 5,
            Place = 6,
            Keyboard = 7,
            DecorationModel = 8,
            CustomLogicObject = 9, //ex MovableObject
            Mirror = 10,
            Tablet = 11,
            Camera = 12,
            Video = 13
            //CustomLogicObject = 13,
        }

        public enum CornerType
        {
            LeftTopCorner = 0,
            RightDownCorner = 1
        }

        public enum ShapeType
        {
            Unknown,
            Line,
            Quad,
            Triangle,
            Circle
        }

        public enum TransactionPurposeTypeDto
        {
            Transfer = 0,
            TimeMining = 1,
            SpaceEntryFee = 2,
            BuySell = 3,
            TimeBurning = 4,
        }

        public enum MovementType
        {
            TriggerMove,
            GrabbingMove
        }

        public class Server
        {
            [Serializable]
            public enum Profile
            {
                Develop,
                Demo,
                Production,
                Test
            }

            [Serializable]
            public enum ServerType
            {
                DemoLinux,
                DevLinux,
                Prod,
                Win,
                LocalWin,
            }

            [Serializable]
            public enum Port
            {
                Win,
                Linux
            }
        }
    }
}