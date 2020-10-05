//====================================================================================================
// Base code generated with Vector: WFA Gen (Build 2.0.4877.28464)
// Layered Architecture Solution Guidance (http://layerguidance.codeplex.com)
//
// Generated by Serena Yeoh at ALIENWARE on 05/09/2013 15:54:13 
//====================================================================================================

using System;
using System.Activities;
using System.Activities.Presentation.Metadata;
using System.ComponentModel;
using System.Windows;
using System.Collections.Generic;
using LeaveSample.Entities;
using LeaveSample.Workflows.Activities;

namespace LeaveSample.Workflows.Designers
{
    /// <summary>
    /// Designer class for Apply.
    /// </summary>
    public partial class ApplyDesigner : IRegisterMetadata
    {
        public ApplyDesigner()
        {
            InitializeComponent();
        }

        public void Register()
        {
            AttributeTableBuilder builder = new AttributeTableBuilder();

            builder.AddCustomAttributes(typeof(Apply), new Attribute[] {
            new DesignerAttribute(typeof(ApplyDesigner))});

            MetadataStore.AddAttributeTable(builder.CreateTable());
        }

        public void leaveEntry_Loaded(object sender, RoutedEventArgs e)
        {
            leaveEntry.ExpressionType = typeof(Leave);
        }

    }
}
