﻿using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Xaml.Interactivity;

namespace Synfonia.Behaviors
{
    /// <summary>
    ///     Drag behavior.
    /// </summary>
    public sealed class ContextDragBehavior : Behavior<Control>
    {
        /// <summary>
        ///     Define <see cref="Context" /> property.
        /// </summary>
        public static readonly StyledProperty<object> ContextProperty =
            AvaloniaProperty.Register<ContextDragBehavior, object>(nameof(Context));

        /// <summary>
        ///     Define <see cref="Handler" /> property.
        /// </summary>
        public static readonly StyledProperty<IDragHandler> HandlerProperty =
            AvaloniaProperty.Register<ContextDragBehavior, IDragHandler>(nameof(Handler));

        private Point _dragStartPoint;
        private bool _lock;
        private PointerEventArgs _triggerEvent;

        /// <summary>
        ///     Gets or sets drag behavior context.
        /// </summary>
        public object Context
        {
            get => GetValue(ContextProperty);
            set => SetValue(ContextProperty, value);
        }

        /// <summary>
        ///     Gets or sets drag handler.
        /// </summary>
        public IDragHandler Handler
        {
            get => GetValue(HandlerProperty);
            set => SetValue(HandlerProperty, value);
        }

        /// <inheritdoc />
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.AddHandler(InputElement.PointerPressedEvent, AssociatedObject_PointerPressed,
                RoutingStrategies.Direct | RoutingStrategies.Tunnel | RoutingStrategies.Bubble);
            AssociatedObject.AddHandler(InputElement.PointerMovedEvent, AssociatedObject_PointerMoved,
                RoutingStrategies.Direct | RoutingStrategies.Tunnel | RoutingStrategies.Bubble);
        }

        /// <inheritdoc />
        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.RemoveHandler(InputElement.PointerPressedEvent, AssociatedObject_PointerPressed);
            AssociatedObject.RemoveHandler(InputElement.PointerMovedEvent, AssociatedObject_PointerMoved);
        }

        private async Task DoDragDrop(PointerEventArgs triggerEvent, object value)
        {
            var data = new DataObject();
            data.Set(ContextDropBehavior.DataFormat, value);

            var effect = DragDropEffects.None;

            if (triggerEvent.KeyModifiers.HasFlag(KeyModifiers.Alt))
                effect |= DragDropEffects.Link;
            else if (triggerEvent.KeyModifiers.HasFlag(KeyModifiers.Shift))
                effect |= DragDropEffects.Move;
            else if (triggerEvent.KeyModifiers.HasFlag(KeyModifiers.Control))
                effect |= DragDropEffects.Copy;
            else
                effect |= DragDropEffects.Move;

            await DragDrop.DoDragDrop(triggerEvent, data, effect);
        }

        private void AssociatedObject_PointerPressed(object sender, PointerPressedEventArgs e)
        {
            var properties = e.GetCurrentPoint(AssociatedObject).Properties;
            if (properties.IsLeftButtonPressed)
                if (e.Source is IControl)
                {
                    _dragStartPoint = e.GetPosition(null);
                    _triggerEvent = e;
                    _lock = true;
                }
        }

        private async void AssociatedObject_PointerMoved(object sender, PointerEventArgs e)
        {
            var properties = e.GetCurrentPoint(AssociatedObject).Properties;
            if (properties.IsLeftButtonPressed && _triggerEvent != null)
            {
                var point = e.GetPosition(null);
                var diff = _dragStartPoint - point;
                if (Math.Abs(diff.X) > 3 || Math.Abs(diff.Y) > 3)
                {
                    if (_lock)
                        _lock = false;
                    else
                        return;

                    Handler?.BeforeDragDrop(sender, _triggerEvent, Context);

                    await DoDragDrop(_triggerEvent, Context);

                    Handler?.AfterDragDrop(sender, _triggerEvent, Context);

                    _triggerEvent = null;
                }
            }
        }
    }
}