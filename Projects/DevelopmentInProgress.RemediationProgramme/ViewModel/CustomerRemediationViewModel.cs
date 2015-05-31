﻿using System;
using System.Collections.Generic;
using System.Windows.Input;
using DevelopmentInProgress.DipState;
using DevelopmentInProgress.RemediationProgramme.Model;
using DevelopmentInProgress.RemediationProgramme.Service;
using DevelopmentInProgress.Origin.Context;
using DevelopmentInProgress.Origin.ViewModel;

namespace DevelopmentInProgress.RemediationProgramme.ViewModel
{
    public class CustomerRemediationViewModel : DocumentViewModel
    {
        private readonly RemediationService remediationService;

        public CustomerRemediationViewModel(ViewModelContext viewModelContext, RemediationService remediationService)
            : base(viewModelContext)
        {
            this.remediationService = remediationService;
            CompleteCommand = new ViewModelCommand(Complete);
        }

        public ICommand CompleteCommand { get; set; }

        public List<Customer> Customers { get; set; }

        protected override ProcessAsyncResult OnPublishedAsync()
        {
            Customers = remediationService.GetCustomers();
            return base.OnPublishedAsync();
        }

        private void Complete(object param)
        {
            var state = param as DipState.DipState;
            remediationService.Run(state, DipStateStatus.Completed);
            OnPropertyChanged(String.Empty);
        }
    }
}