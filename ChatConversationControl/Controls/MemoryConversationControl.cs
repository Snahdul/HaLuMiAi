﻿using Microsoft.KernelMemory;
using System.Collections.ObjectModel;
using System.Windows;

namespace ChatConversationControl.Controls;

/// <summary>
/// A custom control for handling memory-based conversation functionalities.
/// </summary>
public class MemoryConversationControl : ConversationControl
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MemoryConversationControl"/> class.
    /// </summary>
    static MemoryConversationControl()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(MemoryConversationControl), new FrameworkPropertyMetadata(typeof(MemoryConversationControl)));
    }

    /// <summary>
    /// Identifies the StorageIndexes dependency property.
    /// </summary>
    public static readonly DependencyProperty StorageIndexesProperty =
        DependencyProperty.Register(nameof(StorageIndexes), typeof(object), typeof(MemoryConversationControl), new PropertyMetadata(null));

    /// <summary>
    /// Gets or sets the storage indexes for the conversation.
    /// </summary>
    public object StorageIndexes
    {
        get => GetValue(StorageIndexesProperty);
        set => SetValue(StorageIndexesProperty, value);
    }

    /// <summary>
    /// Identifies the SelectedStorageIndex dependency property.
    /// </summary>
    public static readonly DependencyProperty SelectedStorageIndexProperty =
        DependencyProperty.Register(nameof(SelectedStorageIndex), typeof(string), typeof(MemoryConversationControl), new PropertyMetadata(null));

    /// <summary>
    /// Gets or sets the selected storage index.
    /// </summary>
    public string SelectedStorageIndex
    {
        get => (string)GetValue(SelectedStorageIndexProperty);
        set => SetValue(SelectedStorageIndexProperty, value);
    }

    /// <summary>
    /// Identifies the MinRelevance dependency property.
    /// </summary>
    public static readonly DependencyProperty MinRelevanceProperty =
        DependencyProperty.Register(nameof(MinRelevance), typeof(double), typeof(ConversationControl), new PropertyMetadata(0.0));

    /// <summary>
    /// Gets or sets the minimum relevance value.
    /// </summary>
    public double MinRelevance
    {
        get => (double)GetValue(MinRelevanceProperty);
        set => SetValue(MinRelevanceProperty, value);
    }

    /// <summary>
    /// Identifies the RelevantSources dependency property.
    /// </summary>
    public static readonly DependencyProperty RelevantSourcesProperty =
        DependencyProperty.Register(nameof(RelevantSources), typeof(ObservableCollection<Citation>), typeof(MemoryConversationControl), new PropertyMetadata(new ObservableCollection<Citation>()));

    /// <summary>
    /// Gets or sets the relevant sources used to produce the answer.
    /// </summary>
    public ObservableCollection<Citation> RelevantSources
    {
        get => (ObservableCollection<Citation>)GetValue(RelevantSourcesProperty);
        set => SetValue(RelevantSourcesProperty, value);
    }
}