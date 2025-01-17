﻿//-----------------------------------------------------------------------
// <copyright file="DocumentViewBase.cs" company="Development In Progress Ltd">
//     Copyright © 2012. All rights reserved.
// </copyright>
// <author>Grant Colley</author>
//-----------------------------------------------------------------------

using DevelopmentInProgress.Origin.Context;
using DevelopmentInProgress.Origin.Navigation;
using DevelopmentInProgress.Origin.ViewModel;
using System;
using Xceed.Wpf.AvalonDock.Layout;

namespace DevelopmentInProgress.Origin.View
{
    /// <summary>
    /// Base abstract class implemented by document views. Inherits <see cref="ViewBase"/>.
    /// </summary>
    public abstract class DocumentViewBase : ViewBase
    {
        private LayoutAnchorable hostControl;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentViewBase"/> class.
        /// </summary>
        /// <param name="viewContext">The view context.</param>
        /// <param name="authorisationViewModel">The view model.</param>
        /// <param name="moduleName">The mudule name.</param>
        protected DocumentViewBase(IViewContext viewContext, DocumentViewModel authorisationViewModel, string moduleName)
            : base(viewContext)
        {
            ModuleName = moduleName;
            authorisationViewModel.ViewModelContext.UiDispatcher = Dispatcher;
            authorisationViewModel.Activate += ViewModelActivate;
            authorisationViewModel.ShowMessageWindow += ShowMessageBox;
            authorisationViewModel.ShowModalWindow += ShowModalWindow;
            authorisationViewModel.NavigateTarget += NavigateTarget;
            authorisationViewModel.GetViewModels += GetViewModels;
            authorisationViewModel.Publish += Publish;
        }

        /// <summary>
        /// Gets or sets the module name.
        /// </summary>
        public string ModuleName { get; private set; }

        /// <summary>
        /// Gets or sets the control that hosts the view in the Prism region.
        /// </summary>
        public object HostControl
        {
            get
            {
                return hostControl;
            }
            set
            {
                hostControl = value as LayoutAnchorable;
            }
        }

        /// <summary>
        /// Gets the <see cref="DocumentViewModel"/>.
        /// </summary>
        public DocumentViewModel ViewModel
        {
            get
            {
                DocumentViewModel viewModel = null;

                if (Dispatcher.CheckAccess())
                {
                    viewModel = DataContext as DocumentViewModel;
                }
                else
                {
                    Dispatcher.Invoke(() => { viewModel = DataContext as DocumentViewModel; });
                }

                return viewModel;
            }
        }

        /// <summary>
        /// Cleanup before closing the document. Removes the documents parameters
        /// which are stored by the <see cref="NavigationManager"/>.
        /// </summary>
        public void CloseDocument()
        {
            var documentViewModel = DataContext as DocumentViewModel;
            if (documentViewModel != null)
            {
                string navigationId = documentViewModel.NavigationId;
                ViewContext.NavigationManager.CloseDocument(navigationId);
            }
        }

        /// <summary>
        /// Gets one or more view models based on settings in <see cref="FindDocumentViewModel"/>.
        /// A single view model can be fetched by navigation id, multiple view models can be 
        /// fetched by module name, and if neither navigation id or module name is provided then
        /// all document view models will be returned for all views. 
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="FindDocumentViewModel"/>.</param>
        private void GetViewModels(object sender, FindDocumentViewModel e)
        {
            if (String.IsNullOrEmpty(e.NavigationId)
                && String.IsNullOrEmpty(e.Module))
            {
                var documentViewModels = ViewContext.NavigationManager.GetAllViewModels();
                e.ViewModels = documentViewModels;
                return;
            }

            if (!String.IsNullOrEmpty(e.NavigationId))
            {
                var documentViewModel = ViewContext.NavigationManager.GetViewModel(e.NavigationId);
                e.ViewModel = documentViewModel;
            }

            if (!String.IsNullOrEmpty(e.Module))
            {
                var documentViewModels = ViewContext.NavigationManager.GetViewModels(e.Module);
                e.ViewModels = documentViewModels;
            }
        }

        /// <summary>
        /// Handles the activate event raised by the view model
        /// will tells the view to set the title on the host control
        /// and to make the current document the active document.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">Event arguments.</param>
        private void ViewModelActivate(object sender, EventArgs e)
        {
            if (hostControl != null)
            {
                var model = DataContext as DocumentViewModel;
                if (model != null)
                {
                    hostControl.Title = model.Title ?? String.Empty;
                    hostControl.ToolTip = model.Title ?? String.Empty;
                }

                hostControl.IsActive = true;
            }
        }
    }
}
