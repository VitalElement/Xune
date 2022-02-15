﻿using System;
using System.Threading;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Threading;

namespace Xune.Controls;

/// <summary>
/// Displays <see cref="Content"/> according to a <see cref="FuncDataTemplate"/>.
/// Uses <see cref="PageTransition"/> to move between the old and new content values. 
/// </summary>
public class TransitioningContentControl : ContentControl
{
    private CancellationTokenSource? _lastTransitionCts;
    private object? _displayedContent;
    
    /// <summary>
    /// Defines the <see cref="PageTransition"/> property.
    /// </summary>
    public static readonly StyledProperty<IPageTransition?> PageTransitionProperty =
        AvaloniaProperty.Register<TransitioningContentControl, IPageTransition?>(nameof(PageTransition),
            new CrossFade(TimeSpan.FromSeconds(0.125)));


    /// <summary>
    /// Defines the <see cref="DisplayedContent"/> property.
    /// </summary>
    public static readonly DirectProperty<TransitioningContentControl, object?> DisplayedContentProperty =
        AvaloniaProperty.RegisterDirect<TransitioningContentControl, object?>(nameof(DisplayedContent),
            o => o.DisplayedContent);
    
    /// <summary>
    /// Gets or sets the animation played when content appears and disappears.
    /// </summary>
    public IPageTransition? PageTransition
    {
        get => GetValue(PageTransitionProperty);
        set => SetValue(PageTransitionProperty, value);
    }

    /// <summary>
    /// Gets or sets the content displayed whenever there is no page currently routed.
    /// </summary>
    public object? DisplayedContent
    {
        get => _displayedContent;
        private set => SetAndRaise(DisplayedContentProperty, ref _displayedContent, value);
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);

        Dispatcher.UIThread.Post(() => UpdateContentWithTransition(Content));
    }

    protected override void OnPropertyChanged<T>(AvaloniaPropertyChangedEventArgs<T> change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == ContentProperty)
        {
            Dispatcher.UIThread.Post(() => UpdateContentWithTransition(Content));
        }
    }

    /// <summary>
    /// Updates the content with transitions.
    /// </summary>
    /// <param name="content">New content to set.</param>
    private async void UpdateContentWithTransition(object? content)
    {
        if (VisualRoot is null)
        {
            return;
        }
        
        _lastTransitionCts?.Cancel();
        _lastTransitionCts = new CancellationTokenSource();

        if (PageTransition != null)
            await PageTransition.Start(this, null, true, _lastTransitionCts.Token);

        DisplayedContent = content;
        
        if (PageTransition != null)
            await PageTransition.Start(null, this, true, _lastTransitionCts.Token);
    }
}
