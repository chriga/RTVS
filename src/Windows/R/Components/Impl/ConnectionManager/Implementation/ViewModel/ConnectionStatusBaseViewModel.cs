﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using Microsoft.Common.Core.Disposables;
using Microsoft.Common.Core.Services;
using Microsoft.R.Common.Wpf.Controls;

namespace Microsoft.R.Components.ConnectionManager.Implementation.ViewModel {
    public abstract class ConnectionStatusBaseViewModel : BindableBase, IDisposable {
        private readonly DisposableBag _disposableBag;

        private bool _isContainer;
        private bool _isRemote;
        private bool _isActive;
        private bool _isConnected;
        private bool _isRunning;

        protected IServiceContainer Services { get; }
        protected IConnectionManager ConnectionManager { get; }

        protected ConnectionStatusBaseViewModel(IServiceContainer services) {
            Services = services;
            ConnectionManager = services.GetService<IConnectionManager>();

            _disposableBag = DisposableBag.Create<ConnectionStatusBarViewModel>()
                .Add(() => ConnectionManager.ConnectionStateChanged -= ConnectionStateChanged)
                .Add(() => ConnectionManager.RecentConnectionsChanged -= RecentConnectionsChanged);

            ConnectionManager.ConnectionStateChanged += ConnectionStateChanged;
            ConnectionManager.RecentConnectionsChanged += RecentConnectionsChanged;
            IsConnected = ConnectionManager.IsConnected;
            IsRunning = ConnectionManager.IsRunning;
        }

        public virtual void Dispose(bool disposing) => _disposableBag.TryDispose();
        public void Dispose() => Dispose(true);

        protected abstract void UpdateConnections();

        private void RecentConnectionsChanged(object sender, EventArgs e)
            => Services.MainThread().Post(UpdateConnections);
        private void ConnectionStateChanged(object sender, EventArgs e)
            => Services.MainThread().Post(ConnectionStateChangedOnMainThread);

        private void ConnectionStateChangedOnMainThread() {
            IsConnected = ConnectionManager.IsConnected;
            IsRunning = ConnectionManager.IsConnected && ConnectionManager.IsRunning;
            IsActive = ConnectionManager.ActiveConnection != null;
            IsRemote = ConnectionManager.ActiveConnection?.IsRemote ?? false;
            IsContainer = ConnectionManager.ActiveConnection?.IsContainer ?? false;
            UpdateConnections();
        }

        public bool IsConnected {
            get => _isConnected;
            set => SetProperty(ref _isConnected, value);
        }

        public bool IsRunning {
            get => _isRunning;
            set => SetProperty(ref _isRunning, value);
        }

        public bool IsActive {
            get => _isActive;
            set => SetProperty(ref _isActive, value);
        }

        public bool IsRemote {
            get => _isRemote;
            set => SetProperty(ref _isRemote, value);
        }
        public bool IsContainer {
            get => _isContainer;
            set => SetProperty(ref _isContainer, value);
        }
    }
}
