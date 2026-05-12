using System;
using System.Collections.Generic;
using System.IO;
using Unity.Netcode;
using UnityEngine;
using Vuplex.WebView;

namespace Assets.Mechanics.Browser
{
    public enum WebPageInputActionType
    {
        None = 0,
        BeganDrag = 1,
        Dragged = 2,
        PointerMoved = 3,
        PointerDown = 4,
        PointerUp = 5,
        PointerExited = 6,
        Scrolled = 7
    }

    [Serializable]
    public class SerializableWebPageInputData : INetworkSerializable
    {
        public Queue<WebPageInputAction> ActionsQueue;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            if (serializer.IsWriter)
            {
                byte[] bytes = Serialize();
                serializer.SerializeValue(ref bytes);
            }
            else
            {
                byte[] bytes = null;
                serializer.SerializeValue(ref bytes);
                ActionsQueue = Deserialize(bytes);
            }
        }

        public byte[] Serialize()
        {
            using MemoryStream m = new MemoryStream();
            using (BinaryWriter writer = new BinaryWriter(m))
            {
                writer.Write(ActionsQueue.Count);
                foreach (var inputAction in ActionsQueue)
                {
                    writer.Write((byte)inputAction.WebPageInputActionType);
                    byte[] bytes = inputAction.Serialize();
                    writer.Write(bytes.Length);
                    writer.Write(bytes);
                }
            }
            return m.ToArray();
        }

        public static Queue<WebPageInputAction> Deserialize(byte[] bytes)
        {
            Queue<WebPageInputAction> actionsQueue = new();

            using MemoryStream m = new(bytes);
            using BinaryReader reader = new(m);

            int inputActionCount = reader.ReadInt32();
            for (int i = 0; i < inputActionCount; i++)
            {
                var inputActionType = (WebPageInputActionType)reader.ReadByte();
                int actionByteCount = reader.ReadInt32();
                byte[] actionBytes = reader.ReadBytes(actionByteCount);

                WebPageInputAction pageInputAction = null;
                switch (inputActionType)
                {
                    case WebPageInputActionType.None:
                        break;
                    case WebPageInputActionType.BeganDrag:
                        pageInputAction = new BeganDragWebPageInputAction(actionBytes);
                        break;
                    case WebPageInputActionType.Dragged:
                        pageInputAction = new DraggedWebPageInputAction(actionBytes);
                        break;
                    case WebPageInputActionType.PointerMoved:
                        break;
                    case WebPageInputActionType.PointerDown:
                        pageInputAction = new PointerDownWebPageInputAction(actionBytes);
                        break;
                    case WebPageInputActionType.PointerUp:
                        pageInputAction = new PointerUpWebPageInputAction(actionBytes);
                        break;
                    case WebPageInputActionType.PointerExited:
                        pageInputAction = new PointerExitedWebPageInputAction(actionBytes);
                        break;
                    case WebPageInputActionType.Scrolled:
                        pageInputAction = new ScrolledWebPageInputAction(actionBytes);
                        break;
                    default:
                        break;
                }

                if (pageInputAction != null)
                {
                    actionsQueue.Enqueue(pageInputAction);
                }
            }

            return actionsQueue;
        }
    }

    public static class WebPageInputSerialization
    {
        public static byte[] SerializeVector2EventArgs(EventArgs<Vector2> eventArgs)
        {
            using MemoryStream m = new MemoryStream();
            using (BinaryWriter writer = new BinaryWriter(m))
            {
                WriteVector2(eventArgs.Value, writer);
            }
            return m.ToArray();
        }

        public static EventArgs<Vector2> DeserializeVector2EventArgs(byte[] bytes)
        {
            using MemoryStream m = new(bytes);
            using BinaryReader reader = new(m);
            return new EventArgs<Vector2>(ReadVector2(reader));
        }

        public static byte[] SerializeScrolledEventArgs(ScrolledEventArgs eventArgs)
        {
            using MemoryStream m = new MemoryStream();
            using (BinaryWriter writer = new BinaryWriter(m))
            {
                WriteVector2(eventArgs.ScrollDelta, writer);
                WriteVector2(eventArgs.Point, writer);
            }
            return m.ToArray();
        }

        public static ScrolledEventArgs DeserializeScrolledEventArgs(byte[] bytes)
        {
            using MemoryStream m = new(bytes);
            using BinaryReader reader = new(m);

            Vector2 delta = ReadVector2(reader);
            Vector2 point = ReadVector2(reader);

            return new ScrolledEventArgs(delta, point);
        }

        public static byte[] SerializePointerEventArgs(PointerEventArgs eventArgs)
        {
            using MemoryStream m = new MemoryStream();
            using (BinaryWriter writer = new BinaryWriter(m))
            {
                writer.Write((byte)eventArgs.Button);
                writer.Write(eventArgs.ClickCount);
                WriteVector2(eventArgs.Point, writer);
            }
            return m.ToArray();
        }

        public static PointerEventArgs DeserializePointerEventArgs(byte[] bytes)
        {
            using MemoryStream m = new(bytes);
            using BinaryReader reader = new(m);

            PointerEventArgs pointerEventArgs = new();

            pointerEventArgs.Button = (MouseButton)reader.ReadByte();
            pointerEventArgs.ClickCount = reader.ReadInt32();
            pointerEventArgs.Point = ReadVector2(reader);

            return pointerEventArgs;
        }

        private static void WriteVector2(Vector2 vector2, BinaryWriter writer)
        {
            writer.Write(vector2.x);
            writer.Write(vector2.y);
        }

        private static Vector2 ReadVector2(BinaryReader reader)
        {
            float x = reader.ReadSingle();
            float y = reader.ReadSingle();
            return new Vector2(x, y);
        }
    }


    [Serializable]
    public abstract class WebPageInputAction
    {
        public abstract WebPageInputActionType WebPageInputActionType { get; }
        public abstract void ProcessAction(ControllableCanvasPointerInputDetector pointerInputDetector);
        public abstract byte[] Serialize();
    }

    [Serializable]
    public sealed class BeganDragWebPageInputAction : WebPageInputAction
    {
        public override WebPageInputActionType WebPageInputActionType => WebPageInputActionType.BeganDrag;
        public EventArgs<Vector2> EventArgs { get; }

        public BeganDragWebPageInputAction(EventArgs<Vector2> eventArgs)
        {
            EventArgs = eventArgs;
        }

        public BeganDragWebPageInputAction(byte[] bytes)
        {
            EventArgs = WebPageInputSerialization.DeserializeVector2EventArgs(bytes);
        }

        public override void ProcessAction(ControllableCanvasPointerInputDetector pointerInputDetector)
        {
            pointerInputDetector.RaiseBeganDragEvent(EventArgs);
            //Debug.LogWarning($"<color=yellow>BeganDrag.</color> Point={EventArgs.Value}");
        }

        public override byte[] Serialize()
        {
            return WebPageInputSerialization.SerializeVector2EventArgs(EventArgs);
        }
    }

    [Serializable]
    public sealed class DraggedWebPageInputAction : WebPageInputAction
    {
        public override WebPageInputActionType WebPageInputActionType => WebPageInputActionType.Dragged;
        public EventArgs<Vector2> EventArgs { get; set; }

        public DraggedWebPageInputAction(EventArgs<Vector2> eventArgs)
        {
            EventArgs = eventArgs;
        }

        public DraggedWebPageInputAction(byte[] bytes)
        {
            EventArgs = WebPageInputSerialization.DeserializeVector2EventArgs(bytes);
        }

        public override void ProcessAction(ControllableCanvasPointerInputDetector pointerInputDetector)
        {
            pointerInputDetector.RaiseDraggedEvent(EventArgs);
        }

        public override byte[] Serialize()
        {
            return WebPageInputSerialization.SerializeVector2EventArgs(EventArgs);
        }
    }

    [Serializable]
    public sealed class ScrolledWebPageInputAction : WebPageInputAction
    {
        public override WebPageInputActionType WebPageInputActionType => WebPageInputActionType.Scrolled;
        public ScrolledEventArgs EventArgs { get; }

        public ScrolledWebPageInputAction(ScrolledEventArgs eventArgs)
        {
            EventArgs = eventArgs;
        }

        public ScrolledWebPageInputAction(byte[] bytes)
        {
            EventArgs = WebPageInputSerialization.DeserializeScrolledEventArgs(bytes);
        }

        public override void ProcessAction(ControllableCanvasPointerInputDetector pointerInputDetector)
        {
            pointerInputDetector.RaiseScrolledEvent(EventArgs);
        }

        public override byte[] Serialize()
        {
            return WebPageInputSerialization.SerializeScrolledEventArgs(EventArgs);
        }
    }

    [Serializable]
    public sealed class PointerDownWebPageInputAction : WebPageInputAction
    {
        public override WebPageInputActionType WebPageInputActionType => WebPageInputActionType.PointerDown;
        public PointerEventArgs EventArgs { get; }

        public PointerDownWebPageInputAction(PointerEventArgs eventArgs)
        {
            EventArgs = eventArgs;
        }

        public PointerDownWebPageInputAction(byte[] bytes)
        {
            EventArgs = WebPageInputSerialization.DeserializePointerEventArgs(bytes);
        }

        public override void ProcessAction(ControllableCanvasPointerInputDetector pointerInputDetector)
        {
            pointerInputDetector.RaisePointerDownEvent(EventArgs);
        }

        public override byte[] Serialize()
        {
            return WebPageInputSerialization.SerializePointerEventArgs(EventArgs);
        }
    }

    [Serializable]
    public sealed class PointerUpWebPageInputAction : WebPageInputAction
    {
        public override WebPageInputActionType WebPageInputActionType => WebPageInputActionType.PointerUp;
        public PointerEventArgs EventArgs { get; }

        public PointerUpWebPageInputAction(PointerEventArgs eventArgs)
        {
            EventArgs = eventArgs;
        }

        public PointerUpWebPageInputAction(byte[] bytes)
        {
            EventArgs = WebPageInputSerialization.DeserializePointerEventArgs(bytes);
        }

        public override void ProcessAction(ControllableCanvasPointerInputDetector pointerInputDetector)
        {
            pointerInputDetector.RaisePointerUpEvent(EventArgs);
        }

        public override byte[] Serialize()
        {
            return WebPageInputSerialization.SerializePointerEventArgs(EventArgs);
        }
    }

    [Serializable]
    public sealed class PointerExitedWebPageInputAction : WebPageInputAction
    {
        public override WebPageInputActionType WebPageInputActionType => WebPageInputActionType.PointerExited;
        public EventArgs EventArgs { get; }

        public PointerExitedWebPageInputAction(EventArgs eventArgs)
        {
            EventArgs = eventArgs;
        }

        public PointerExitedWebPageInputAction(byte[] bytes)
        {
            EventArgs = new EventArgs();
        }

        public override void ProcessAction(ControllableCanvasPointerInputDetector pointerInputDetector)
        {
            pointerInputDetector.RaisePointerExitedEvent(EventArgs);
        }

        public override byte[] Serialize()
        {
            return new byte[0];
        }
    }

}